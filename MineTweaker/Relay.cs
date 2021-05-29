using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class Relay
    {
        private TcpListener localListener = new TcpListener(IPAddress.Loopback, 5000);
        private TcpClient relayToServer;
        private TcpClient relayToClient;
        private Stream relayToServerNetStream;
        private Stream relayToServerStream;
        private Stream relayToClientStream;

        public string RemoteServerHostname { get; set; } = "";
        public int RemoteServerPort { get; set; } = 25565;
        public bool UseDefaultLoginHandler { get; set; } = true;

        private bool encryptionEnabled = false;
        private RijndaelManaged encryptionProvider;
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;
        private CryptoStream cryptoWriteStream;
        private CryptoStream cryptoReadStream;

        private Thread readFromClientThread;
        private Thread readFromServerThread;

        private bool compressionEnabled = false;
        private int compressionThreshold = 256;

        private List<PacketHandler> packetHandlers = new List<PacketHandler>();

        public ConnectionState ConnectionState { get; set; } = ConnectionState.NotConnected;

        public void Start()
        {
            if (UseDefaultLoginHandler)
            {
                RegisterPacketHandler(new LoginHandler());
            }
            localListener.Start();
            Console.WriteLine("Now listening for a localhost connection...");
            relayToClient = localListener.AcceptTcpClient();
            relayToClientStream = relayToClient.GetStream();
            Console.WriteLine("Got localhost connection, patching through to remote server...");
            ConnectionState = ConnectionState.Handshake;
            relayToServer = new TcpClient();
            try
            {
                relayToServer.Connect(RemoteServerHostname, RemoteServerPort);
            } catch
            {
                Console.WriteLine("Failed to connect to remote server. Make sure the host name and port is correct.");
                return;
            }
            relayToServerNetStream = relayToServer.GetStream();
            relayToServerStream = relayToServerNetStream;

            readFromClientThread = new Thread(readFromClientLoop);
            readFromServerThread = new Thread(readFromServerLoop);

            readFromClientThread.Start();
            readFromServerThread.Start();
        }
        public void EnableEncryption(byte[] AESKey) // Enabled encryption on the connection between the relay and the server.
        {
            encryptionProvider = new RijndaelManaged();
            encryptionProvider.Mode = CipherMode.CFB;
            encryptionProvider.FeedbackSize = 8;
            encryptionProvider.Padding = PaddingMode.None;
            encryptionProvider.KeySize = 128;
            encryptionProvider.Key = AESKey;
            encryptionProvider.IV = AESKey;
            encryptor = encryptionProvider.CreateEncryptor();
            decryptor = encryptionProvider.CreateDecryptor();
            cryptoWriteStream = new CryptoStream(relayToServerNetStream, encryptor, CryptoStreamMode.Write);
            cryptoReadStream = new CryptoStream(relayToServerNetStream, decryptor, CryptoStreamMode.Read);
            relayToServerStream = new MergedStream(cryptoReadStream, cryptoWriteStream);
            encryptionEnabled = true;
        }
        public void SetCompression(bool Enabled, int Theshold)
        {
            compressionEnabled = Enabled;
            compressionThreshold = Theshold;
        }
        public void RegisterPacketHandler(PacketHandler handler)
        {
            packetHandlers.Add(handler);
        }
        public void UnregisterPacketHandler(PacketHandler handler)
        {
            packetHandlers.Remove(handler);
        }
        public void InsertPacket(Packet Packet, PacketDirection Direction)
        {
            int dataLength = DataUtils.MeasureVarInt(Packet.PacketID) + Packet.Body.Length;
            if (Direction == PacketDirection.FromServerToClient)
            {
                relayToClientStream.WriteVarInt(dataLength);
                relayToClientStream.WriteVarInt(Packet.PacketID);
                relayToClientStream.Write(Packet.Body, 0, Packet.Body.Length);
            } else
            {
                if (compressionEnabled)
                {
                    bool compressed;
                    byte[] data;
                    if (dataLength >= compressionThreshold)
                    {
                        compressed = true;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (ZlibStream deflate = new ZlibStream(ms, Ionic.Zlib.CompressionMode.Compress))
                            {
                                deflate.WriteVarInt(Packet.PacketID);
                                deflate.Write(Packet.Body, 0, Packet.Body.Length);
                            }
                            data = ms.ToArray();
                        }
                    } else
                    {
                        compressed = false;
                        data = new byte[dataLength];
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            ms.WriteVarInt(Packet.PacketID);
                            ms.Write(Packet.Body, 0, Packet.Body.Length);
                        }
                    }
                    int uncompressedDataLength;
                    if (compressed)
                    {
                        uncompressedDataLength = dataLength;
                    } else
                    {
                        uncompressedDataLength = 0;
                    }
                    int totalPacketLength = DataUtils.MeasureVarInt(uncompressedDataLength) + data.Length;

                    relayToServerStream.WriteVarInt(totalPacketLength);
                    relayToServerStream.WriteVarInt(uncompressedDataLength);
                    relayToServerStream.Write(data, 0, data.Length);
                } else
                {
                    relayToServerStream.WriteVarInt(dataLength);
                    relayToServerStream.WriteVarInt(Packet.PacketID);
                    relayToServerStream.Write(Packet.Body, 0, Packet.Body.Length);
                }
            }
        }
        private void readFromClientLoop()
        {
            while (true)
            {
                //try
                //{
                    Packet packet = new Packet();
                    int length = relayToClientStream.ReadVarInt();
                    byte[] data = new byte[length];
                    relayToClientStream.Read(data, 0, length);
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        packet.PacketID = ms.ReadVarInt();
                        packet.Body = new byte[length - DataUtils.MeasureVarInt(packet.PacketID)];
                        ms.Read(packet.Body, 0, packet.Body.Length);
                    }
                    bool deliver = true;
                    foreach (PacketHandler handler in packetHandlers)
                    {
                        if (!handler.HandlePacket(packet, PacketDirection.FromClientToServer, this))
                        {
                            deliver = false;
                        }
                    }
                    if (deliver)
                    {
                        InsertPacket(packet, PacketDirection.FromClientToServer);
                    }
                /*} catch
                {
                    relayToClientStream.Close();
                    relayToServerStream.Close();
                    relayToServerNetStream.Close();
                    relayToClient.Close();
                    relayToServer.Close();
                    readFromServerThread.Abort();
                    Console.WriteLine("Disconnected");
                    return;
                }*/
            }
        }
        private void readFromServerLoop()
        {
            while (true)
            {
                //try
                //{
                    Packet packet = new Packet();
                    int length = relayToServerStream.ReadVarInt();
                    byte[] data = new byte[length];
                    relayToServerStream.Read(data, 0, length);
                    byte[] uncompressedData;
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        if (compressionEnabled)
                        {
                            int dataLength = ms.ReadVarInt();
                            if (dataLength == 0)
                            {
                                uncompressedData = new byte[length - DataUtils.MeasureVarInt(0)];
                                ms.Read(uncompressedData, 0, uncompressedData.Length);
                            }
                            else
                            {
                                uncompressedData = new byte[dataLength];
                                using (ZlibStream deflate = new ZlibStream(ms, Ionic.Zlib.CompressionMode.Decompress))
                                {
                                    deflate.Read(uncompressedData, 0, dataLength);
                                }
                            }
                        }
                        else
                        {
                            uncompressedData = new byte[length];
                            ms.Read(uncompressedData, 0, uncompressedData.Length);
                        }
                    }
                    using (MemoryStream ms = new MemoryStream(uncompressedData))
                    {
                        packet.PacketID = ms.ReadVarInt();
                        packet.Body = new byte[uncompressedData.Length - DataUtils.MeasureVarInt(packet.PacketID)];
                        ms.Read(packet.Body, 0, packet.Body.Length);
                    }
                    bool deliver = true;
                    foreach (PacketHandler handler in packetHandlers)
                    {
                        if (!handler.HandlePacket(packet, PacketDirection.FromServerToClient, this))
                        {
                            deliver = false;
                        }
                    }
                    if (deliver)
                    {
                        InsertPacket(packet, PacketDirection.FromServerToClient);
                    }
                //} catch
                /*{
                    relayToClientStream.Close();
                    relayToServerStream.Close();
                    relayToServerNetStream.Close();
                    relayToClient.Close();
                    relayToServer.Close();
                    Console.WriteLine("Disconnected");
                    readFromClientThread.Abort();
                    return;
                }*/
            }
        }
    }
}

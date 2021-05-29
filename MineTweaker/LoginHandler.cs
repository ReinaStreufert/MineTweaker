using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    class LoginHandler : PacketHandler
    {
        private string playerUsername;
        public override bool HandlePacket(Packet Packet, PacketDirection Direction, Relay Relay)
        {
            if (Relay.ConnectionState == ConnectionState.Handshake)
            {
                if ((ServerboundHandshakePackets)Packet.PacketID == ServerboundHandshakePackets.Handshake)
                {
                    PacketManipulators.Handshake manipulator = new PacketManipulators.Handshake();
                    manipulator.ParseFromBody(Packet.Body);
                    manipulator.ServerAddress = Relay.RemoteServerHostname;
                    manipulator.ServerPort = Relay.RemoteServerPort;
                    Packet.Body = manipulator.GeneratePacketBody();
                    Relay.ConnectionState = manipulator.NextState;
                    Console.WriteLine("Connected to Minecraft.");
                    return true;
                }
            } else if (Relay.ConnectionState == ConnectionState.Login)
            {
                if (Direction == PacketDirection.FromClientToServer)
                {
                    if ((ServerboundLoginPackets)Packet.PacketID == ServerboundLoginPackets.LoginStart)
                    {
                        PacketManipulators.LoginStart manipulator = new PacketManipulators.LoginStart();
                        manipulator.ParseFromBody(Packet.Body);
                        playerUsername = manipulator.PlayerUsername;
                        Console.WriteLine("The login process has started.");
                        return true;
                    }
                } else
                {
                    if ((ClientboundLoginPackets)Packet.PacketID == ClientboundLoginPackets.EncryptionRequest)
                    {
                        Console.WriteLine("Got encryption request, manipulating encryption...");
                        PacketManipulators.EncryptionRequest manipulator = new PacketManipulators.EncryptionRequest();
                        manipulator.ParseFromBody(Packet.Body);
                        byte[] sharedSecret = new byte[16];
                        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                        {
                            rng.GetBytes(sharedSecret);
                        }
                        string hash = CryptoUtils.MinecraftShaDigest(manipulator.ServerID, sharedSecret, manipulator.PublicKeyData);
                        UserData data = CryptoUtils.GetUserDataFromLauncher();
                        if (data.AccessToken == "")
                        {
                            Console.WriteLine("Failed to steal auth token from Minecraft launcher. Disconnecting.");
                            throw new Exception();
                        } else
                        {
                            Console.WriteLine("Successfully stole auth token from Minecraft launcher.");
                        }
                        JObject requestObject = new JObject();
                        requestObject.Add("accessToken", data.AccessToken);
                        requestObject.Add("selectedProfile", data.UUID);
                        requestObject.Add("serverId", hash);
                        byte[] requestData = Encoding.ASCII.GetBytes(requestObject.ToString());

                        HttpWebRequest request = HttpWebRequest.CreateHttp("https://sessionserver.mojang.com/session/minecraft/join");
                        request.ContentType = "application/json";
                        request.Method = "POST";
                        Stream stream = request.GetRequestStream();
                        stream.Write(requestData, 0, requestData.Length);
                        stream.Close();
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            Console.WriteLine("Successfully impresonated client and authenticated with Mojang.");
                        }

                        byte[] sharedSecretEnrypted;
                        byte[] verifyTokenEncrypted;
                        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
                        {
                            rsa.ImportParameters(CryptoUtils.ParamsFromDERKey(manipulator.PublicKeyData));
                            sharedSecretEnrypted = rsa.Encrypt(sharedSecret, false);
                            verifyTokenEncrypted = rsa.Encrypt(manipulator.VerifyToken, false);
                        }

                        PacketManipulators.EncryptionResponse encryptionResponse = new PacketManipulators.EncryptionResponse();
                        encryptionResponse.EncryptedSharedSecret = sharedSecretEnrypted;
                        encryptionResponse.EncryptedVerifyToken = verifyTokenEncrypted;

                        Packet responsePacket = new Packet();
                        responsePacket.PacketID = encryptionResponse.TargetPacketID;
                        responsePacket.Body = encryptionResponse.GeneratePacketBody();

                        Relay.InsertPacket(responsePacket, PacketDirection.FromClientToServer);

                        Relay.EnableEncryption(sharedSecret);
                        return false;
                    } else if ((ClientboundLoginPackets)Packet.PacketID == ClientboundLoginPackets.SetCompression)
                    {
                        PacketManipulators.SetCompression manipulator = new PacketManipulators.SetCompression();
                        manipulator.ParseFromBody(Packet.Body);
                        if (manipulator.Threshold > 0)
                        {
                            Console.WriteLine("Compression set with threshold of " + manipulator.Threshold);
                            Relay.SetCompression(true, manipulator.Threshold);
                        }
                        return false;
                    } else if ((ClientboundLoginPackets)Packet.PacketID == ClientboundLoginPackets.LoginSuccess)
                    {
                        Console.WriteLine("Login successful! MineTweaker has successfully acquired the encryption key.");
                        Relay.ConnectionState = ConnectionState.Play;
                        return true;
                    }
                }
            }
            return true;
        }
    }
}

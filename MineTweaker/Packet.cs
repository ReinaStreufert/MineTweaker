using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class Packet
    {
        public int PacketID;
        public byte[] Body;
    }
    public enum ServerboundHandshakePackets : int
    {
        Handshake = 0x00
    }
    public enum ClientboundStatusPackets : int
    {
        Response = 0x00,
        Pong = 0x01
    }
    public enum ServerboundStatusPackets : int
    {
        Request = 0x00,
        Ping = 0x01
    }
    public enum ClientboundLoginPackets : int
    {
        Disconnect = 0x00,
        EncryptionRequest = 0x01,
        LoginSuccess = 0x02,
        SetCompression = 0x03,
        LoginPluginRequest = 0x04
    }
    public enum ServerboundLoginPackets : int
    {
        LoginStart = 0x00,
        EncryptionResponse = 0x01,
        LoginPluginResponse = 0x02
    }
    public enum ClientboundPlayPackets : int
    {
        ChatMessage = 0x0E,
        JoinGame = 0x24,
        EntityPositionAndRotation = 0x28,
        EntityHeadLook = 0x3A,
        EntityTeleport = 0x56,
        EntityMetadata = 0x44,
        EntityEffect = 0x59,
        RemoveEntityEffect = 0x37,
        EntityProperties = 0x58,
        PlayerAbilities = 0x30
    }
    public enum ServerboundPlayPackets : int
    {
        ChatMessage = 0x03,
        PlayerMovement = 0x15,
        PlayerRotation = 0x14,
        PlayerPosition = 0x12,
        PlayerPositionAndRotation = 0x13,
        PlayerFlying = 0x1A
    }
}

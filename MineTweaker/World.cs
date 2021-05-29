using MineTweaker.PacketManipulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class World
    {
        public Relay Relay { get; set; }
        public int ClientEntityID { get; set; }
        public bool AlwaysOnGround { get; set; } = false;
        public bool NotifyServerOfFlying { get; set; } = true;
        public List<PacketCatcher> PacketCatchers { get; } = new List<PacketCatcher>();

        public void SetClientGamemode(Gamemode Gamemode)
        {
            ChangeGameState gamestate = new ChangeGameState();
            gamestate.ChangeReason = ChangeGameState.Reason.ChangeGamemode;
            gamestate.Argument = (int)Gamemode;

            Packet packet = new Packet();
            packet.PacketID = gamestate.TargetPacketID;
            packet.Body = gamestate.GeneratePacketBody();
            Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        public void SetClientAbilities(PlayerAbilities.Ability Abilities)
        {
            PlayerAbilities abilities = new PlayerAbilities();
            abilities.Abilities = Abilities;
            abilities.FlyingSpeed = 0.3F;
            abilities.FOV = 0.1F;

            Packet packet = new Packet();
            packet.PacketID = abilities.TargetPacketID;
            packet.Body = abilities.GeneratePacketBody();
            Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        public void SetClientAttribute(string Key, double Value)
        {
            EntityProperties entityProperties = new EntityProperties();
            entityProperties.EntityID = ClientEntityID;
            entityProperties.Attributes = new EntityProperties.AttributeEntry[1];
            entityProperties.Attributes[0] = new EntityProperties.AttributeEntry(Key, Value);
            Console.WriteLine(entityProperties.Attributes[0].Key);
            Console.WriteLine(entityProperties.Attributes[0].Value);

            Packet packet = new Packet();
            packet.PacketID = entityProperties.TargetPacketID;
            packet.Body = entityProperties.GeneratePacketBody();
            Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        public void GiveClientEffect(Effect Effect, byte Amplifier, int Duration = int.MaxValue, bool ShowParticles = false, bool ShowIcon = false)
        {
            EntityEffect entityEffect = new EntityEffect();
            entityEffect.EntityID = ClientEntityID;
            entityEffect.Effect = Effect;
            entityEffect.Amplifier = Amplifier;
            entityEffect.Duration = Duration;
            entityEffect.Flags = 0;
            if (ShowParticles)
            {
                entityEffect.Flags = entityEffect.Flags | EntityEffect.EffectFlags.ShowParticles;
            }
            if (ShowIcon)
            {
                entityEffect.Flags = entityEffect.Flags | EntityEffect.EffectFlags.ShowIcon;
            }
            Packet packet = new Packet();
            packet.PacketID = entityEffect.TargetPacketID;
            packet.Body = entityEffect.GeneratePacketBody();
            Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        public void RemoveClientEffect(Effect Effect)
        {
            RemoveEntityEffect removeEntityEffect = new RemoveEntityEffect();
            removeEntityEffect.Effect = Effect;
            removeEntityEffect.EntityID = ClientEntityID;

            Packet packet = new Packet();
            packet.PacketID = removeEntityEffect.TargetPacketID;
            packet.Body = removeEntityEffect.GeneratePacketBody();

            Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        public void SendClientMessage(Chat Chat)
        {
            ClientboundChatMessage message = new ClientboundChatMessage();
            message.Message = Chat;
            Packet packet = new Packet();
            packet.PacketID = message.TargetPacketID;
            packet.Body = message.GeneratePacketBody();
            Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
    }
}

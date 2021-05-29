using MineTweaker.PacketManipulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    class Tweaker : PacketHandler
    {
        private List<TweakerCommand> commands = new List<TweakerCommand>();
        private World world = null;
        public void RegisterCommand(TweakerCommand Command)
        {
            string keyword = Command.Keyword;
            if (keyword.Contains(" "))
            {
                throw new ArgumentException("Command had invalid keyword");
            }
            foreach (TweakerCommand checkCommand in commands)
            {
                if (checkCommand.Keyword == keyword)
                {
                    throw new ArgumentException("There is already a command registered with that keyword");
                }
            }
            commands.Add(Command);
        }
        public override bool HandlePacket(Packet Packet, PacketDirection Direction, Relay Relay)
        {
            if (world == null)
            {
                world = new World();
                world.Relay = Relay;
            }
            if (Relay.ConnectionState == ConnectionState.Play)
            {
                foreach (PacketCatcher catcher in world.PacketCatchers)
                {
                    if (catcher.TargetID == Packet.PacketID && catcher.Direction == Direction)
                    {
                        catcher.Callback(Packet);
                    }
                }
                if (Direction == PacketDirection.FromClientToServer)
                {
                    if ((ServerboundPlayPackets)Packet.PacketID == ServerboundPlayPackets.ChatMessage)
                    {
                        ServerboundChatMessage manipulator = new ServerboundChatMessage();
                        manipulator.ParseFromBody(Packet.Body);
                        if (manipulator.Message.Length > 1 && manipulator.Message[0] == '\\')
                        {
                            string cmd = manipulator.Message.Substring(1);
                            string[] split = cmd.Split(' ');
                            string keyword = split[0];
                            string[] args = split.Skip(1).ToArray();
                            foreach (TweakerCommand tCommand in commands)
                            {
                                if (tCommand.Keyword == keyword)
                                {
                                    tCommand.Execute(world, args);
                                    break;
                                }
                            }
                            return false;
                        }
                        return true;
                    } else if ((ServerboundPlayPackets)Packet.PacketID == ServerboundPlayPackets.PlayerMovement && world.AlwaysOnGround)
                    {
                        PlayerMovement manipulator = new PlayerMovement();
                        manipulator.ParseFromBody(Packet.Body);
                        if (!manipulator.OnGround)
                        {
                            manipulator.OnGround = true;
                            Packet.Body = manipulator.GeneratePacketBody();
                        }
                    } else if ((ServerboundPlayPackets)Packet.PacketID == ServerboundPlayPackets.PlayerRotation && world.AlwaysOnGround)
                    {
                        PlayerRotation manipulator = new PlayerRotation();
                        manipulator.ParseFromBody(Packet.Body);
                        if (!manipulator.OnGround)
                        {
                            manipulator.OnGround = true;
                            Packet.Body = manipulator.GeneratePacketBody();
                        }
                    } else if ((ServerboundPlayPackets)Packet.PacketID == ServerboundPlayPackets.PlayerPosition && world.AlwaysOnGround)
                    {
                        PlayerPosition manipulator = new PlayerPosition();
                        manipulator.ParseFromBody(Packet.Body);
                        if (!manipulator.OnGround)
                        {
                            manipulator.OnGround = true;
                            Packet.Body = manipulator.GeneratePacketBody();
                        }
                    } else if ((ServerboundPlayPackets)Packet.PacketID == ServerboundPlayPackets.PlayerPositionAndRotation && world.AlwaysOnGround)
                    {
                        PlayerPositionAndRotation manipulator = new PlayerPositionAndRotation();
                        manipulator.ParseFromBody(Packet.Body);
                        if (!manipulator.OnGround)
                        {
                            manipulator.OnGround = true;
                            Packet.Body = manipulator.GeneratePacketBody();
                        }
                    } else if ((ServerboundPlayPackets)Packet.PacketID == ServerboundPlayPackets.PlayerFlying && !world.NotifyServerOfFlying)
                    {
                        return false;
                    }
                } else if (Direction == PacketDirection.FromServerToClient)
                {
                    if ((ClientboundPlayPackets)Packet.PacketID == ClientboundPlayPackets.JoinGame)
                    {
                        JoinGame manipulator = new JoinGame();
                        manipulator.ParseFromBody(Packet.Body);
                        Console.WriteLine("Client EID: " + manipulator.PlayerEntityID);
                        Console.WriteLine("Client Game mode: " + manipulator.Gamemode);
                        world.ClientEntityID = manipulator.PlayerEntityID;
                        Console.WriteLine();
                    } else if ((ClientboundPlayPackets)Packet.PacketID == ClientboundPlayPackets.EntityEffect)
                    {
                        /*EntityEffect manipulator = new EntityEffect();
                        manipulator.ParseFromBody(Packet.Body);
                        Console.WriteLine("Entity effect");
                        Console.WriteLine("EID: " + manipulator.EntityID);
                        Console.WriteLine("Effect ID: " + manipulator.Effect);
                        Console.WriteLine("Amplifier: " + manipulator.Amplifier);
                        Console.WriteLine("Duration: " + manipulator.Duration);
                        if (manipulator.Flags.HasFlag(EntityEffect.EffectFlags.IsAmbient))
                        {
                            Console.Write("[Ambient] ");
                        }
                        if (manipulator.Flags.HasFlag(EntityEffect.EffectFlags.ShowParticles))
                        {
                            Console.Write("[Show particles] ");
                        }
                        if (manipulator.Flags.HasFlag(EntityEffect.EffectFlags.ShowIcon))
                        {
                            Console.Write("[Show icon] ");
                        }
                        Console.WriteLine();
                        Console.WriteLine();*/
                    } else if ((ClientboundPlayPackets)Packet.PacketID == ClientboundPlayPackets.EntityProperties)
                    {
                        /*EntityProperties manipulator = new EntityProperties();
                        manipulator.ParseFromBody(Packet.Body);
                        if (manipulator.EntityID == world.ClientEntityID)
                        {
                            Console.WriteLine("Entity properties");
                            Console.WriteLine("EID: " + manipulator.EntityID);
                            Console.WriteLine("Attributes:");
                            for (int i = 0; i < manipulator.Attributes.Length; i++)
                            {
                                EntityProperties.AttributeEntry attribute = manipulator.Attributes[i];
                                Console.WriteLine("\tKey: " + attribute.Key);
                                Console.WriteLine("\tValue: " + attribute.Value);
                                Console.WriteLine("\tModifiers: " + attribute.Value);
                                if (attribute.Modifiers.Length == 0)
                                {
                                    Console.WriteLine("\t\tNone");
                                }
                                else
                                {
                                    for (int ii = 0; ii < attribute.Modifiers.Length; ii++)
                                    {
                                        EntityProperties.AttributeModifier modifier = attribute.Modifiers[ii];
                                        Console.WriteLine("\t\tUUID: " + modifier.UUID.ToString());
                                        Console.WriteLine("\t\tAmount: " + modifier.Amount);
                                        Console.WriteLine("\t\tOperation: " + modifier.Operation);
                                        Console.WriteLine("\t\t----");
                                    }
                                }
                                Console.WriteLine("\t----");
                            }
                            Console.WriteLine();
                        }*/
                    }
                }
            }
            
            return true;
        }
    }
}

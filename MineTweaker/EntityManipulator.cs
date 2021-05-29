using MineTweaker.PacketManipulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class EntityManipulator : IDisposable
    {
        [Flags]
        protected enum BaseFlags : byte
        {
            IsOnFire = 0x01,
            IsCrouching = 0x02,
            Unused = 0x04,
            IsSprinting = 0x08,
            IsSwimming = 0x10,
            IsInvisible = 0x20,
            IsGlowing = 0x40,
            IsFlyingWithElytra = 0x080
        }
        protected Entity ManipulatingEntity;
        public EntityManipulator(Entity Entity)
        {
            this.ManipulatingEntity = Entity;
            ManipulatingEntity.OnServersidePositionUpdate += ManipulatingEntity_OnServersidePositionUpdate;
            ManipulatingEntity.OnServersideMetadataUpdate += ManipulatingEntity_OnServersideMetadataUpdate;
        }
        private void ManipulatingEntity_OnServersideMetadataUpdate(Entity.ServersideMetadataUpdateInfo Info)
        {
            if (Info.FieldID == 0)
            {
                byte[] fromServerData = Info.NewServersideValue;
                byte[] currentClientData = ManipulatingEntity.GetClientsideMetadata(0);
                BaseFlags fromServer;
                BaseFlags currentClient;
                if (fromServerData == null || fromServerData.Length != 1)
                {
                    fromServer = 0;
                } else
                {
                    fromServer = (BaseFlags)fromServerData[0];
                }
                if (currentClientData == null || currentClientData.Length != 1)
                {
                    currentClient = 0;
                }
                else
                {
                    currentClient = (BaseFlags)currentClientData[0];
                }
                bool fire;
                bool glow;
                if (onFireSynchronized)
                {
                    fire = fromServer.HasFlag(BaseFlags.IsOnFire);
                } else
                {
                    fire = currentClient.HasFlag(BaseFlags.IsOnFire);
                }
                if (isGlowingSynchronized)
                {
                    glow = fromServer.HasFlag(BaseFlags.IsGlowing);
                } else
                {
                    glow = currentClient.HasFlag(BaseFlags.IsOnFire);
                }

                if (fromServer.HasFlag(BaseFlags.IsOnFire) && !fire)
                {
                    fromServer = fromServer ^ BaseFlags.IsOnFire;
                } else if (!fromServer.HasFlag(BaseFlags.IsOnFire) && fire)
                {
                    fromServer = fromServer | BaseFlags.IsOnFire;
                }

                if (fromServer.HasFlag(BaseFlags.IsGlowing) && !glow)
                {
                    fromServer = fromServer ^ BaseFlags.IsGlowing;
                }
                else if (!fromServer.HasFlag(BaseFlags.IsGlowing) && glow)
                {
                    fromServer = fromServer | BaseFlags.IsGlowing;
                }
                Info.NewServersideValue = new byte[1] { (byte)fromServer };
            }
        }
        private void ManipulatingEntity_OnServersidePositionUpdate(Entity.ServersidePositionUpdateInfo Info)
        {
            if (!positionSynchronized)
            {
                Info.NotifyClient = false;
            }
        }
        public void Dispose()
        {
            ManipulatingEntity.OnServersidePositionUpdate -= ManipulatingEntity_OnServersidePositionUpdate;
            ManipulatingEntity.OnServersideMetadataUpdate -= ManipulatingEntity_OnServersideMetadataUpdate;
        }

        protected bool positionSynchronized = true;
        protected bool onFireSynchronized = true;
        //protected bool isInvisibleSyncrhonized = true; // idc rn lmao
        protected bool isGlowingSynchronized = true;

        public virtual void SynchronizeProperty<T>(Synchronizable<T> Property)
        {
            Type propertyType = Property.GetType();
            if ((object)Property == ClientsidePosition)
            {
                ClientsidePosition = ServersidePosition;
                positionSynchronized = true;
            } else if ((object)Property == ClientsideOnFire)
            {
                ClientsideOnFire = ServersideOnFire;
                onFireSynchronized = true;
            } else if ((object)Property == ClientsideIsGlowing)
            {
                ClientsideIsGlowing = ServersideIsGlowing;
                isGlowingSynchronized = true;
            }
        }
        public void GiveEffect(Effect Effect, byte Amplifier, int Duration = int.MaxValue, bool ShowParticles = false, bool ShowIcon = false)
        {
            EntityEffect manipulator = new EntityEffect();
            manipulator.Effect = Effect;
            manipulator.EntityID = ManipulatingEntity.EntityID;
            manipulator.Amplifier = Amplifier;
            manipulator.Duration = Duration;
            manipulator.Flags = 0;
            if (ShowParticles)
            {
                manipulator.Flags = manipulator.Flags | EntityEffect.EffectFlags.ShowParticles;
            }
            if (ShowIcon)
            {
                manipulator.Flags = manipulator.Flags | EntityEffect.EffectFlags.ShowIcon;
            }

            Packet packet = new Packet();
            packet.PacketID = manipulator.TargetPacketID;
            packet.Body = manipulator.GeneratePacketBody();

            ManipulatingEntity.Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        public void RemoveEffect(Effect Effect)
        {
            RemoveEntityEffect manipulator = new RemoveEntityEffect();
            manipulator.Effect = Effect;
            manipulator.EntityID = ManipulatingEntity.EntityID;

            Packet packet = new Packet();
            packet.PacketID = manipulator.TargetPacketID;
            packet.Body = manipulator.GeneratePacketBody();

            ManipulatingEntity.Relay.InsertPacket(packet, PacketDirection.FromServerToClient);
        }
        
        protected void SetClientsideMetadataValue(int FieldID, int Type, byte[] Data)
        {
            EntityMetadata manipulator = new EntityMetadata();
            manipulator.EntityID = ManipulatingEntity.EntityID;
            manipulator.ChangedMetadata = new EntityMetadata.MetadataEntry[1];
            EntityMetadata.MetadataEntry entry = new EntityMetadata.MetadataEntry();
            entry.TypeID = Type;
            entry.FieldID = FieldID;
            entry.Data = Data;
            manipulator.ChangedMetadata[0] = entry;

            Packet packet = new Packet();
            packet.PacketID = manipulator.TargetPacketID;
            packet.Body = manipulator.GeneratePacketBody();

            ManipulatingEntity.Relay.InsertPacket(packet, PacketDirection.FromServerToClient);

            ManipulatingEntity.NotifyClientsideMetadataChange(FieldID, Data);
        }

        public EntityPosition ServersidePosition
        {
            get
            {
                return ManipulatingEntity.ServersidePosition;
            }
        }
        public Synchronizable<EntityPosition> ClientsidePosition
        {
            get
            {
                return ManipulatingEntity.ClientsidePosition;
            }
            set
            {
                positionSynchronized = false;
                EntityPosition newValue = value;
                EntityPosition oldValue = ManipulatingEntity.ClientsidePosition;
                if (Math.Abs(newValue.Location.X - oldValue.Location.X) > 8 || Math.Abs(newValue.Location.Y - oldValue.Location.Y) > 8 || Math.Abs(newValue.Location.Z - oldValue.Location.Z) > 8)
                {
                    EntityTeleport entityTeleport = new EntityTeleport();
                    entityTeleport.EntityID = ManipulatingEntity.EntityID;
                    entityTeleport.Position = newValue;

                    Packet entityTeleportPacket = new Packet();
                    entityTeleportPacket.PacketID = entityTeleport.TargetPacketID;
                    entityTeleportPacket.Body = entityTeleport.GeneratePacketBody();
                    ManipulatingEntity.Relay.InsertPacket(entityTeleportPacket, PacketDirection.FromServerToClient);
                } else
                {
                    EntityPositionAndRotation relativeMove = new EntityPositionAndRotation();
                    relativeMove.EntityID = ManipulatingEntity.EntityID;
                    relativeMove.NewPosition = newValue;
                    relativeMove.OldPosition = oldValue;

                    Packet relativeMovePacket = new Packet();
                    relativeMovePacket.PacketID = relativeMove.TargetPacketID;
                    relativeMovePacket.Body = relativeMove.GeneratePacketBody();
                    ManipulatingEntity.Relay.InsertPacket(relativeMovePacket, PacketDirection.FromServerToClient);
                }
                if (newValue.HeadYaw.Steps != oldValue.HeadYaw.Steps)
                {
                    EntityHeadLook headLook = new EntityHeadLook();
                    headLook.EntityID = ManipulatingEntity.EntityID;
                    headLook.HeadYaw = newValue.HeadYaw;

                    Packet headLookPacket = new Packet();
                    headLookPacket.PacketID = headLook.TargetPacketID;
                    headLookPacket.Body = headLook.GeneratePacketBody();
                    ManipulatingEntity.Relay.InsertPacket(headLookPacket, PacketDirection.FromServerToClient);
                }

                ManipulatingEntity.NotifyClientsidePositionChange(newValue);
            }
        }
        public bool ServersideOnFire
        {
            get
            {
                byte[] data = ManipulatingEntity.GetServersideMetadata(0);
                if (data == null || data.Length != 1)
                {
                    return false;
                } else
                {
                    BaseFlags flags = (BaseFlags)data[0];
                    return flags.HasFlag(BaseFlags.IsOnFire);
                }
            }
        }
        public Synchronizable<bool> ClientsideOnFire
        {
            get
            {
                byte[] data = ManipulatingEntity.GetClientsideMetadata(0);
                if (data == null || data.Length != 1)
                {
                    return false;
                }
                else
                {
                    BaseFlags flags = (BaseFlags)data[0];
                    return flags.HasFlag(BaseFlags.IsOnFire);
                }
            }
            set
            {
                onFireSynchronized = false;
                byte[] data = ManipulatingEntity.GetClientsideMetadata(0);
                BaseFlags flags;
                if (data == null || data.Length != 1)
                {
                    flags = 0;
                } else
                {
                    flags = (BaseFlags)data[0];
                }
                if (value)
                {
                    flags = flags | BaseFlags.IsOnFire;
                } else if (!value && flags.HasFlag(BaseFlags.IsOnFire))
                {
                    flags = flags ^ BaseFlags.IsOnFire;
                }

                SetClientsideMetadataValue(0, 0, new byte[1] { (byte)flags });
            }
        }
        public bool ServersideIsGlowing
        {
            get
            {
                byte[] data = ManipulatingEntity.GetServersideMetadata(0);
                if (data == null || data.Length != 1)
                {
                    return false;
                }
                else
                {
                    BaseFlags flags = (BaseFlags)data[0];
                    return flags.HasFlag(BaseFlags.IsGlowing);
                }
            }
        }
        public Synchronizable<bool> ClientsideIsGlowing
        {
            get
            {
                byte[] data = ManipulatingEntity.GetClientsideMetadata(0);
                if (data == null || data.Length != 1)
                {
                    return false;
                }
                else
                {
                    BaseFlags flags = (BaseFlags)data[0];
                    return flags.HasFlag(BaseFlags.IsGlowing);
                }
            }
            set
            {
                isGlowingSynchronized = false;
                byte[] data = ManipulatingEntity.GetClientsideMetadata(0);
                BaseFlags flags;
                if (data == null || data.Length != 1)
                {
                    flags = 0;
                }
                else
                {
                    flags = (BaseFlags)data[0];
                }
                if (value)
                {
                    flags = flags | BaseFlags.IsGlowing;
                }
                else if (!value && flags.HasFlag(BaseFlags.IsGlowing))
                {
                    flags = flags ^ BaseFlags.IsGlowing;
                }

                SetClientsideMetadataValue(0, 0, new byte[1] { (byte)flags });
            }
        }
    }
    public class Synchronizable<T>
    {
        private T value;
        public static implicit operator T(Synchronizable<T> s)
        {
            return s.value;
        }
        public static implicit operator Synchronizable<T>(T v)
        {
            Synchronizable<T> result = new Synchronizable<T>();
            result.value = v;
            return result;
        }
    }
}

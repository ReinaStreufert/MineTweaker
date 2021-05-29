using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class Entity
    {
        public class ServersidePositionUpdateInfo
        {
            public EntityPosition OldServersidePosition { get; private set; }
            public EntityPosition NewServersidePosition { get; set; }
            public bool NotifyClient { get; set; } = true;
            public ServersidePositionUpdateInfo(EntityPosition Old)
            {
                OldServersidePosition = Old;
            }
        }
        public delegate void ServersidePositionUpdateEvent(ServersidePositionUpdateInfo Info);
        public class ServersideMetadataUpdateInfo
        {
            public int FieldID { get; private set; }
            public byte[] OldServersideValue { get; private set; }
            public byte[] NewServersideValue { get; set; }
            public bool NotifyClient { get; set; } = true;
            public ServersideMetadataUpdateInfo(int ID, byte[] Old)
            {
                FieldID = ID;
                OldServersideValue = Old;
            }
        }
        public delegate void ServersideMetadataUpdateEvent(ServersideMetadataUpdateInfo Info);

        public event ServersidePositionUpdateEvent OnServersidePositionUpdate;
        public event ServersideMetadataUpdateEvent OnServersideMetadataUpdate;

        public int EntityID { get; private set; }
        public EntityType EntityType { get; private set; }
        public Relay Relay { get; private set; }

        public Entity(int ID, EntityType Type, Relay Relay) // Creates a representation of an entity that already exists serverside
        {
            EntityID = ID;
            EntityType = Type;
            this.Relay = Relay;
        }
        private EntityPosition serversidePosition;
        private EntityPosition clientsidePosition;

        public EntityPosition ServersidePosition
        {
            get
            {
                return serversidePosition;
            }
        }
        public EntityPosition ClientsidePosition
        {
            get
            {
                return clientsidePosition;
            }
        }
        private byte[][] serversideMetadataTable = new byte[1][];
        private byte[][] clientsideMetadataTable = new byte[1][];

        public void NotifyClientsidePositionChange(EntityPosition NewPosition)
        {
            clientsidePosition = NewPosition;
        }
        public void NotifyClientsideMetadataChange(int FieldID, byte[] Data)
        {
            if (FieldID >= clientsideMetadataTable.Length)
            {
                Array.Resize(ref clientsideMetadataTable, FieldID + 1);
            }
            clientsideMetadataTable[FieldID] = Data;
        }
        public EntityPosition NotifyServersidePositionChange(EntityPosition NewPosition, out bool NotifyClient)
        {
            ServersidePositionUpdateInfo info = new ServersidePositionUpdateInfo(serversidePosition);
            info.NewServersidePosition = NewPosition;
            info.NotifyClient = true;

            OnServersidePositionUpdate?.Invoke(info);
            NotifyClient = info.NotifyClient;
            if (info.NotifyClient)
            {
                NotifyClientsidePositionChange(info.NewServersidePosition);
            }
            serversidePosition = NewPosition;

            return info.NewServersidePosition;
        }
        public byte[] NotifyServersideMetadataChange(int FieldID, byte[] Data, out bool NotifyClient)
        {
            ServersideMetadataUpdateInfo info = new ServersideMetadataUpdateInfo(FieldID, GetServersideMetadata(FieldID));
            info.NewServersideValue = Data;
            info.NotifyClient = true;

            OnServersideMetadataUpdate?.Invoke(info);
            NotifyClient = info.NotifyClient;
            if (info.NotifyClient)
            {
                NotifyClientsideMetadataChange(FieldID, info.NewServersideValue);
            }
            if (FieldID >= serversideMetadataTable.Length)
            {
                Array.Resize(ref serversideMetadataTable, FieldID + 1);
            }
            serversideMetadataTable[FieldID] = Data;

            return info.NewServersideValue;
        }
        public byte[] GetServersideMetadata(int FieldID)
        {
            if (FieldID >= serversideMetadataTable.Length)
            {
                return null;
            } else
            {
                return serversideMetadataTable[FieldID];
            }
        }
        public byte[] GetClientsideMetadata(int FieldID)
        {
            if (FieldID >= clientsideMetadataTable.Length)
            {
                return null;
            }
            else
            {
                return clientsideMetadataTable[FieldID];
            }
        }
    }
    public enum EntityType : int
    {
        AreaEffectCloud = 0,
        ArmorStand = 1,
        Arrow = 2,
        Bat = 3,
        Bee = 4,
        Blaze = 5,
        Boat = 6,
        Cat = 7,
        CaveSpider = 8,
        Chicken = 9,
        Cod = 10,
        Cow = 11,
        Creeper = 12,
        Dolphin = 13,
        Donkey = 14,
        DragonFireball = 15,
        Drowned = 16,
        ElderGuardian = 17,
        EndCrystal = 18,
        EnderDragon = 19,
        Enderman = 20,
        Endermite = 21,
        Evoker = 22,
        EvokerFangs = 23,
        ExperienceOrb = 24,
        EyeOfEnder = 24,
        FallingBlock = 26,
        FireworkRocketEntity = 27,
        Fox = 28,
        Ghast = 29,
        Giant = 30,
        Guardian = 31,
        Hoglin = 32,
        Horse = 33,
        Husk = 34,
        Illusioner = 35,
        IronGolem = 36,
        Item = 37,
        ItemFrame = 38,
        Fireball = 39,
        LeashKnot = 40,
        LightningBolt = 41,
        Llama = 42,
        LlamaSpit = 43,
        MagmaCube = 44,
        Minecart = 45,
        MinecartChest = 46,
        MinecartCommandBlock = 47,
        MinecartFurnace = 48,
        MinecartHopper = 49,
        MinecartSpawner = 50,
        MinecartTNT = 51,
        Mule = 52,
        Mushroom = 53,
        Ocelot = 54,
        Painting = 55,
        Panda = 56,
        Parrot = 57,
        Phantom = 58,
        Pig = 59,
        Piglin = 60,
        PiglinBrute = 61,
        Pillager = 62,
        PolarBear = 63,
        PrimedTNT = 64,
        Pufferfish = 65,
        Rabbit = 66,
        Ravager = 67,
        Salmon = 68,
        Sheep = 69,
        Shulker = 70,
        ShulkerBullet = 71,
        Silverfish = 72,
        Skeleton = 73,
        SkeletonHorse = 74,
        Slime = 75,
        SmallFireball = 76,
        SnowGolem = 77,
        Snowball = 78,
        SpectralArrow = 79,
        Spider = 80,
        Squid = 81,
        Stray = 82,
        Strider = 83,
        ThrownEgg = 84,
        ThrownEnderPearl = 85,
        ThrownExpierienceBottle = 86,
        ThrownPotion = 87,
        ThrownTrident = 88,
        TraderLlama = 89,
        TropicalFish = 90,
        Turtle = 91,
        Vex = 92,
        Villager = 93,
        Vindicator = 94,
        WanderingTrader = 95,
        Witch = 96,
        Wither = 97,
        WitherSkeleton = 98,
        WitherSkull = 99,
        Wolf = 100,
        Zoglin = 101,
        Zombie = 102,
        ZombieHorse = 103,
        ZombieVillager = 104,
        ZombifiedPiglin = 105,
        Player = 106,
        FishingHook = 107
    }
}

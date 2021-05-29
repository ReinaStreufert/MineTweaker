using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.PacketManipulators
{
    public class EntityProperties : PacketManipulator
    {
        public int EntityID;
        public AttributeEntry[] Attributes;
        public override int TargetPacketID
        {
            get
            {
                return 0x58;
            }
        }

        public override byte[] GeneratePacketBody()
        {
            int len = DataUtils.MeasureVarInt(EntityID) + 4;
            for (int i = 0; i < Attributes.Length; i++)
            {
                len += DataUtils.MeasureString(Attributes[i].Key) + 8 + DataUtils.MeasureVarInt(0);
            }
            byte[] buf = new byte[len];
            using (MemoryStream ms = new MemoryStream(buf))
            {
                ms.WriteVarInt(EntityID);
                ms.WriteNum((int)Attributes.Length);
                for (int i = 0; i < Attributes.Length; i++)
                {
                    ms.WriteString(Attributes[i].Key, 32767);
                    ms.WriteNum((double)Attributes[i].Value);
                    ms.WriteVarInt(Attributes[i].Modifiers.Length);
                    for (int ii = 0; ii < Attributes[0].Modifiers.Length; ii++)
                    {
                        AttributeModifier modifier = Attributes[i].Modifiers[ii];
                        ms.WriteUUID(modifier.UUID);
                        ms.WriteNum((double)modifier.Amount);
                        ms.WriteByte((byte)modifier.Operation);
                    }
                }
            }
            return buf;
        }

        public override void ParseFromBody(byte[] PacketBody)
        {
            using (MemoryStream ms = new MemoryStream(PacketBody))
            {
                EntityID = ms.ReadVarInt();
                Attributes = new AttributeEntry[ms.ReadInt()];
                for (int i = 0; i < Attributes.Length; i++)
                {
                    Attributes[i].Key = ms.ReadString(32767);
                    Attributes[i].Value = ms.ReadDouble();
                    Attributes[i].Modifiers = new AttributeModifier[ms.ReadVarInt()];
                    for (int ii = 0; ii < Attributes[i].Modifiers.Length; ii++)
                    {
                        Attributes[i].Modifiers[ii].UUID = ms.ReadUUID();
                        Attributes[i].Modifiers[ii].Amount = ms.ReadDouble();
                        Attributes[i].Modifiers[ii].Operation = (ModifierOperation)ms.ReadByte();
                    }
                }
            }
        }
        public struct AttributeEntry
        {
            public string Key;
            public double Value;
            public AttributeModifier[] Modifiers;
            public AttributeEntry(string Key, double Value)
            {
                this.Key = Key;
                this.Value = Value;
                Modifiers = new AttributeModifier[0];
            }
        }
        public struct AttributeModifier
        {
            public Guid UUID;
            public double Amount;
            public ModifierOperation Operation;
        }
    }
    
    public static class EntityPropertyKeys
    {
        public const string Generic_MaxHealth = "minecraft:minecraft:generic.max_health";
        public const string Generic_FollowRange = "minecraft:generic.follow_range";
        public const string Generic_KnockbackResistance = "minecraft:generic.knockback_resistance";
        public const string Generic_MovementSpeed = "minecraft:generic.movement_speed";
        public const string Generic_AttackDamage = "minecraft:generic.attack_damage";
        public const string Generic_AttackSpeed = "minecraft:generic.attack_speed";
        public const string Generic_FlyingSpeed = "minecraft:generic.flying_speed";
        public const string Generic_Armor = "minecraft:generic.armor";
        public const string Generic_ArmorToughness = "minecraft:generic.armor_toughness";
        public const string Generic_AttackKnockback = "minecraft:generic.attack_knockback";
        public const string Generic_Luck = "minecraft:generic.luck";
        public const string Horse_JumpStrength = "minecraft:horse.jump_strength";
        public const string Zombie_SpawnReinforcements = "minecraft:zombie.spawn_reinforcements";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public enum PacketDirection : byte
    {
        FromClientToServer = 0,
        FromServerToClient = 1
    }
    public enum ConnectionState : byte
    {
        NotConnected = 0,
        Handshake = 1,
        Status = 2,
        Login = 3,
        Play = 4
    }
    public enum Gamemode : byte
    {
        Survival = 0,
        Creative = 1,
        Adventure = 2,
        Spectator = 3,
        None = unchecked((byte)-1)
    }
    public enum Effect
    {
        Speed = 1,
        Slowness = 2,
        Haste = 3,
        MiningFatigue = 4,
        Strength = 5,
        InstantHealth = 6,
        InstantDamage = 7,
        JumpBoost = 8,
        Nausea = 9,
        Regeneration = 10,
        Resistance = 11,
        FireResistance = 12,
        WaterBreathing = 13,
        Invisibility = 14,
        Blindness = 15,
        NightVision = 16,
        Hunger = 17,
        Weakness = 18,
        Poision = 19,
        Wither = 20,
        HealthBoost = 21,
        Absorption = 22,
        Saturation = 23,
        Glowing = 24,
        Levitation = 25,
        Luck = 26,
        Unluck = 27,
        SlowFalling = 28,
        ConduitPower = 29,
        DolphinsGrace = 30,
        BadOmen = 31,
        HeroOfTheVillage = 32
    }
    public enum ModifierOperation : byte
    {
        Add = 0,
        AddPercent = 1,
        MultiplyPercent = 2
    }
}

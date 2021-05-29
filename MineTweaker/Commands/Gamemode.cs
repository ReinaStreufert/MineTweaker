using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class Gamemode : TweakerCommand
    {
        public override string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Keyword
        {
            get
            {
                return "gamemode";
            }
        }

        public override void Execute(World World, string[] Args)
        {
            if (Args.Length == 1)
            {
                if (Args[0] == "survival")
                {
                    World.SetClientGamemode(MineTweaker.Gamemode.Survival);
                } else if (Args[0] == "creative")
                {
                    World.SetClientGamemode(MineTweaker.Gamemode.Creative);
                } else if (Args[0] == "adventure")
                {
                    World.SetClientGamemode(MineTweaker.Gamemode.Adventure);
                } else if (Args[0] == "spectator")
                {
                    World.SetClientGamemode(MineTweaker.Gamemode.Spectator);
                } else
                {
                    World.SendClientMessage(new Chat("Invalid arguments."));
                }
            } else
            {
                World.SendClientMessage(new Chat("Invalid arguments."));
            }
        }
    }
}

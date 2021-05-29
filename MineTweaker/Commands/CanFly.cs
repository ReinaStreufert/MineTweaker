using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class CanFly : TweakerCommand
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
                return "canfly";
            }
        }

        public override void Execute(World World, string[] Args)
        {
            if (Args.Length == 1)
            {
                if (Args[0] == "true")
                {
                    World.SetClientAbilities(PacketManipulators.PlayerAbilities.Ability.AllowFlying);
                    if (!World.AlwaysOnGround)
                    {
                        World.SendClientMessage(new Chat("Flying enabled. Please use \"\\falldamage false\" before flying to stop anticheat."));
                    } else
                    {
                        World.SendClientMessage(new Chat("Flying enabled."));
                    }
                } else if (Args[0] == "false")
                {
                    World.SetClientAbilities(0);
                    World.SendClientMessage(new Chat("Flying disabled."));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class FallDamage : TweakerCommand
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
                return "falldamage";
            }
        }

        public override void Execute(World World, string[] Args)
        {
            if (Args.Length == 1)
            {
                if (Args[0] == "true")
                {
                    World.AlwaysOnGround = false;
                    World.SendClientMessage(new Chat("Fall damage set to true"));
                } else if (Args[0] == "false")
                {
                    World.AlwaysOnGround = true;
                    World.SendClientMessage(new Chat("Fall damage set to false"));
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

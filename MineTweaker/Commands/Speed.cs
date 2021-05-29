using MineTweaker.PacketManipulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class Speed : TweakerCommand
    {
        public bool affectApplied = false;
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
                return "speed";
            }
        }

        public override void Execute(World World, string[] Args)
        {
            if (Args.Length == 1)
            {
                if (Args[0] == "default")
                {
                    World.SetClientAttribute(EntityPropertyKeys.Generic_MovementSpeed, 0.1D);
                    World.SendClientMessage(new Chat("Your speed was reset."));
                    return;
                }
                double val;
                if (double.TryParse(Args[0], out val))
                {
                    World.SetClientAttribute(EntityPropertyKeys.Generic_MovementSpeed, val / 10D);
                    World.SendClientMessage(new Chat("New speed applied."));
                    return;
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

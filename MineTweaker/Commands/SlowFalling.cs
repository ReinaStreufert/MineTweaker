using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class SlowFalling : TweakerCommand
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
                return "slowfalling";
            }
        }

        public override void Execute(World World, string[] Args)
        {
            byte val;
            if (Args.Length == 1 && byte.TryParse(Args[0], out val))
            {
                if (val == 0)
                {
                    World.RemoveClientEffect(Effect.SlowFalling);
                    World.SendClientMessage(new Chat("Slow falling removed."));
                }
                else
                {
                    World.GiveClientEffect(Effect.SlowFalling, (byte)(val - 1));
                    World.SendClientMessage(new Chat("Slow falling given."));
                }
            }
            else
            {
                World.SendClientMessage(new Chat("Invalid args."));
            }
        }
    }
}

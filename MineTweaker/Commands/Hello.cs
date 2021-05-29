using MineTweaker.PacketManipulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class Hello : TweakerCommand
    {
        public override string Description
        {
            get
            {
                return "Responds with \"world:)\", can be used to check that MineTweaker is working properly (although if you have successfully connected to the server, it almost definitely is)";
            }
        }
        public override string Keyword
        {
            get
            {
                return "hello";
            }
        }
        public override void Execute(World World, string[] Args)
        {
            World.SendClientMessage(new Chat("world :)"));
        }
    }
}

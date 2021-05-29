using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker.Commands
{
    class Record : TweakerCommand
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
                return "record";
            }
        }

        private List<Packet> recording = new List<Packet>();
        private List<Packet> 

        public override void Execute(World World, string[] Args)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public class PacketCatcher
    {
        public int TargetID;
        public PacketDirection Direction;
        public Action<Packet> Callback;
        public PacketCatcher(int TargetID, Action<Packet> Callback)
        {
            this.TargetID = TargetID;
            this.Callback = Callback;
        }
    }
}

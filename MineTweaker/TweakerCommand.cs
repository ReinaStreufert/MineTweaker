using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTweaker
{
    public abstract class TweakerCommand
    {
        public abstract string Keyword { get; }
        public abstract string Description { get; }
        public abstract void Execute(World World, string[] Args);
    }
}

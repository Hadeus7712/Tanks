using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects
{
    abstract class Bonus : Block
    {
        public abstract void ProccessEffect(Player player);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Objects.Decorator
{
    abstract class Decorator : Bonus
    {
        protected Bonus Bonus;

        public Decorator(Bonus Bonus)
        {
            this.Bonus = Bonus;
        }
    }
}

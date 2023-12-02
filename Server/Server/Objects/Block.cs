using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects.Types;

namespace Server.Objects
{
    public abstract class Block
    {
        public abstract ObjectType ObjectType { get; }
        public abstract RectangleF Rect { get; set; }
        public abstract CollisionType CollisionBlockType { get; }
        public abstract CollisionType CollisionProjectileType { get; }
        public int IntObjectType()
        {
            return (int)ObjectType;
        }
    }
}

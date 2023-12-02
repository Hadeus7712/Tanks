using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects.Types;

namespace Server.Objects
{
    //не разрушаемое препятствие
    public class InsurmountableObstacle : Block
    {
        private ObjectType _objectType;
        public override ObjectType ObjectType { get => _objectType; }

        private RectangleF _rect;
        public override RectangleF Rect { get => _rect; set => _rect = value; }

        private CollisionType _collisionBlockType;
        public override CollisionType CollisionBlockType { get => _collisionBlockType; }

        private CollisionType _collisionProjectileType;
        public override CollisionType CollisionProjectileType { get => _collisionProjectileType; }

        public InsurmountableObstacle()
        {
            _collisionBlockType = CollisionType.Unthrougthable;
            _collisionProjectileType = CollisionType.Unthrougthable;
            _objectType = ObjectType.Insurmountable;
        }
    }
}

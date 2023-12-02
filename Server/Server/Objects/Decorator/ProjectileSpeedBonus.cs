using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects.Types;

namespace Server.Objects.Decorator
{
    class ProjectileSpeedBonus : Decorator
    {
        private ObjectType _objectType;
        public override ObjectType ObjectType { get => _objectType; }

        private RectangleF _rect;
        public override RectangleF Rect { get => _rect; set => _rect = value; }

        private CollisionType _collisionBlockType;
        public override CollisionType CollisionBlockType { get => _collisionBlockType; }

        private CollisionType _collisionProjectileType;
        public override CollisionType CollisionProjectileType { get => _collisionProjectileType; }

        public ProjectileSpeedBonus(Bonus bonus) : base(bonus)
        {
            _collisionBlockType = Bonus.CollisionBlockType;
            _collisionProjectileType = Bonus.CollisionProjectileType;
            _rect = Bonus.Rect;
            _objectType = ObjectType.ProjectileSpeed;
        }
        public override void ProccessEffect(Player player)
        {
            Bonus.ProccessEffect(player);
            player.ProjectileSpeedUp();

        }
    }
}

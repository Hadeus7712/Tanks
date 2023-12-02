using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects.Types;

namespace Server.Objects
{
    public class Projectile : Block
    {
        private ObjectType _objectType;
        public override ObjectType ObjectType { get => _objectType; }

        private RectangleF _rect;
        public override RectangleF Rect { get => _rect; set => _rect = value; }

        private CollisionType _collisionBlockType;
        public override CollisionType CollisionBlockType { get => _collisionBlockType; }

        private CollisionType _collisionProjectileType;
        public override CollisionType CollisionProjectileType { get => _collisionProjectileType; }

        private Vector2 _direction;
        public Vector2 Direction { get => _direction; }

        private float _defaultSpeed;

        private float _speed;
        public float Speed { get => _speed; }

        private bool _display;
        public bool Display { get => _display; set => _display = value; }
        public Projectile()
        {
            _collisionBlockType = CollisionType.Througthable;
            _collisionProjectileType = CollisionType.Througthable;
            _objectType = ObjectType.Projectile;
            //_defaultSpeed = 3.0f;
            _display = false;
            ResetSpeed();
        }

        public void DisplayToggle(bool flag)
        {
            _display = flag;
        }
        public void SetSpeed(float speed)
        {
            _defaultSpeed = speed;
            ResetSpeed();
        }
        public void ResetSpeed()
        {
            _speed = _defaultSpeed;
        }
        public void UpdateSpeed(float speed)
        {
            _defaultSpeed += speed;
            ResetSpeed();
        }

        public void TemporaryUpdateSpeed(float speed)
        {
            _speed = speed;
        }
        public void SetPosition(float x, float y)
        {
            _rect.X = x;
            _rect.Y = y;
        }
        public void ChangePosition()
        {
            _rect.X += _direction.X * _speed;
            _rect.Y += _direction.Y * _speed;
        }

        public void ResetPosition()
        {
            _rect.X = 0;
            _rect.Y = 0;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }
    }
}

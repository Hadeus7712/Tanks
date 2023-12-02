using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Server.Objects.Types;

namespace Server.Objects
{
    public class Tank : Block
    {
        private ObjectType _objectType;
        public override ObjectType ObjectType { get => _objectType; }

        private RectangleF _rect;
        public override RectangleF Rect { get => _rect; set => _rect = value; }

        private CollisionType _collisionBlockType;
        public override CollisionType CollisionBlockType { get => _collisionBlockType; }

        private CollisionType _collisionProjectileType;
        public override CollisionType CollisionProjectileType { get => _collisionProjectileType; }
        private float _defaultSpeed;
        private float _speed;
        public float Speed { get => _speed; }

        private int _armor;
        public int Armor { get => _armor; }
        public Tank()
        {
            _collisionBlockType = CollisionType.Unthrougthable;
            _collisionProjectileType = CollisionType.Unthrougthable;
            _objectType = ObjectType.Tank;
            _defaultSpeed = 1.0f;
            _armor = 5;
            ResetSpeed();
        }
        public void UpdateArmor(int armor)
        {
            _armor += armor;
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
        public void UpdateSpeed()
        {
            _defaultSpeed += _speed / 2;
            ResetSpeed();
        }
        public void TemporarySpeedUp()
        {
            ResetSpeed();
            _speed += _speed / 2;
        }
        public void TemporarySpeedDown()
        {
            ResetSpeed();
            _speed -= _speed / 2;
        }
        public void ChangePosition(int posX, int posY)
        {
            _rect.X += posX * _speed;
            _rect.Y += posY * _speed;
        }
        public Vector2 GetCenter()
        {
            return new Vector2(_rect.X + _rect.Width / 2, _rect.Y + _rect.Height / 2);
        }

        public (Vector2, Vector2) GetDirectionCenterCoord(byte direction)
        {
            switch ((Directions)direction)
            {
                case Directions.Up:
                    return (new Vector2(GetCenter().X - _rect.Width / 6f, GetCenter().Y - _rect.Height), -Vector2.UnitY);
                case Directions.Down:
                    return (new Vector2(GetCenter().X - _rect.Width / 6f, GetCenter().Y + _rect.Height / 3f), Vector2.UnitY);
                case Directions.Right:
                    return (new Vector2(GetCenter().X + _rect.Width / 3f, GetCenter().Y - _rect.Height / 6f), Vector2.UnitX);
                case Directions.Left:
                    return (new Vector2(GetCenter().X - _rect.Width, GetCenter().Y - _rect.Height / 6f), -Vector2.UnitX);
                default:
                    return (Vector2.Zero, Vector2.Zero);
            }
        }
    }
}


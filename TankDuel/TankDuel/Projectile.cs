using SharpDX;
using System;
using Types;

namespace TankDuel
{
    public class Projectile
    {
        private float _angle;
        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                if (_angle > (float)Math.PI) _angle -= 2.0f * (float)Math.PI;
                else if (_angle < - (float)Math.PI) _angle += 2.0f * (float)Math.PI;
            }
        }

        private Block _block;
        public Block Block { get => _block; set => _block = value; }

        //private Vector2 _currentVecDirection;
        //public Vector2 CurrentVecDirection { get => _currentVecDirection; set => _currentVecDirection = value; }

        private RectangleF _bitmapRect;
        public RectangleF BitmapRect { get => _bitmapRect; set => _bitmapRect = value; }

        public Vector2 offset;

        public bool Display;
        public Projectile()
        {
            _block.ObjectType = ObjectType.Projectile;
            Display = false;
        }

        public void DisplayToggle(byte flag)
        {
            Display = flag == 1;
        }
        public void ChangePosition(float stepX, float stepY)
        {
            offset = new Vector2((_block.Rect.Width - _bitmapRect.Width) / 2, (_block.Rect.Height - _bitmapRect.Height) / 2);
            _block.Rect.X = stepX;
            _block.Rect.Y = stepY;
            _bitmapRect.X = _block.Rect.X + offset.X;
            _bitmapRect.Y = _block.Rect.Y + offset.Y;
        }

        public Vector2 GetCenter()
        {
            return new Vector2(_block.Rect.X + _block.Rect.Width / 2, _block.Rect.Y + _block.Rect.Height / 2);
        }

        public Vector2 GetBitCenter()
        {
            return new Vector2(_bitmapRect.X + _bitmapRect.Width / 2, _bitmapRect.Y + _bitmapRect.Height / 2);
        }

        public void InitDirection(float angle)
        {
            _angle = angle/*+ MathUtil.DegreesToRadians(90)*/;
            //_currentVecDirection = direction;
        }
    }
}

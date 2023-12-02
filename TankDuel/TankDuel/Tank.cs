using System;
using System.Collections.Generic;
using DataStructuresManipulation;
using SharpDX;
using Types;

namespace TankDuel
{
    public class Tank
    {
        private float _angle;
        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                if (_angle > (float)Math.PI) _angle -= 2.0f * (float)Math.PI;
                else if (_angle < -(float)Math.PI) _angle += 2.0f * (float)Math.PI;
            }
        }
        private Block _block;
        public Block Block { get => _block; set => _block = value; }

        private Directions _currentDirection;
        public Directions CurrentDirection { get => _currentDirection; set => _currentDirection = value; }

        private Vector2 _currentVecDirection;
        public Vector2 CurrentVecDirection { get => _currentVecDirection; set => _currentVecDirection = value; }

        private RectangleF _bitmapRect;
        public RectangleF BitmapRect { get => _bitmapRect; set => _bitmapRect = value; }

        public Vector2 offset;

        public PlayerType PlayerType;


        //public Projectile CurrentProjectile;
        public Projectile[] Projectiles;

        public ProjectileData[] data = new ProjectileData[3];

        public int Armor;
        public Tank()
        {
            _block.ObjectType = ObjectType.Tank;
            //CurrentProjectile = new Projectile();

            Projectiles = new Projectile[3];
        }

        public void InitProjectilesData(params ProjectileData[] param)
        {
            for(int i = 0; i < data.Length; ++i)
            {
                data[i] = param[i];
                Projectiles[data[i].Index].DisplayToggle(data[i].Display);
            }
        }
        public void UpdateProjectilesData(params ProjectileData[] param)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = param[i];
                Projectiles[data[i].Index].DisplayToggle(data[i].Display);
                if (Projectiles[data[i].Index].Display)
                {
                    Projectiles[data[i].Index].ChangePosition(data[i].X, data[i].Y);
                }
            }
        }

        public void UpdateStatsValues(int armor, float projectileSpeed, float speed)
        {
            Armor = armor;
        }
        //-----------------------------------------------
        public void SetProjectile(int index, byte display)
        {
            //Projectiles[index].DisplayToggle(display);
            //Projectiles[index].InitDirection(_angle);
            //CurrentProjectile = projectile;
            //CurrentProjectile.InitDirection(_angle, _currentVecDirection);
            //CurrentProjectile.DisplayToggle(1);
        }
        public void UpdateProjectile(int index, float stepX, float stepY, byte display)
        {
            //Projectiles[index].DisplayToggle(display);
            //Projectiles[index].ChangePosition(stepX, stepY);
        }
        //------------------------------------------------
        public void ChangePosition(float xStep, float yStep)
        {
            offset = new Vector2((_block.Rect.Width - _bitmapRect.Width) / 2, (_block.Rect.Height - _bitmapRect.Height) / 2);
            _block.Rect.X = xStep;
            _block.Rect.Y = yStep;
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
        public void ChangeDirection(Directions direction)
        {
            _currentDirection = direction;
            switch (direction)
            {
                case Directions.Up:
                    _angle = MathUtil.DegreesToRadians(0);
                    _currentVecDirection = -Vector2.UnitX;
                    break;
                case Directions.Down:
                    _angle = MathUtil.DegreesToRadians(180);
                    _currentVecDirection = Vector2.UnitY;
                    break;
                case Directions.Left:
                    _angle = MathUtil.DegreesToRadians(-90);
                    _currentVecDirection = -Vector2.UnitX;
                    break;
                case Directions.Right:
                    _angle = MathUtil.DegreesToRadians(90);
                    _currentVecDirection = Vector2.UnitX;
                    break;
                default:
                    break;
            }
        }
    }
}


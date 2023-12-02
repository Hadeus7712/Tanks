using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using DataStructuresManipulation;

namespace TankDuel
{
    class SceneObjectGenerator
    {
        private List<Bitmap> _bitmaps;

        private byte[,] _map;

        public SceneObjectGenerator(List<Bitmap> bitmaps)
        {
            _bitmaps = bitmaps;
        }

        public Tank GenerateTank(DataPackageOutput data, float size)
        {
            int index = (int)ObjectType.Tank;
            float width = _bitmaps[index].Size.Width, height = _bitmaps[index].Size.Height;
            RectangleF rect = new RectangleF(data.x, data.y, size, size);
            Tank tank = new Tank();
            Block block = new Block(ObjectType.Tank, rect);
            tank.Block = block;
            tank.BitmapRect = new RectangleF(data.x/* + 8*/, data.y/* - 5*/, width, height);
            tank.PlayerType = (PlayerType)data.playerType;
            tank.Armor = data.Armor;
            tank.ChangeDirection((Directions)data.CurrentDirection);
            return tank;

        }

        public Projectile GenerateProjectile(Vector2 pos, float projectileSize)
        {
            int index = (int)ObjectType.Projectile;
            float width = _bitmaps[index].Size.Width, height = _bitmaps[index].Size.Height;
            RectangleF rect = new RectangleF(pos.X, pos.Y, projectileSize, projectileSize);
            Projectile template = new Projectile();
            Block block = new Block(ObjectType.Projectile, rect);
            template.Block = block;
            template.BitmapRect = new RectangleF(pos.X, pos.Y, width, height);
            template.DisplayToggle(0);
            return template;
            //template.InitDirection(tank.Angle);
            //tank.Add(template);
        }

        public void SetMap(byte[,] map)
        {
            _map = map;
        }

        public List<Block> GenerateField(float offset, float size, float bonusSize, float quarterOffset)
        {
            List<Block> blocks = new List<Block>();
            for (int i = 0; i < _map.GetLength(1); ++i)
            {
                for (int j = 0; j < _map.GetLength(0); ++j)
                {
                    if (_map[j, i] != 99 && _map[j, i] < 5)
                    {
                        RectangleF rect = new RectangleF(offset + (i * size), offset + (j * size), size, size);
                        blocks.Add(new Block((ObjectType)_map[j, i], rect));
                    }
                    else if (_map[j, i] != 99 && _map[j, i] >= 8 && _map[j, i] <= 10)
                    {
                        RectangleF rect = new RectangleF(offset + quarterOffset + (i * size), offset + quarterOffset + (j * size), bonusSize, bonusSize);
                        blocks.Add(new Block((ObjectType)_map[j, i], rect));
                    }
                }
            }
            return blocks;
        }
    }
}

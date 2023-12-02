using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Server.Objects;
using Server.Objects.Factory;
using Server.Objects.Decorator;
using Server.Objects.Types;
using Server.DataStructuresManipulation;

namespace Server
{
    class SceneGenerator
    {
        private static readonly string _path = "map.png";
        private byte[,] _map;
        private List<Block> _gameobjects;
        public List<Block> GameObjects { get => _gameobjects; private set => _gameobjects = value; }

        private ObjectBuilder _objectBuilder;

        private float _size;
        private float _offset;
        private float _semiSize;
        private float _quarterSize;
        private float _playerSize;
        private float _quarterOffset;

        public List<byte> CurrentIndexesForDelete = new List<byte>();

        public SettingsData Settings { get; private set; }

        public SceneGenerator()
        {
            _size = 50f;
            _offset = _size;
            _semiSize = _size/2;
            _quarterSize = _size / 4;
            _quarterOffset = _offset/4;
            _playerSize = _size - _quarterOffset;
            Settings = new SettingsData(_size, _semiSize, _quarterSize, _playerSize, _offset, _quarterOffset);
            CreateMap();
            _gameobjects = new List<Block>();
            _objectBuilder = new ObjectBuilder();
        }

        private void CreateMap()
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(_path);
            _map = new byte[bitmap.Height, bitmap.Width];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for(int j = 0; j < bitmap.Height; j++)
                {
                    if(bitmap.GetPixel(i,j) == System.Drawing.Color.FromArgb(255, 0, 0, 0))
                    {
                        _map[j, i] = 0;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 255, 255, 0))
                    {
                        _map[j, i] = 1;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 0, 255, 255))
                    {
                        _map[j, i] = 2;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 0, 255, 0))
                    {
                        _map[j, i] = 4;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 0, 0, 255))
                    {
                        _map[j, i] = 3;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 127, 0, 0))
                    {
                        _map[j, i] = 8;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 0, 127, 0))
                    {
                        _map[j, i] = 9;
                    }
                    else if (bitmap.GetPixel(i, j) == System.Drawing.Color.FromArgb(255, 0, 0, 127))
                    {
                        _map[j, i] = 10;
                    }
                    else
                    {
                        _map[j, i] = 99;
                    }
                }
            }
        }

        public byte[,] GetMapPoints()
        {
            return _map;
        }

        public void GenerateField()
        {
            SharpDX.RectangleF rect = new SharpDX.RectangleF();
            for (int i = 0; i < _map.GetLength(1); ++i)
            {
                for (int j = 0; j < _map.GetLength(0); ++j)
                {
                    if (_map[j, i] != 99 && _map[j, i] < 5)
                    {
                        rect = new SharpDX.RectangleF(_offset + (i * _size), _offset + (j * _size), _size, _size);
                        _gameobjects.Add(_objectBuilder.ObjectsCreate((ObjectType)_map[j, i], rect));
                    }
                    else if(_map[j, i] != 99 && (_map[j, i] >= 8 && _map[j, i] <= 10))
                    {
                        rect = new SharpDX.RectangleF(_offset + _quarterOffset + (i * _size), _offset + _quarterOffset + (j * _size), _semiSize, _semiSize);
                        Block bonus = _objectBuilder.ObjectsCreate(ObjectType.Bonus, rect);
                        switch ((ObjectType)_map[j, i])
                        {
                            case ObjectType.Armor:
                                bonus = new ArmorBonus((Bonus)bonus);
                                break;
                            case ObjectType.ProjectileSpeed:
                                bonus = new ProjectileSpeedBonus((Bonus)bonus);
                                break;
                            case ObjectType.SpeedUp:
                                bonus = new SpeedBonus((Bonus)bonus);
                                break;
                        }
                        _gameobjects.Add(bonus);
                    }
                }
            }
        }

        public void RemoveGameobject(Block element)
        {
            int index = _gameobjects.IndexOf(element);
            CurrentIndexesForDelete.Add((byte)index);
            _gameobjects.RemoveAt(index);
        }

        public void IndexerReset()
        {
            CurrentIndexesForDelete.Clear();
        }
        public void GeneratePlayers(List<Player> players)
        {
            for(int i = 0; i < players.Count; ++i)
            {
                SharpDX.RectangleF rect = new SharpDX.RectangleF(players[i].dataOutput.x, players[i].dataOutput.y, _playerSize, _playerSize);
                players[i].GameObject = (Tank)_objectBuilder.ObjectsCreate(ObjectType.Tank, rect);
                _gameobjects.Add(players[i].GameObject);
                GenerateProjectile(players[i], 3);
            }
        }

        public void GenerateProjectile(Player player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                //(SharpDX.Vector2, SharpDX.Vector2) tupleVec = player.GetDirectionCenterCoord();
                SharpDX.RectangleF rect = new SharpDX.RectangleF(0, 0, _quarterSize, _quarterSize);
                Projectile projectile = (Projectile)_objectBuilder.ObjectsCreate(ObjectType.Projectile, rect);
                //projectile.SetDirection(tupleVec.Item2);
                player.Projectiles[i] = projectile;
                //return projectile;
            }
        }
    }
}


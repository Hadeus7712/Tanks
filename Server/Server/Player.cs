using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Server.DataStructuresManipulation;
using Server.Objects;

namespace Server
{
    class Player
    {
        private EndPoint _address;
        public EndPoint Address { get => _address; }

        private bool[] _directionBlocker = { true, true, true, true };

        private Tank _gameObject;
        public Tank GameObject { get => _gameObject; set => _gameObject = value; }

        //private Projectile _projectile;
        //public Projectile Projectile { get => _projectile; set => _projectile = value; }
        //private Vector2 _projectileDirection;

        public DataPackageInput dataInput;
        public DataPackageOutput dataOutput;

        //private bool _genLock;

        /*private int _armor;
        public int Armor { get => _armor; }*/

        private float _projectileSpeed;
        public float ProjectileSpeed { get => _projectileSpeed; }

        //public bool GenLock { get => _genLock; private set => _genLock = value; }


        //public ProjectileStorage ProjectileStorage;
        public Projectile[] Projectiles;
        private int CurrentProjectileIndex = 0;
        public List<ProjectileData> projectileData = new List<ProjectileData>();
        public Player(EndPoint address, DataPackageOutput data)
        {
            _address = address;
            dataOutput = data;
            //_genLock = false;
            InitializePlayerSettings();
            InitializeProjectilesData();
            Projectiles = new Projectile[3];
            //ProjectileStorage = new ProjectileStorage();
        }
        public void UpdatePlayerStats()
        {
            dataOutput.Armor = _gameObject.Armor;
            //трабла со сменой скоростей
            dataOutput.ProjectileSpeed = _projectileSpeed;
            dataOutput.Speed = _gameObject.Speed;
        }
        private void InitializePlayerSettings()
        {
            _projectileSpeed = 3.0f;
            //_projectile = new Projectile();
        }
        /*public void UpdateArmor(int armor)
        {
            _armor += armor;
        }*/

        public void ProjectileSpeedUp()
        {
            _projectileSpeed += _projectileSpeed / 2;
        }

        /*public void DisplayToggle(bool flag)
        {
            dataOutput.ProjectileDisplay = (byte)(flag ? 1 : 0);
        }*/

        public void ChangeShootingToggle(bool flag)
        {
            dataOutput.IsShoot = (byte)(flag ? 1 : 0);
        }

        /*public void ProjectileGenLockerToggle(bool flag)
        {
            _genLock = flag;
        }*/

        public void ChangeDirection(int direction)
        {
            dataOutput.CurrentDirection = (byte)direction;
        }

        public void ChangePosition(int posX, int posY)
        {
            _gameObject.ChangePosition(posX, posY);
            dataOutput.x = _gameObject.Rect.X;
            dataOutput.y = _gameObject.Rect.Y;
        }

        private void InitializeProjectilesData()
        {
            for (int i = 0; i < 3; i++)
            {
                projectileData.Add(new ProjectileData((byte)i, 0, 0, 0));
            }
        }
        public void SetProjectilePosition()
        {
            if (CheckProjectileDisplays() == 3) return;
            if (CurrentProjectileIndex > 2) CurrentProjectileIndex = 0;
            (Vector2, Vector2) tupleVec = GetDirectionCenterCoord();
            Projectiles[CurrentProjectileIndex].DisplayToggle(true);
            Projectiles[CurrentProjectileIndex].SetPosition(tupleVec.Item1.X, tupleVec.Item1.Y);
            Projectiles[CurrentProjectileIndex].SetDirection(tupleVec.Item2);
            Projectiles[CurrentProjectileIndex].SetSpeed(ProjectileSpeed);
            CurrentProjectileIndex++;
            //dataOutput.PrjPosX = _projectile.Rect.X;
            //dataOutput.PrjPosY = _projectile.Rect.Y;
        }
        public int CheckProjectileDisplays()
        {
            int count = 0;
            for (int i = 0; i < Projectiles.Length; ++i)
            {
                if (Projectiles[i].Display)
                {
                    count++;
                }
            }
            return count;
        }
        public void ResetProjectilePosition(int index)
        {
            Projectiles[index].DisplayToggle(false);
            Projectiles[index].ResetPosition();
            CurrentProjectileIndex = index;
            //dataOutput.PrjPosX = _projectile.Rect.X;
            //dataOutput.PrjPosY = _projectile.Rect.Y;
        }

        public void ChangeProjectilePosition()
        {
            //проджектайлы крутятся во время полета
            for (int i = 0; i < Projectiles.Length; ++i)
            {
                if (Projectiles[i].Display)
                {
                    Projectiles[i].ChangePosition();
                    projectileData.Insert(i, new ProjectileData((byte)i, Projectiles[i].Rect.X, Projectiles[i].Rect.Y, 1));
                }
                else
                {
                    projectileData.Insert(i, new ProjectileData((byte)i, 0, 0, 0));
                }
            }
            dataOutput.ProjectileData1 = projectileData[0];
            dataOutput.ProjectileData2 = projectileData[1];
            dataOutput.ProjectileData3 = projectileData[2];
            //dataOutput.PrjPosX = _projectile.Rect.X;
            //dataOutput.PrjPosY = _projectile.Rect.Y;
        }


        public int GetLengthBlocker()
        {
            return _directionBlocker.Length;
        }
        public bool GetDirectionBlocker(int index)
        {
            return _directionBlocker[index];
        }
        public void SetDirectionBlocker(int index, bool value)
        {
            if (_directionBlocker.Length > index && index >= 0) _directionBlocker[index] = value;
        }

        /// <summary>
        /// item1 = point , item2 = direction
        /// </summary>
        /// <returns></returns>
        public (Vector2, Vector2) GetDirectionCenterCoord()
        {
            return _gameObject.GetDirectionCenterCoord(dataInput.CurrentDirection);
        }
    }
}

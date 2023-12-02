namespace DataStructuresManipulation
{
    public struct DataPackageOutput
    {
        public float x, y;
        public byte CurrentDirection;
        public byte playerType;
        public byte IsShoot;
        public int Armor;
        public float ProjectileSpeed;
        public float Speed;
        public ProjectileData ProjectileData1;
        public ProjectileData ProjectileData2;
        public ProjectileData ProjectileData3;
        //public (byte, float, float, byte) ProjectileData;
        //public byte ProjectileDisplay;

        public DataPackageOutput(float x, float y, byte direction, byte playerType,
            bool isShoot, int armor, float projectileSpeed, float speed, 
            ProjectileData projectileData1, ProjectileData projectileData2, ProjectileData projectileData3)
        {
            this.x = x;
            this.y = y;
            CurrentDirection = direction;
            this.playerType = playerType;
            IsShoot = (byte)(isShoot ? 1 : 0);
            //PrjPosX = projectilePosX;
            //PrjPosY = projectilePosY;
            //ProjectileDisplay = (byte)(projectileDisplay ? 1 : 0);
            Armor = armor;
            ProjectileSpeed = projectileSpeed;
            Speed = speed;
            ProjectileData1 = projectileData1;
            ProjectileData2 = projectileData2;
            ProjectileData3 = projectileData3;
        }
        public override string ToString()
        {
            return $"[pType({playerType}): Pos({x}, {y}), dir({CurrentDirection}), shoot({IsShoot})]" +
                $"\n[stats(armor: {Armor}, PRJspeed: {ProjectileSpeed}, speed: {Speed})]" +
                $"\n[PRJdata1({ProjectileData1})]\n[PRJdata2({ProjectileData2})]\n[PRJdata3({ProjectileData3})]";
        }
    }
}


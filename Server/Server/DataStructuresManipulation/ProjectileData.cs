namespace Server.DataStructuresManipulation
{
    public struct ProjectileData
    {
        public byte Index;
        public float X;
        public float Y;
        public byte Display;
        public ProjectileData(byte index, float x, float y, byte display)
        {
            Index = index;
            X = x;
            Y = y;
            Display = display;
        }
        public override string ToString()
        {
            return $"[({Index})->(x:{X}, y:{Y}), Display: {Display}]";
        }
    }
}

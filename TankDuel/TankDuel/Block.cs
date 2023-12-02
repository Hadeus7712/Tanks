using SharpDX;
using Types;

namespace TankDuel
{
    public struct Block
    {
        public ObjectType ObjectType;
        public RectangleF Rect;
        public Block(ObjectType objectType, RectangleF rect)
        {
            ObjectType = objectType;
            Rect = rect;
        }
        public int IntObjectType()
        {
            return (int)ObjectType;
        }
    }
}

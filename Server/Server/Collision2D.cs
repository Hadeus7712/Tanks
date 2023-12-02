using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects;
using Server.Objects.Types;

namespace Server
{
    public static class Collision2D
    {
        public static List<Block> CollisionDirectionsArrayPlS1(Block dynamicObj, List<Block> staticObj)
        {
            List<Block> buffer = new List<Block>();
            foreach (Block staticBlock in staticObj)
            {
                if (staticBlock.Rect.Intersects(dynamicObj.Rect))
                {
                    buffer.Add(staticBlock);
                }
            }
            return buffer;
        }

        //для коллизии снаряда с блоками
        public static bool CollisionDirectionsArrayPlS(Block dynamicObj, List<Block> staticObj)
        {
            foreach (Block staticBlock in staticObj)
            {
                if (dynamicObj.Rect.Intersects(staticBlock.Rect))
                {
                    return true;
                }
            }
            return false;
        }

        public static Block CollisionDirectionsDD1(Block dynamicObj1, Block dynamicObj2)
        {
            if (dynamicObj1.Rect.Intersects(dynamicObj2.Rect))
            {
                return dynamicObj2;
            }
            return null;
        }

        /*
        //для коллизий двух танков
        public static Directions CollisionDirectionsDD(Block dynamicObj1, Block dynamicObj2)
        {
            Directions buffer = Directions.Empty;
            if (dynamicObj1.Rect.Intersects(dynamicObj2.Rect)) buffer = DirectionIntersects(dynamicObj1.Rect, dynamicObj2.Rect);
            return buffer;
        }*/
        //для коллизий танка с блоками
        public static List<Tuple<Directions, Block>> CollisionDirectionsArrayDlS(Block dynamicObj, List<Block> staticObj)
        {
            List<Tuple<Directions, Block>> tupleDirColType = new List<Tuple<Directions, Block>>();
            foreach (Block staticBlock in staticObj)
            {
                if (dynamicObj.Rect.Intersects(staticBlock.Rect))
                {
                    tupleDirColType.Add(new Tuple<Directions, Block>(DirectionIntersects(dynamicObj.Rect, staticBlock.Rect), staticBlock));
                }
            }
            return tupleDirColType;
        }
        private static Directions DirectionIntersects(RectangleF dynamicRect, RectangleF staticRect)
        {
            float diffX = dynamicRect.Center.X - staticRect.Center.X;
            float diffY = dynamicRect.Center.Y - staticRect.Center.Y;
            float minXDist = dynamicRect.Width / 2 + staticRect.Width / 2;
            float minYDist = dynamicRect.Height / 2 + staticRect.Height / 2;
            float depthX = diffX > 0 ? minXDist - diffX : -minXDist - diffX;
            float depthY = diffY > 0 ? minYDist - diffY : -minYDist - diffY;
            if (depthX != 0 && depthY != 0)
            {
                if (Math.Abs(depthX) < Math.Abs(depthY))
                {
                    // Collision along the X axis. React accordingly
                    if (depthX > 0) return Directions.Left;
                    else return Directions.Right;
                }
                else if (Math.Abs(depthX) > Math.Abs(depthY))
                {
                    // Collision along the Y axis.
                    if (depthY > 0) return Directions.Up;
                    else return Directions.Down;
                }
            }
            return Directions.Empty;
        }
    }
}

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Objects.Types;

namespace Server.Objects.Factory
{
    public class ObjectBuilder
    {
        private List<ObjectsFactory> creatorsTemplate;

        public ObjectBuilder()
        {
            creatorsTemplate = new List<ObjectsFactory>();
            CreatorsInit();
        }

        private void CreatorsInit()
        {
            creatorsTemplate.Add(new InsurObstacleCreator());
            creatorsTemplate.Add(new DestrObstacleCreator());
            creatorsTemplate.Add(new IceGroundObstacleCreator());
            creatorsTemplate.Add(new RiverObstacleCreator());
            creatorsTemplate.Add(new SwampObstacleCreator());
            creatorsTemplate.Add(new TankCreator());
            creatorsTemplate.Add(new ProjectileCreator());
            creatorsTemplate.Add(new BonusCreator());
        }

        public Block ObjectsCreate(ObjectType type, RectangleF rect)
        {
            Block block;
            switch(type)
            {
                case ObjectType.Insurmountable:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.Destructible:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.IceGround:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.River:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.Swamp:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.Tank:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.Projectile:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                case ObjectType.Bonus:
                    block = creatorsTemplate[(int)type].CreateBlock();
                    break;
                default:
                    throw new Exception("type doesnt exist");
            }
            block.Rect = rect;
            return block;
        }
    }
}

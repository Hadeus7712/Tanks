namespace Server.Objects.Factory
{
    public class RiverObstacleCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new RiverObstacle();
        }
    }
}

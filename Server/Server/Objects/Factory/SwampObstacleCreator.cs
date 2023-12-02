namespace Server.Objects.Factory
{
    public class SwampObstacleCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new SwampObstacle();
        }
    }
}

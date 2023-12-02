namespace Server.Objects.Factory
{
    public class DestrObstacleCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new DestructibleObsctacle();
        }
    }
}

namespace Server.Objects.Factory
{
    public class IceGroundObstacleCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new IceGroundObsctacle();
        }
    }
}

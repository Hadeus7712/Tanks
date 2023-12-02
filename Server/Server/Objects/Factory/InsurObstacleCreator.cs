namespace Server.Objects.Factory
{
    public class InsurObstacleCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new InsurmountableObstacle();
        }
    }
}

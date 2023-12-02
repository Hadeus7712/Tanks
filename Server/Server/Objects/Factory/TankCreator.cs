namespace Server.Objects.Factory
{
    public class TankCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new Tank();
        }
    }
}

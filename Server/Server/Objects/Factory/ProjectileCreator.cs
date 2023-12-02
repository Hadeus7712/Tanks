namespace Server.Objects.Factory
{
    class ProjectileCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new Projectile();
        }
    }
}

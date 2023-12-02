namespace Server.Objects.Factory
{
    class BonusCreator : ObjectsFactory
    {
        public override Block CreateBlock()
        {
            return new TemplateBonus();
        }
    }
}

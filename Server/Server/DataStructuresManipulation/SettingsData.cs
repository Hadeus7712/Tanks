namespace Server.DataStructuresManipulation
{
    public struct SettingsData
    {
        public float BlockSize;
        public float BonusSize;
        public float ProjectileSize;
        public float PlayerSize;
        public float BlockOffset;
        public float QuarterOffset;

        public SettingsData(float blockSize, float bonusSize, float projectileSize,
            float playerSize, float blockOffset, float quarterOffset)
        {
            BlockSize = blockSize;
            BonusSize = bonusSize;
            ProjectileSize = projectileSize;
            PlayerSize = playerSize;
            BlockOffset = blockOffset;
            QuarterOffset = quarterOffset;
        }
    }
}

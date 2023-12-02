namespace DataStructuresManipulation
{
    public struct DataPackageInput
    {
        public byte CurrentInput;
        public byte CurrentDirection;
        public byte IsShooting;

        public DataPackageInput(byte input, byte direction, bool isShooting)
        {
            CurrentInput = input;
            CurrentDirection = direction;
            IsShooting = (byte)(isShooting ? 1 : 0);
        }

        public override string ToString()
        {
            return $"[cInput({CurrentInput}), Direction({CurrentDirection}), IsShooting({IsShooting})]";
        }
    }
}

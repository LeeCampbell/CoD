namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class Duration
    {
        public Duration(int length, DurationUnit unit)
        {
            Length = length;
            Unit = unit;
        }

        public int Length { get; }
        public DurationUnit Unit { get; }
    }
}
namespace Yow.CoD.Finance.Domain.Contracts
{
    public class FeeModel
    {
        public FeeModel(decimal simpleInterestRate)
        {
            SimpleInterestRate = simpleInterestRate;
        }

        public decimal SimpleInterestRate { get; }
    }
}
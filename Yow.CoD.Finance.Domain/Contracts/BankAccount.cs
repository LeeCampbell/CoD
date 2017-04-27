namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class BankAccount
    {
        public BankAccount(string bsb, string accountNumber)
        {
            Bsb = bsb;
            AccountNumber = accountNumber;
        }

        public string Bsb { get; }
        public string AccountNumber { get; }
    }
}
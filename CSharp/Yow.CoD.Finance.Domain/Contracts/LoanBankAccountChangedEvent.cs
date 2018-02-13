namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanBankAccountChangedEvent : Event
    {
        public LoanBankAccountChangedEvent(string bankAccountBsb, string bankAccountAccountNumber)
        {
            BankAccount = new BankAccount(bankAccountBsb, bankAccountAccountNumber);
        }

        public BankAccount BankAccount { get; }
    }
}
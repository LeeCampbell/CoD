using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanOverPaidEvent : Event
    {
        public LoanOverPaidEvent(DateTimeOffset transactionDate, decimal amount)
        {
            TransactionDate = transactionDate;
            Amount = amount;
        }
        public DateTimeOffset TransactionDate { get; }
        public decimal Amount { get; }
    }
}
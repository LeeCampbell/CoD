using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanSettledEvent : Event
    {
        public LoanSettledEvent(DateTimeOffset transactionDate)
        {
            TransactionDate = transactionDate;
        }

        public DateTimeOffset TransactionDate { get; }
    }
}
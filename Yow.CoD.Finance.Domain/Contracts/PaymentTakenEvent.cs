using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class PaymentTakenEvent : Event
    {
        public PaymentTakenEvent(DateTimeOffset transactionDate, decimal amount)
        {
            TransactionDate = transactionDate;
            Amount = amount;
        }

        public DateTimeOffset TransactionDate { get; }
        public decimal Amount { get; }
    }
}
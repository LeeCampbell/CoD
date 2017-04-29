using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class PaymentTakenEvent : Event
    {
        public PaymentTakenEvent(string transactionId,DateTimeOffset transactionDate, decimal amount)
        {
            TransactionId = transactionId;
            TransactionDate = transactionDate;
            Amount = amount;
        }

        public string TransactionId { get;  }
        public DateTimeOffset TransactionDate { get; }
        public decimal Amount { get; }
    }
}
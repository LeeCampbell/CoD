using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanCreatedEvent : Event
    {
        public LoanCreatedEvent(DateTimeOffset createdOn, decimal amount, Duration term, PaymentPlan paymentPlan)
        {
            CreatedOn = createdOn;
            Amount = amount;
            Term = term;
            PaymentPlan = paymentPlan;
        }

        public DateTimeOffset CreatedOn { get; }        
        public decimal Amount { get; }
        public Duration Term { get; }
        public PaymentPlan PaymentPlan { get; }
    }

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
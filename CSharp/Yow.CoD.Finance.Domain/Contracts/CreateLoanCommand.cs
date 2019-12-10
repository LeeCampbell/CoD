using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public class CreateLoanCommand : Command
    {
        public CreateLoanCommand(Guid commandId,
            Guid aggregateId,
            DateTimeOffset createdOn,
            CustomerContact customerContact,
            BankAccount bankAccount,
            PaymentPlan paymentPlan,
            decimal amount,
            Duration term)
            : base(commandId, aggregateId)
        {
            if (createdOn == default(DateTimeOffset)) throw new ArgumentException("CreatedOn must be non default value", nameof(createdOn));
            if (customerContact == null) throw new ArgumentException("CustomerContact must be non null", nameof(customerContact));
            if (bankAccount == null) throw new ArgumentException("BankAccount must be non null", nameof(bankAccount));
            if (paymentPlan == PaymentPlan.None) throw new ArgumentException("PaymentPlan must be non default value", nameof(paymentPlan));
            if (amount == 0) throw new ArgumentException("Amount must be non zero value", nameof(amount));
            if (term == null) throw new ArgumentException("Term must be non null", nameof(term));

            CreatedOn = createdOn;
            CustomerContact = customerContact;
            BankAccount = bankAccount;
            PaymentPlan = paymentPlan;
            Amount = amount;
            Term = term;
        }

        public DateTimeOffset CreatedOn { get; }

        public CustomerContact CustomerContact { get; }

        public BankAccount BankAccount { get; }

        public PaymentPlan PaymentPlan { get; }

        public decimal Amount { get; }

        public Duration Term { get; }

        public override string ToString()
        {
            return $"CreateLoanCommand{{ CreatedOn:'{CreatedOn}', Amount:'{Amount}', Term:{Term}}}";
        }
    }
}
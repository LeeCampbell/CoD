using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public class CreateLoanCommand : Command
    {
        public CreateLoanCommand(Guid commandId, 
            Guid loanId,
            DateTimeOffset createdOn,
            CustomerContact customerContact, 
            BankAccount bankAccount, PaymentPlan paymentPlan, decimal amount, Duration term) 
            : base(commandId)
        {
            LoanId = loanId;
            CustomerContact = customerContact;
            BankAccount = bankAccount;
            PaymentPlan = paymentPlan;
            Amount = amount;
            Term = term;
            CreatedOn = createdOn;
        }

        public Guid LoanId { get; }
        public DateTimeOffset CreatedOn { get; }

        public CustomerContact CustomerContact { get; }

        public BankAccount BankAccount { get; }

        public PaymentPlan PaymentPlan { get; }

        public decimal Amount { get; }
        public Duration Term { get; }
    }
}
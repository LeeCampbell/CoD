using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class TakePaymentCommand : Command
    {
        public TakePaymentCommand(Guid commandId, Guid loanId, decimal amount, DateTimeOffset transactionDateTime) 
            : base(commandId)
        {
            LoanId = loanId;
            Amount = amount;
            TransactionDateTime = transactionDateTime;
        }

        public Guid LoanId { get; }
        public decimal Amount { get; }
        public DateTimeOffset TransactionDateTime { get;}
        
    }
}
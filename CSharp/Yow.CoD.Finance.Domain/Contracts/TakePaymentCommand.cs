using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class TakePaymentCommand : Command
    {
        public TakePaymentCommand(Guid commandId, Guid aggregateId, decimal amount, DateTimeOffset transactionDateTime) 
            : base(commandId, aggregateId)
        {
            Amount = amount;
            TransactionDateTime = transactionDateTime;
        }

        public decimal Amount { get; }
        public DateTimeOffset TransactionDateTime { get;}
    }
}
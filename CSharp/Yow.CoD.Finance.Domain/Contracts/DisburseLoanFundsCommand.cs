using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public class DisburseLoanFundsCommand : Command
    {
        public DisburseLoanFundsCommand(Guid commandId, Guid aggregateId, DateTimeOffset transactionDate)  
            : base(commandId, aggregateId)
        {
            TransactionDate = transactionDate;
        }

        public DateTimeOffset TransactionDate { get; }
    }
}

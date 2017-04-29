using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class TransactionReceipt : Receipt
    {
        public TransactionReceipt(
            Guid aggregateId, 
            int version,
            string transactionId) 
            : base(aggregateId, version)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}
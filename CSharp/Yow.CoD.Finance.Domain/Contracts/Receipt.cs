using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public class Receipt
    {
        public Receipt(Guid aggregateId, int version)
        {
            if(aggregateId == Guid.Empty) throw new ArgumentException("AggregateId must be non default value", nameof(aggregateId));

            AggregateId = aggregateId;
            Version = version;
        }
        public Guid AggregateId { get; }
        public int Version { get; }
    }
}
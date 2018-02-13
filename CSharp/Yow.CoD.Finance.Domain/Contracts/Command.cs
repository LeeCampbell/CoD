using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public abstract class Command
    {
        protected Command(Guid commandId, Guid aggregateId)
        {
            if (commandId == Guid.Empty) throw new ArgumentException("CommandId must be non default value", nameof(commandId));
            if(aggregateId == Guid.Empty) throw new ArgumentException("AggregateId must be non default value", nameof(aggregateId));

            CommandId = commandId;
            AggregateId = aggregateId;
        }
        public Guid CommandId { get; }
        public Guid AggregateId { get; }
    }
}
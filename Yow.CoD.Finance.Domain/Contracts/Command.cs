using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public abstract class Command
    {
        protected Command(Guid commandId, Guid aggregateId)
        {
            CommandId = commandId;
            AggregateId = aggregateId;
        }
        public Guid CommandId { get; }
        public Guid AggregateId { get; }
    }
}
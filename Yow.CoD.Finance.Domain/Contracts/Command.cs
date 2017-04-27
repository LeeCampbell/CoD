using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public abstract class Command
    {
        protected Command(Guid commandId)
        {
            CommandId = commandId;
        }
        public Guid CommandId { get; }
    }
}
using System;
using System.Collections.Generic;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Model
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _uncommitedEvents = new List<Event>();

        protected AggregateRoot(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
        public int Version { get; private set; }

        public void ApplyEvent(Event payload)
        {
            //Build up state...
            Version++;
        }

        protected void AddEvent(Event uncommittedEvent)
        {
            _uncommitedEvents.Add(uncommittedEvent);
            ApplyEvent(uncommittedEvent);
        }

        public Event[] GetUncommitedEvents()
        {
            return _uncommitedEvents.ToArray();
        }

        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }
    }
}
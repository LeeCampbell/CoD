using System;
using System.Collections.Generic;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Model
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _uncommittedEvents = new List<Event>();
        private readonly Dictionary<Type, Action<object>> _handlers =new Dictionary<Type, Action<object>>();

        protected AggregateRoot(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
        public int Version { get; private set; }

        public void ApplyEvent(Event payload)
        {
            _handlers[payload.GetType()](payload);
            Version++;
        }

        protected void AddEvent(Event uncommittedEvent)
        {
            _uncommittedEvents.Add(uncommittedEvent);
            ApplyEvent(uncommittedEvent);
        }

        protected void RegisterHandler<T>(Action<T> handler)
        {
            _handlers.Add(typeof(T), e=>handler((T)e));
        }

        public Event[] GetUncommittedEvents()
        {
            return _uncommittedEvents.ToArray();
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }
    }
}
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

        /// <summary>
        /// Apply events to rehydrate the instance from existing events
        /// </summary>
        /// <param name="payload">An event to apply to the instance.</param>
        public void ApplyEvent(Event payload)
        {
            _handlers[payload.GetType()](payload);
            Version++;
        }

        /// <summary>
        /// Get all events that have been emmitted but not yet saved.
        /// </summary>
        /// <returns>An array of uncommitted events.</returns>
        public Event[] GetUncommittedEvents()
        {
            return _uncommittedEvents.ToArray();
        }

        /// <summary>
        /// Acknowledges the uncommitted events as committed.
        /// </summary>
        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
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
    }
}
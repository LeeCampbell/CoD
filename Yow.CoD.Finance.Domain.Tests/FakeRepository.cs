using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public sealed class FakeRepository<T> : IRepository<T> where T : AggregateRoot
    {
        private readonly Dictionary<Guid, T> _items = new Dictionary<Guid, T>();
        
        public Task<T> Get(Guid id)
        {
            T item;
            if (!_items.TryGetValue(id, out item))
            {
                item = (T)Activator.CreateInstance(typeof(T), id);
                _items[id] = item;
            }
            return Task.FromResult(item);
        }

        public Task Save(T item)
        {
            CommitedEvents.AddRange(item.GetUncommitedEvents());
            item.ClearUncommitedEvents();
            return Task.CompletedTask;
        }

        public List<Event> CommitedEvents { get; } = new List<Event>();
    }
}
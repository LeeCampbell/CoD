﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.NancyWebHost.Adapters
{
    public sealed class InMemoryRepository<T> : IRepository<T> where T : AggregateRoot
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
            item.ClearUncommitedEvents();
            return Task.CompletedTask;
        }
    }
}
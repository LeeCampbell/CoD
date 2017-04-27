using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public abstract class Specification<T, TCommand> 
        where T : AggregateRoot 

        where TCommand : Command
    {
        private FakeRepository<T> _repository;
        protected abstract IEnumerable<Event> Given();
        protected abstract TCommand When();
        protected abstract IHandler<TCommand> CreateHandler();

        protected T Sut { get; set; }
        protected List<Event> Produced { get; private set; }
        protected Exception Caught { get; private set; }

        protected IRepository<T> Repository => _repository;

        [SetUp]
        public async Task SetUp()
        {
            try
            {
                _repository = new FakeRepository<T>();
                Sut = await Repository.Get(Guid.NewGuid());
                var initialState = Given();
                foreach (var @event in initialState)
                {
                    Sut.ApplyEvent(@event);
                }
                var cmd = When();
                var handler = CreateHandler();
                await handler.Handle(cmd);
                Produced = _repository.CommitedEvents;
            }
            catch (Exception e)
            {
                Caught = e;
            }
        }
    }
}
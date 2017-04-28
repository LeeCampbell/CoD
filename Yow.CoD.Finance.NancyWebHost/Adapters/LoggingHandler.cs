using System;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.NancyWebHost.Adapters
{
    public class LoggingHandler<T> : IHandler<T> where T : Command
    {
        private readonly IHandler<T> _inner;

        public LoggingHandler(IHandler<T> inner)
        {
            _inner = inner;
        }

        public async Task Handle(T command)
        {
            Console.WriteLine($"Receiving {command.GetType().Name} command.");
            await _inner.Handle(command);
            Console.WriteLine($"Processed {command.GetType().Name} command.");
        }
    }
}
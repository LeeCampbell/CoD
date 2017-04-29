using System;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.NancyWebHost.Adapters
{
    public class LoggingHandler<TCommand, TReceipt> : IHandler<TCommand, TReceipt> 
        where TCommand : Command
        where TReceipt : Receipt

    {
        private readonly IHandler<TCommand, TReceipt> _inner;

        public LoggingHandler(IHandler<TCommand, TReceipt> inner)
        {
            _inner = inner;
        }

        public async Task<TReceipt> Handle(TCommand command)
        {
            Console.WriteLine($"Receiving {command.GetType().Name} command.");
            var receipt = await _inner.Handle(command);
            Console.WriteLine($"Processed {command.GetType().Name} command. ");
            return receipt;
        }
    }
}
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Application
{
    public static class LoggingExtensions
    {
        private static readonly ILoggerFactory LogFactory;

        static LoggingExtensions()
        {
            LogFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddFilter("Yow.CoD.Finance", LogLevel.Debug)
                       .AddConsole();
            });
        }
        public static IHandler<TCommand, TReceipt> WithLogging<TCommand, TReceipt>(this IHandler<TCommand, TReceipt> source)
            where TCommand : Command
            where TReceipt : Receipt
        {
            ILogger logger = LogFactory.CreateLogger(source.GetType());
            return new LogDecorator<TCommand, TReceipt>(source, logger);
        }

        private sealed class LogDecorator<TCommand, TReceipt> : IHandler<TCommand, TReceipt>
            where TCommand : Command
            where TReceipt : Receipt
        {
            private readonly IHandler<TCommand, TReceipt> source;
            private readonly ILogger logger;

            public LogDecorator(IHandler<TCommand, TReceipt> source, ILogger logger)
            {
                this.source = source;
                this.logger = logger;
            }

            public Task<TReceipt> Handle(TCommand command)
            {
                try
                {
                    logger.LogDebug("Handling command {0}...", command);
                    var receipt = source.Handle(command);
                    logger.LogDebug("Handled command {0}.", command);
                    return receipt;
                }
                catch (System.Exception)
                {
                    logger.LogWarning("Failed to handle command {0}", command);
                    throw;
                }
            }
        }
    }
}

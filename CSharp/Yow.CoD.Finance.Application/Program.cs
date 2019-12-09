using Microsoft.Extensions.Logging;
using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.SqlDataAdapter;

namespace Yow.CoD.Finance.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddFilter("SampleApp.Program", LogLevel.Debug)
                       .AddConsole();
            });

            //var repo = new InMemoryRepository<Loan>();
            var connStr = @"Server=db;Database=CoD;User=sa;Password=Your_password123;";
            var repo = new SqlStreamStoreRepository(connStr);
            var createLoanHandler = new CreateLoanCommandHandler(repo);
            var disbursementHandler = new DisburseLoanFundsCommandHandler(repo);
            var takePaymentHandler = new TakePaymentCommandHandler(repo);

            var server = new Server(createLoanHandler, disbursementHandler, takePaymentHandler);
            server.Serve();
        }
    }
}

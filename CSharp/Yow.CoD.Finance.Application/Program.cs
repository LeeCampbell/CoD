using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.SqlDataAdapter;
using Yow.CoD.Finance.WebCommsAdapter;

namespace Yow.CoD.Finance.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //TODO: Get ConnStr from ENV vars or args. -LC
            var repo = new SqlStreamStoreRepository(@"Server=db;Database=CoD;User=sa;Password=Your_password123;");
            var createLoanHandler = new CreateLoanCommandHandler(repo).WithLogging();
            var disbursementHandler = new DisburseLoanFundsCommandHandler(repo).WithLogging();
            var takePaymentHandler = new TakePaymentCommandHandler(repo).WithLogging();

            var server = new Server(new RepositoryHealthCheck(repo), createLoanHandler, disbursementHandler, takePaymentHandler);
            server.Serve();
        }
    }
}

using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var repo = new InMemoryRepository<Loan>();
            var createLoanHandler = new CreateLoanCommandHandler(repo);
            var disbursementHandler = new DisburseLoanFundsCommandHandler(repo);
            var takePaymentHandler = new TakePaymentCommandHandler(repo);

            var server = new Server(createLoanHandler, disbursementHandler, takePaymentHandler);
            server.Serve();
        }
    }
}

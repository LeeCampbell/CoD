using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.WebCommsAdapter
{
    public class Server
    {
        private readonly IHandler<CreateLoanCommand, Receipt> createLoanHandler;
        private readonly IHandler<DisburseLoanFundsCommand, Receipt> disbursementHandler;
        private readonly IHandler<TakePaymentCommand, TransactionReceipt> takePaymentHandler;

        public Server(IHandler<CreateLoanCommand, Receipt> createLoanHandler,
            IHandler<DisburseLoanFundsCommand, Receipt> disbursementHandler,
            IHandler<TakePaymentCommand, TransactionReceipt> takePaymentHandler)
        {
            this.createLoanHandler = createLoanHandler;
            this.disbursementHandler = disbursementHandler;
            this.takePaymentHandler = takePaymentHandler;
        }

        public void Serve()
        {
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.TryAddSingleton(this.createLoanHandler);
                    serviceCollection.TryAddSingleton(this.disbursementHandler);
                    serviceCollection.TryAddSingleton(this.takePaymentHandler);
                })
                .Build()
                .Run();
        }
    }
}

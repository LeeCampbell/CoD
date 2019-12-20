using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Yow.CoD.Finance.WebCommsAdapter
{
    public class Server
    {
        private readonly IHealthCheck healthCheck;
        private readonly IHandler<CreateLoanCommand, Receipt> createLoanHandler;
        private readonly IHandler<DisburseLoanFundsCommand, Receipt> disbursementHandler;
        private readonly IHandler<TakePaymentCommand, TransactionReceipt> takePaymentHandler;

        public Server(
            IHealthCheck healthCheck,
            IHandler<CreateLoanCommand, Receipt> createLoanHandler,
            IHandler<DisburseLoanFundsCommand, Receipt> disbursementHandler,
            IHandler<TakePaymentCommand, TransactionReceipt> takePaymentHandler)
        {
            this.healthCheck = healthCheck;
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
                .ConfigureServices(services =>
                {
                    services.TryAddSingleton(this.createLoanHandler);
                    services.TryAddSingleton(this.disbursementHandler);
                    services.TryAddSingleton(this.takePaymentHandler);

                    services.AddHealthChecks()
                            .AddCheck("db", healthCheck);
                })
                .Build()
                .Run();
        }
    }
}

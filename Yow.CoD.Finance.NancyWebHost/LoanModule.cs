using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.NancyWebHost.Models;

namespace Yow.CoD.Finance.NancyWebHost
{
    public sealed class LoanModule : NancyModule
    {
        private readonly IHandler<CreateLoanCommand, Receipt> _createLoanCommandHandler;
        private readonly IHandler<TakePaymentCommand, TransactionReceipt> _takePaymentCommandHandler;

        public LoanModule(IHandler<CreateLoanCommand, Receipt> createLoanCommandHandler,
            IHandler<TakePaymentCommand, TransactionReceipt> takePaymentCommandHandler)
        {
            _createLoanCommandHandler = createLoanCommandHandler;
            _takePaymentCommandHandler = takePaymentCommandHandler;
            Get["/"] = p => @"cd C:\Users\Lee\Documents\GitHub\CoD\Yow.CoD.Finance.NancyWebHost" + "\r\ncurl -i -H \"Content - Type: application/json\" -X POST -d @CreateLoanExamplePayload.json http://localhost:64181/Loan";
            Post["/Loan/"] = CreateLoan;
            Post["/Loan/{id:guid}/"] = parameters => TakePayment(parameters.id);
        }

        private async Task<LoadCreatedModel> CreateLoan(object _)
        {
            var model = this.Bind<CreateLoanModel>();
            var command = model.ToCommand();
            var receipt = await _createLoanCommandHandler.Handle(command);
            return new LoadCreatedModel
            {
                LoanId = receipt.AggregateId
            };
        }

        private async Task<PaymentTakenModel> TakePayment(Guid loanId)
        {

            var model = this.Bind<LoanPaymentModel>();
            var command = model.ToCommand(loanId);

            var receipt = await _takePaymentCommandHandler.Handle(command);
            return new PaymentTakenModel{ TransactionId = receipt.TransactionId};
        }
    }
}
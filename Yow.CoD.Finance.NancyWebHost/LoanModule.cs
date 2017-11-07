using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.NancyWebHost.Models;

namespace Yow.CoD.Finance.NancyWebHost
{
    /*
     cd C:\Users\Lee\Documents\GitHub\CoD\Yow.CoD.Finance.NancyWebHost

     curl -i -H "Content-Type: application/json" -X POST -d @CreateLoanExamplePayload.json http://localhost:64181/Loan
     //Get the loanId returned and add to the suffix of this command
     curl -i -H "Content-Type: application/json" -X POST -d @TakePaymentExamplePayload.json http://localhost:64181/Loan/{GUID}

     --Powershell
     Invoke-WebRequest -ContentType "application/json" -Headers @{"accept"="application/json"} -Method Post -InFile "CreateLoanExamplePayload.json" -UseBasicParsing -Uri "http://localhost:64181/Loan"
     //Get the loanId returned and add to the suffix of this command
     Invoke-WebRequest -ContentType "application/json" -Headers @{"accept"="application/json"} -Method Post -InFile "TakePaymentExamplePayload.json" -UseBasicParsing -Uri "http://localhost:64181/Loan/{GUID}" 
    */

    public sealed class LoanModule : NancyModule
    {
        private readonly IHandler<CreateLoanCommand, Receipt> _createLoanCommandHandler;
        private readonly IHandler<TakePaymentCommand, TransactionReceipt> _takePaymentCommandHandler;

        public LoanModule(IHandler<CreateLoanCommand, Receipt> createLoanCommandHandler,
            IHandler<TakePaymentCommand, TransactionReceipt> takePaymentCommandHandler)
        {
            _createLoanCommandHandler = createLoanCommandHandler;
            _takePaymentCommandHandler = takePaymentCommandHandler;
            Get["/"] = p => @"YOW 2017 - Cost Of a Dependency";
            Post["/Loan/"] = CreateLoan;
            Post["/Loan/{id:guid}/"] = parameters => TakePayment(parameters.id);
        }

        private async Task<LoanCreatedModel> CreateLoan(object _)
        {
            var model = this.Bind<CreateLoanModel>();
            var command = model.ToCommand();
            var receipt = await _createLoanCommandHandler.Handle(command);
            return new LoanCreatedModel
            {
                LoanId = receipt.AggregateId
            };
        }

        private async Task<PaymentTakenModel> TakePayment(Guid loanId)
        {
            //Get http payload
            var model = this.Bind<LoanPaymentModel>();
            //Convert that to the Domain Model's representation
            var command = model.ToCommand(loanId);
            //Execute Domain Model logic
            var receipt = await _takePaymentCommandHandler.Handle(command);
            //Return an Acknowledgement to the client
            return new PaymentTakenModel { TransactionId = receipt.TransactionId };
        }
    }
}
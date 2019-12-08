using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Yow.CoD.Finance.AspNetCommsAdapter.Models;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.AspNetCommsAdapter.Controllers
{
    [Route("Loan")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IHandler<CreateLoanCommand, Receipt> createLoanCommandHandler;
        private readonly IHandler<TakePaymentCommand, TransactionReceipt> takePaymentCommandHandler;

        public LoansController(IHandler<CreateLoanCommand, Receipt> createLoanCommandHandler,
            IHandler<TakePaymentCommand, TransactionReceipt> takePaymentCommandHandler)
        {
            this.createLoanCommandHandler = createLoanCommandHandler;
            this.takePaymentCommandHandler = takePaymentCommandHandler;
        }
        
        [HttpPost("")]
        public async Task<ActionResult<LoanCreatedModel>> CreateLoan(CreateLoanModel model)
        {
            var command = model.ToCommand();
            var receipt = await createLoanCommandHandler.Handle(command);
            return new LoanCreatedModel
            {
                LoanId = receipt.AggregateId
            };
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<PaymentTakenModel>> TakePayment(Guid id, LoanPaymentModel model)
        {
            //Convert that to the Domain Model's representation
            var command = model.ToCommand(id);
            //Execute Domain Model logic
            var receipt = await takePaymentCommandHandler.Handle(command);
            //Return an Acknowledgement to the client
            return new PaymentTakenModel { TransactionId = receipt.TransactionId };
        }
    }
}

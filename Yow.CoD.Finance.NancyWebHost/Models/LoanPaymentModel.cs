using System;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.NancyWebHost.Models
{
    public sealed class LoanPaymentModel
    {
        public LoanPaymentModel(decimal amount)
        {
            Amount = amount;
        }
        
        public decimal Amount { get; }

        public TakePaymentCommand ToCommand(Guid loanId)
        {
            var command = new TakePaymentCommand(Guid.NewGuid(), loanId,
                this.Amount,
                DateTimeOffset.Now);
            return command;
        }
    }
}
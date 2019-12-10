using System;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.WebCommsAdapter.Models
{
    public sealed class LoanPaymentModel
    {
        public decimal Amount { get; set; }

        public TakePaymentCommand ToCommand(Guid loanId)
        {
            var command = new TakePaymentCommand(Guid.NewGuid(), loanId,
                this.Amount,
                DateTimeOffset.Now);
            return command;
        }
    }
}
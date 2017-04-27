using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;

namespace Yow.CoD.Finance.Domain.Services
{
    public class TakePaymentCommandHandler : IHandler<TakePaymentCommand>
    {
        private readonly IRepository<Loan> _repository;

        public TakePaymentCommandHandler(IRepository<Loan> repository)
        {
            _repository = repository;
        }

        public async Task Handle(TakePaymentCommand command)
        {
            var loan = await _repository.Get(command.LoanId);
            loan.TakePayment(command);
            await _repository.Save(loan);
        }
    }
}
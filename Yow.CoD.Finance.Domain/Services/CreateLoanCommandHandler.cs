using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;

namespace Yow.CoD.Finance.Domain.Services
{
    public class CreateLoanCommandHandler : IHandler<CreateLoanCommand>
    {
        private readonly IRepository<Loan> _repository;

        public CreateLoanCommandHandler(IRepository<Loan> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateLoanCommand command)
        {
            var loan = new Loan(command.LoanId);
            loan.Create(command);
            await _repository.Save(loan);
        }
    }
}

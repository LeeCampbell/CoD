using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;

namespace Yow.CoD.Finance.Domain.Services
{
    public class CreateLoanCommandHandler : IHandler<CreateLoanCommand, Receipt>
    {
        private readonly IRepository<Loan> _repository;

        public CreateLoanCommandHandler(IRepository<Loan> repository)
        {
            _repository = repository;
        }

        public async Task<Receipt> Handle(CreateLoanCommand command)
        {
            var loan = await _repository.Get(command.AggregateId);
            loan.Create(command);
            await _repository.Save(loan);
            return new Receipt(loan.Id, loan.Version);
        }
    }
}

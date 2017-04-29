using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;

namespace Yow.CoD.Finance.Domain.Services
{
    public class DisburseLoanFundsCommandHandler : IHandler<DisburseLoanFundsCommand, Receipt>
    {
        private readonly IRepository<Loan> _repository;

        public DisburseLoanFundsCommandHandler(IRepository<Loan> repository)
        {
            _repository = repository;
        }

        public async Task<Receipt> Handle(DisburseLoanFundsCommand command)
        {
            var loan = await _repository.Get(command.AggregateId);
            loan.DisburseFunds(command);
            await _repository.Save(loan);
            return new Receipt(loan.Id, loan.Version);
        }
    }
}
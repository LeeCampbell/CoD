using System.Linq;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;

namespace Yow.CoD.Finance.Domain.Services
{
    public class TakePaymentCommandHandler : IHandler<TakePaymentCommand, TransactionReceipt>
    {
        private readonly IRepository<Loan> _repository;

        public TakePaymentCommandHandler(IRepository<Loan> repository)
        {
            _repository = repository;
        }

        public async Task<TransactionReceipt> Handle(TakePaymentCommand command)
        {
            var loan = await _repository.Get(command.AggregateId);
            loan.TakePayment(command);
            var txId = loan.GetUncommitedEvents().OfType<PaymentTakenEvent>().Single().TransactionId;
            var receipt = new TransactionReceipt(loan.Id, loan.Version, txId);
            await _repository.Save(loan);
            return receipt;
        }
    }
}
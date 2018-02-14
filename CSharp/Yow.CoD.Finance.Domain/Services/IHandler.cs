using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Services
{
    public interface IHandler<in TCommand, TReceipt> 
        where TCommand : Command
        where TReceipt : Receipt
    {
        Task<TReceipt> Handle(TCommand command);
    }
}
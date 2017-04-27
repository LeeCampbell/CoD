using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Services
{
    public interface IHandler<in T> where T : Command
    {
        Task Handle(T command);
    }
}
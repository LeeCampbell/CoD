using System;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Model;

namespace Yow.CoD.Finance.Domain.Services
{
    public interface IRepository<T> where T: AggregateRoot
    {
        Task<T> Get(Guid id);
        Task Save(T item);
    }
}
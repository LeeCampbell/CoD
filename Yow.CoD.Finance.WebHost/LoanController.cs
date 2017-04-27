using System.Web.Http;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.WebHost
{
    public class LoanController : ApiController
    {
        // POST api/<controller>
        public void Post([FromBody]CreateLoanCommand command)
        {
            //Probably replace this with DI. But only this project cares about DI now.
            var cmdHandler = new CreateLoanCommandHandler(new SqlStreamStoreRepository());
            cmdHandler.Handle(command).GetAwaiter().GetResult();
        }
    }
}
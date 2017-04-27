using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.NancyWebHost.Adapters;
using Yow.CoD.Finance.NancyWebHost.Models;

namespace Yow.CoD.Finance.NancyWebHost
{
    public class LoanModule :NancyModule
    {
        public LoanModule()
        {
            Get["/"] = p => "curl -H \"Content - Type: application / json\" -X POST -d @CreateLoanExamplePayload.json http://localhost:64181/";
            Post["/"] = CreateLoan;
        }

        private async Task<HttpStatusCode> CreateLoan(object x)
        {
            var model = this.Bind<CreateLoanModel>();
            CreateLoanCommand command;
            try
            {
                command = model.ToCommand();
            }
            catch (Exception e)
            {
                return HttpStatusCode.BadRequest;
            }
            

            //Probably replace this with DI. But only this project cares about DI now.
            var cmdHandler = new CreateLoanCommandHandler(new InMemoryRepository<Loan>());
            await cmdHandler.Handle(command);

            return HttpStatusCode.Created;
        }
    }
}
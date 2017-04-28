using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.NancyWebHost.Models;

namespace Yow.CoD.Finance.NancyWebHost
{
    public sealed class LoanModule :NancyModule
    {
        private readonly IHandler<CreateLoanCommand> _createLoanCommandHandler;

        public LoanModule(IHandler<CreateLoanCommand> createLoanCommandHandler)
        {
            _createLoanCommandHandler = createLoanCommandHandler;
            Get["/"] = p => "curl -H \"Content - Type: application / json\" -X POST -d @CreateLoanExamplePayload.json http://localhost:64181/";
            Post["/"] = CreateLoan;
        }

        private async Task<HttpStatusCode> CreateLoan(object x)
        {
            CreateLoanCommand command;
            try
            {
                var model = this.Bind<CreateLoanModel>();
                command = model.ToCommand();
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }

            try
            {
                await _createLoanCommandHandler.Handle(command);
                return HttpStatusCode.Created;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
using Nancy;
using Nancy.TinyIoc;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;
using Yow.CoD.Finance.NancyWebHost.Adapters;

namespace Yow.CoD.Finance.NancyWebHost
{
    public sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IRepository<Loan>, InMemoryRepository<Loan>>().AsSingleton();
            container.Register<IHandler<CreateLoanCommand>, CreateLoanCommandHandler>().AsSingleton();
        }
    }
}
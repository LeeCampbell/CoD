using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using Yow.CoD.Finance.SqlDataAdapter;

namespace Yow.CoD.Finance.Application
{
    public sealed class RepositoryHealthCheck : IHealthCheck
    {
        private const string HealthCheckDescription = "Reports degraded status is SqlServer is not accessible";
        private readonly SqlStreamStoreRepository sqlStreamStoreRepository;

        public RepositoryHealthCheck(SqlStreamStoreRepository sqlStreamStoreRepository)
        {
            this.sqlStreamStoreRepository = sqlStreamStoreRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await sqlStreamStoreRepository.Ping(cancellationToken);
                return new HealthCheckResult(HealthStatus.Healthy, HealthCheckDescription);
            }
            catch (System.Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, HealthCheckDescription, ex);
            }
        }
    }
}

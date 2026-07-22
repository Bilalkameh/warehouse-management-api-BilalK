using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Warehouse.Presentation.HealthChecks;

public class MinioHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public MinioHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var endpoint = _configuration["Minio:Endpoint"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return HealthCheckResult.Unhealthy(
                "MinIO endpoint is not configured.");
        }

        var useSsl = _configuration.GetValue<bool>("Minio:UseSSL");
        var scheme = useSsl ? "https" : "http";

        var healthUrl = $"{scheme}://{endpoint}/minio/health/ready";

        try
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(healthUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("MinIO is ready.");
            }

            return HealthCheckResult.Unhealthy($"MinIO returned HTTP {(int)response.StatusCode}.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("MinIO could not be reached.", exception);
        }
    }
}
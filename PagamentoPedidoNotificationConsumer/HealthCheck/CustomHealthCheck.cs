using Microsoft.Extensions.Diagnostics.HealthChecks;
using PagamentoPedidoNotificationConsumer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PagamentoPedidoNotificationConsumer.HealthCheck
{
    public class CustomHealthCheck : IHealthCheck
    {
        private const int MAX_DELAY = 120;

        private readonly PagamentoPedidoConsumer _pagamentoPedidoConsumerHostedService;

        public CustomHealthCheck(PagamentoPedidoConsumer queueConsumerHostedService)
        {
            _pagamentoPedidoConsumerHostedService = queueConsumerHostedService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var lastActivity = _pagamentoPedidoConsumerHostedService.LastActivity;
            var now = DateTime.Now;
            var diff = now - lastActivity;

            if (diff.TotalSeconds > MAX_DELAY)
            {
                return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy));
            }

            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
        }
    }
}

using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoPedidoNotificationConsumer.Service
{
    public class ConsumerBackgroundService : BackgroundService
    {
        private readonly TcpListener _listener;
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<ConsumerBackgroundService> _logger;

        public ConsumerBackgroundService(HealthCheckService healthCeck, ILogger<ConsumerBackgroundService> logger, IConfiguration config)
        {
            _healthCheckService = healthCeck;
            _logger = logger;

            var port = config.GetValue<int?> ("HealthProbe:TcpPort") ?? 5000;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(300, cancellationToken);

            _logger.LogInformation("Started health check service");
            await Task.Yield();
            _listener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                await updateHeartbeatAsync(cancellationToken);
                Thread.Sleep(TimeSpan.FromSeconds(1));

                _listener.Stop();
            }
        }

        private async Task updateHeartbeatAsync(CancellationToken token)
        {
            try
            {
                var result = await _healthCheckService.CheckHealthAsync(token);
                var isHealthy = result.Status == HealthStatus.Healthy;

                if (!isHealthy)
                {
                    _listener.Stop();
                    _logger.LogWarning("Service is unhealth.Listener stopped.");
                    return;
                }

                _listener.Start();

                while (_listener.Server.IsBound && _listener.Pending())
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    client.Close();

                    _logger.LogDebug("successfully processed health check request.");
                }

                _logger.LogDebug("Heartbeat check executed.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while checking heartbeat");
            }
        }
    }
}

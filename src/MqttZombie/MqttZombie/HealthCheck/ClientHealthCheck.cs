using Microsoft.Extensions.Diagnostics.HealthChecks;
using MqttZombie.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MqttZombie.HealthCheck
{
    public class ClientHealthCheck : IHealthCheck
    {
        private readonly MqttClientFactory _mqttClientFactory;

        public ClientHealthCheck(MqttClientFactory mqttClientFactory)
        {
            _mqttClientFactory = mqttClientFactory;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_mqttClientFactory.AnyClientDisconnected())
                return Task.FromResult(HealthCheckResult.Unhealthy("Not all mqtt connection ok"));
            return Task.FromResult(HealthCheckResult.Healthy("Http connections are ok"));
        }
    }
}

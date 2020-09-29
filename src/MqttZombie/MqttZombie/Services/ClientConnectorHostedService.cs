using Microsoft.Extensions.Hosting;
using MqttZombie.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MqttZombie.Services
{
    public class ClientConnectorHostedService : IHostedService
    {
        private readonly MqttClientFactory _mqttClientFactory;
        private readonly ILogger _logger;

        public ClientConnectorHostedService(MqttClientFactory mqttClientFactory, ILogger logger)
        {
            _mqttClientFactory = mqttClientFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < ServiceOptions.MqttClientSettings.TotalClients; i++)
            {
                _mqttClientFactory.Clients.Add(new MqttClient(_logger));
            }
            _mqttClientFactory.Clients.AsParallel().ForAll(c => Task.Run(() => c.StartAsync(CancellationToken.None)));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _mqttClientFactory.Clients.AsParallel().ForAll(c => Task.Run(() => c.StopAsync(CancellationToken.None)));
            return Task.CompletedTask;
        }


    }
}

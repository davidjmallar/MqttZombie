using Microsoft.Extensions.Hosting;
using MqttZombie.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MqttZombie.Services
{
    public class MqttClientFactory : IHostedService
    {
        private readonly ILogger _logger;
        private List<MqttClient> _clients;

        public MqttClientFactory(ILogger logger)
        {
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            _clients = new List<MqttClient>();
            for (int i = 0; i < ServiceOptions.MqttClientSettings.TotalClients; i++)
            {
                _clients.Add(new MqttClient(_logger));
            }
            _clients.AsParallel().ForAll(c => Task.Run(()=>c.StartAsync(CancellationToken.None)));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _clients.AsParallel().ForAll(c => Task.Run(() => c.StopAsync(CancellationToken.None)));
            return Task.CompletedTask;
        }
    }
}

using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MqttZombie.Options;
using Prometheus;
using Serilog;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttZombie.Services
{
    public class MqttClient
    {
        private IMqttClient _mqttClient;
        private string _clientId;
        private IMqttClientOptions _options;
        private Timer _timer;
        private readonly ILogger _logger;
        private static readonly Counter _totalClientPublishes = Metrics.CreateCounter("client_publishes_total", "Total mqtt publishes from zombie clients");
        private static readonly Counter _total_reconnections = Metrics.CreateCounter("reconnections_total", "Total reconnections to mqtt broker");
        private static readonly Counter _clientPublishes = Metrics.CreateCounter("client_publishes", "Mqtt publishes count per zombie client", new CounterConfiguration()
        {
            LabelNames = new[] { "client_id" }
        });
        public bool IsConnected { get; private set; } = false;

        public MqttClient(ILogger logger)
        {
            var x = ServiceOptions.MqttClientSettings.Topic;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            _clientId = Guid.NewGuid().ToString();
            _options = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(ServiceOptions.MqttBrokerSettings.Host, ServiceOptions.MqttBrokerSettings.Port)

                .WithCredentials(ServiceOptions.MqttBrokerSettings.User, ServiceOptions.MqttBrokerSettings.Password)
                .WithTls(opt =>
                {
                    opt.UseTls = ServiceOptions.MqttBrokerSettings.Tls;
                    opt.AllowUntrustedCertificates = ServiceOptions.MqttBrokerSettings.Tls;
                })
                .WithCleanSession()
                .Build();
            _logger = logger.ForContext<MqttClient>().ForContext("ClientId", _clientId);
        }

        private void onSend(object state)
        {
            var topic = ServiceOptions.MqttClientSettings.Topic.ProcessTemplate(ServiceOptions.TemplateVariables.Variables, _clientId);
            var payload = ServiceOptions.MqttClientSettings.Payload.ProcessTemplate(ServiceOptions.TemplateVariables.Variables, _clientId);
            var messagebuilder = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload);

            if (ServiceOptions.MqttClientSettings.Qos == 1) messagebuilder.WithAtLeastOnceQoS();
            else if (ServiceOptions.MqttClientSettings.Qos == 2) messagebuilder.WithExactlyOnceQoS();
            else messagebuilder.WithAtMostOnceQoS();

            var message = messagebuilder.Build();
            
            _logger.Debug($"Zombie publishing {payload} to {topic}");
            _totalClientPublishes.Inc();
            _clientPublishes.WithLabels(_clientId).Inc();
            Task.Run(() => _mqttClient.PublishAsync(message, CancellationToken.None));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _mqttClient.UseDisconnectedHandler(async e =>
            {
                IsConnected = false;
                _logger.Information("Client reconnecting");
                _total_reconnections.Inc();
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await _mqttClient.ConnectAsync(_options, cancellationToken); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    _logger.Information("Reconnecting failed");

                }
            });
            _mqttClient.UseConnectedHandler(e =>
            {
                _logger.Information("Client connected");
                IsConnected = true;
                _timer = new Timer(onSend, null, 0, 60000 / ServiceOptions.MqttClientSettings.Frequency);
            });
            await _mqttClient.ConnectAsync(_options, cancellationToken);

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(
                    new MqttClientDisconnectOptions() { ReasonCode = MqttClientDisconnectReason.NormalDisconnection, ReasonString = "" }, cancellationToken);
                _logger.Information("Client stopped");

            }
        }
    }
}

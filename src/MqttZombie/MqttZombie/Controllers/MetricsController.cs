using Microsoft.AspNetCore.Mvc;
using MqttZombie.Services;
using Prometheus;
using System.IO;
using System.Threading.Tasks;

namespace MqttZombie.Controllers
{
    [Route("metrics")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly MqttClientFactory _mqttClientFactory;
        private static readonly Gauge _connectedClientsGauge = Metrics.CreateGauge("connected_clients", "Total number of connected clients to the MQTT");
        public MetricsController(MqttClientFactory mqttClientFactory)
        {
            _mqttClientFactory = mqttClientFactory;
            _connectedClientsGauge.Set(_mqttClientFactory.ConnectedClientsCount());
        }
        [HttpGet()]
        public async Task<ActionResult<string>> GetMetrics()
        {
            var s = new MemoryStream();
            await Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(s);
            s.Position = 0;
            var reader = new StreamReader(s);
            var ret = await reader.ReadToEndAsync();
            return ret;
        }
    }
}

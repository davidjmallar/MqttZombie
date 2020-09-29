using System.Collections.Generic;
using System.Linq;

namespace MqttZombie.Services
{
    public class MqttClientFactory
    {
        public List<MqttClient> Clients = new List<MqttClient>();

        public MqttClientFactory()
        {
        }
        
        public bool AnyClientDisconnected()
        {
            return Clients.Exists(x => x.IsConnected == false);
        }
        public int ConnectedClientsCount()
        {
            return Clients.Count(x => x.IsConnected == true);
        }
    }
}

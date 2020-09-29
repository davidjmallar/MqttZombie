using System.Collections.Generic;

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

    }
}

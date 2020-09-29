using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MqttZombie.Services
{
    public class ClientRandomMemory
    {
        private readonly Dictionary<string, string> _clientRandoms = new Dictionary<string, string>();
        public string GetClientValue(string clientId, string VariableName)
        {
            if (_clientRandoms.ContainsKey(clientId + VariableName))
            {
                return _clientRandoms[clientId + VariableName];
            }
            return null;
        }
        public void SetClientValue(string clientId, string VariableName, string value)
        {
            if (!_clientRandoms.ContainsKey(clientId + VariableName))
            {
                _clientRandoms.Add(clientId + VariableName,value);
            }
        }

    }
}

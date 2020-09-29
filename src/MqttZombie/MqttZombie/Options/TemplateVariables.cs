using System;
using System.Collections.Generic;

namespace MqttZombie.Options
{
    public class TemplateVariables
    {
        public TemplateVariable[] Variables { get; set; }
    }

    public class TemplateVariable
    {
        public string Name { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public bool ClientRandom { get; set; }

        private readonly object _lock = new object();
        private static readonly Dictionary<string, string> _clientUnique = new Dictionary<string, string>();
        private readonly Random _random = new Random();
        
        public string GetTemplateValue(string clientId)
        {
            lock (_lock)
            {
                if (_clientUnique.ContainsKey(clientId + Name))
                {
                    return _clientUnique[clientId + Name];
                }
                var min = Min.GetVariableOfString();
                var max = Max.GetVariableOfString();

                if (min is int ix && max is int iy)
                {
                    var ret = _random.Next(ix, iy + 1).ToString();
                    if (ClientRandom) _clientUnique.Add(clientId + Name, ret);
                    return ret;
                }
                if ((min is float || min is int) && (max is float || max is int))
                {
                    var fx = (float)min;
                    var fy = (float)max;
                    var ret = _random.Next(fx, fy).ToString();
                    if (ClientRandom) _clientUnique.Add(clientId + Name, ret);
                    return ret;
                }
                if (min is bool && max is bool)
                {
                    var ret = (_random.NextDouble() >= 0.5).ToString();
                    if (ClientRandom) _clientUnique.Add(clientId + Name, ret);
                    return ret;
                }
                var retval = Guid.NewGuid().ToString();
                if (ClientRandom) _clientUnique.Add(clientId + Name, retval);
                return retval;
            }
        }
    }
}

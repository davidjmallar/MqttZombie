namespace MqttZombie.Options
{
    public class MqttClientSettings
    {
        public int TotalClients { get; set; }
        public int Frequency { get; set; }
        public string Topic { get; set; }
        public string Payload { get; set; }
        public bool Retain { get; set; }
        public int Qos { get; set; }
    }
}

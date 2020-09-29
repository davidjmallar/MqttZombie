namespace MqttZombie.Options
{
    public class MqttBrokerSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tls { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}

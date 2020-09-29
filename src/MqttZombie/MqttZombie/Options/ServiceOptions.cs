using Microsoft.Extensions.Configuration;

namespace MqttZombie.Options
{
    public static class ServiceOptions
    {
        public static MqttBrokerSettings MqttBrokerSettings { get; set; } = new MqttBrokerSettings();
        public static MqttClientSettings MqttClientSettings { get; set; } = new MqttClientSettings();
        public static TemplateVariables TemplateVariables { get; set; } = new TemplateVariables();
        public static void Setup(IConfiguration configuration)
        {
            // TODO validation
            configuration.GetSection(nameof(MqttBrokerSettings)).Bind(MqttBrokerSettings);
            configuration.GetSection(nameof(MqttClientSettings)).Bind(MqttClientSettings);
            configuration.GetSection(nameof(TemplateVariables)).Bind(TemplateVariables);
        }
    }
}

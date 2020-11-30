using MqttZombie.Options;
using MqttZombie.Services;

namespace System
{
    public static class StringExtensions
    {
        public static string ProcessTemplate(this string input, TemplateVariable[] templateVariables ,string clientId)
        {
            foreach (var template in templateVariables)
            {
                input = input
                    .Replace($"{{{{{template.Name}}}}}", template.GetTemplateValue(clientId))
                    .Replace($"__{template.Name}__", template.GetTemplateValue(clientId));
            }
            return input;
        }
        public static object GetVariableOfString(this string var)
        {
            var = var?.Replace('.', ',');
            if (int.TryParse(var, out int i)) return i;

            if (float.TryParse(var, out float f)) return f;

            bool b;
            if (bool.TryParse(var, out b)) return b;

            return "";
        }
    }
}

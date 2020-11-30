using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace MqttZombie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Enum.TryParse(configuration.GetValue<string>("Logging:Console:Level") ?? "Information", out Serilog.Events.LogEventLevel consoleLevel);
            Enum.TryParse(configuration.GetValue<string>("Logging:Seq:Level") ?? "Debug", out Serilog.Events.LogEventLevel seqLevel);
            var seqHost = configuration.GetValue<string>("Logging:Seq:Host") ?? "";

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(
                restrictedToMinimumLevel: consoleLevel,
                outputTemplate: "[{Timestamp:HH:mm:ss+fff}{EventType:x8} {Level:u3}] {Message:lj}[{SourceContext}]{NewLine}{Exception}");

            if (!string.IsNullOrWhiteSpace(seqHost))
            {
                loggerConfig.WriteTo.Seq(seqHost, seqLevel);
            }

            Log.Logger = loggerConfig.CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseSerilog()
                    .UseStartup<Startup>();
                });
    }
}

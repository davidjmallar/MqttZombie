using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MqttZombie.HealthCheck;
using MqttZombie.Options;
using MqttZombie.Services;
using Prometheus;
using Serilog;

namespace MqttZombie
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped<IHostedService, MqttClient>();
            //services.AddHostedService<MqttClient>();
            services.AddSingleton(Log.Logger);
            services.AddHostedService<ClientConnectorHostedService>();
            services.AddSingleton<MqttClientFactory>();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddHealthChecks()
                .AddCheck<ClientHealthCheck>("mqtt_connection_health_check");
            ServiceOptions.Setup(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseHttpMetrics();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}

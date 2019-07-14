using System;
using System.IO;
using System.Reflection;
using ExchangeRatesGateway.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;

#pragma warning disable 1591

namespace ExchangeRatesGateway.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging((builder) => 
                    builder.AddSerilog(
                        dispose: true, 
                        logger: new LoggerConfiguration()
                            .MinimumLevel.Override("Microsoft",new Serilog.Core.LoggingLevelSwitch(LogEventLevel.Error))
                            .Enrich.FromLogContext()
                            .WriteTo.File("Logs/exchange-rates-gateway.txt", rollingInterval: RollingInterval.Day ,outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] | {Message:l}{NewLine}{Exception}")
                            .CreateLogger()))
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDomainModule();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info() {Title = "Exchange Rate Gateway", Version = "v1"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
             options.SwaggerEndpoint("/swagger/v1/swagger.json", "Exchange Rate Gateway v1");
             options.DefaultModelsExpandDepth(-1);
            });
    
            app.UseMvc();
        }
    }
}
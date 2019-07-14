using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

#pragma warning disable 1591

namespace ExchangeRatesGateway.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
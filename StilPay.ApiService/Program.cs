using AspNetCoreRateLimit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StilPay.ApiService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost webHost = CreateHostBuilder(args).Build();
            IIpPolicyStore IpPolicy = webHost.Services.GetRequiredService<IIpPolicyStore>();
            await IpPolicy.SeedAsync();
            await webHost.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }); 
    }
}

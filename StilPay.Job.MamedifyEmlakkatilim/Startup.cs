using Microsoft.Extensions.Configuration;
using StilPay.Job.MamedifyEmlakkatilim.Helpers;
using System.IO;

namespace StilPay.Job.MamedifyEmlakkatilim
{
    internal class Startup
    {
        public EmlakkatilimApiHelper EmlakkatilimApi { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            EmlakkatilimApi = config.GetSection("EmlakkatilimApi").Get<EmlakkatilimApiHelper>();
        }
    }
}
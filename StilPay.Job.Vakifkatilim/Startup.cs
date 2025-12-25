using Microsoft.Extensions.Configuration;
using StilPay.Job.Vakifkatilim.Helpers;
using System.IO;

namespace StilPay.Job.Vakifkatilim
{
    internal class Startup
    {
        public VakifkatilimApiHelper VakifkatilimApi { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            VakifkatilimApi = config.GetSection("VakifkatilimApi").Get<VakifkatilimApiHelper>();
        }
    }
}
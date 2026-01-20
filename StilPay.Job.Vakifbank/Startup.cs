using Microsoft.Extensions.Configuration;
using StilPay.Job.Vakifbank.Helpers;
using System.IO;

namespace StilPay.Job.Vakifbank
{
    internal class Startup
    {
        public VakifbankApiHelper VakifbankApi { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            VakifbankApi = config.GetSection("VakifbankApi").Get<VakifbankApiHelper>();
        }
    }
}
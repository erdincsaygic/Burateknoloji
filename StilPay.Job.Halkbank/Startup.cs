using Microsoft.Extensions.Configuration;
using StilPay.Job.Halkbank.Helpers;
using System.IO;

namespace StilPay.Job.Halkbank
{
    internal class Startup
    {
        public HalkbankApiHelper HalkbankApi { get; private set; }
        public HalkbankAccountHelper HalkbankAccount { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            HalkbankApi = config.GetSection("HalkbankApi").Get<HalkbankApiHelper>();
            HalkbankAccount = config.GetSection("HalkbankAccount").Get<HalkbankAccountHelper>();

        }
    }
}
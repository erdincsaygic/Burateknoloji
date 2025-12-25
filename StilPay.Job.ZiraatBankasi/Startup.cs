using Microsoft.Extensions.Configuration;
using StilPay.Job.ZiraatBankasi.Helpers;
using System.IO;

namespace StilPay.Job.ZiraatBankasi
{
    internal class Startup
    {
        public ZiraatApiHelper ZiraatApi { get; private set; }
        public ZiraatAccountHelper ZiraatAccount { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            ZiraatApi = config.GetSection("ZiraatApi").Get<ZiraatApiHelper>();
            ZiraatAccount = config.GetSection("ZiraatAccount").Get<ZiraatAccountHelper>();

        }
    }
}
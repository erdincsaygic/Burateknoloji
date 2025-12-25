using Microsoft.Extensions.Configuration;
using StilPay.Job.GarantiBankasi.Helpers;
using System.IO;

namespace StilPay.Job.GarantiBankasi
{
    internal class Startup
    {
        public GarantiApiHelper GarantiApi { get; private set; }
        public GarantiAuthHelper GarantiAuth { get; private set; }
        public GarantiAccountHelper GarantiAccount { get; private set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            GarantiApi = config.GetSection("GarantiApi").Get<GarantiApiHelper>();
            GarantiAuth = config.GetSection("GarantiAuth").Get<GarantiAuthHelper>();
            GarantiAccount = config.GetSection("GarantiAccount").Get<GarantiAccountHelper>();

        }
    }
}
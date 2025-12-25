using StilPay.Job.Papara.Helpers;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace StilPay.Job.Papara
{
    internal class Startup
    {
        public PaparaApiHelper PaparaApi { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            PaparaApi = config.GetSection("PaparaApi").Get<PaparaApiHelper>();
        }
    }
}

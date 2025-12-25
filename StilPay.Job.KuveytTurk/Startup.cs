using Microsoft.Extensions.Configuration;
using StilPay.Job.KuveytTurk.Helpers;
using System.IO;

namespace StilPay.Job.KuveytTurk
{
    internal class Startup
    {
        public KuveytTurkApiHelper KuveytTurkApi { get; private set; }
        public KuveytTurkTokenHelper KuveytTurkToken { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            KuveytTurkApi = config.GetSection("KuveytTurkApi").Get<KuveytTurkApiHelper>();
            KuveytTurkToken = config.GetSection("KuveytTurkToken").Get<KuveytTurkTokenHelper>();
        }
    }
}
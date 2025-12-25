using Microsoft.Extensions.Configuration;
using StilPay.Job.IsBankasi.Helpers;
using System.IO;

namespace StilPay.Job.IsBankasi
{
    internal class Startup
    {
        public IsApiHelper IsApi { get; private set; }
        public IsAccountHelper IsAuth { get; private set; }
        public IsAccountHelper IsAccount { get; private set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            IsApi = config.GetSection("IsApi").Get<IsApiHelper>();
            IsAuth = config.GetSection("IsAuth").Get<IsAccountHelper>();
            IsAccount = config.GetSection("IsAccount").Get<IsAccountHelper>();

        }
    }
}
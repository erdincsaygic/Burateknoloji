using Microsoft.Extensions.Configuration;
using StilPay.Job.GeneralBank.Helpers;
using System.IO;

namespace StilPay.Job.GeneralBank
{
    internal class Startup
    {
        public GeneralBankApiHelper GeneralApi { get; private set; }
        public GeneralBankAccountHelper GeneralBankAccount { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            GeneralApi = config.GetSection("GeneralApi").Get<GeneralBankApiHelper>();
            GeneralBankAccount = config.GetSection("GeneralAccount").Get<GeneralBankAccountHelper>();

        }
    }
}
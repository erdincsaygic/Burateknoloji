using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StilPay.Desktop.Callback
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            Application.Run(ServiceProvider.GetRequiredService<CallbackList>());
        }
        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.AddTransient<ICallbackResponseLogManager, CallbackResponseLogManager>();
                    services.AddTransient<ICallbackResponseLogDAL, CallbackResponseLogDAL>();

                    services.AddTransient<CallbackList>();
                });
        }
    }
}

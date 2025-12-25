using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StilPay.BLL.Abstract;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StilPay.ApiService.Infrastructures
{
    public class IPControlMiddleware
    {
        readonly RequestDelegate _next;
        private readonly ICompanyIntegrationManager _companyIntegrationManager;
        public IPControlMiddleware(RequestDelegate next, ICompanyIntegrationManager companyIntegrationManager)
        {
            _next = next;
            _companyIntegrationManager = companyIntegrationManager;
        }
        public async Task Invoke(HttpContext context)
        {
            //Client'ın IP adresini alıyoruz.
            IPAddress remoteIp = context.Connection.RemoteIpAddress;

            //CompanyIntegrations Tablosundaki Üye İşyerlerine Ait IPAddress'leri çekiyoruz.
            var ips = _companyIntegrationManager.GetList(null).Where(x => !string.IsNullOrEmpty(x.IPAddress)).Select(s => s.IPAddress).ToList();

            //Client IP, IPAddress Listesinde  var mı kontrol ediyoruz.
            if (!ips.Where(ip => ip.Contains(remoteIp.ToString())).Any())
            {
                //Eğer yoksa 403 hatası veriyoruz.
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync($"{remoteIp} İçin IP Yetkisi Bulunamadı. Lütfen Bizimle İletişime Geçiniz.");
                return;
            }

            await _next.Invoke(context);
        }
    }
}

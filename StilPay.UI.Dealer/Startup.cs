using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.UI.Dealer.Infrastructures;
using System;

namespace StilPay.UI.Dealer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromHours(12);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new ActionFilter());
            }).AddNewtonsoftJson();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.Name = "Cookie";
                options.LoginPath = "/Login/Index";
                options.AccessDeniedPath = "/Login/Index";
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(17);
                //options.SlidingExpiration = true;
            });

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICompanyDAL, CompanyDAL>();

            services.AddTransient<ICompanyUserManager, CompanyUserManager>();
            services.AddTransient<ICompanyUserDAL, CompanyUserDAL>();

            services.AddTransient<ICompanyCommissionManager, CompanyCommissionManager>();
            services.AddTransient<ICompanyCommissionDAL, CompanyCommissionDAL>();

            services.AddTransient<ICompanyIntegrationManager, CompanyIntegrationManager>();
            services.AddTransient<ICompanyIntegrationDAL, CompanyIntegrationDAL>();

            services.AddTransient<ICompanyApplicationManager, CompanyApplicationManager>();
            services.AddTransient<ICompanyApplicationDAL, CompanyApplicationDAL>();

            services.AddTransient<ICompanyBankManager, CompanyBankManager>();
            services.AddTransient<ICompanyBankDAL, CompanyBankDAL>();

            services.AddTransient<IPaymentNotificationManager, PaymentNotificationManager>();
            services.AddTransient<IPaymentNotificationDAL, PaymentNotificationDAL>();

            services.AddTransient<INotificationTransactionManager, NotificationTransactionManager>();
            services.AddTransient<INotificationTransactionDAL, NotificationTransactionDAL>();

            services.AddTransient<ICompanyTransactionManager, CompanyTransactionManager>();
            services.AddTransient<ICompanyTransactionDAL, CompanyTransactionDAL>();

            services.AddTransient<IBankManager, BankManager>();
            services.AddTransient<IBankDAL, BankDAL>();

            services.AddTransient<ICompanyWithdrawalRequestManager, CompanyWithdrawalRequestManager>();
            services.AddTransient<ICompanyWithdrawalRequestDAL, CompanyWithdrawalRequestDAL>();

            services.AddTransient<ICompanyBankAccountManager, CompanyBankAccountManager>();
            services.AddTransient<ICompanyBankAccountDAL, CompanyBankAccountDAL>();

            services.AddTransient<ICompanyPaymentRequestManager, CompanyPaymentRequestManager>();
            services.AddTransient<ICompanyPaymentRequestDAL, CompanyPaymentRequestDAL>();

            services.AddTransient<ICompanyRebateRequestManager, CompanyRebateRequestManager>();
            services.AddTransient<ICompanyRebateRequestDAL, CompanyRebateRequestDAL>();

            services.AddTransient<ISupportManager, SupportManager>();
            services.AddTransient<ISupportDAL, SupportDAL>();

            services.AddTransient<IMainManager, MainManager>();
            services.AddTransient<IMainDAL, MainDAL>();

            services.AddTransient<IMailManager, MailManager>();
            services.AddTransient<IMailDAL, MailDAL>();

            services.AddTransient<IAnnouncementManager, AnnouncementManager>();
            services.AddTransient<IAnnouncementDAL, AnnouncementDAL>();

            services.AddTransient<ICreditCardPaymentNotificationManager, CreditCardPaymentNotificationManager>();
            services.AddTransient<ICreditCardPaymentNotificationDAL, CreditCardPaymentNotificationDAL>();

            services.AddTransient<IForeignCreditCardPaymentNotificationManager, ForeignCreditCardPaymentNotificationManager>();
            services.AddTransient<IForeignCreditCardPaymentNotificationDAL, ForeignCreditCardPaymentNotificationDAL>();

            services.AddTransient<ISystemSettingManager, SystemSettingManager>();
            services.AddTransient<ISystemSettingDAL, SystemSettingDAL>();

            services.AddTransient<IPublicHolidayManager, PublicHolidayManager>();
            services.AddTransient<IPublicHolidayDAL, PublicHolidayDAL>();

            services.AddTransient<ISmsLogManager, SmsLogManager>();
            services.AddTransient<ISmsLogDAL, SmsLogDAL>();

            services.AddTransient<ICallbackResponseLogManager, CallbackResponseLogManager>();
            services.AddTransient<ICallbackResponseLogDAL, CallbackResponseLogDAL>();

            services.AddTransient<ICompanyPaymentInstitutionManager, CompanyPaymentInstitutionManager>();
            services.AddTransient<ICompanyPaymentInstitutionDAL, CompanyPaymentInstitutionDAL>();

            services.AddTransient<ICompanyProgressPaymentCalendarManager, CompanyProgressPaymentCalendarManager>();
            services.AddTransient<ICompanyProgressPaymentCalendarDAL, CompanyProgressPaymentCalendarDAL>();

            services.AddTransient<ICurrencyManager, CurrencyManager>();
            services.AddTransient<ICurrencyDAL, CurrencyDAL>();

            services.AddTransient<ICompanyCurrencyManager, CompanyCurrencyManager>();
            services.AddTransient<ICompanyCurrencyDAL, CompanyCurrencyDAL>();

            services.AddTransient<IMemberManager, MemberManager>();
            services.AddTransient<IMemberDAL, MemberDAL>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCookiePolicy();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "ProcessDetail",
                    pattern: "{controller=Process}/{action=Detail}/{idActionType}/{id}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=login}/{action=index}/{id?}"
                );
            });
        }
    }
}

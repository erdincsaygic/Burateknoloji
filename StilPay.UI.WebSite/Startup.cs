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
using StilPay.UI.WebSite.Areas.Panel.Infrastructures;
using System;

namespace StilPay.UI.WebSite
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
                option.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new ActionFilter());
            });

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
                options.LoginPath = "/Panel/Login/index";
                options.AccessDeniedPath = "/Panel/Login/index";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IMemberManager, MemberManager>();
            services.AddTransient<IMemberDAL, MemberDAL>();

            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICompanyDAL, CompanyDAL>();

            services.AddTransient<ICompanyIntegrationManager, CompanyIntegrationManager>();
            services.AddTransient<ICompanyIntegrationDAL, CompanyIntegrationDAL>();

            services.AddTransient<IBankManager, BankManager>();
            services.AddTransient<IBankDAL, BankDAL>();

            services.AddTransient<ICompanyBankManager, CompanyBankManager>();
            services.AddTransient<ICompanyBankDAL, CompanyBankDAL>();

            services.AddTransient<IPaymentNotificationManager, PaymentNotificationManager>();
            services.AddTransient<IPaymentNotificationDAL, PaymentNotificationDAL>();

            services.AddTransient<IMemberMoneyTransferRequestManager, MemberMoneyTransferRequestManager>();
            services.AddTransient<IMemberMoneyTransferRequestDAL, MemberMoneyTransferRequestDAL>();

            services.AddTransient<IMemberWithdrawalRequestManager, MemberWithdrawalRequestManager>();
            services.AddTransient<IMemberWithdrawalRequestDAL, MemberWithdrawalRequestDAL>();

            services.AddTransient<IMemberPaymentRequestManager, MemberPaymentRequestManager>();
            services.AddTransient<IMemberPaymentRequestDAL, MemberPaymentRequestDAL>();

            services.AddTransient<IMemberInvoiceInformationManager, MemberInvoiceInformationManager>();
            services.AddTransient<IMemberInvoiceInformationDAL, MemberInvoiceInformationDAL>();

            services.AddTransient<IMemberProcessManager, MemberProcessManager>();
            services.AddTransient<IMemberProcessDAL, MemberProcessDAL>();

            services.AddTransient<IMemberTransactionManager, MemberTransactionManager>();
            services.AddTransient<IMemberTransactionDAL, MemberTransactionDAL>();

            services.AddTransient<ISupportManager, SupportManager>();
            services.AddTransient<ISupportDAL, SupportDAL>();

            services.AddTransient<ICityManager, CityManager>();
            services.AddTransient<ICityDAL, CityDAL>();

            services.AddTransient<ITownManager, TownManager>();
            services.AddTransient<ITownDAL, TownDAL>();

            services.AddTransient<ICreditCardPaymentNotificationManager, CreditCardPaymentNotificationManager>();
            services.AddTransient<ICreditCardPaymentNotificationDAL, CreditCardPaymentNotificationDAL>();

            services.AddTransient<IForeignCreditCardPaymentNotificationManager, ForeignCreditCardPaymentNotificationManager>();
            services.AddTransient<IForeignCreditCardPaymentNotificationDAL, ForeignCreditCardPaymentNotificationDAL>();

            services.AddTransient<IBlogManager, BlogManager>();
            services.AddTransient<IBlogDAL, BlogDAL>();

            services.AddTransient<IBlogCategoryManager, BlogCategoryManager>();
            services.AddTransient<IBlogCategoryDAL, BlogCategoryDAL>();

            services.AddTransient<ISystemSettingManager, SystemSettingManager>();
            services.AddTransient<ISystemSettingDAL, SystemSettingDAL>();

            services.AddTransient<IPublicHolidayManager, PublicHolidayManager>();
            services.AddTransient<IPublicHolidayDAL, PublicHolidayDAL>();

            services.AddTransient<ISmsLogManager, SmsLogManager>();
            services.AddTransient<ISmsLogDAL, SmsLogDAL>();

            services.AddTransient<ICallbackResponseLogManager, CallbackResponseLogManager>();
            services.AddTransient<ICallbackResponseLogDAL, CallbackResponseLogDAL>();

            services.AddTransient<IPaymentInstitutionManager, PaymentInstitutionManager>();
            services.AddTransient<IPaymentInstitutionDAL, PaymentInstitutionDAL>();

            services.AddTransient<ICompanyPaymentInstitutionManager, CompanyPaymentInstitutionManager>();
            services.AddTransient<ICompanyPaymentInstitutionDAL, CompanyPaymentInstitutionDAL>();

            services.AddTransient<ICompanyBankAccountManager, CompanyBankAccountManager>();
            services.AddTransient<ICompanyBankAccountDAL, CompanyBankAccountDAL>();

            services.AddTransient<ISliderManager, SliderManager>();
            services.AddTransient<ISliderDAL, SliderDAL>();

            services.AddTransient<ICurrencyManager, CurrencyManager>();
            services.AddTransient<ICurrencyDAL, CurrencyDAL>();

            services.AddTransient<ICompanyCurrencyManager, CompanyCurrencyManager>();
            services.AddTransient<ICompanyCurrencyDAL, CompanyCurrencyDAL>();

            services.AddTransient<IMemberTypeManager, MemberTypeManager>();
            services.AddTransient<IMemberTypeDAL, MemberTypeDAL>();

            services.AddTransient<ICompanyFraudControlManager, CompanyFraudControlManager>();
            services.AddTransient<ICompanyFraudControlDAL, CompanyFraudControlDAL>();

            services.AddTransient<ICustomerInfoManager, CustomerInfoManager>();
            services.AddTransient<ICustomerInfoDAL, CustomerInfoDAL>();

            services.AddTransient<IPaymentTransferPoolDescriptionControlManager, PaymentTransferPoolDescriptionControlManager>();
            services.AddTransient<IPaymentTransferPoolDescriptionControlDAL, PaymentTransferPoolDescriptionControlDAL>();
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
                endpoints.MapAreaControllerRoute(
                    name: "Panel",
                    areaName: "Panel",
                    pattern: "Panel/{controller=Login}/{action=Index}/{id?}"
                );

                endpoints.MapAreaControllerRoute(
                    name: "MasterDetail",
                    areaName: "Panel",
                    pattern: "Panel/{controller=Master}/{action=Detail}/{idActionType}/{id}"
                );

                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern: "{controller=anasayfa}/{action=index}/{id?}"
                );
            });
        }
    }
}

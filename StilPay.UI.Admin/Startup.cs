using Coravel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.BLL.Jobs;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.UI.Admin.Infrastructures;
using System;

namespace StilPay.UI.Admin
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
                config.Filters.Add(new ActionFilter(new AdministratorManager(new AdministatorDAL())));
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
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(4);
                //options.Cookie.MaxAge = options.ExpireTimeSpan;
                options.SlidingExpiration = false;
            });

            //services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IAdministratorManager, AdministratorManager>();
            services.AddTransient<IAdministratorDAL, AdministatorDAL>();

            services.AddTransient<IMemberManager, MemberManager>();
            services.AddTransient<IMemberDAL, MemberDAL>();

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

            services.AddTransient<ICompanyPaymentRequestManager, CompanyPaymentRequestManager>();
            services.AddTransient<ICompanyPaymentRequestDAL, CompanyPaymentRequestDAL>();

            services.AddTransient<ICompanyWithdrawalRequestManager, CompanyWithdrawalRequestManager>();
            services.AddTransient<ICompanyWithdrawalRequestDAL, CompanyWithdrawalRequestDAL>();

            services.AddTransient<ICompanyTransactionManager, CompanyTransactionManager>();
            services.AddTransient<ICompanyTransactionDAL, CompanyTransactionDAL>();

            services.AddTransient<ICompanyRebateRequestManager, CompanyRebateRequestManager>();
            services.AddTransient<ICompanyRebateRequestDAL, CompanyRebateRequestDAL>();

            services.AddTransient<IMemberPaymentRequestManager, MemberPaymentRequestManager>();
            services.AddTransient<IMemberPaymentRequestDAL, MemberPaymentRequestDAL>();

            services.AddTransient<IMemberWithdrawalRequestManager, MemberWithdrawalRequestManager>();
            services.AddTransient<IMemberWithdrawalRequestDAL, MemberWithdrawalRequestDAL>();

            services.AddTransient<IMemberMoneyTransferRequestManager, MemberMoneyTransferRequestManager>();
            services.AddTransient<IMemberMoneyTransferRequestDAL, MemberMoneyTransferRequestDAL>();

            services.AddTransient<IMemberTransactionManager, MemberTransactionManager>();
            services.AddTransient<IMemberTransactionDAL, MemberTransactionDAL>();

            services.AddTransient<ISupportManager, SupportManager>();
            services.AddTransient<ISupportDAL, SupportDAL>();

            services.AddTransient<IMainManager, MainManager>();
            services.AddTransient<IMainDAL, MainDAL>();

            services.AddTransient<IAnnouncementManager, AnnouncementManager>();
            services.AddTransient<IAnnouncementDAL, AnnouncementDAL>();

            services.AddTransient<IMemberTypeManager, MemberTypeManager>();
            services.AddTransient<IMemberTypeDAL, MemberTypeDAL>();

            services.AddTransient<IBlogManager, BlogManager>();
            services.AddTransient<IBlogDAL, BlogDAL>();

            services.AddTransient<IBlogCategoryManager, BlogCategoryManager>();
            services.AddTransient<IBlogCategoryDAL, BlogCategoryDAL>();

            services.AddTransient<IMailManager, MailManager>();
            services.AddTransient<IMailDAL, MailDAL>();

            services.AddTransient<ICreditCardPaymentNotificationManager, CreditCardPaymentNotificationManager>();
            services.AddTransient<ICreditCardPaymentNotificationDAL, CreditCardPaymentNotificationDAL>();

            services.AddTransient<IForeignCreditCardPaymentNotificationManager, ForeignCreditCardPaymentNotificationManager>();
            services.AddTransient<IForeignCreditCardPaymentNotificationDAL, ForeignCreditCardPaymentNotificationDAL>();

            services.AddTransient<ICompanyInvoiceManager, CompanyInvoiceManager>();
            services.AddTransient<ICompanyInvoiceDAL, CompanyInvoiceDAL>();

            services.AddTransient<IActionTypeDAL, ActionTypeDAL>();

            services.AddTransient<ISettingDAL, SettingDAL>();

            services.AddTransient<ISystemSettingManager, SystemSettingManager>();
            services.AddTransient<ISystemSettingDAL, SystemSettingDAL>();

            services.AddTransient<IPublicHolidayManager, PublicHolidayManager>();
            services.AddTransient<IPublicHolidayDAL, PublicHolidayDAL>();

            services.AddTransient<IBankManager, BankManager>();
            services.AddTransient<IBankDAL, BankDAL>();

            services.AddTransient<ICompanyBankAccountManager, CompanyBankAccountManager>();
            services.AddTransient<ICompanyBankAccountDAL, CompanyBankAccountDAL>();

            services.AddTransient<ICompanyFinanceTransactionManager, CompanyFinanceTransactionManager>();
            services.AddTransient<ICompanyFinanceTransactionDAL, CompanyFinanceTransactionDAL>();

            services.AddTransient<ICallbackResponseLogManager, CallbackResponseLogManager>();
            services.AddTransient<ICallbackResponseLogDAL, CallbackResponseLogDAL>();

            services.AddTransient<IPaymentTransferPoolManager, PaymentTransferPoolManager>();
            services.AddTransient<IPaymentTransferPoolDAL, PaymentTransferPoolDAL>();

            services.AddTransient<IPaymentCreditCardPoolManager, PaymentCreditCardPoolManager>();
            services.AddTransient<IPaymentCreditCardPoolDAL, PaymentCreditCardPoolDAL>();

            services.AddTransient<IAccountSummaryManager, AccountSummaryManager>();
            services.AddTransient<IAccountSummaryDAL, AccountSummaryDAL>();

            services.AddTransient<IBankBalancesLogManager, BankBalancesLogManager>();
            services.AddTransient<IBankBalancesLogDAL, BankBalancesLogDAL>();

            services.AddTransient<IPaymentInstitutionManager, PaymentInstitutionManager>();
            services.AddTransient<IPaymentInstitutionDAL, PaymentInstitutionDAL>();

            services.AddTransient<ICreditCardAccountSummaryReportDetailManager, CreditCardAccountSummaryReportDetailManager>();
            services.AddTransient<ICreditCardAccountSummaryReportDetailDAL, CreditCardAccountSummaryReportDetailDAL>();

            services.AddTransient<IBankTransferAccountSummaryReportDetailManager, BankTransferAccountSummaryReportDetailManager>();
            services.AddTransient<IBankTransferAccountSummaryReportDetailDAL, BankTransferAccountSummaryReportDetailDAL>();

            services.AddTransient<ICompanyPaymentInstitutionManager, CompanyPaymentInstitutionManager>();
            services.AddTransient<ICompanyPaymentInstitutionDAL, CompanyPaymentInstitutionDAL>();

            services.AddTransient<IWithdrawalPoolManager, WithdrawalPoolManager>();
            services.AddTransient<IWithdrawalPoolDAL, WithdrawalPoolDAL>();

            services.AddTransient<IProgressPaymentCalendarManager, ProgressPaymentCalendarManager>();
            services.AddTransient<IProgressPaymentCalendarDAL, ProgressPaymentCalendarDAL>();

            services.AddTransient<ICompanyProgressPaymentCalendarManager, CompanyProgressPaymentCalendarManager>();
            services.AddTransient<ICompanyProgressPaymentCalendarDAL, CompanyProgressPaymentCalendarDAL>();

            services.AddTransient<ISliderManager, SliderManager>();
            services.AddTransient<ISliderDAL, SliderDAL>();

            services.AddTransient<ICurrencyManager, CurrencyManager>();
            services.AddTransient<ICurrencyDAL, CurrencyDAL>();

            services.AddTransient<ICompanyCurrencyManager, CompanyCurrencyManager>();
            services.AddTransient<ICompanyCurrencyDAL, CompanyCurrencyDAL>();

            services.AddTransient<ICompanyFraudControlManager, CompanyFraudControlManager>();
            services.AddTransient<ICompanyFraudControlDAL, CompanyFraudControlDAL>();

            services.AddTransient<ICustomerInfoManager, CustomerInfoManager>();
            services.AddTransient<ICustomerInfoDAL, CustomerInfoDAL>();

            services.AddTransient<IMailLogManager, MailLogManager>();
            services.AddTransient<IMailLogDAL, MailLogDAL>();

            services.AddTransient<ITodoManager, TodoManager>();
            services.AddTransient<ITodoDAL, TodoDAL>();

            services.AddTransient<IPaymentTransferPoolDescriptionControlManager, PaymentTransferPoolDescriptionControlManager>();
            services.AddTransient<IPaymentTransferPoolDescriptionControlDAL, PaymentTransferPoolDescriptionControlDAL>();

            services.AddTransient<ICompanyAutoNotificationSettingManager, CompanyAutoNotificationSettingManager>();
            services.AddTransient<ICompanyAutoNotificationSettingDAL, CompanyAutoNotificationSettingDAL>();
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
                    name: "DealerUserDetail",
                    pattern: "{controller=DealerUser}/{action=DealerUserDetail}/{id}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=login}/{action=index}/{id?}"
                );
            });
        }
    }
}

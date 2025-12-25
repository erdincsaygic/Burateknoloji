using AspNetCoreRateLimit;
using Coravel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using StilPay.ApiService.Infrastructures;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.BLL.Jobs;
using StilPay.BLL.Jobs.CreditCardPayPool;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using System;
using System.Text;

namespace StilPay.ApiService
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
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StilPay-Services", Version = "v1" });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

               .AddJwtBearer((options) =>
               {
                   options.RequireHttpsMetadata = false;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true, 
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                   };
               });

            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            //services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddTransient<ICompanyIntegrationManager, CompanyIntegrationManager>();
            services.AddTransient<ICompanyIntegrationDAL, CompanyIntegrationDAL>();

            services.AddTransient<ICompanyBankManager, CompanyBankManager>();
            services.AddTransient<ICompanyBankDAL, CompanyBankDAL>();

            services.AddTransient<ICompanyWithdrawalRequestManager, CompanyWithdrawalRequestManager>();
            services.AddTransient<ICompanyWithdrawalRequestDAL, CompanyWithdrawalRequestDAL>();

            services.AddTransient<IPublicHolidayManager, PublicHolidayManager>();
            services.AddTransient<IPublicHolidayDAL, PublicHolidayDAL>();

            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICompanyDAL, CompanyDAL>();

            services.AddTransient<ICompanyCommissionManager, CompanyCommissionManager>();
            services.AddTransient<ICompanyCommissionDAL, CompanyCommissionDAL>();

            services.AddTransient<IBankManager, BankManager>();
            services.AddTransient<IBankDAL, BankDAL>();

            services.AddTransient<ISystemSettingManager, SystemSettingManager>();
            services.AddTransient<ISystemSettingDAL, SystemSettingDAL>();

            services.AddTransient<ICompanyRebateRequestManager, CompanyRebateRequestManager>();
            services.AddTransient<ICompanyRebateRequestDAL, CompanyRebateRequestDAL>();

            services.AddTransient<ICreditCardPaymentNotificationManager, CreditCardPaymentNotificationManager>();
            services.AddTransient<ICreditCardPaymentNotificationDAL, CreditCardPaymentNotificationDAL>();

            services.AddTransient<IForeignCreditCardPaymentNotificationManager, ForeignCreditCardPaymentNotificationManager>();
            services.AddTransient<IForeignCreditCardPaymentNotificationDAL, ForeignCreditCardPaymentNotificationDAL>();

            services.AddTransient<IPaymentNotificationManager, PaymentNotificationManager>();
            services.AddTransient<IPaymentNotificationDAL, PaymentNotificationDAL>();

            services.AddTransient<ICallbackResponseLogManager, CallbackResponseLogManager>();
            services.AddTransient<ICallbackResponseLogDAL, CallbackResponseLogDAL>();

            services.AddTransient<ICompanyTransactionManager, CompanyTransactionManager>();
            services.AddTransient<ICompanyTransactionDAL, CompanyTransactionDAL>();

            services.AddTransient<IPaymentCreditCardPoolManager, PaymentCreditCardPoolManager>();
            services.AddTransient<IPaymentCreditCardPoolDAL, PaymentCreditCardPoolDAL>();

            services.AddTransient<ICompanyPaymentInstitutionManager, CompanyPaymentInstitutionManager>();
            services.AddTransient<ICompanyPaymentInstitutionDAL, CompanyPaymentInstitutionDAL>();

            services.AddTransient<IAccountSummaryManager, AccountSummaryManager>();
            services.AddTransient<IAccountSummaryDAL, AccountSummaryDAL>();

            services.AddTransient<IPaymentInstitutionManager, PaymentInstitutionManager>();
            services.AddTransient<IPaymentInstitutionDAL, PaymentInstitutionDAL>();

            services.AddTransient<ICreditCardAccountSummaryReportDetailManager, CreditCardAccountSummaryReportDetailManager>();
            services.AddTransient<ICreditCardAccountSummaryReportDetailDAL, CreditCardAccountSummaryReportDetailDAL>();

            services.AddTransient<IBankTransferAccountSummaryReportDetailManager, BankTransferAccountSummaryReportDetailManager>();
            services.AddTransient<IBankTransferAccountSummaryReportDetailDAL, BankTransferAccountSummaryReportDetailDAL>();

            services.AddTransient<ICompanyBankAccountManager, CompanyBankAccountManager>();
            services.AddTransient<ICompanyBankAccountDAL, CompanyBankAccountDAL>();

            services.AddTransient<IMemberManager, MemberManager>();
            services.AddTransient<IMemberDAL, MemberDAL>();

            services.AddTransient<IPaymentInstitutionManager, PaymentInstitutionManager>();
            services.AddTransient<IPaymentInstitutionDAL, PaymentInstitutionDAL>();

            services.AddTransient<IWithdrawalPoolManager, WithdrawalPoolManager>();
            services.AddTransient<IWithdrawalPoolDAL, WithdrawalPoolDAL>();

            services.AddTransient<ICurrencyManager, CurrencyManager>();
            services.AddTransient<ICurrencyDAL, CurrencyDAL>();

            services.AddTransient<ICompanyCurrencyManager, CompanyCurrencyManager>();
            services.AddTransient<ICompanyCurrencyDAL, CompanyCurrencyDAL>();

            services.AddTransient<ICustomerInfoManager, CustomerInfoManager>();
            services.AddTransient<ICustomerInfoDAL, CustomerInfoDAL>();

            services.AddTransient<ICompanyTransactionManager, CompanyTransactionManager>();
            services.AddTransient<ICompanyTransactionDAL, CompanyTransactionDAL>();

            services.AddCors();

            #region Scheduler Jobs

            services.AddScheduler();

            #region Çekim Taleplerini Ýþleme Alan Job
            services.AddScoped<AutoWithdrawal>();
            #endregion

            #region 15 Dakika Ýçerisinde Tamamlanmayan Kredi Kartý Ýþlemlerini Ýptal Eden Job
            services.AddScoped<CreditCardAutoCancel>();
            #endregion

            #region Üye Ýþyeri Bakiye ve Cari Hareket Farkýný Kontrol Eden Job
            services.AddScoped<DealerBalanceControl>();
            #endregion

            #region 5 Dakikada Bir Ýþlemleri Karþý Tarafa Gönderen Job
            services.AddScoped<AutoCallBack>();
            #endregion

            #region Kredi Kartý Havuzuna Kayýtlarý Aktaran Job 
            //services.AddScoped<PayNKolay>();
            //services.AddScoped<AKODE>();
            services.AddScoped<Paybull>();
            services.AddScoped<EsnekPos>();
            services.AddScoped<LidioPos>();
            services.AddScoped<LidioPosYD>();
            #endregion

            #region Bekliyor Konumunda Kalan Kredi Kartý Ýþlemlerini Sorgulayan Job
            services.AddScoped<CreditCardCheckStatus>(); 
            services.AddScoped<ForeignCreditCardCheckStatus>();
            #endregion

            #region Aylýk Hesap Özeti Oluþturan Job
            services.AddScoped<CreateMonthlyAccountReport>();
            #endregion

            //#region Giden Hesaplarý Sorgulayýp Havuza Aktaran Job
            //services.AddScoped<WithdrawalPoolJob>();
            //#endregion

            #endregion Scheduler Jobs
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            //app.UseMiddleware<IPControlMiddleware>();

            app.UseWhen(
            predicate: context => context.Request.Path.StartsWithSegments("/api/transfer"),
            configuration: builder =>
            {
                builder.UseMiddleware<IPControlMiddleware>();
            });

            app.UseRouting();
            app.UseIpRateLimiting();
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Scheluder Zamaný Ayar
            app.ApplicationServices.UseScheduler(scheduler =>
            {

                #region Çekim Taleplerini Ýþleme Alan Job Saat Ayarý
                scheduler.Schedule<AutoWithdrawal>().Cron("*/2 * * * *");
                #endregion

                #region 15 Dakika Ýçerisinde Tamamlanmayan Kredi Kartý Ýþlemlerini Ýptal Eden Job Saat Ayarý
                scheduler.Schedule<CreditCardAutoCancel>().EveryMinute();
                #endregion

                #region Üye Ýþyeri Bakiye ve Cari Hareket Farkýný Kontrol Eden Job Saat Ayarý
                scheduler.Schedule<DealerBalanceControl>().EveryThirtyMinutes();
                #endregion

                #region 5 Dakikada Bir Ýþlemleri Karþý Tarafa Gönderen Job Saat Ayarý
                //scheduler.Schedule<AutoCallBack>().EveryFiveMinutes();
                #endregion

                #region Kredi Kartý Havuzuna Kayýtlarý Aktaran Job Saat Ayarý
                //scheduler.Schedule<PayNKolay>().EveryFiveMinutes();
                //scheduler.Schedule<AKODE>().EveryFiveMinutes();
                scheduler.Schedule<Paybull>().EveryFiveMinutes();
                scheduler.Schedule<EsnekPos>().EveryFiveMinutes();
                scheduler.Schedule<LidioPos>().EveryFiveMinutes();
                scheduler.Schedule<LidioPosYD>().EveryFiveMinutes();
                #endregion

                #region Bekliyor Konumunda Kalan Kredi Kartý Ýþlemlerini Sorgulayan Job Saat Ayarý
                scheduler.Schedule<CreditCardCheckStatus>().Cron("*/4 * * * *");
                scheduler.Schedule<ForeignCreditCardCheckStatus>().Cron("*/4 * * * *");
                #endregion

                #region Aylýk Hesap Özeti Oluþturan Job Saat Ayarý
                scheduler.Schedule<CreateMonthlyAccountReport>().Cron("0 21 * * *");
                scheduler.Schedule<CreateMonthlyAccountReport>().Cron("59 20 * * *");
                scheduler.Schedule<CreateMonthlyAccountReport>().Cron("01 21 * * *");
                #endregion

                //#region Giden Hesaplarý Sorgulayýp Havuza Aktaran Job Saat Ayarý

                //string cronExpression = Configuration.GetValue<string>("SchedulerSettings:WithdrawalPoolSchedule");

                //scheduler.Schedule<WithdrawalPoolJob>().Cron(cronExpression);
                //#endregion

            });
            #endregion
        }
    }
}

using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Utility.PayNKolay.Models.PaymentList;
using System.Security.Cryptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using StilPay.Utility.PayNKolay;
using StilPay.Entities.Concrete;
using static StilPay.Utility.Helper.Enums;
using RestSharp;
using static StilPay.Utility.Parasut.Models.InvoiceResponseModel;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using StilPay.Utility.Helper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml;
using System.Globalization;

namespace StilPay.BLL.Jobs.CreditCardPayPool
{
    public class PayNKolay : IInvocable
    {
        public readonly IPaymentCreditCardPoolManager _paymentCreditCardPoolManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly ISystemSettingManager _systemSettingManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public PayNKolay(IPaymentCreditCardPoolManager paymentCreditCardPoolManager, ICompanyManager companyManager, ISystemSettingManager systemSettingManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager)
        {
            _paymentCreditCardPoolManager = paymentCreditCardPoolManager;
            _companyManager = companyManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _systemSettingManager = systemSettingManager;
        }

        public async Task Invoke()
        {
            var payNKolayIntegrationValues = _settingDAL.GetList(null).Where(x => x.ParamType == "PayNKolayCreditCard").ToList();

            var endDate = DateTime.Now; 
            var startDate = DateTime.Now;

            var sx = payNKolayIntegrationValues.FirstOrDefault(f => f.ParamDef == "list_sx").ParamVal;
            var secretKey = payNKolayIntegrationValues.FirstOrDefault(f => f.ParamDef == "mercant_secret_key").ParamVal;

            String str = sx + startDate.ToString("dd.MM.yyyy") + endDate.ToString("dd.MM.yyyy") + secretKey;
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            byte[] hashingbytes = sha.ComputeHash(bytes);
            String hashData = Convert.ToBase64String(hashingbytes);

            var paymentListRequestModel = new PaymentListRequestModel
            {
                Sx = sx,
                SecretKey = secretKey,
                StartDate = startDate.ToString("dd.MM.yyyy"),
                EndDate = endDate.ToString("dd.MM.yyyy"),
                HashData = hashData
            };

            var response = PayNKolayPaymentList.PaymentListRequest(paymentListRequestModel);

            if(response.Status == "OK" && response.Data != null && response.Data.result != null && response.Data.result.LIST != null && response.Data.result.RESPONSE_CODE == 2)
            {
                foreach (var item in response.Data.result.LIST.OrderByDescending(o => Convert.ToDateTime(o.TRX_DATE)))
                {
                    if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.REFERENCE_CODE))
                    {
                        var paymentCreditCardPool = new PaymentCreditCardPool
                        {
                            Amount = decimal.Parse(item.TRANSACTION_AMOUNT, CultureInfo.InvariantCulture),
                            BankName = item.BANK_NAME,
                            Commission = Convert.ToDecimal(item.COMMISION, CultureInfo.InvariantCulture),
                            InstallmentCount = int.Parse(item.INSTALLMENT_COUNT),
                            PaymentMethodID = (int)CreditCardPaymentMethodType.PayNKolay,
                            PaymentMethodName = "PayNKolay",
                            SenderName = item.CARD_HOLDER_NAME,
                            TransactionDate = Convert.ToDateTime(item.TRX_DATE),
                            TransactionType = item.TRANSACTION_TYPE == "SALES" ? "SATIS" : item.TRANSACTION_TYPE == "CANCEL" ? "IPTAL" : item.TRANSACTION_TYPE == "REFUND" ? "IADE" : item.TRANSACTION_TYPE == "REFUNDP" ? "PARCALI IADE" : item.TRANSACTION_TYPE,
                            Status = (byte)(item.STATUS == "SUCCESS" ? (byte)Enums.StatusType.Confirmed : item.STATUS == "ERROR" ? (byte)Enums.StatusType.Canceled : item.STATUS == "NEW" ? (byte)Enums.StatusType.Pending : 0),
                            TransactionKey = item.REFERENCE_CODE,
                            TransactionID = item.CLIENT_REFERENCE_CODE
                        };

                        _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                    }                 
                }
            }
        }
    }
}

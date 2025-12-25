using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Paybull.PaybullGetTransactions;
using StilPay.Utility.Paybull;
using System;
using System.Collections.Generic;
using System.Text;
using static StilPay.Utility.Helper.Enums;
using System.Threading.Tasks;
using System.Linq;
using StilPay.Utility.AKODESanalPOS.Models.AKODEGetTransactions;
using StilPay.Utility.AKODESanalPOS.Models.AKODECreditCardInfo;
using StilPay.Utility.AKODESanalPOS;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;

namespace StilPay.BLL.Jobs.CreditCardPayPool
{
    public class AKODE : IInvocable
    {
        public readonly IPaymentCreditCardPoolManager _paymentCreditCardPoolManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public AKODE(IPaymentCreditCardPoolManager paymentCreditCardPoolManager, ICompanyManager companyManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, IPaymentInstitutionManager paymentInstitutionManager)
        {
            _paymentCreditCardPoolManager = paymentCreditCardPoolManager;
            _companyManager = companyManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public async Task Invoke()
        {
            var sendReq = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == ((byte)Enums.CreditCardPaymentMethodType.AKODE).ToString()).IsActive;

            if (sendReq)
            {
                string previousDay = null;
                DateTime dateNow = DateTime.Now;
                TimeSpan timeLimit = new TimeSpan(00, 5, 0);

                if (dateNow.TimeOfDay < timeLimit)
                    previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                else
                    previousDay = null;


                var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

                if (previousDay != null)
                {
                    var randomGeneratorPrevious = new Random();
                    var rndPrevious = randomGeneratorPrevious.Next(1, 1000000).ToString();

                    var akOdePreviousGetTransactionRequestModel = new AKODEGetTransactionRequestModel()
                    {
                        ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                        ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                        Rnd = rndPrevious,
                        TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        TransactionDate = int.Parse(DateTime.Today.AddDays(-1).ToString("yyyyMMdd")),
                        Page = 1,
                        PageSize = int.MaxValue,
                    };

                    akOdePreviousGetTransactionRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdePreviousGetTransactionRequestModel.ClientId, akOdePreviousGetTransactionRequestModel.ApiUser, akOdePreviousGetTransactionRequestModel.Rnd, akOdePreviousGetTransactionRequestModel.TimeSpan);

                    var responsePrevious = AKODEGetTransactionRequest.GetTransactionList(akOdePreviousGetTransactionRequestModel);

                    if (responsePrevious.Status == "OK" && responsePrevious.Data != null && responsePrevious.Data.Transactions != null)
                    {
                        foreach (var item in responsePrevious.Data.Transactions.Where(x => x.TransactionType == 1 && x.RequestStatus != 11 && DateTime.ParseExact(x.CreateDate, "yyyyMMddHHmmss", null) >= DateTime.Now.AddHours(-1)))
                        {
                            var formatStatus = item.RequestStatus == 1 ? (byte)Enums.StatusType.Confirmed
                                : item.RequestStatus == 0 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;

                            var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar, item.OrderId) });

                            if (entity != null && entity.Status != formatStatus)
                            {
                                _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.OrderId, formatStatus);
                            }

                            if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.OrderId))
                            {
                                var paymentCreditCardPool = new PaymentCreditCardPool
                                {
                                    Amount = Convert.ToDecimal(item.Amount) / 100,
                                    BankName = item.CardBankId.ToString(),
                                    Commission = (decimal)(item.MerchantCommissionRate != null ? item.MerchantCommissionRate : 0.0M),
                                    InstallmentCount = item.InstallmentCount,
                                    PaymentMethodID = (int)CreditCardPaymentMethodType.AKODE,
                                    PaymentMethodName = "AKODE",
                                    SenderName = null,
                                    TransactionDate = DateTime.ParseExact(item.CreateDate, "yyyyMMddHHmmss", null),
                                    TransactionType = item.TransactionType == 1 ? "SATIS" : item.TransactionType == 4 ? "IPTAL" : "IADE",
                                    Status = formatStatus,
                                    TransactionKey = item.OrderId,
                                    TransactionID = item.OrderId,
                                    Description = item.BankResponseMessage ?? item.Message
                                };

                                var resp = _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                            }
                        }
                    }
                }

                else
                {
                    var randomGenerator = new Random();
                    var rnd = randomGenerator.Next(1, 1000000).ToString();
                    var akOdeGetTransactionRequestModel = new AKODEGetTransactionRequestModel()
                    {
                        ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                        ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                        Rnd = rnd,
                        TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        TransactionDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                        Page = 1,
                        PageSize = int.MaxValue,
                    };

                    akOdeGetTransactionRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeGetTransactionRequestModel.ClientId, akOdeGetTransactionRequestModel.ApiUser, akOdeGetTransactionRequestModel.Rnd, akOdeGetTransactionRequestModel.TimeSpan);

                    var response = AKODEGetTransactionRequest.GetTransactionList(akOdeGetTransactionRequestModel);

                    if (response.Status == "OK" && response.Data != null && response.Data.Transactions != null)
                    {
                        foreach (var item in response.Data.Transactions.Where(x => x.TransactionType == 1 && x.RequestStatus != 11 && DateTime.ParseExact(x.CreateDate, "yyyyMMddHHmmss", null) >= DateTime.Now.AddHours(-1)))
                        {
                            var formatStatus = item.RequestStatus == 1 ? (byte)Enums.StatusType.Confirmed 
                                : item.RequestStatus == 0 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;

                            var entity = _paymentCreditCardPoolManager.GetSingle(new List<FieldParameter>() { new FieldParameter("TransactionKey", FieldType.NVarChar,item.OrderId) });

                            if (entity != null && entity.Status != formatStatus)
                            {
                                _paymentCreditCardPoolManager.CheckStatusAndUpdate(item.OrderId, formatStatus);
                            }

                            if (!_paymentCreditCardPoolManager.CheckTransactionKey(item.OrderId))
                            {
                                var paymentCreditCardPool = new PaymentCreditCardPool
                                {
                                    Amount = Convert.ToDecimal(item.Amount) / 100,
                                    BankName = item.CardBankId.ToString(),
                                    Commission = (decimal)(item.MerchantCommissionRate != null ? item.MerchantCommissionRate : 0.0M),
                                    InstallmentCount = item.InstallmentCount,
                                    PaymentMethodID = (int)CreditCardPaymentMethodType.AKODE,
                                    PaymentMethodName = "AKODE",
                                    SenderName = null,
                                    TransactionDate = DateTime.ParseExact(item.CreateDate, "yyyyMMddHHmmss", null),
                                    TransactionType = item.TransactionType == 1 ? "SATIS" : item.TransactionType == 4 ? "IPTAL" : "IADE",
                                    Status = formatStatus,
                                    TransactionKey = item.OrderId,
                                    TransactionID = item.OrderId,
                                    Description = item.BankResponseMessage??item.Message
                                };

                               var resp =  _paymentCreditCardPoolManager.Insert(paymentCreditCardPool);
                            }
                        }
                    }
                }
            }
        }
    }
}

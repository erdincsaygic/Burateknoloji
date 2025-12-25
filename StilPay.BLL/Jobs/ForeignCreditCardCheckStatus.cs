using Coravel.Invocable;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;
using StilPay.Utility.AKODESanalPOS;
using StilPay.Utility.EsnekPos.Models.EsnekPosTransactionQuery;
using StilPay.Utility.EsnekPos;
using StilPay.Utility.Helper;
using StilPay.Utility.LidioPos.Models.LidioPosTransactionQuery;
using StilPay.Utility.LidioPos;
using StilPay.Utility.Models;
using StilPay.Utility.Paybull.PaybullCheckStatus;
using StilPay.Utility.Paybull;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using Coravel.Invocable;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualBasic;
using StilPay.BLL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.AKODESanalPOS.Models.AKODEPaymentRequest;
using StilPay.Utility.AKODESanalPOS.Models.AKODETransactionQuery;
using StilPay.Utility.AKODESanalPOS;
using StilPay.Utility.Helper;
using StilPay.Utility.Models;
using StilPay.Utility.Paybull;
using StilPay.Utility.Paybull.PaybullCheckStatus;
using StilPay.Utility.Paybull.PaybullPayment;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static StilPay.Utility.Helper.Enums;
using StilPay.Utility.EsnekPos.Models.EsnekPosTransactionQuery;
using StilPay.Utility.EsnekPos;
using StilPay.Utility.LidioPos.Models.LidioPosTransactionQuery;
using StilPay.Utility.LidioPos;

namespace StilPay.BLL.Jobs
{
    public class ForeignCreditCardCheckStatus : IInvocable
    {
        public readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public ForeignCreditCardCheckStatus(IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager)
        {
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
        }

        public async Task Invoke()
        {
            var creditCardList = _foreignCreditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
            {
                new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                new FieldParameter("IDCompany", Enums.FieldType.NVarChar, null),
                new FieldParameter("IsAutoNotification", Enums.FieldType.Tinyint, 1),
                new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null)
            });

            var callbackEntity = new CallbackResponseLog();
            var opt = new JsonSerializerOptions() { WriteIndented = true };

            foreach (var item in creditCardList.Where(x => DateTime.Now >= x.CDate.AddMinutes(4)))
            {
                var integration = _companyIntegrationManager.GetByServiceId(item.ServiceID);

                if (item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.ForeignLidioPosTL || item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.ForeignLidioPosCurrency)
                {
                    var lidioSenderName = "";
                    var lidioPosTransactionQueryRequestModel = new LidioPosTransactionQueryRequestModel()
                    {
                        orderId = item.TransactionID,
                        paymentInstrument = "NewCard",
                        totalAmount = item.Amount,
                        paymentInquiryInstrumentInfo = new PaymentInquiryInstrumentInfo()
                        {
                            Card = new PaymentInquiryCard()
                            {
                                ProcessType = "sales"
                            }
                        }
                    };

                    var lidioPosTransactionQueryRequestResponseModel = LidioPosTransactionQuery.TransactionQueryRequest(lidioPosTransactionQueryRequestModel, true);

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "LidioPos YD";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosTransactionQueryRequestResponseModel, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI TIMEOUT QUERY RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "OK")
                    {
                        item.Status = (byte)Enums.StatusType.Confirmed;
                        item.PaymentInstitutionNetAmount = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].PaybackAmount;
                        item.PaymentInstitutionCommissionRate = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].BankTotalCommissionRate;
                        item.TransactionReferenceCode = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                        item.Description = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;

                        lidioSenderName = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.AcquirerResultDetail.Pos.CardHolderNameFromBank;

                    }
                    else
                    {
                        if (lidioPosTransactionQueryRequestResponseModel.Status == "ERROR" && lidioPosTransactionQueryRequestResponseModel.Data != null)
                        {
                            item.Status = lidioPosTransactionQueryRequestResponseModel.Data.Result != "Success" && lidioPosTransactionQueryRequestResponseModel.Data.ResultDetail != "Success" ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;
                            item.TransactionReferenceCode = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                            item.Description = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;

                            lidioSenderName = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.AcquirerResultDetail.Pos.CardHolderNameFromBank;

                        }
                    }

                    if (item.Status != (byte)Enums.StatusType.Pending)
                    {
                        var response = _foreignCreditCardPaymentNotificationManager.SetStatus(item);

                        item.Description = item.Status == (byte)Enums.StatusType.Confirmed ? lidioSenderName == "" ? item.Description : item.Description + " Banka Gönderici Adı: " + lidioSenderName : item.Description;

                        if (response.Status == "OK")
                        {
                            var dataCallback = new
                            {
                                status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                                service_id = item.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description, currencyCode = item.CurrencyCode },
                                user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort, cardSenderNameFromBank = lidioSenderName }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                    }
                }
            }
        }
    }
}

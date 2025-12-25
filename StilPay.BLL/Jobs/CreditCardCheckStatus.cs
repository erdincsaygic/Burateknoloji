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
using StilPay.BLL.Concrete;
using StilPay.Utility.EfixPos;

namespace StilPay.BLL.Jobs
{
    public class CreditCardCheckStatus : IInvocable
    {
        public readonly ICreditCardPaymentNotificationManager _creditCardPaymentNotificationManager;
        public readonly IForeignCreditCardPaymentNotificationManager _foreignCreditCardPaymentNotificationManager;
        public readonly ICompanyIntegrationManager _companyIntegrationManager;
        public readonly ICallbackResponseLogManager _callbackResponseLogManager;
        public readonly IPaymentInstitutionManager _paymentInstitutionManager;
        private readonly SettingDAL _settingDAL = new SettingDAL();

        public CreditCardCheckStatus(ICreditCardPaymentNotificationManager creditCardPaymentNotificationManager, IForeignCreditCardPaymentNotificationManager foreignCreditCardPaymentNotificationManager, ICompanyIntegrationManager companyIntegrationManager, ICallbackResponseLogManager callbackResponseLogManager, IPaymentInstitutionManager paymentInstitutionManager)
        {
            _creditCardPaymentNotificationManager = creditCardPaymentNotificationManager;
            _foreignCreditCardPaymentNotificationManager = foreignCreditCardPaymentNotificationManager;
            _companyIntegrationManager = companyIntegrationManager;
            _callbackResponseLogManager = callbackResponseLogManager;
            _paymentInstitutionManager = paymentInstitutionManager;
        }

        public async Task Invoke()
        {
            var creditCardList = _creditCardPaymentNotificationManager.GetList(new List<FieldParameter>()
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

            foreach (var item in creditCardList.Where(x => DateTime.Now >= x.CDate.AddMinutes(4)) )
            {
                var integration = _companyIntegrationManager.GetByServiceId(item.ServiceID);

                if (item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.Paybull)
                {
                    var paybullCheckStatusRequestModel = new PaybullCheckStatusRequestModel()
                    {
                        merchant_key = _settingDAL.GetList(null).FirstOrDefault(x => x.ParamType == "PaybullCreditCard" && x.ParamDef == "secret_key").ParamVal,
                        invoice_id = item.TransactionID,
                        include_pending_status = false,
                    };

                    var paybullCheckStatusResponseModel = PaybullCheckStatusRequest.CheckStatus(paybullCheckStatusRequestModel);

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "Paybull";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(paybullCheckStatusResponseModel, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (paybullCheckStatusResponseModel.Status == "OK" && paybullCheckStatusResponseModel.Data != null && paybullCheckStatusResponseModel.Data.status_code == 100)
                    {
                        item.Status = paybullCheckStatusResponseModel.Data.transaction_status == "Completed" ? (byte)Enums.StatusType.Confirmed : paybullCheckStatusResponseModel.Data.transaction_status == "Failed" ? (byte)Enums.StatusType.Canceled : item.Status;
                        item.MUser = "00000000-0000-0000-0000-000000000000";
                        item.MDate = DateTime.Now;
                        item.Description = paybullCheckStatusResponseModel.Data.status_description;
                        item.TransactionReferenceCode = paybullCheckStatusResponseModel.Data.order_id.ToString();

                        if (item.Status != (byte)Enums.StatusType.Pending)
                        {
                            var response = _creditCardPaymentNotificationManager.SetStatus(item);

                            if (response.Status == "OK")
                            {
                                var dataCallback = new
                                {
                                    status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                                    service_id = item.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Ödeme İşlemi Başarılı" },
                                    user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort }
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

                if(item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.AKODE)
                {
                    var akOdeSanalPOSIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "AKODECreditCard") });

                    var randomGenerator = new Random();
                    var rnd = randomGenerator.Next(1, 1000000).ToString();

                    var akOdeTransactionQueryRequestModel = new AKODETransactionQueryRequestModel()
                    {
                        ApiUser = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_user").ParamVal,
                        ClientId = akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "client_id").ParamVal,
                        OrderId = item.TransactionID,
                        Rnd = rnd,
                        TimeSpan = DateTime.Now.ToString("yyyyMMddHHmmss")
                    };

                    akOdeTransactionQueryRequestModel.Hash = AKODECreateHash.CreateHash(akOdeSanalPOSIntegrationValues.FirstOrDefault(f => f.ParamDef == "api_pass").ParamVal, akOdeTransactionQueryRequestModel.ClientId, akOdeTransactionQueryRequestModel.ApiUser, akOdeTransactionQueryRequestModel.Rnd, akOdeTransactionQueryRequestModel.TimeSpan);

                    var akOdeTransactionQueryResponseModel = AKODETransactionQueryRequest.TransactionQueryRequest(akOdeTransactionQueryRequestModel);

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "AKODE";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(akOdeTransactionQueryResponseModel, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI QUERY RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (akOdeTransactionQueryResponseModel.Data != null && akOdeTransactionQueryResponseModel.Status == "OK")
                    {
                        item.Status = akOdeTransactionQueryResponseModel.Data.BankResponseCode == "00" && akOdeTransactionQueryResponseModel.Data.RequestStatus == 1 ? (byte)Enums.StatusType.Confirmed : akOdeTransactionQueryResponseModel.Data.RequestStatus == 0 ? (byte)Enums.StatusType.Canceled : item.Status;
                        item.MUser = "00000000-0000-0000-0000-000000000000";
                        item.MDate = DateTime.Now;
                        item.Description = item.Status == (byte)Enums.StatusType.Confirmed ? "Ödeme İşlemi Başarılı" : akOdeTransactionQueryResponseModel.Data != null ?
                                akOdeTransactionQueryResponseModel.Data.BankResponseMessage : akOdeTransactionQueryResponseModel.Message;
                        item.TransactionReferenceCode = akOdeTransactionQueryResponseModel.Data.HostReferenceNumber;

                        if (item.Status != (byte)Enums.StatusType.Pending)
                        {
                            var response = _creditCardPaymentNotificationManager.SetStatus(item);

                            if (response.Status == "OK")
                            {
                                var dataCallback = new
                                {
                                    status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                                    service_id = item.ServiceID,
                                    status_type = 1,
                                    ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                    data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = "Ödeme İşlemi Başarılı" },
                                    user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort }
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

                if(item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.EsnekPos)
                {
                    var esnekPosIntegrationValues = _settingDAL.GetList(new List<FieldParameter>() { new FieldParameter("ParamType", FieldType.NVarChar, "EsnekPos") });

                    var esnekPosTransactionQueryRequestModel = new EsnekPosTransactionQueryRequestModel()
                    {
                        ORDER_REF_NUMBER = item.TransactionID
                    };

                    var esnekPosTransactionQueryRequestResponseModel = EsnekPosTransactionQueryRequest.TransactionQueryRequest(esnekPosTransactionQueryRequestModel);

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "EsnekPos";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(esnekPosTransactionQueryRequestResponseModel, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI TIMEOUT QUERY RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (esnekPosTransactionQueryRequestResponseModel.Data != null && esnekPosTransactionQueryRequestResponseModel.Data.TRANSACTIONS.Count > 0 && esnekPosTransactionQueryRequestResponseModel.Status == "OK")
                    {
                        foreach(var transaction in esnekPosTransactionQueryRequestResponseModel.Data.TRANSACTIONS.Where(x => x.STATUS_ID == 3 || x.STATUS_ID == 4))
                        {
                            item.Status = transaction.STATUS_ID == 3 && transaction.STATUS_NAME == "Ödeme - Başarılı" ? (byte)Enums.StatusType.Confirmed 
                                : transaction.STATUS_ID == 4 ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;

                            item.MUser = "00000000-0000-0000-0000-000000000000";
                            item.MDate = DateTime.Now;
                            item.Description = transaction.STATUS_NAME;
                            item.TransactionReferenceCode = esnekPosTransactionQueryRequestResponseModel.Data.REFNO;

                            if (item.Status != (byte)Enums.StatusType.Pending)
                            {

                                item.Description = transaction.STATUS_NAME;
                                item.PaymentInstitutionCommissionRate =Convert.ToDecimal(esnekPosTransactionQueryRequestResponseModel.Data.COMMISSION)
                                    / Convert.ToDecimal(transaction.AMOUNT)  * 100;

                                item.PaymentInstitutionNetAmount = Convert.ToDecimal(transaction.AMOUNT) - Convert.ToDecimal(esnekPosTransactionQueryRequestResponseModel.Data.COMMISSION);
                                item.TransactionReferenceCode = esnekPosTransactionQueryRequestResponseModel.Data.REFNO;

                                var response = _creditCardPaymentNotificationManager.SetStatus(item);

                                if (response.Status == "OK")
                                {
                                    var dataCallback = new
                                    {
                                        status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                                        service_id = item.ServiceID,
                                        status_type = 1,
                                        ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                        data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description },
                                        user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort }
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

                if(item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.LidioPos)
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

                    var lidioPosTransactionQueryRequestResponseModel = LidioPosTransactionQuery.TransactionQueryRequest(lidioPosTransactionQueryRequestModel);

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "LidioPos";
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
                        if(lidioPosTransactionQueryRequestResponseModel.Status == "ERROR" && lidioPosTransactionQueryRequestResponseModel.Data != null)
                        {
                            item.Status = lidioPosTransactionQueryRequestResponseModel.Data.Result != "Success" && lidioPosTransactionQueryRequestResponseModel.Data.ResultDetail != "Success" ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;
                            item.TransactionReferenceCode = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                            item.Description = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;

                            lidioSenderName = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.AcquirerResultDetail.Pos.CardHolderNameFromBank;

                        }
                    }

                    if (item.Status != (byte)Enums.StatusType.Pending)
                    {
                        var response = _creditCardPaymentNotificationManager.SetStatus(item);

                        item.Description = item.Status == (byte)Enums.StatusType.Confirmed ? lidioSenderName == "" ? item.Description : item.Description + " Banka Gönderici Adı: " + lidioSenderName : item.Description;
                         
                        if (response.Status == "OK")
                        {
                            var dataCallback = new
                            {
                                status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                                service_id = item.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description },
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

                if (item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.EfixPos)
                {

                    var efix = _paymentInstitutionManager.GetList(null).FirstOrDefault(x => x.ID == Convert.ToString((int)CreditCardPaymentMethodType.EfixPos));
               
                    var getDetail = EfixPosTransactionDetailRequest.GetDetail(item.TransactionID);

                    callbackEntity.TransactionID = item.TransactionID;
                    callbackEntity.ServiceType = "EfixPos";
                    callbackEntity.IDCompany = integration.ID;
                    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(getDetail, opt);
                    callbackEntity.TransactionType = "KREDI KARTI ODEMESI TIMEOUT QUERY RESPONSE";
                    _callbackResponseLogManager.Insert(callbackEntity);

                    if (getDetail.Data != null && getDetail.Status == "OK")
                    {
                        decimal netAmount = Math.Round(getDetail.Data.TotalAmount - (getDetail.Data.TotalAmount * efix.Rate / 100m), 2);

                        item.Status = !string.IsNullOrEmpty(getDetail.Data.ExternalStatusCode) && getDetail.Data.ExternalStatusCode == "0000" ? (byte)Enums.StatusType.Confirmed : !string.IsNullOrEmpty(getDetail.Data.ExternalStatusCode) && getDetail.Data.ExternalStatusCode != "0000" ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;
                        item.PaymentInstitutionNetAmount = netAmount;
                        item.PaymentInstitutionCommissionRate = efix.Rate;
                        item.TransactionReferenceCode = getDetail.Data.PaymentTransactionId.ToString();
                        item.Description = getDetail.Data.ExternalStatusDesc;
                    }

                    if (item.Status != (byte)Enums.StatusType.Pending)
                    {
                        var response = _creditCardPaymentNotificationManager.SetStatus(item);

                        if (response.Status == "OK")
                        {
                            var dataCallback = new
                            {
                                status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                                service_id = item.ServiceID,
                                status_type = 1,
                                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                                data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description },
                                user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort, cardSenderNameFromBank = "" }
                            };

                            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                            callbackEntity.ServiceType = "STILPAY";
                            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                            _callbackResponseLogManager.Insert(callbackEntity);
                        }
                    }
                }

                //if (item.CreditCardPaymentMethodID == (byte)Enums.CreditCardPaymentMethodType.NomupayPos)
                //{
                //    var lidioPosTransactionQueryRequestModel = new LidioPosTransactionQueryRequestModel()
                //    {
                //        orderId = item.TransactionID,
                //        paymentInstrument = "NewCard",
                //        totalAmount = item.Amount,
                //        paymentInquiryInstrumentInfo = new PaymentInquiryInstrumentInfo()
                //        {
                //            Card = new PaymentInquiryCard()
                //            {
                //                ProcessType = "sales"
                //            }
                //        }
                //    };

                //    var lidioPosTransactionQueryRequestResponseModel = LidioPosTransactionQuery.TransactionQueryRequest(lidioPosTransactionQueryRequestModel, true);

                //    callbackEntity.TransactionID = item.TransactionID;
                //    callbackEntity.ServiceType = "LidioPos YD";
                //    callbackEntity.IDCompany = integration.ID;
                //    callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(lidioPosTransactionQueryRequestResponseModel, opt);
                //    callbackEntity.TransactionType = "KREDI KARTI ODEMESI TIMEOUT QUERY RESPONSE";
                //    _callbackResponseLogManager.Insert(callbackEntity);

                //    if (lidioPosTransactionQueryRequestResponseModel.Data != null && lidioPosTransactionQueryRequestResponseModel.Status == "OK")
                //    {
                //        item.Status = (byte)Enums.StatusType.Confirmed;
                //        item.PaymentInstitutionNetAmount = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].PaybackAmount;
                //        item.PaymentInstitutionCommissionRate = (decimal?)lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.PaybackTransactionList[0].BankTotalCommissionRate;
                //        item.TransactionReferenceCode = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                //        item.Description = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
                //    }
                //    else
                //    {
                //        if (lidioPosTransactionQueryRequestResponseModel.Status == "ERROR" && lidioPosTransactionQueryRequestResponseModel.Data != null)
                //        {
                //            item.Status = lidioPosTransactionQueryRequestResponseModel.Data.Result != "Success" && lidioPosTransactionQueryRequestResponseModel.Data.ResultDetail != "Success" ? (byte)Enums.StatusType.Canceled : (byte)Enums.StatusType.Pending;
                //            item.TransactionReferenceCode = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.SystemTransId;
                //            item.Description = lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.RecommendedUIMessageTR ?? lidioPosTransactionQueryRequestResponseModel.Data.PaymentInfo.ResultCategory.CategoryName;
                //        }
                //    }

                //    if (item.Status != (byte)Enums.StatusType.Pending)
                //    {
                //        var response = _creditCardPaymentNotificationManager.SetStatus(item);

                //        if (response.Status == "OK")
                //        {
                //            var dataCallback = new
                //            {
                //                status_code = item.Status == (byte)Enums.StatusType.Confirmed ? "OK" : "ERROR",
                //                service_id = item.ServiceID,
                //                status_type = 1,
                //                ciphered = tMD5Manager.EncryptBasic(integration.SecretKey),
                //                data = new { transaction_id = item.TransactionID, sp_transactionNr = item.TransactionNr, amount = item.Amount, sp_id = item.ID, message = item.Description },
                //                user_entered_data = new { member = item.Member, sender_name = item.SenderName, action_date = item.ActionDate, action_time = item.ActionTime, creditCard = item.CardNumber, amount = item.Amount, user_ip = item.MemberIPAddress, user_port = item.MemberPort }
                //            };

                //            var responseCallBack = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(integration.CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                //            callbackEntity.ServiceType = "STILPAY";
                //            callbackEntity.Callback = System.Text.Json.JsonSerializer.Serialize(dataCallback, opt);
                //            callbackEntity.ResponseStatus = (byte)(responseCallBack != null && responseCallBack.Result != null && responseCallBack.Result.Status == "OK" ? 1 : 0);
                //            _callbackResponseLogManager.Insert(callbackEntity);
                //        }
                //    }
                //}
            }        
        }
    }
}

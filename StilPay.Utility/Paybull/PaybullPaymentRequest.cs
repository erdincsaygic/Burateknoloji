using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using StilPay.Utility.Paybull.PaybullPayment;
using StilPay.Utility.Paybull.PaybullToken;
using System.Linq;
using System.Security.Cryptography;
using static StilPay.Utility.IsBankSanalPos.IsBankSanalPOSComplatePaymentXMLResponseModel.IsBankSanalPOSComplatePaymentXMLResponseModel;
using System.Globalization;

namespace StilPay.Utility.Paybull
{
    public class PaybullPaymentRequest
    {
        public static string PaymentRequest(PaybullPaymentRequestModel paybullPaymentRequestModel)
        {
            try
            {
                var systemSettingValues = tSQLBankManager.GetSystemSettingValues("PaybullCreditCard");

                var tokenModel = new PaybullTokenRequestModel()
                {
                    app_id = systemSettingValues.FirstOrDefault(f => f.ParamDef == "app_id").ParamVal,
                    app_password = systemSettingValues.FirstOrDefault(f => f.ParamDef == "app_password").ParamVal
                };

                var token = PaybullGetToken.GetAccessToken(tokenModel);

                if(token != null && token.Data != null && token.Status == "OK" && token.Data.data.token != null)
                {
                    var options = new RestClientOptions("https://app.paybull.com");
                    var client = new RestClient(options);
                    var request = new RestRequest("/ccpayment/api/paySmart3D", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + token.Data.data.token);
                    request.AddParameter("cc_holder_name", paybullPaymentRequestModel.cc_holder_name);
                    request.AddParameter("cc_no", paybullPaymentRequestModel.cc_no);
                    request.AddParameter("expiry_month", paybullPaymentRequestModel.expiry_month);
                    request.AddParameter("expiry_year", paybullPaymentRequestModel.expiry_year);
                    request.AddParameter("cvv", paybullPaymentRequestModel.cvv);
                    request.AddParameter("currency_code", paybullPaymentRequestModel.currency_code);
                    request.AddParameter("invoice_id", paybullPaymentRequestModel.invoice_id);
                    request.AddParameter("invoice_description", "123");
                    request.AddParameter("name", paybullPaymentRequestModel.name);
                    request.AddParameter("surname", paybullPaymentRequestModel.surname);
                    request.AddParameter("total", paybullPaymentRequestModel.total.ToString(CultureInfo.InvariantCulture));
                    request.AddParameter("merchant_key", systemSettingValues.FirstOrDefault(f => f.ParamDef == "secret_key").ParamVal);
                    request.AddParameter("items", "[{\"name\":\"URUN\",\"price\":" + paybullPaymentRequestModel.total.ToString(CultureInfo.InvariantCulture) + ",\"quantity\":1,\"description\":\"ACIKLAMA\"}]");
                    request.AddParameter("cancel_url", paybullPaymentRequestModel.cancel_url);
                    request.AddParameter("return_url", paybullPaymentRequestModel.return_url);
                    request.AddParameter("hash_key", paybullPaymentRequestModel.hash_key);
                    request.AddParameter("installments_number", paybullPaymentRequestModel.installments_number);

                    var response = client.Execute(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content;

                    }
                    else
                    {
                        return "1001 - Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz.";
                    }
                }
                else
                {
                    return "1001 - Şu anda İşleminiz Gerçekleştirilemiyor. Lütfen Bir Süre Sonra Tekrar Deneyiniz.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

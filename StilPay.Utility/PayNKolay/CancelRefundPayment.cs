using Newtonsoft.Json;
using RestSharp;
using StilPay.Utility.PayNKolay.Models;
using StilPay.Utility.PayNKolay.Models.CancelRefundPayment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StilPay.Utility.PayNKolay
{
    public class CancelRefundPayment
    {
        public static ResponseModel<CancelRefundPaymentResponseModel> CancelPaymentRequest(CancelRefundPaymentRequestModel.CancelPaymentRequestModel cancelPaymentRequestModel)
        {
            try
            {
                var client = new RestClient("https://paynkolay.nkolayislem.com.tr/Vpos/v1/CancelRefundPayment");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("sx", cancelPaymentRequestModel.Sx);
                request.AddParameter("referenceCode", cancelPaymentRequestModel.ReferenceCode);
                request.AddParameter("resultUrl", cancelPaymentRequestModel.ResultUrl);
                request.AddParameter("referenceCode", cancelPaymentRequestModel.ReferenceCode);
                request.AddParameter("type", cancelPaymentRequestModel.Type);
                //request.AddParameter("trxDate", cancelPaymentRequestModel.tr);
                request.AddParameter("amount", cancelPaymentRequestModel.Amount.ToString(CultureInfo.InvariantCulture));
                request.AddParameter("hashData", cancelPaymentRequestModel.HashData);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<CancelRefundPaymentResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseModel<CancelRefundPaymentResponseModel>
                    {
                        Status = deserialize.RESPONSE_CODE == 2 ? "OK" : "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                        Data = deserialize
                    };

                }
                else
                {
                    return new ResponseModel<CancelRefundPaymentResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<CancelRefundPaymentResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }

        public static ResponseModel<CancelRefundPaymentResponseModel> RefundPaymentRequest(CancelRefundPaymentRequestModel.RefundRequestModel refundRequestModel)
        {
            try
            {
                var client = new RestClient("https://paynkolay.nkolayislem.com.tr/Vpos/v1/CancelRefundPayment");
                var request = new RestRequest
                {
                    Method = Method.Post,
                    AlwaysMultipartFormData = true
                };
                request.AddParameter("sx", refundRequestModel.Sx);
                request.AddParameter("referenceCode", refundRequestModel.ReferenceCode);
                request.AddParameter("resultUrl", refundRequestModel.ResultUrl);
                request.AddParameter("referenceCode", refundRequestModel.ReferenceCode);
                request.AddParameter("type", refundRequestModel.Type);
                request.AddParameter("trxDate", refundRequestModel.TrxDate);
                request.AddParameter("amount", refundRequestModel.Amount.ToString(CultureInfo.InvariantCulture));
                request.AddParameter("hashData", refundRequestModel.HashData);
                var response = client.Execute(request);
                var deserialize = JsonConvert.DeserializeObject<CancelRefundPaymentResponseModel>(response.Content);

                if (response.IsSuccessStatusCode)
                {
                    return new ResponseModel<CancelRefundPaymentResponseModel>
                    {
                        Status = deserialize.RESPONSE_CODE == 2 ? "OK" : "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                        Data = deserialize
                    };

                }
                else
                {
                    return new ResponseModel<CancelRefundPaymentResponseModel>
                    {
                        Status = "ERROR",
                        Message = deserialize.RESPONSE_DATA,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<CancelRefundPaymentResponseModel>
                {
                    Status = "ERROR",
                    Message = ex.Message,
                };
            }
        }
    }
}

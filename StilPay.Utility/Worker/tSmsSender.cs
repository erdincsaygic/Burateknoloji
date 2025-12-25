using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;

namespace StilPay.Utility.Worker
{
    public class tSmsSender
    {
        private static string _defaultConnection;
        static tSmsSender()
        {
            _defaultConnection = tAES3.Instance.Decrypt();
        }

        public SmsResponse SendSms(string phone, string text)
        {
            try
            {
                var fieldParams = new List<FieldParameter>() { new FieldParameter("ParamType", Enums.FieldType.NVarChar, "SMS") };
                var connector = new tSQLConnector();
                var dt = connector.GetDataTable("Settings_GetList", fieldParams);

                Dictionary<string, string> fields = new Dictionary<string, string>();
                foreach (DataRow item in dt.Rows)
                    fields.Add(item["ParamDef"].ToString(), item["ParamVal"].ToString());

                var smsRequest = new SmsRequest()
                {
                    username = fields["SmsUserName"],
                    password = fields["SmsPassword"],
                    source_addr = fields["SmsSourceAddr"],
                    messages = new Message[] { new Message { dest = phone, msg = text } }
                };

                string payload = JsonConvert.SerializeObject(smsRequest);

                var wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers["Content-Type"] = "application/json";

                string campaign_id = wc.UploadString("http://sms.verimor.com.tr/v2/send.json", payload);

                using (SqlConnection connection = new SqlConnection(_defaultConnection))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "INSERT INTO SmsLogs(ID, CDate, IDCompany, IDMember, Phone, SmsMessage, OperationType)" + Environment.NewLine +
                        "VALUES(@IDOut, @CDate,@IDCompany,@IDMember,@Phone,@SmsMessage,@OperationType);" + Environment.NewLine +
                        "SELECT @IDOut;";

                    cmd.Parameters.Add("@CDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@IDCompany", SqlDbType.NVarChar, 50).Value = (object)DBNull.Value;
                    cmd.Parameters.Add("@IDMember", SqlDbType.NVarChar, 50).Value = (object)DBNull.Value;
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = phone;
                    cmd.Parameters.Add("@SmsMessage", SqlDbType.NVarChar, -1).Value = smsRequest.messages.FirstOrDefault().msg;
                    cmd.Parameters.Add("@OperationType", SqlDbType.NVarChar, 100).Value = "";
                    cmd.ExecuteNonQuery();

                    connection.Close();
                }

                return new SmsResponse() { Status = "OK", Message = smsRequest.messages.FirstOrDefault().msg, ConfirmCode = 1 };
            }
            catch (WebException ex) // 400 hatalarında response body'de hatanın ne olduğunu yakalıyoruz
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) // 400 hataları
                {
                    var responseBody = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    return new SmsResponse() { Status = "ERROR", Message = string.Concat("Mesaj gönderilemedi. (", responseBody, ")"), ConfirmCode = -1 };
                }
                else // diğer hatalar
                {
                    return new SmsResponse() { Status = "ERROR", Message = "Mesaj gönderilemedi. (Web-Exception)", ConfirmCode = -1 };
                }
            }
        }

        public SmsResponse SendConfirmCode(string phone, string operationType, string message = null)
        {
            Random rnd = new Random();
            var confirmCode = rnd.Next(123456, 987654);

            if (phone == "05382998128") //Silinecek
                return new SmsResponse() { Status = "OK", Message = "Success", ConfirmCode = 222222 };

            if (phone == "05418557879") //Silinecek
                return new SmsResponse() { Status = "OK", Message = "Success", ConfirmCode = 111111 };

            var response = SendSms(phone, string.Concat(string.IsNullOrEmpty(message) ? "" : message, "2 DK geçerlilik süresine sahip ", confirmCode, " doğrulama kodu ile işleminizi yapabilirsiniz. İyi Günler."));

            if (response.Status.Equals("OK"))
            {
                response.ConfirmCode = confirmCode;

                try
                {
                    using SqlConnection connection = new SqlConnection(_defaultConnection);
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "declare @IDOut nvarchar(50) = NEWID();" + Environment.NewLine +
                        "INSERT INTO SmsLogs(ID, CDate, IDCompany, IDMember, Phone, SmsMessage, OperationType)" + Environment.NewLine +
                        "VALUES(@IDOut, @CDate,@IDCompany,@IDMember,@Phone,@SmsMessage,@OperationType);" + Environment.NewLine +
                        "SELECT @IDOut;";

                    cmd.Parameters.Add("@CDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@IDCompany", SqlDbType.NVarChar, 50).Value = (object)DBNull.Value;
                    cmd.Parameters.Add("@IDMember", SqlDbType.NVarChar, 50).Value = (object)DBNull.Value;
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = phone;
                    cmd.Parameters.Add("@SmsMessage", SqlDbType.NVarChar, -1).Value = response.Message;
                    cmd.Parameters.Add("@OperationType", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(operationType) ? (object)DBNull.Value : operationType;
                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                catch { }
            }

            return response;
        }
    }

    public class Message
    {
        public string dest { get; set; }
        public string msg { get; set; }
    }

    public class SmsRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string source_addr { get; set; }
        public Message[] messages { get; set; }
    }


}

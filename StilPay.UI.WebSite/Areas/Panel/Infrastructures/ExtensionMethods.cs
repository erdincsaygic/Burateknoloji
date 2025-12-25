using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StilPay.Utility.Helper;
using System;

namespace StilPay.UI.WebSite.Areas.Panel.Infrastructures
{
    public static class ExtensionMethods
    {
        public static void Write<T>(this ISession session, string key, T data) where T : class, new()
        {
            var json = JsonConvert.SerializeObject(data);
            session.SetString(key, json);
        }

        public static T Read<T>(this ISession session, string key) where T : class, new()
        {
            string json = session.GetString(key);
            if (string.IsNullOrEmpty(json))
                return null;
            else
            {
                T data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
        }

        public static bool HasSentSms(this ISession session, string key, string operationType)
        {
            string json = session.GetString(operationType);
            if (string.IsNullOrEmpty(json))
                return false;
            else
            {
                var sms = JsonConvert.DeserializeObject<Sms>(json);

                if (sms.Key != key || sms.SmsDate < DateTime.Now.AddMinutes(-2))
                    return false;
                else
                    return true;
            }
        }

        public static void SaveSms(this ISession session, string key, string operationType, int confirmCode, byte counter = 0, byte blockCounter = 0)
        {
            session.Remove(operationType);

            var json = JsonConvert.SerializeObject(new Sms { Key = key, OperationType = operationType, SmsDate = DateTime.Now, Counter = counter, ConfirmCode = confirmCode , BlockCounter = blockCounter });
            session.SetString(operationType, json);
        }

        public static GenericResponse ValidateConfirmCode(this ISession session, string operationType, int confirmCode)
        {
            var json = session.GetString(operationType);
            if (string.IsNullOrEmpty(json))
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = "Yeni doğrulama kodu alınız!"
                };

            var sms = JsonConvert.DeserializeObject<Sms>(json);

            if (sms.SmsDate < DateTime.Now.AddMinutes(-2))
            {
                session.Remove(operationType);
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = "Geçerlilik süresi doldu. Yeni doğrulama kodu alınız!"
                };
            }

            if (sms.ConfirmCode != confirmCode)
            {
                sms.Counter++;

                if (sms.Counter >= 5)
                {
                    session.Remove(operationType);
                    return new GenericResponse
                    {
                        Status = "ERROR",
                        Message = "Deneme hakkınız bitti. Yeni doğrulama kodu alınız!"
                    };
                }

                SaveSms(session, sms.Key, operationType, sms.ConfirmCode, sms.Counter);

                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = string.Format("Hatalı giriş! Kalan deneme hakkınız: {0}", (5 - sms.Counter))
                };
            }

            session.Remove(operationType);

            return new GenericResponse
            {
                Status = "OK",
                Data = sms.Key
            };
        }

        public static void SaveCaptchaCode(this ISession session, string name, string code)
        {
            session.SetString(name, code);
        }

        public static GenericResponse ValidateCaptchaCode(this ISession session, string name, string code)
        {
            var captchaCode = session.GetString(name);
            if (captchaCode != code)
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = "Kod hatalı! Tekrar giriniz."
                };

            return new GenericResponse
            {
                Status = "OK",
                Data = code
            };
        }

        class Sms
        {
            public string Key { get; set; }
            public string OperationType { get; set; }
            public DateTime SmsDate { get; set; }
            public byte Counter { get; set; }
            public byte BlockCounter { get; set; }
            public int ConfirmCode { get; set; }
        }

    }
}

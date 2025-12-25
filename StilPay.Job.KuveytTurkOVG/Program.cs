using StilPay.Utility.Helper;
using StilPay.Utility.KuveytTurk;
using StilPay.Utility.KuveytTurk.KuveytTurkAccountTransaction;
using StilPay.Utility.KuveytTurk.KuveytTurkToken;
using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.KuveytTurkOVG
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, object> body = new Dictionary<string, object>();


            var bankId = startUp.KuveytTurkApi.bank_id;
            var transaction_url = startUp.KuveytTurkApi.transaction_url;
            var startDate = startUp.KuveytTurkApi.startDate;
            var endDate = startUp.KuveytTurkApi.endDate;
            var queryperiodintervalsecond = startUp.KuveytTurkApi.query_period_interval_second;
            var notificationRangeMinute = startUp.KuveytTurkApi.notification_range_minute;

            var auth_transaction_url = startUp.KuveytTurkToken.transaction_url;
            var auth_grant_type = startUp.KuveytTurkToken.grant_type;
            var auth_scope = startUp.KuveytTurkToken.scope;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;


            var reqEndDate = DateTime.Now;
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            string previousDay = null;
            var companyBankAccountID = startUp.KuveytTurkApi.companyBankAccountID;
            Console.WriteLine(
                    string.Concat("-------------------------------------------------",
                                   Environment.NewLine, Environment.NewLine,
                                  "            STILPAY KUVEYTTURK-BANK WORKING..            ",
                                   Environment.NewLine, Environment.NewLine,
                                  "-------------------------------------------------")
            );

            while (true)
            {
                #region KuveytTurk Api
                try
                {
                    int responseStatus = 0;

                    DateTime dateNow = DateTime.Now;
                    TimeSpan timeLimit = new TimeSpan(10, 0, 0);

                    if (dateNow.TimeOfDay < timeLimit)
                        previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                    else
                        previousDay = null;

                    var systemSettingValues = tSQLBankManager.GetSystemSettingValues("KuveytTurkClient");

                    var tokenModel = new KuveytTurkTokenRequestModel()
                    {
                        client_id = "c0b9e377-2d6b-47ed-ab92-05a0fabf6f6f",
                        client_secret = "9e72452f-44b9-4026-aa04-fe02da8d4889-1be13037-3120-4248-9171-b7176d563f1b"
                    };

                    var token = KuveytTurkGetToken.GetAccessToken(tokenModel);

                    if (token != null && token.Status == "OK" && token.Data != null)
                    {
                        var url = !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) ? String.Concat(transaction_url, $"?beginDate={startDate}&endDate={endDate}")
                            : String.Concat(transaction_url, $"?beginDate={previousDay ?? today}&endDate={today}");

                        var rsa = KuveytTurkRSAKeyGenerator.RSAKeyGenerator(@"-----BEGIN RSA PRIVATE KEY-----
        MIIEogIBAAKCAQBqmwZrSKTUTooNq+VHWDT9V+p+qxCuh+YWUb+8qX8Jeq6d0+TW
        zx6moOhJyc4lbIO8gilWle1fN3embSNAT8bMoRi6Xr2ZEWAZPF04VI8/WpyZNQwh
        WPwZHLFfd5jAucprbelTevE5X8WMLiL3bveMvgjcvpTsexLA5lmgdou4plQUVqJc
        nHJJJO0kaR4wPbKbEeTUZ511Uaw9OZ4rXq5DOHy1CqVhsCiKfwXMy6t8x8wh2ijj
        +11Zvh3wH+4pDDiR5CKdMXvglnXfwxObmbKTbBi/cfziFa8zSm7gaTP2gCQ7a2zs
        TtgQl0DNQNKdBHYCf6LOj106cepRp01dnHsVAgMBAAECggEAJb/lowHjVEbHfhXb
        p8rlYLzMDbS3wIXhBRBHrB/9Gzc1NDA/fY10Vh7ugoqSlA/8CjmxN7b5ilkS5n0J
        GZHmXLnDDuPTkatkcys7+2F+JDoK7/mn5PsksiPF739jOQPRWP9fuy7y0pVGV+BS
        g3no8Q6uBrT5+U+PkX1ASaEQ0v6jD3+wkUXrplEoymWtz0SK1U5vUzYNxB+9S1lE
        8bLRPuPPy5NXv/hux5FBsWDm9UPVvAcJ7W3qdpGAdkywluN12iCrROebOfv99rF1
        DTjN9j8awLcIYxvyXFaLH0RbAYQTNtvclT3xkU9H2mmOhX9MH+6G+Jo8w/if/CV0
        WW4YAQKBgQDPKBNcBsOTkODVzkYgS4mGECIhnfuHVL8r1RhietOmYoBaJtatKU+x
        JQGTWOv2ipHcr8BvrMmh5ryJ+LFuDo3T3RsE9zUBcFGeOIsNEEGNJ/u1K46x4Pit
        67ckJ5yqNcsiwqcldxEATwc2x2MYCJF/ZtJvgiGuKV5Ot+DZyZIRFQKBgQCDvbbN
        pNX7DmNoTIuhiogBD0CXhaeiAudnXXTba8ywVnnpZhea3C/8uuG9TZQB4CTZG/BK
        faFCJTQtdMm6sdGMkPkFDbl+43ymStE30CAKKUBZKpCUjChGDkIvjeHsq6ihu6t5
        6dAhCIRHNIBW/XIS7v+rnSnFoldwWZz4B9dCAQKBgDhct/+222l/5pxldhD9XFp8
        czzgRfpJJYZggTTyJDnF3RQqMwiED+mrnuUfMXwvsYXwz5PS2D1TkQKdBnFiRlZZ
        dyt/sw1EKQC6c6LHRH6KXWKqijV9d0uisX6FxItO/YjkmyOHZLnHxrexwhVc53FZ
        YXHzXwSKvtz+DJBU1ogNAoGAQUfR/McQjY5MrhM4Ib0+tZ+0NyEwtvRPbIX/8PbT
        ABJp6MEBM2imksqcL6zwiZljSP4yLQdh0CAVYez8RXn1x3zTGLD7WSgqzVBHqiuE
        pORaEZUo/aMSFdzc6SmaaSeKsVIIn6m/y46n1Yzrh6+hRkaOBKElYNyYDYEqajGg
        dgECgYEAo5clCbOsF9skOVC6e61k3aUz0d7eGuJuwPO1Ceo1tZ9CsLpVMXBtOqqP
        kjdw2yIQfSRAD237XKAgoREJR4S4eUofp0KLGDl8neEsOoqRx8iQKgJp/7kgC/Y8
        1oSsNa4CI5OkEkd4m6HqNktWw7kEvCFb/vxUU4fisDREEqCxJPQ=
        -----END RSA PRIVATE KEY-----", token.Data.access_token, "GET", null, url);

                        var accClass = new KuveytTurkAccountTransactionRequestModel()
                        {
                            Authorization = token.Data.access_token,
                            Signature = rsa,
                            url = url
                        };

                        var transactionModel = KuveytTurkGetAccountTransaction.GetAccountTransaction(accClass);


                        if (transactionModel != null && transactionModel.Data != null && transactionModel.Status == "OK")
                        {
                            foreach (var hareketDetay in transactionModel.Data.value.accountActivities.OrderByDescending(o => Convert.ToDateTime(o.date)))
                            {
                                if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey))
                                {
                                    string[] splitSenderName = hareketDetay.description.Split(new string[] { "Gönderen:", "Gönderen=" }, StringSplitOptions.None);

                                    var senderName = "";
                                    if (splitSenderName.Length > 1)
                                        senderName = splitSenderName.Length > 1 ? splitSenderName[1].Split(',')[0].Trim() : " ";

                                    var isCaughtInFraudControl = false;
                                    var fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";
                                    var isTrusted = false;
                                    var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) = tSQLBankManager.CheckReferenceNr(hareketDetay.description);

                                    var paymentTransferPoolDescriptionControlList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                                    var isHaveBlockedWord = paymentTransferPoolDescriptionControlList.Any(x =>
                                       hareketDetay.description.ToLower().Contains(x.Name)
                                    );

                                    if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && Result != "" && ReferenceNr != "" && ServiceId != "")
                                    {
                                        var (IsTrusted, FraudResult, FraudDescription) = tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, senderName, ServiceId, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture));
                                        isCaughtInFraudControl = !FraudResult;
                                        fraudDescription = IsTrusted ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı" : FraudDescription;
                                        isTrusted = IsTrusted;
                                    }

                                    if (!string.IsNullOrEmpty(Result) && !string.IsNullOrWhiteSpace(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrWhiteSpace(ReferenceNr) && Result == "OK" && ServiceId != "" && CallbackUrl != "" && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey) && !tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && (isTrusted || Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture) <= AutoTransferLimit) && !isHaveBlockedWord && !isHaveBlockedWord && !isCaughtInFraudControl)
                                    {
                                        string transactionId = DateTime.Now.Ticks.ToString("D16");

                                        var transactionNr = tSQLBankManager.AddNotificationTransaction(DateTime.Now, Convert.ToDateTime(hareketDetay.date), Convert.ToDateTime(hareketDetay.date), bankId, ServiceId, transactionId, hareketDetay.businessKey, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.description, "00000000-0000-0000-0000-000000000000", senderName, "11111111111", false, true);

                                        if (!string.IsNullOrEmpty(transactionNr))
                                        {
                                            var companyIntegration = tSQLBankManager.GetCompanyIntegration(ServiceId);

                                            var dataCallback = new
                                            {
                                                status_code = "OK",
                                                status_type = 0,
                                                service_id = ServiceId,
                                                ciphered = tMD5Manager.EncryptBasic(companyIntegration.SecretKey),
                                                data = new
                                                {
                                                    transaction_id = transactionId,
                                                    reference_nr = ReferenceNr,
                                                    transfer_date = Convert.ToDateTime(hareketDetay.date),
                                                    amount = Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture)
                                                },
                                                user_entered_data = new
                                                {
                                                    sender_name = senderName,
                                                    bank_description = hareketDetay.description
                                                }
                                            };

                                            var pyID = tSQLBankManager.AddAutoPaymentNotification(Convert.ToDateTime(hareketDetay.date), bankId, senderName, ServiceId, transactionId, hareketDetay.businessKey, Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), "Otomatik Bakiye Yükleme İşlemi Bildirimi", "", companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                            var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                            var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(Convert.ToDateTime(hareketDetay.date), bankId, senderName, "", Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.businessKey, hareketDetay.description, true, companyBankAccountID, pyTransactionNr, transactionId, isCaughtInFraudControl, fraudDescription);


                                            if (pyID != null && IDOutAuto != null)
                                            {
                                                tSQLBankManager.SetPaymentTransactionStatus(pyID, (int)StatusType.Confirmed, "Otomatik Bakiye Yükleme İşlemi Bildirimi");

                                                var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(CallbackUrl, new Dictionary<string, string>(), new Dictionary<string, object>() { { "transaction", dataCallback } });

                                                if (response != null && response.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                                                {
                                                    tSQLBankManager.AcceptNotificationTransaction(transactionNr);

                                                    responseStatus = response.Result.Status switch
                                                    {
                                                        "OK" => 1,
                                                        "RED" => 2,
                                                        "ERROR" => 3,
                                                        _ => 0,
                                                    };
                                                }

                                                var opt = new JsonSerializerOptions() { WriteIndented = true };
                                                tSQLBankManager.AddCallbackResponseLog(transactionId, "STILPAY", System.Text.Json.JsonSerializer.Serialize(dataCallback, opt), companyIntegration.ID, "Ödeme Bildirimi", responseStatus);

                                                tableInsertionErrorCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                                tableInsertionSuccessCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!tSQLBankManager.HasPaymentTransferPool(hareketDetay.businessKey) && !tSQLBankManager.HasNotificationTransaction(hareketDetay.businessKey))
                                        {
                                            var IDOut = tSQLBankManager.AddPaymentTransferPool(Convert.ToDateTime(hareketDetay.date), bankId, senderName, hareketDetay.iban ?? "", Convert.ToDecimal(hareketDetay.amount, CultureInfo.InvariantCulture), hareketDetay.businessKey, hareketDetay.description, companyBankAccountID, isHaveBlockedWord ? (byte)Enums.StatusType.Risk : 1, isCaughtInFraudControl, fraudDescription);

                                            tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                            tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(
                            string.Concat(Environment.NewLine, Environment.NewLine,
                     $"Hata: {ex.Message}",
                      Environment.NewLine, Environment.NewLine,
                     "-------------------------------------------------"));
                }



                #endregion

                reqEndDate = DateTime.Now;
                today = DateTime.Today.ToString("yyyy-MM-dd");
                previousDay = null;

                Console.WriteLine(
                string.Concat(Environment.NewLine, Environment.NewLine,
                              $"Bankaya Atılan Sorgu Başlangıç Tarihi: {previousDay ?? today}\n",
                              $"Bankaya Atılan Sorgu Bitiş Tarihi: {reqEndDate}\n",
                              $"Hesap Hareketleri Sayısı : {accountTransactionCount}\n",
                              $"Tabloya Kayıt Edilen Başarılı İşlem Sayısı : {tableInsertionSuccessCount}\n",
                              $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı Sayısı: {tableInsertionErrorCount}\n",
                              $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                              "-------------------------------------------------"));

                Thread.Sleep(queryperiodintervalsecond * 1000);



                //#endregion

            }
        }
    }
}
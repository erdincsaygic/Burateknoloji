using StilPay.Utility.Models;
using StilPay.Utility.Worker;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static StilPay.Utility.Helper.Enums;

namespace StilPay.Job.Vakifbank
{
    internal class Program
    {
        // Vakıfbank SOAP Namespace'leri
        private const string NS_SOAP = "http://www.w3.org/2003/05/soap-envelope";
        private const string NS_PEAK = "Peak.Integration.ExternalInbound.Ekstre";
        private const string NS_DTO = "http://schemas.datacontract.org/2004/07/Peak.Integration.ExternalInbound.Ekstre.DataTransferObjects";
        private const string NS_WSA = "http://www.w3.org/2005/08/addressing";

        // SOAPAction değerleri
        private const string SOAP_ACTION_GETIR_HAREKET = "Peak.Integration.ExternalInbound.Ekstre/ISOnlineEkstreServis/GetirHareket";

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var startUp = new Startup();

            var bankId = startUp.VakifbankApi.bank_id;
            var transaction_url = startUp.VakifbankApi.transaction_url;
            var musteriNo = startUp.VakifbankApi.musteriNo;
            var hesapNo = startUp.VakifbankApi.hesapNo;
            var kurumKullanici = startUp.VakifbankApi.kurumKullanici;
            var sifre = startUp.VakifbankApi.sifre;
            var queryperiodintervalsecond = startUp.VakifbankApi.query_period_interval_second;
            var notificationRangeMinute = startUp.VakifbankApi.notification_range_minute;
            var companyBankAccountID = startUp.VakifbankApi.companyBankAccountID;

            int accountTransactionCount = 0;
            int tableInsertionErrorCount = 0;
            int tableInsertionSuccessCount = 0;
            int timeoutNotificationsCount = 0;

            var startDate = startUp.VakifbankApi.startDate;  
            var endDate = startUp.VakifbankApi.endDate;

            var reqEndDate = DateTime.Now;
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            string previousDay = null;

            Console.WriteLine(
                string.Concat("-------------------------------------------------",
                              Environment.NewLine, Environment.NewLine,
                              "            BURATEKNOLOJI VAKIFBANK WORKING..            ",
                              Environment.NewLine, Environment.NewLine,
                              "-------------------------------------------------")
            );

            while (true)
            {
                #region Vakifbank Api
                try
                {
                    int responseStatus = 0;

                    DateTime dateNow = DateTime.Now;
                    TimeSpan timeLimit = new TimeSpan(10, 0, 0);

                    if (dateNow.TimeOfDay < timeLimit)
                        previousDay = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                    else
                        previousDay = null;

                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };

                    string beginDateF = previousDay ?? today;
                    string endDateF = today;

                    string sorguBaslangic = !string.IsNullOrEmpty(startDate) ? startDate : $"{beginDateF} 00:00";
                    string sorguBitis = !string.IsNullOrEmpty(endDate) ? endDate : $"{endDateF} 23:59";

                    var soap = BuildVakifbankSoapEnvelope(
                        musteriNo, kurumKullanici, sifre,
                        hesapNo, sorguBaslangic, sorguBitis
                    );

                    using var req = new HttpRequestMessage(HttpMethod.Post, transaction_url);
                    req.Content = new StringContent(soap, Encoding.UTF8, "application/soap+xml");
                    req.Content.Headers.ContentType.Parameters.Add(
                        new System.Net.Http.Headers.NameValueHeaderValue("action", $"\"{SOAP_ACTION_GETIR_HAREKET}\"")
                    );

                    var res = http.Send(req);
                    var xmlText = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!res.IsSuccessStatusCode)
                        throw new Exception($"VAKIFBANK SOAP HTTP {(int)res.StatusCode} - {res.ReasonPhrase}\n{xmlText}");

                    var parsed = VakifbankParse(xmlText);

                    if (parsed.IslemKodu != "VBB0001")
                    {
                        Console.WriteLine($"VAKIFBANK HATA: Code={parsed.IslemKodu} Msg={parsed.IslemAciklamasi}");
                        Thread.Sleep(queryperiodintervalsecond * 1000);
                        continue;
                    }

                    foreach (var hesap in parsed.Hesaplar)
                    {
                        var movements = hesap.Hareketler;
                        accountTransactionCount += movements.Count;

                        foreach (var hareket in movements.OrderByDescending(o => o.IslemTarihi))
                        {
                            var businessKey = hareket.IslemNo;

                            if (tSQLBankManager.HasPaymentTransferPool(businessKey) ||
                                tSQLBankManager.HasNotificationTransaction(businessKey))
                            {
                                continue;
                            }

                            if (hareket.BorcAlacak != "A")
                                continue;

                            var (parsedSender, parsedRef) = ParseRefAndSender(hareket.Aciklama);

                            string senderName = parsedSender;
                            string senderIban = null;

                            if (hareket.Detaylar != null)
                            {
                                if (hareket.Detaylar.TryGetValue("GonderenAdi", out var gonderenAdi))
                                    senderName = string.IsNullOrEmpty(senderName) ? gonderenAdi : senderName;

                                hareket.Detaylar.TryGetValue("GonderenIbanKumarasi", out senderIban);
                            }

                            bool isCaughtInFraudControl = false;
                            bool isTrusted = false;
                            string fraudDescription = "Fraud kontrolleri başarıyla tamamlandı.";

                            var (Result, ReferenceNr, ServiceId, CallbackUrl, AutoTransferLimit) =
                                tSQLBankManager.CheckReferenceNr(parsedRef ?? "");

                            var trCulture = new CultureInfo("tr-TR");
                            string normalizedDescription = hareket.Aciklama != null
                                                           ? hareket.Aciklama.Replace(" ", "").ToLower(trCulture)
                                                           : "";

                            var paymentTransferPoolDescriptionControlList = tSQLBankManager.GetPaymentTransferPoolDescriptionControls();
                            var isHaveBlockedWord = paymentTransferPoolDescriptionControlList.Any(x =>
                                !string.IsNullOrEmpty(x.Name) &&
                                normalizedDescription.Contains(x.Name.Replace(" ", "").ToLower(trCulture))
                            );

                            if (!string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(ReferenceNr) && !string.IsNullOrEmpty(ServiceId))
                            {
                                var (IsTrusted, FraudResult, FraudDescription) =
                                    tSQLBankManager.TransferCheckFraudControl(null, ReferenceNr, senderName, ServiceId,
                                        hareket.Tutar);

                                isCaughtInFraudControl = !FraudResult;
                                fraudDescription = IsTrusted
                                    ? "Aynı kişi 24 saat veya öncesinde ödeme yaptı, kişi güvenilir olduğu için fraud kontrolü yapılmadı"
                                    : FraudDescription;
                                isTrusted = IsTrusted;
                            }

                            var amountDec = hareket.Tutar;
                            var tranDate = hareket.IslemTarihi ?? DateTime.Now;

                            if (!string.IsNullOrEmpty(Result)
                                && !string.IsNullOrWhiteSpace(ReferenceNr)
                                && Result == "OK"
                                && !string.IsNullOrEmpty(ServiceId)
                                && !string.IsNullOrEmpty(CallbackUrl)
                                && !tSQLBankManager.HasNotificationTransaction(businessKey)
                                && !tSQLBankManager.HasPaymentTransferPool(businessKey)
                                && (isTrusted || amountDec <= AutoTransferLimit)
                                && !isHaveBlockedWord
                                && !isCaughtInFraudControl)
                            {
                                string transactionId = DateTime.Now.Ticks.ToString("D16");

                                var transactionNr = tSQLBankManager.AddNotificationTransaction(
                                    DateTime.Now,
                                    tranDate, tranDate,
                                    bankId, ServiceId, transactionId,
                                    businessKey, amountDec,
                                    hareket.Aciklama, Guid.Empty.ToString(),
                                    senderName, "11111111111",
                                    false, true);

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
                                            transfer_date = tranDate,
                                            amount = amountDec
                                        },
                                        user_entered_data = new
                                        {
                                            sender_name = senderName,
                                            bank_description = hareket.Aciklama
                                        }
                                    };

                                    var pyID = tSQLBankManager.AddAutoPaymentNotification(
                                        tranDate, bankId, senderName, ServiceId, transactionId,
                                        businessKey, amountDec, "Otomatik Bakiye Yükleme İşlemi Bildirimi", "",
                                        companyBankAccountID, isCaughtInFraudControl, fraudDescription);

                                    var pyTransactionNr = tSQLBankManager.GetPaymentNotificationTransactionNr(pyID);

                                    var IDOutAuto = tSQLBankManager.AddPaymentTransferPoolWithReference(
                                        tranDate, bankId, senderName, "",
                                        amountDec, businessKey, hareket.Aciklama,
                                        true, companyBankAccountID, pyTransactionNr,
                                        transactionId, isCaughtInFraudControl, fraudDescription);

                                    if (pyID != null && IDOutAuto != null)
                                    {
                                        tSQLBankManager.SetPaymentTransactionStatus(pyID, (int)StatusType.Confirmed,
                                            "Otomatik Bakiye Yükleme İşlemi Bildirimi");

                                        var response = tHttpClientManager<CallbackResponseModel>.PostJsonDataGetJsonAsync(
                                            CallbackUrl,
                                            new Dictionary<string, string>(),
                                            new Dictionary<string, object> { { "transaction", dataCallback } });

                                        responseStatus = 0;
                                        if (response?.Result != null && !string.IsNullOrEmpty(response.Result.Status))
                                        {
                                            tSQLBankManager.AcceptNotificationTransaction(transactionNr);
                                            responseStatus = response.Result.Status switch
                                            {
                                                "OK" => 1,
                                                "RED" => 2,
                                                "ERROR" => 3,
                                                _ => 0
                                            };
                                        }

                                        tSQLBankManager.AddCallbackResponseLog(
                                            transactionId, "STILPAY",
                                            System.Text.Json.JsonSerializer.Serialize(dataCallback, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }),
                                            companyIntegration.ID, "Ödeme Bildirimi", responseStatus);

                                        tableInsertionErrorCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                        tableInsertionSuccessCount = string.IsNullOrEmpty(IDOutAuto) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
                                    }
                                }
                            }
                            else
                            {
                                if (!tSQLBankManager.HasPaymentTransferPool(businessKey) &&
                                    !tSQLBankManager.HasNotificationTransaction(businessKey))
                                {
                                    var IDOut = tSQLBankManager.AddPaymentTransferPool(
                                        tranDate, bankId, senderName, senderIban ?? "",
                                        amountDec, businessKey, hareket.Aciklama,
                                        companyBankAccountID, isHaveBlockedWord ? (byte)StatusType.Risk : (byte)1,
                                        isCaughtInFraudControl, fraudDescription);
                                    tableInsertionErrorCount = string.IsNullOrEmpty(IDOut) ? tableInsertionErrorCount + 1 : tableInsertionErrorCount;
                                    tableInsertionSuccessCount = string.IsNullOrEmpty(IDOut) ? tableInsertionSuccessCount : tableInsertionSuccessCount + 1;
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
                                  $"Tabloya Kayıt Edilen Hatalı İşlem Sayısı: {tableInsertionErrorCount}\n",
                                  $"Zaman Aşımına Uğrayan Bildirim Sayısı: {timeoutNotificationsCount}\n",
                                  "-------------------------------------------------"));

                Thread.Sleep(queryperiodintervalsecond * 1000);
            }
        }

        #region SOAP Envelope Builder

        private static string BuildVakifbankSoapEnvelope(
            string musteriNo,
            string kurumKullanici,
            string sifre,
            string hesapNo,
            string sorguBaslangicTarihi,
            string sorguBitisTarihi,
            string hareketTipi = "",
            decimal enDusukTutar = 0,
            decimal enYuksekTutar = 0)
        {
            return $@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" 
                xmlns:peak=""Peak.Integration.ExternalInbound.Ekstre"" 
                xmlns:peak1=""http://schemas.datacontract.org/2004/07/Peak.Integration.ExternalInbound.Ekstre.DataTransferObjects"">
    <soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
        <wsa:Action>{SOAP_ACTION_GETIR_HAREKET}</wsa:Action>
        <wsa:To>https://vbservice.vakifbank.com.tr/HesapHareketleri.OnlineEkstre/SOnlineEkstreServis.svc</wsa:To>
    </soap:Header>
    <soap:Body>
        <peak:GetirHareket>
            <peak:sorgu>
                <peak1:MusteriNo>{XmlEncode(musteriNo)}</peak1:MusteriNo>
                <peak1:KurumKullanici>{XmlEncode(kurumKullanici)}</peak1:KurumKullanici>
                <peak1:Sifre>{XmlEncode(sifre)}</peak1:Sifre>
                <peak1:SorguBaslangicTarihi>{sorguBaslangicTarihi}</peak1:SorguBaslangicTarihi>
                <peak1:SorguBitisTarihi>{sorguBitisTarihi}</peak1:SorguBitisTarihi>
                <peak1:HesapNo>{XmlEncode(hesapNo)}</peak1:HesapNo>
                <peak1:HareketTipi>{hareketTipi}</peak1:HareketTipi>
                <peak1:EnDusukTutar>{enDusukTutar}</peak1:EnDusukTutar>
                <peak1:EnYuksekTutar>{enYuksekTutar}</peak1:EnYuksekTutar>
            </peak:sorgu>
        </peak:GetirHareket>
    </soap:Body>
</soap:Envelope>".Trim();
        }

        #endregion

        #region XML Models

        private class VakifbankParsed
        {
            public string BankaKodu { get; set; }
            public string BankaAdi { get; set; }
            public string IslemKodu { get; set; }
            public string IslemAciklamasi { get; set; }
            public List<VakifbankHesap> Hesaplar { get; set; } = new();
        }

        private class VakifbankHesap
        {
            public string HesapNo { get; set; }
            public string MusteriNo { get; set; }
            public string MusteriUnvani { get; set; }
            public string SubeKodu { get; set; }
            public string SubeAdi { get; set; }
            public string DovizTipi { get; set; }
            public decimal AcilisBakiye { get; set; }
            public decimal CariBakiye { get; set; }
            public decimal KullanilabilirBakiye { get; set; }
            public string HesapNoIban { get; set; }
            public bool HesapDurumu { get; set; }
            public DateTime? SonHareketTarihi { get; set; }
            public List<VakifbankHareket> Hareketler { get; set; } = new();
        }

        private class VakifbankHareket
        {
            public DateTime? IslemTarihi { get; set; }
            public string IslemNo { get; set; }         
            public string IslemAdi { get; set; }
            public decimal Tutar { get; set; }
            public string BorcAlacak { get; set; }  
            public string Aciklama { get; set; }
            public decimal IslemOncesiBakiye { get; set; }
            public decimal IslemSonrasiBakiye { get; set; }
            public string IslemYeri { get; set; }
            public string IslemKanal { get; set; }
            public string KartNo { get; set; }
            public string IslemKodu { get; set; }
            public Dictionary<string, string> Detaylar { get; set; }
        }

        #endregion

        #region XML Parser

        private static VakifbankParsed VakifbankParse(string xml)
        {
            var doc = XDocument.Parse(xml);

            XNamespace soap = "http://www.w3.org/2003/05/soap-envelope";
            XNamespace peak = "Peak.Integration.ExternalInbound.Ekstre";
            XNamespace dto = "http://schemas.datacontract.org/2004/07/Peak.Integration.ExternalInbound.Ekstre.DataTransferObjects";
            XNamespace arrays = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

            var parsed = new VakifbankParsed();

            // GetirHareketResult node'unu bul
            var resultNode = doc.Descendants(peak + "GetirHareketResult").FirstOrDefault()
                          ?? doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "GetirHareketResult");

            if (resultNode == null)
            {
                parsed.IslemKodu = "ERROR";
                parsed.IslemAciklamasi = "GetirHareketResult bulunamadı";
                return parsed;
            }

            // Temel bilgiler
            parsed.BankaKodu = GetElementValue(resultNode, dto, "BankaKodu");
            parsed.BankaAdi = GetElementValue(resultNode, dto, "BankaAdi");
            parsed.IslemKodu = GetElementValue(resultNode, dto, "IslemKodu");
            parsed.IslemAciklamasi = GetElementValue(resultNode, dto, "IslemAciklamasi");

            // Hesaplar
            var hesaplarNode = resultNode.Element(dto + "Hesaplar");
            if (hesaplarNode != null)
            {
                foreach (var hesapNode in hesaplarNode.Elements(dto + "DtoEkstreHesap"))
                {
                    var hesap = new VakifbankHesap
                    {
                        HesapNo = GetElementValue(hesapNode, dto, "HesapNo"),
                        MusteriNo = GetElementValue(hesapNode, dto, "MusteriNo"),
                        MusteriUnvani = GetElementValue(hesapNode, dto, "MusteriUnvani"),
                        SubeKodu = GetElementValue(hesapNode, dto, "SubeKodu"),
                        SubeAdi = GetElementValue(hesapNode, dto, "SubeAdi"),
                        DovizTipi = GetElementValue(hesapNode, dto, "DovizTipi"),
                        AcilisBakiye = TryDecimal(GetElementValue(hesapNode, dto, "AcilisBakiye")),
                        CariBakiye = TryDecimal(GetElementValue(hesapNode, dto, "CariBakiye")),
                        KullanilabilirBakiye = TryDecimal(GetElementValue(hesapNode, dto, "KullanilabilirBakiye")),
                        HesapNoIban = GetElementValue(hesapNode, dto, "HesapNoIban"),
                        HesapDurumu = GetElementValue(hesapNode, dto, "HesapDurumu") == "true",
                        SonHareketTarihi = TryDateTime(GetElementValue(hesapNode, dto, "SonHareketTarihi"))
                    };

                    // Hareketler
                    var hareketlerNode = hesapNode.Element(dto + "Hareketler");
                    if (hareketlerNode != null)
                    {
                        foreach (var hareketNode in hareketlerNode.Elements(dto + "DtoEkstreHareket"))
                        {
                            var hareket = new VakifbankHareket
                            {
                                IslemTarihi = TryDateTime(GetElementValue(hareketNode, dto, "IslemTarihi")),
                                IslemNo = GetElementValue(hareketNode, dto, "IslemNo"),
                                IslemAdi = GetElementValue(hareketNode, dto, "IslemAdi"),
                                Tutar = Math.Abs(TryDecimal(GetElementValue(hareketNode, dto, "Tutar"))),
                                BorcAlacak = GetElementValue(hareketNode, dto, "BorcAlacak"),
                                Aciklama = GetElementValue(hareketNode, dto, "Aciklama"),
                                IslemOncesiBakiye = TryDecimal(GetElementValue(hareketNode, dto, "IslemOncesiBakiye")),
                                IslemSonrasiBakiye = TryDecimal(GetElementValue(hareketNode, dto, "IslemSonrasıBakiye")), // Türkçe ı karakteri var
                                IslemYeri = GetElementValue(hareketNode, dto, "IslemYeri"),
                                IslemKanal = GetElementValue(hareketNode, dto, "IslemKanal"),
                                KartNo = GetElementValue(hareketNode, dto, "KartNo"),
                                IslemKodu = GetElementValue(hareketNode, dto, "IslemKodu"),
                                Detaylar = new Dictionary<string, string>()
                            };

                            // Detaylar (Key-Value pairs)
                            var detaylarNode = hareketNode.Element(dto + "Detaylar");
                            if (detaylarNode != null && detaylarNode.Attribute(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") + "nil")?.Value != "true")
                            {
                                foreach (var kvNode in detaylarNode.Elements(arrays + "KeyValueOfstringstring"))
                                {
                                    var key = kvNode.Element(arrays + "Key")?.Value;
                                    var value = kvNode.Element(arrays + "Value")?.Value;
                                    if (!string.IsNullOrEmpty(key))
                                    {
                                        hareket.Detaylar[key] = value ?? "";
                                    }
                                }
                            }

                            hesap.Hareketler.Add(hareket);
                        }
                    }

                    parsed.Hesaplar.Add(hesap);
                }
            }

            return parsed;
        }

        private static string GetElementValue(XElement parent, XNamespace ns, string localName)
        {
            return parent.Element(ns + localName)?.Value ?? "";
        }

        private static decimal TryDecimal(string s)
        {
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                return d;
            return 0m;
        }

        private static DateTime? TryDateTime(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                return dt;
            // Vakıfbank formatı: yyyy-MM-dd HH:mm:ss
            if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt;
            return null;
        }

        #endregion

        #region Helper Methods

        private static (string SenderName, string LeadingRef) ParseRefAndSender(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return ("", null);

            string sender = "";
            string refCode = null;

            var matchLabelRef = Regex.Match(description,
                @"(?i)(?:aciklama|açıklama|ref)\s*[:=]?\s*(\d{6,30})",
                RegexOptions.CultureInvariant);

            var matchPrefixRef = Regex.Match(description,
                @"(\d{6,30})(?=\s*(?:g[öo]nderen|amir))",
                RegexOptions.IgnoreCase);

            var matchStartRef = Regex.Match(description, @"^\s*(\d{6,30})");

            if (matchLabelRef.Success)
            {
                refCode = matchLabelRef.Groups[1].Value;
            }
            else if (matchPrefixRef.Success)
            {
                refCode = matchPrefixRef.Value;
            }
            else if (matchStartRef.Success)
            {
                refCode = matchStartRef.Groups[1].Value;
            }

            // Gönderen adı arama
            var matchSender = Regex.Match(description,
                @"(?i)(?:amir|g[öo]nderen)\s*[:=]?\s*(?<name>.+?)(?=\s*(?:al[ıi]c[ıi]|sorgu|aciklama|açıklama|g[öo]nd|bank|hesap|$))",
                RegexOptions.CultureInvariant);

            if (matchSender.Success)
            {
                sender = matchSender.Groups["name"].Value.Trim();
            }

            if (!string.IsNullOrEmpty(sender))
            {
                sender = Regex.Replace(sender, @"\s{2,}", " ").Trim();
            }

            return (sender, refCode);
        }

        private static string XmlEncode(string s)
            => string.IsNullOrEmpty(s) ? s : System.Security.SecurityElement.Escape(s);

        #endregion
    }
}
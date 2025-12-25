using System.Collections.Generic;
using System.Xml.Serialization;

namespace StilPay.Job.IsBankasi.Models
{


    [XmlRoot(ElementName = "XMLEXBAT")]
    public class IsTransactionModel
    {

        [XmlElement(ElementName = "Tarih")]
        public string Tarih { get; set; }

        [XmlElement(ElementName = "Saat")]
        public string Saat { get; set; }

        [XmlElement(ElementName = "Hesaplar")]
        public Hesaplar Hesaplar { get; set; }
    }


    [XmlRoot(ElementName = "Hesaplar")]
    public class Hesaplar
    {
        [XmlElement(ElementName = "Hesap")]
        public List<Hesap> Hesap { get; set; }
    }


    [XmlRoot(ElementName = "Hesap")]
    public class Hesap
    {
        [XmlElement(ElementName = "Tanimlamalar")]
        public Tanimlamalar Tanimlamalar { get; set; }

        [XmlElement(ElementName = "Hareketler")]
        public Hareketler Hareketler { get; set; }
    }


    [XmlRoot(ElementName = "Tanimlamalar")]
    public class Tanimlamalar
    {
        [XmlElement(ElementName = "HesapTuru")]
        public string HesapTuru { get; set; }

        [XmlElement(ElementName = "HesapNo")]
        public string HesapNo { get; set; }

        [XmlElement(ElementName = "MusteriNo")]
        public string MusteriNo { get; set; }

        [XmlElement(ElementName = "SubeKodu")]
        public string SubeKodu { get; set; }

        [XmlElement(ElementName = "SubeAdi")]
        public string SubeAdi { get; set; }

        [XmlElement(ElementName = "DovizTuru")]
        public string DovizTuru { get; set; }

        [XmlElement(ElementName = "SonHareketTarihi")]
        public string SonHareketTarihi { get; set; }

        [XmlElement(ElementName = "Bakiye")]
        public decimal Bakiye { get; set; }
    }

    [XmlRoot(ElementName = "Hareketler")]
    public class Hareketler
    {
        [XmlElement(ElementName = "Hareket")]
        public List<Hareket> Hareket { get; set; }
    }


    [XmlRoot(ElementName = "Hareket")]
    public class Hareket
    {
        [XmlElement(ElementName = "Tarih")]
        public string Tarih { get; set; }

        [XmlElement(ElementName = "HareketSirano")]
        public string HareketSirano { get; set; }

        [XmlElement(ElementName = "Miktar")]
        public decimal Miktar { get; set; }

        [XmlElement(ElementName = "Bakiye")]
        public decimal Bakiye { get; set; }

        [XmlElement(ElementName = "Aciklama")]
        public string Aciklama { get; set; }

        [XmlElement(ElementName = "timeStamp")]
        public string TimeStamp { get; set; }

        [XmlElement(ElementName = "KarsiHesSahipAdUnvan")]
        public string KarsiHesSahipAdUnvan { get; set; }

        [XmlElement(ElementName = "MüşteriAçıklama")]
        public string MüşteriAçıklama { get; set; }

        [XmlElement(ElementName = "KarsiHesap")]
        public string KarsiHesap { get; set; }

    }

}

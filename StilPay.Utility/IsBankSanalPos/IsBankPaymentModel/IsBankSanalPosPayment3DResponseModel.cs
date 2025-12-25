using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.IsBankSanalPos.IsBankPaymentModel
{
	public class IsBankSanalPosPayment3DResponseModel
	{
		public string oid { get; set; }
		public string cavv { get; set; }
		public string hashAlgorithm { get; set; }
		public string encoding { get; set; }
		public string Ecom_Payment_Card_ExpDate_Month { get; set; }
		public string currency { get; set; }
		public string callbackCall { get; set; }
		public string amount { get; set; }
		public string eci { get; set; }
		public string maskedCreditCard { get; set; }
		public string islemtipi { get; set; }
		public string Ecom_Payment_Card_ExpDate_Year { get; set; }
		public object ShipToStateProv { get; set; }
		public string storetype { get; set; }
		public string mdStatus { get; set; }
		public string failUrl { get; set; }
		public string clientIp { get; set; }
		public string mdErrorMsg { get; set; }
		public string clientid { get; set; }
		public string MaskedPan { get; set; }
		public object BillToStateProv { get; set; }
		public string okUrl { get; set; }
		public string md { get; set; }
		public string xid { get; set; }
		public string lang { get; set; }
		public string HASH { get; set; }
		public string rnd { get; set; }
		public string apiUserName { get; set; }
		public string apiUserPassword { get; set; }
    }
}

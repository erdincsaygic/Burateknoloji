using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;

namespace StilPay.DAL.Concrete
{
    public class PaymentTransferPoolDescriptionControlDAL : BaseDAL<PaymentTransferPoolDescriptionControl>, IPaymentTransferPoolDescriptionControlDAL
    {
        public override string TableName
        {
            get { return "PaymentTransferPoolDescriptionControls"; }
        }

        public bool PaymentWillBlocked(string senderName, string phone, string cardNumber)
        {
            try
            {
                // 1. Parametreleri hazırla
                var parameters = new List<FieldParameter> {
                    new FieldParameter("Name", Enums.FieldType.NVarChar, senderName ?? ""),
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, phone ?? ""),
                    new FieldParameter("CardNumber", Enums.FieldType.NVarChar, cardNumber ?? "")
                };
                _connector = new tSQLConnector();
                var result = _connector.GetBoolean("PaymentTransferPoolDescriptionControls_Check", parameters);

                return result.HasValue && result.Value;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
            }
        }
    }
}

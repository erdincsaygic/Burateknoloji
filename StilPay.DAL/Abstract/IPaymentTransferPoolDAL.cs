using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Abstract
{
    public interface IPaymentTransferPoolDAL : IBaseDAL<PaymentTransferPool>
    {
        public BankLastActivity GetBankLastActivity(string idBank);

        string SetStatus(string id, byte status);
    }
}

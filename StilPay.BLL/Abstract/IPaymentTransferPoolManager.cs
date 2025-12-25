using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface IPaymentTransferPoolManager : IBaseBLL<PaymentTransferPool>
    {
        public BankLastActivity GetBankLastActivity(string idBank);
        GenericResponse SetStatus(string id, byte status);

    }
}

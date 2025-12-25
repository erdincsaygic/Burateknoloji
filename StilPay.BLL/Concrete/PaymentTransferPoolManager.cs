using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Concrete
{
    public class PaymentTransferPoolManager : BaseBLL<PaymentTransferPool>, IPaymentTransferPoolManager
    {
        public PaymentTransferPoolManager(IPaymentTransferPoolDAL dal) : base(dal)
        {
        }

        public BankLastActivity GetBankLastActivity(string idBank)
        {
            return ((IPaymentTransferPoolDAL)_dal).GetBankLastActivity(idBank);
        }

        public GenericResponse SetStatus(string id, byte status)
        {
            try
            {
                var response = ((IPaymentTransferPoolDAL)_dal).SetStatus(id, status);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}

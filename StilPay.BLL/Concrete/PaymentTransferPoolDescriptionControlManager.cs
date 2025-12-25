using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.DAL.Concrete;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class PaymentTransferPoolDescriptionControlManager : BaseBLL<PaymentTransferPoolDescriptionControl>, IPaymentTransferPoolDescriptionControlManager
    {
        public PaymentTransferPoolDescriptionControlManager(IPaymentTransferPoolDescriptionControlDAL dal) : base(dal)
        {
        }

        public GenericResponse PaymentWillBlocked(string senderName, string phone, string cardNumber)
        {
            try
            {
                var response = ((IPaymentTransferPoolDescriptionControlDAL)_dal).PaymentWillBlocked(senderName, phone, cardNumber);

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

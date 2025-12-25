using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class MemberPaymentRequestManager : BaseBLL<MemberPaymentRequest>, IMemberPaymentRequestManager
    {
        public MemberPaymentRequestManager(IMemberPaymentRequestDAL dal) : base(dal)
        {
        }
        public GenericResponse SetStatus(MemberPaymentRequest entity)
        {
            try
            {
                var id = ((IMemberPaymentRequestDAL)_dal).SetStatus(entity);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
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

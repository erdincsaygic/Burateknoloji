using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class MemberMoneyTransferRequestManager : BaseBLL<MemberMoneyTransferRequest>, IMemberMoneyTransferRequestManager
    {
        public MemberMoneyTransferRequestManager(IMemberMoneyTransferRequestDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(MemberMoneyTransferRequest entity)
        {
            try
            {
                var id = ((IMemberMoneyTransferRequestDAL)_dal).SetStatus(entity);

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

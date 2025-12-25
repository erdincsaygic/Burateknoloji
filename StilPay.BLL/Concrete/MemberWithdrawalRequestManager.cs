using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class MemberWithdrawalRequestManager : BaseBLL<MemberWithdrawalRequest>, IMemberWithdrawalRequestManager
    {
        public MemberWithdrawalRequestManager(IMemberWithdrawalRequestDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(MemberWithdrawalRequest entity)
        {
            try
            {
                var id = ((IMemberWithdrawalRequestDAL)_dal).SetStatus(entity);

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

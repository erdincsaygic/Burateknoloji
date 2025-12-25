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
    public class WithdrawalPoolManager : BaseBLL<WithdrawalPool>, IWithdrawalPoolManager
    {
        public WithdrawalPoolManager(IWithdrawalPoolDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(WithdrawalPool entity)
        {
            try
            {
                var id = ((IWithdrawalPoolDAL)_dal).SetStatus(entity);

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

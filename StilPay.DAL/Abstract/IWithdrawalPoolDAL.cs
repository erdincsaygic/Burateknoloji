using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Abstract
{
    public interface IWithdrawalPoolDAL : IBaseDAL<WithdrawalPool>
    {
        string SetStatus(WithdrawalPool entity);
    }
}

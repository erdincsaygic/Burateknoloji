using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Abstract
{
    public interface IWithdrawalPoolManager : IBaseBLL<WithdrawalPool>
    {
        GenericResponse SetStatus(WithdrawalPool entity);
    }

    
}

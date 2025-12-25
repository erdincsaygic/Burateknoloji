using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Abstract
{
    public interface ICallbackResponseLogManager : IBaseBLL<CallbackResponseLog>
    {
        List<AutoCallbackService> AutoCallbackService();
    }
}

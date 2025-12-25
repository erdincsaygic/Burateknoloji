using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System;
using DocumentFormat.OpenXml.ExtendedProperties;
using StilPay.Entities.Dto;

namespace StilPay.BLL.Concrete
{
    public class CallbackResponseLogManager : BaseBLL<CallbackResponseLog>, ICallbackResponseLogManager
    {
        public CallbackResponseLogManager(ICallbackResponseLogDAL dal) : base(dal)
        {
        }

        public List<AutoCallbackService> AutoCallbackService()
        {
            return ((ICallbackResponseLogDAL)_dal).AutoCallbackService();
        }
    }
}

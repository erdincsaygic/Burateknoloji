using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Concrete
{
    public class CurrencyManager : BaseBLL<Currency>, ICurrencyManager
    {
        public CurrencyManager(ICurrencyDAL dal) : base(dal)
        {
        }
    }
}

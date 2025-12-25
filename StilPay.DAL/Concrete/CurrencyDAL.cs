using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class CurrencyDAL : BaseDAL<Currency>, ICurrencyDAL
    {
        public override string TableName
        {
            get { return "Currencies"; }
        }
    }
}

using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Concrete
{
    public class CompanyCurrencyManager : BaseBLL<CompanyCurrency>, ICompanyCurrencyManager
    {
        public CompanyCurrencyManager(ICompanyCurrencyDAL dal) : base(dal)
        {
        }
    }
}

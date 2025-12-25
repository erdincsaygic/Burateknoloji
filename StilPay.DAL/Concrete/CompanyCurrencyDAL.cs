using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.UI.Admin.Models;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class CompanyCurrencyDAL : BaseDAL<CompanyCurrency>, ICompanyCurrencyDAL
    {
        public override string TableName
        {
            get { return "CompanyCurrencies"; }
        }
    }
}

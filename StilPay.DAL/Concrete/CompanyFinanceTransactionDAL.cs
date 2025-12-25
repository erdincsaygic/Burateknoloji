using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System.Dynamic;
using System;

namespace StilPay.DAL.Concrete
{
    public class CompanyFinanceTransactionDAL : BaseDAL<CompanyFinanceTransaction>, ICompanyFinanceTransactionDAL
    {
        public override string TableName
        {
            get { return "CompanyFinanceTransaction"; }
        }
    }
}

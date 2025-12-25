using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class CompanyProgressPaymentCalendarDAL : BaseDAL<CompanyProgressPaymentCalendar>, ICompanyProgressPaymentCalendarDAL
    {
        public override string TableName
        {
            get { return "CompanyProgressPaymentCalendars"; }
        }
    }
}

using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Concrete
{
    public class CompanyProgressPaymentCalendarManager : BaseBLL<CompanyProgressPaymentCalendar>, ICompanyProgressPaymentCalendarManager
    {
        public CompanyProgressPaymentCalendarManager(ICompanyProgressPaymentCalendarDAL dal) : base(dal)
        {
        }
    }
}

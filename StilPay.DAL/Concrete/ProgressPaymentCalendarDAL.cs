using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;


namespace StilPay.DAL.Concrete
{
    public class ProgressPaymentCalendarDAL : BaseDAL<ProgressPaymentCalendar>, IProgressPaymentCalendarDAL
    {
        public override string TableName
        {
            get { return "ProgressPaymentCalendars"; }
        }
    }
}

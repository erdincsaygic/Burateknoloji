using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class ProgressPaymentCalendarManager : BaseBLL<ProgressPaymentCalendar>, IProgressPaymentCalendarManager
    {
        public ProgressPaymentCalendarManager(IProgressPaymentCalendarDAL dal) : base(dal)
        {
        }
    }
}

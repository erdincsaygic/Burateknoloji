using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class NotificationTransactionManager : BaseBLL<NotificationTransaction>, INotificationTransactionManager
    {
        public NotificationTransactionManager(INotificationTransactionDAL dal) : base(dal)
        {
        }
    }
}

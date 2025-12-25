using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class NotificationTransactionDAL : BaseDAL<NotificationTransaction>, INotificationTransactionDAL
    {
        public override string TableName
        {
            get { return "NotificationTransactions"; }
        }
    }
}

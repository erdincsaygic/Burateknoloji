using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;


namespace StilPay.BLL.Concrete
{
    public class MailLogManager : BaseBLL<MailLog>, IMailLogManager
    {
        public MailLogManager(IMailLogDAL dal) : base(dal)
        {
        }
    }
}

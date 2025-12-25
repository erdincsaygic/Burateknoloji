using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class AnnouncementManager : BaseBLL<Announcement>, IAnnouncementManager
    {
        public AnnouncementManager(IAnnouncementDAL dal) : base(dal)
        {
        }
    }
}

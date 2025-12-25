using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class AnnouncementDAL : BaseDAL<Announcement>, IAnnouncementDAL
    {
        public override string TableName
        {
            get { return "Announcements"; }
        }
    }
}

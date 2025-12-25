using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class MainManager : BaseBLL<Main>, IMainManager
    {
        public MainManager(IMainDAL dal) : base(dal)
        {
        }

        public Main GetNotifyCounts()
        {
            return ((IMainDAL)_dal).GetNotifyCounts();
        }
    }
}

using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class TownManager : BaseBLL<Town>, ITownManager
    {
        public TownManager(ITownDAL dal) : base(dal)
        {
        }
    }
}

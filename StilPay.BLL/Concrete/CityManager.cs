using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CityManager : BaseBLL<City>, ICityManager
    {
        public CityManager(ICityDAL dal) : base(dal)
        {
        }
    }
}

using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CityDAL : BaseDAL<City>, ICityDAL
    {
        public override string TableName
        {
            get { return "Cities"; }
        }
    }
}

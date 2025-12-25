using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class TownDAL : BaseDAL<Town>, ITownDAL
    {
        public override string TableName
        {
            get { return "Towns"; }
        }
    }
}

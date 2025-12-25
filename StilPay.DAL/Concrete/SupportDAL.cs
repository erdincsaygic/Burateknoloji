using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class SupportDAL : BaseDAL<Support>, ISupportDAL
    {
        public override string TableName
        {
            get { return "Supports"; }
        }
    }
}

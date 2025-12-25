using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CustomerInfoDAL : BaseDAL<CustomerInfo>, ICustomerInfoDAL
    {
        public override string TableName
        {
            get { return "CustomerInfos"; }
        }
    }
}

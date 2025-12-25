using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CustomerInfoManager : BaseBLL<CustomerInfo>, ICustomerInfoManager
    {
        public CustomerInfoManager(ICustomerInfoDAL dal) : base(dal)
        {
        }
    }
}

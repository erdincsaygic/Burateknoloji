using StilPay.BLL.Abstract;
using StilPay.DAL;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class SupportManager : BaseBLL<Support>, ISupportManager
    {
        public SupportManager(ISupportDAL dal) : base(dal)
        {
        }
    }
}


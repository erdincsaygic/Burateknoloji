using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class BankBalancesLogManager : BaseBLL<BankBalancesLog>, IBankBalancesLogManager
    {
        public BankBalancesLogManager(IBankBalancesLogDAL dal) : base(dal)
        {
        }
    }
}

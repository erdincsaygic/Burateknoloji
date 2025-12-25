using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class BankManager : BaseBLL<Bank>, IBankManager
    {
        public BankManager(IBankDAL dal) : base(dal)
        {
        }

        public List<Bank> GetBanksForIframeSetting()
        {
            return ((IBankDAL)_dal).GetBanksForIframeSetting();
        }
    }
}

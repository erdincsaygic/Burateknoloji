using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CompanyBankManager : BaseBLL<CompanyBank>, ICompanyBankManager
    {
        public CompanyBankManager(ICompanyBankDAL dal) : base(dal)
        {
        }
    }
}

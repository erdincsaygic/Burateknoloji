using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyWithdrawalRequestDAL : IBaseDAL<CompanyWithdrawalRequest>
    {
        string SetStatus(CompanyWithdrawalRequest entity);
        CompanyWithdrawalRequest GetSingleByRequestNr(string requestNr);
        public BankLastActivity GetBankLastActivity(string idBank);
    }
}

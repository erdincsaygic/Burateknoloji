using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyWithdrawalRequestManager : IBaseBLL<CompanyWithdrawalRequest>
    {
        GenericResponse SetStatus(CompanyWithdrawalRequest entity);
        CompanyWithdrawalRequest GetSingleByRequestNr(string requestNr);
        public BankLastActivity GetBankLastActivity(string idBank);
    }
}

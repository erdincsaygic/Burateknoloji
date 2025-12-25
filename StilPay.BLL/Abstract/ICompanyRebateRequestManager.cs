using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyRebateRequestManager : IBaseBLL<CompanyRebateRequest>
    {
        GenericResponse SetStatus(CompanyRebateRequest entity);
        CompanyRebateRequest GetSingleByTransactionID(string transactionID);
        CompanyRebateRequest GetSingleByTransactionNr(string transactionNr);
    }
}

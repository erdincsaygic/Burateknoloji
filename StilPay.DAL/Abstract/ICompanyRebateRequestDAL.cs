using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyRebateRequestDAL : IBaseDAL<CompanyRebateRequest>
    {
        string SetStatus(CompanyRebateRequest entity);
        CompanyRebateRequest GetSingleByTransactionID(string transactionID);
        CompanyRebateRequest GetSingleByTransactionNr(string transactionNr);
    }
}

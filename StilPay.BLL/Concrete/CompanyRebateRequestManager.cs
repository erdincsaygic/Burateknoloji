using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class CompanyRebateRequestManager : BaseBLL<CompanyRebateRequest>, ICompanyRebateRequestManager
    {
        public CompanyRebateRequestManager(ICompanyRebateRequestDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(CompanyRebateRequest entity)
        {
            try
            {
                var id = ((ICompanyRebateRequestDAL)_dal).SetStatus(entity);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }

        public CompanyRebateRequest GetSingleByTransactionID(string transactionID)
        {
            return ((ICompanyRebateRequestDAL)_dal).GetSingleByTransactionID(transactionID);
        }

        public CompanyRebateRequest GetSingleByTransactionNr(string transactionNr)
        {
            return ((ICompanyRebateRequestDAL)_dal).GetSingleByTransactionNr(transactionNr);
        }
    }
}

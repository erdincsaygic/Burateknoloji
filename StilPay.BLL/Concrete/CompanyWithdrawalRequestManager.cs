using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class CompanyWithdrawalRequestManager : BaseBLL<CompanyWithdrawalRequest>, ICompanyWithdrawalRequestManager
    {
        public CompanyWithdrawalRequestManager(ICompanyWithdrawalRequestDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(CompanyWithdrawalRequest entity)
        {
            try
            {
                var id = ((ICompanyWithdrawalRequestDAL)_dal).SetStatus(entity);

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

        public CompanyWithdrawalRequest GetSingleByRequestNr(string requestNr)
        {
            return ((ICompanyWithdrawalRequestDAL)_dal).GetSingleByRequestNr(requestNr);
        }

        public BankLastActivity GetBankLastActivity(string idBank)
        {
            return ((ICompanyWithdrawalRequestDAL)_dal).GetBankLastActivity(idBank);
        }
    }
}

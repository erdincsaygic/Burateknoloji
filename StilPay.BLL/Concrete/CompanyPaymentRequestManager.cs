using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class CompanyPaymentRequestManager : BaseBLL<CompanyPaymentRequest>, ICompanyPaymentRequestManager
    {
        public CompanyPaymentRequestManager(ICompanyPaymentRequestDAL dal) : base(dal)
        {
        }
        public GenericResponse SetStatus(CompanyPaymentRequest entity)
        {
            try
            {
                var id = ((ICompanyPaymentRequestDAL)_dal).SetStatus(entity);

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
    }
}

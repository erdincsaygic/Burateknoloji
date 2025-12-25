using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class PaymentInstitutionDAL : BaseDAL<PaymentInstitution>, IPaymentInstitutionDAL
    {
        public override string TableName
        {
            get { return "PaymentInstitutions"; }
        }
    }
}

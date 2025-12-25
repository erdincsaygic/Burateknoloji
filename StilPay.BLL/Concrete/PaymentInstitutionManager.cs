using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class PaymentInstitutionManager : BaseBLL<PaymentInstitution>, IPaymentInstitutionManager
    {
        public PaymentInstitutionManager(IPaymentInstitutionDAL dal) : base(dal)
        {
        }
    }
}

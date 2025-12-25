using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class MemberInvoiceInformationManager : BaseBLL<MemberInvoiceInformation>, IMemberInvoiceInformationManager
    {
        public MemberInvoiceInformationManager(IMemberInvoiceInformationDAL dal) : base(dal)
        {
        }
    }
}

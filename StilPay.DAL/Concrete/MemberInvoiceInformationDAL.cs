using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class MemberInvoiceInformationDAL : BaseDAL<MemberInvoiceInformation>, IMemberInvoiceInformationDAL
    {
        public override string TableName
        {
            get { return "MemberInvoiceInformations"; }
        }
    }
}

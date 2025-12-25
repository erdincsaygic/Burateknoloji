using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class CreditCardAccountSummaryReportDetailDAL : BaseDAL<CreditCardAccountSummaryReportDetail>, ICreditCardAccountSummaryReportDetailDAL
    {
        public override string TableName
        {
            get { return "CreditCardAccountSummaryReportDetails"; }
        }
    }
}

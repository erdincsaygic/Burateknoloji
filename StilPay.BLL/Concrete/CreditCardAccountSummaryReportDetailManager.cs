using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class CreditCardAccountSummaryReportDetailManager : BaseBLL<CreditCardAccountSummaryReportDetail>, ICreditCardAccountSummaryReportDetailManager
    {
        public CreditCardAccountSummaryReportDetailManager(ICreditCardAccountSummaryReportDetailDAL dal) : base(dal)
        {
        }
    }
}

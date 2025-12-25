using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class BankTransferAccountSummaryReportDetailManager : BaseBLL<BankTransferAccountSummaryReportDetail>, IBankTransferAccountSummaryReportDetailManager
    {
        public BankTransferAccountSummaryReportDetailManager(IBankTransferAccountSummaryReportDetailDAL dal) : base(dal)
        {
        }
    }
}

using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class BankTransferAccountSummaryReportDetailDAL : BaseDAL<BankTransferAccountSummaryReportDetail>, IBankTransferAccountSummaryReportDetailDAL
    {
        public override string TableName
        {
            get { return "BankTransferAccountSummaryReportDetails"; }
        }
    }
}

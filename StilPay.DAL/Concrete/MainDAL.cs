using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Worker;
using System.Data;
using System;

namespace StilPay.DAL.Concrete
{
    public class MainDAL : BaseDAL<Main>, IMainDAL
    {
        public override string TableName
        {
            get { return "Main"; }
        }

        public Main GetNotifyCounts()
        {
            try
            {
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetNotifyCounts", null);

                if(dr != null)
                {
                    var main = new Main()
                    {
                        PaymentNotifications = Convert.ToInt16(dr["PaymentNotifications"]),
                        CreditCardPaymentNotifications = Convert.ToInt16(dr["CreditCardPaymentNotifications"]),
                        ForeignCreditCardPaymentNotifications = Convert.ToInt16(dr["ForeignCreditCardPaymentNotifications"]),
                        CompanyPaymentRequests = Convert.ToInt16(dr["CompanyPaymentRequests"]),
                        MemberPaymentRequests = Convert.ToInt16(dr["MemberPaymentRequests"]),
                        CompanyWithdrawalRequests = Convert.ToInt16(dr["CompanyWithdrawalRequests"]),
                        MemberWithdrawalRequests = Convert.ToInt16(dr["MemberWithdrawalRequests"]),
                        CompanyRebateRequests = Convert.ToInt16(dr["CompanyRebateRequests"]),
                        MemberMoneyTransferRequests = Convert.ToInt16(dr["MemberMoneyTransferRequests"]),
                        CompanyApplications = Convert.ToInt16(dr["CompanyApplications"]),
                        TotalPending = Convert.ToInt16(dr["TotalPending"]),
                        Supports = Convert.ToInt16(dr["Supports"]),
                    };

                    return main;
                }

                return new Main();
            }
            catch { }

            return new Main();
        }
    }
}

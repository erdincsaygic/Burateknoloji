using Coravel.Invocable;
using Microsoft.AspNetCore.Http;
using StilPay.BLL.Abstract;
using StilPay.BLL.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StilPay.BLL.Jobs
{
    public class DealerBalanceControl : IInvocable
    {
        public readonly ICompanyTransactionManager _companyTransactionManager;
        public readonly ICompanyManager _companyManager;
        public readonly ICompanyWithdrawalRequestManager _companyWithdrawalRequestManager;
        public readonly ICompanyRebateRequestManager _companyRebateRequestManager;
        public readonly ICompanyCurrencyManager _companyCurrencyManager;
        public DealerBalanceControl(ICompanyTransactionManager companyTransactionManager, ICompanyManager companyManager, ICompanyWithdrawalRequestManager companyWithdrawalRequestManager, ICompanyRebateRequestManager companyRebateRequestManager, ICompanyCurrencyManager companyCurrencyManager) 
        {
            _companyTransactionManager = companyTransactionManager;
            _companyManager = companyManager;
            _companyWithdrawalRequestManager = companyWithdrawalRequestManager;
            _companyRebateRequestManager = companyRebateRequestManager;
            _companyCurrencyManager = companyCurrencyManager;
        }
        
        public async Task Invoke()
        {
            var companies = _companyManager.GetActiveList(null);

            foreach (var company in companies)
            {
                bool sendSms = false;
                decimal companyTotalBalance = 0;
                decimal companyTransactionTotalBalance = 0;

                for (int i = 0; i < 5; i++)
                {
                    var companyBalance = _companyManager.GetBalance(company.ID);
                    //decimal rebateNetTotal = 0.0M;

                    var companyWithdrawal = _companyWithdrawalRequestManager.GetList(new List<FieldParameter>
                    {
                        new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                        new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                        new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                    });

                    var companyRebate = _companyRebateRequestManager.GetList(new List<FieldParameter>
                    {
                        new FieldParameter("Status", Enums.FieldType.Tinyint, (byte)Enums.StatusType.Pending),
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("IDMember", Enums.FieldType.NVarChar, null),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("PageLenght", Enums.FieldType.Int, 9999),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                        new FieldParameter("SearchValue", Enums.FieldType.NVarChar, null),
                        new FieldParameter("WithProcess", Enums.FieldType.Bit, true)
                    });

                    var companyTransactionBalance = _companyTransactionManager.GetDealerTransactionBalance(new List<FieldParameter>
                    {
                        new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID),
                        new FieldParameter("StartDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("EndDate", Enums.FieldType.DateTime, null),
                        new FieldParameter("IDActionType", Enums.FieldType.Int, null),
                        new FieldParameter("OffsetValue", Enums.FieldType.Int, 0),
                    });

                    
                    //if(companyRebate.Count > 0)
                    //{
                    //    foreach (var item in companyRebate)
                    //    {
                    //        var companyTransaction = _companyTransactionManager.GetSingle(new List<FieldParameter>() { new FieldParameter("ID", Enums.FieldType.NVarChar, item.PaymentRecordID) });
                    //        rebateNetTotal += item.IsPartialRebate ? item.Amount - ((item.Amount * companyTransaction.CommissionRate) / 100) : companyTransaction.NetTotal;
                    //    }
                    //}

                    companyTotalBalance = Math.Round(companyBalance.TotalBalance, 2) + companyWithdrawal.Sum(s => s.Amount + s.CostTotal) + companyRebate.Sum(s => s.Amount);
                    companyTransactionTotalBalance = Math.Round(companyTransactionBalance, 2);

                    var getCompaniesCurrencyBalance = _companyCurrencyManager.GetList(new List<FieldParameter> { new FieldParameter("IDCompany", Enums.FieldType.NVarChar, company.ID) });

                    companyTotalBalance += getCompaniesCurrencyBalance.Sum(s => s.Balance + s.BlockedBalance);


                    decimal difference = Math.Abs(companyTotalBalance - companyTransactionTotalBalance);

                    if (difference > 50)
                        sendSms = true;
                    else
                    {
                        sendSms = false;
                        break;
                    }

                    await Task.Delay(5000);
                }

                if (sendSms)
                {
                    var phones = new List<string>
                    {
                        "05382998128",
                        "05418557879"
                    };

                    foreach (var item in phones)
                    {
                        tSmsSender sender = new tSmsSender();
                        string msg = $"{company.Title} Üye İşyerinin Toplam Bakiyesi İle Cari Hareketlerindeki Bakiyesi Arasında {companyTotalBalance - companyTransactionTotalBalance} TL Fark Bulunmaktadır.";
                        sender.SendSms(item, msg);
                    }
                }
            }
        }
    }
}

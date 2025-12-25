using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyDAL : IBaseDAL<Company>
    {
        dynamic GetBalance(string idCompany);
        string SetBalance(string idCompany, decimal usingBalance, string idActionType, string paymentTransferPoolID);
        string BalanceTransfer(string idCompany, string receiverIdCompany, decimal amount);
        string SetStatus(string idCompany, bool status);
        string SetNegativeBalanceLimit(string idCompany, decimal negativeBalanceLimit); 
        string SetAutoWithdrawalLimit(string idCompany, decimal autoWithdrawalLimit);
        string SetAutoTransferLimit(string idCompany, decimal autoTransferLimit);
        string SetAutoCreditCardLimit(string idCompany, decimal autoCreditCardLimit);
        string SetAutoForeignCreditCardLimit(string idCompany, decimal autoForeignCreditCardLimit);
        string NewDealerInsert(NewDealerDto newDealerDto);

    }
}

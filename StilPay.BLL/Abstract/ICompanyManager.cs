using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyManager : IBaseBLL<Company>
    {
        dynamic GetBalance(string idCompany);

        GenericResponse SetBalance(string idCompany, decimal usingBalance, string idActionType, string paymentTransferPoolID);

        GenericResponse BalanceTransfer(string idCompany, string receiverIdCompany, decimal amount);

        GenericResponse SetNegativeBalanceLimit(string idCompany, decimal negativeBalanceLimit);

        GenericResponse SetStatus(string idCompany, bool status);

        GenericResponse SetAutoWithdrawalLimit(string idCompany, decimal autoWithdrawalLimit);
        GenericResponse SetAutoTransferLimit(string idCompany, decimal autoTransferLimit);
        GenericResponse SetAutoCreditCardLimit(string idCompany, decimal autoCreditCardLimit);
        GenericResponse SetAutoForeignCreditCardLimit(string idCompany, decimal autoForeignCreditCardLimit);

        GenericResponse NewDealerInsert(NewDealerDto newDealerDto);
    }
}

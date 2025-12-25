using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface ICreditCardPaymentNotificationManager : IBaseBLL<CreditCardPaymentNotification>
    {
        GenericResponse SetStatus(CreditCardPaymentNotification entity); 
        //GenericResponse SetDescriptions(CreditCardPaymentNotification entity);
        List<CreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue);
        List<CreditCardPaymentNotification> GetNotBlockeds(string IDCompany , int length, int start, string searchValue);
        CreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionID);
        CreditCardPaymentNotification GetSingleByTransactionID(string transactionID);
        GenericResponse SetMemberIPAdress(string IDEntity, string ipAddress, string port);

        List<CreditCardPaymentNotification> GetPendingList();

        List<GetGetCreditCardTransactionsAPIModel> GetCreditCardTransactionsAPI(List<FieldParameter> parameters);

        List<CreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber);
        CreditCardTransactionCheckFraudControlDto CreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday);
        GenericResponse SetAutoNotification(string entityId, string description, bool isAutoNotification);
    }
}

using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface ICreditCardPaymentNotificationDAL : IBaseDAL<CreditCardPaymentNotification>
    {
        string SetStatus(CreditCardPaymentNotification entity);
        //string SetDescriptions(CreditCardPaymentNotification entity);
        List<CreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue);
        List<CreditCardPaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue);
        CreditCardPaymentNotification GetSingleByTransactionID(string transactionID);
        CreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionID);
        string SetMemberIPAdress(string IDEntity, string ipAddress, string port);
        List<CreditCardPaymentNotification> GetPendingList();
        List<GetGetCreditCardTransactionsAPIModel> GetCreditCardTransactionsAPI(List<FieldParameter> parameters);
        List<CreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber);
        CreditCardTransactionCheckFraudControlDto CreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday);

        string SetAutoNotification(string entityId, string description, bool isAutoNotification);

    }
}

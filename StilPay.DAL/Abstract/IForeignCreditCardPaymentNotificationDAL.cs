using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface IForeignCreditCardPaymentNotificationDAL : IBaseDAL<ForeignCreditCardPaymentNotification>
    {
        string SetStatus(ForeignCreditCardPaymentNotification entity);
        List<ForeignCreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue);
        List<ForeignCreditCardPaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue);
        ForeignCreditCardPaymentNotification GetSingleByTransactionID(string transactionID);
        ForeignCreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionID);
        string SetMemberIPAdress(string IDEntity, string ipAddress, string port);

        List<ForeignCreditCardPaymentNotification> GetPendingList();

        List<ForeignCreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber);

        CreditCardTransactionCheckFraudControlDto ForeignCreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday);

        string SetAutoNotification(string entityId, string description, bool isAutoNotification);

    }
}

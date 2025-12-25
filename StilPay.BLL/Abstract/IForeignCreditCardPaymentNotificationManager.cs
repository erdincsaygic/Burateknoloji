using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface IForeignCreditCardPaymentNotificationManager : IBaseBLL<ForeignCreditCardPaymentNotification>
    {
        GenericResponse SetStatus(ForeignCreditCardPaymentNotification entity);
        List<ForeignCreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue);
        List<ForeignCreditCardPaymentNotification> GetNotBlockeds(string IDCompany , int length, int start, string searchValue);
        ForeignCreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionID);
        ForeignCreditCardPaymentNotification GetSingleByTransactionID(string transactionID);
        GenericResponse SetMemberIPAdress(string IDEntity, string ipAddress, string port);

        List<ForeignCreditCardPaymentNotification> GetPendingList();

        List<ForeignCreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber);

        CreditCardTransactionCheckFraudControlDto ForeignCreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday);

        GenericResponse SetAutoNotification(string entityId, string description, bool isAutoNotification);

    }
}

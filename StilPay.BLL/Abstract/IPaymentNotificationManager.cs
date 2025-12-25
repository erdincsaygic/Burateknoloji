using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface IPaymentNotificationManager : IBaseBLL<PaymentNotification>
    {
        GenericResponse SetStatus(PaymentNotification entity);
        List<PaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue);
        List<PaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue);
        PaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionID);
        PaymentNotification GetSingleByTransactionID(string transactionID);
        PaymentNotification GetSingleByTransactionKey(string transactionKey);
        GenericResponse SetMemberIPAdress(string IDEntity, string ipAddress, string port);

        List<GetPaymentNotificationsAPIModel> GetPaymentNotificationsAPI(List<FieldParameter> parameters);

    }
}

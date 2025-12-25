using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface IPaymentNotificationDAL : IBaseDAL<PaymentNotification>
    {
        string SetStatus(PaymentNotification entity);

        List<PaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue);

        List<PaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue);

        PaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionID);

        PaymentNotification GetSingleByTransactionID(string transactionID);
        string SetMemberIPAdress(string IDEntity, string ipAddress, string port);
        PaymentNotification GetSingleByTransactionKey(string transactionKey);

        List<GetPaymentNotificationsAPIModel> GetPaymentNotificationsAPI(List<FieldParameter> parameters);
    }
}

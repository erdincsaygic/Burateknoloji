using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class PaymentNotificationManager : BaseBLL<PaymentNotification>, IPaymentNotificationManager
    {
        public PaymentNotificationManager(IPaymentNotificationDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(PaymentNotification entity)
        {
            try
            {
                var id = ((IPaymentNotificationDAL)_dal).SetStatus(entity);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }

        public GenericResponse SetMemberIPAdress(string IDEntity, string ipAddress, string port)
        {
            try
            {
                var id = ((IPaymentNotificationDAL)_dal).SetMemberIPAdress(IDEntity, ipAddress, port);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }

        public List<PaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            return ((IPaymentNotificationDAL)_dal).GetBlockeds(IDCompany, length, start, searchValue);
        }

        public List<PaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            return ((IPaymentNotificationDAL)_dal).GetNotBlockeds(IDCompany, length, start, searchValue);
        }

        public PaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionNr)
        {
            return ((IPaymentNotificationDAL)_dal).GetSingleByTransactionNr(IDCompany, transactionNr);
        }
        public PaymentNotification GetSingleByTransactionID(string transactionID)
        {
            return ((IPaymentNotificationDAL)_dal).GetSingleByTransactionID(transactionID);
        }

        public PaymentNotification GetSingleByTransactionKey(string transactionKey)
        {
            return ((IPaymentNotificationDAL)_dal).GetSingleByTransactionKey(transactionKey);
        }

        public List<GetPaymentNotificationsAPIModel> GetPaymentNotificationsAPI(List<FieldParameter> parameters)
        {
            return ((IPaymentNotificationDAL)_dal).GetPaymentNotificationsAPI(parameters);
        }
    }
}

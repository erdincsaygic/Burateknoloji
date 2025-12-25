using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Concrete
{
    public class ForeignCreditCardPaymentNotificationManager : BaseBLL<ForeignCreditCardPaymentNotification>, IForeignCreditCardPaymentNotificationManager
    {
        public ForeignCreditCardPaymentNotificationManager(IForeignCreditCardPaymentNotificationDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(ForeignCreditCardPaymentNotification entity)
        {
            try
            {
                var id = ((IForeignCreditCardPaymentNotificationDAL)_dal).SetStatus(entity);

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
                var id = ((IForeignCreditCardPaymentNotificationDAL)_dal).SetMemberIPAdress(IDEntity, ipAddress, port);

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

        public List<ForeignCreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).GetBlockeds(IDCompany, length, start, searchValue);
        }

        public List<ForeignCreditCardPaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).GetNotBlockeds(IDCompany, length, start, searchValue);
        }

        public ForeignCreditCardPaymentNotification GetSingleByTransactionID(string transactionID)
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).GetSingleByTransactionID(transactionID);
        }
        public ForeignCreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionNr)
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).GetSingleByTransactionNr(IDCompany, transactionNr);
        }

        public List<ForeignCreditCardPaymentNotification> GetPendingList()
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).GetPendingList();
        }
        public List<ForeignCreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber)
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).GetEncryptedCardNumberData(encryptedCardNumber);
        }

        public CreditCardTransactionCheckFraudControlDto ForeignCreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday)
        {
            return ((IForeignCreditCardPaymentNotificationDAL)_dal).ForeignCreditCardTransactionCheckFraudControl(encryptedCardNumber, timeSpanInMinutes, transactionLimitToday);
        }

        public GenericResponse SetAutoNotification(string entityId, string desription, bool isAutoNotification)
        {
            try
            {
                var id = ((IForeignCreditCardPaymentNotificationDAL)_dal).SetAutoNotification(entityId, desription, isAutoNotification);

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
    }
}

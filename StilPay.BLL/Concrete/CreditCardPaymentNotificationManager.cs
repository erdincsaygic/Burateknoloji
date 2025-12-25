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
    public  class CreditCardPaymentNotificationManager : BaseBLL<CreditCardPaymentNotification>, ICreditCardPaymentNotificationManager
    {
        public CreditCardPaymentNotificationManager(ICreditCardPaymentNotificationDAL dal) : base(dal)
        {
        }

        public GenericResponse SetStatus(CreditCardPaymentNotification entity)
        {
            try
            {
                var id = ((ICreditCardPaymentNotificationDAL)_dal).SetStatus(entity);

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

        //public GenericResponse SetDescriptions(CreditCardPaymentNotification entity)
        //{
        //    try
        //    {
        //        var id = ((ICreditCardPaymentNotificationDAL)_dal).SetDescriptions(entity);

        //        return new GenericResponse
        //        {
        //            Status = "OK",
        //            Data = id
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GenericResponse
        //        {
        //            Status = "ERROR",
        //            Message = ex.Message
        //        };
        //    }
        //}

        public GenericResponse SetMemberIPAdress(string IDEntity, string ipAddress, string port)
        {
            try
            {
                var id = ((ICreditCardPaymentNotificationDAL)_dal).SetMemberIPAdress(IDEntity, ipAddress, port);

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

        public List<CreditCardPaymentNotification> GetBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetBlockeds(IDCompany, length, start, searchValue);
        }

        public List<CreditCardPaymentNotification> GetNotBlockeds(string IDCompany, int length, int start, string searchValue)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetNotBlockeds(IDCompany, length, start, searchValue);
        }

        public CreditCardPaymentNotification GetSingleByTransactionID(string transactionID)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetSingleByTransactionID(transactionID);
        }
        public CreditCardPaymentNotification GetSingleByTransactionNr(string IDCompany, string transactionNr)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetSingleByTransactionNr(IDCompany, transactionNr);
        }

        public List<CreditCardPaymentNotification> GetPendingList()
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetPendingList();
        }

        public List<GetGetCreditCardTransactionsAPIModel> GetCreditCardTransactionsAPI(List<FieldParameter> parameters)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetCreditCardTransactionsAPI(parameters);
        }

        public List<CreditCardPaymentNotification> GetEncryptedCardNumberData(string encryptedCardNumber)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).GetEncryptedCardNumberData(encryptedCardNumber);
        }

        public CreditCardTransactionCheckFraudControlDto CreditCardTransactionCheckFraudControl(string encryptedCardNumber, int timeSpanInMinutes, int transactionLimitToday)
        {
            return ((ICreditCardPaymentNotificationDAL)_dal).CreditCardTransactionCheckFraudControl(encryptedCardNumber, timeSpanInMinutes, transactionLimitToday);
        }

        public GenericResponse SetAutoNotification(string entityId, string desription, bool isAutoNotification)
        {
            try
            {
                var id = ((ICreditCardPaymentNotificationDAL)_dal).SetAutoNotification(entityId, desription, isAutoNotification);

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

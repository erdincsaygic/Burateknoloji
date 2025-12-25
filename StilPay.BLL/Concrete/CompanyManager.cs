using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Entities.Dto;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class CompanyManager : BaseBLL<Company>, ICompanyManager
    {
        public CompanyManager(ICompanyDAL dal) : base(dal)
        {
        }

        public dynamic GetBalance(string idCompany)
        {
            return ((ICompanyDAL)_dal).GetBalance(idCompany);
        }

        public GenericResponse SetBalance(string idCompany, decimal usingBalance, string idActionType, string paymentTransferPoolID)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetBalance(idCompany, usingBalance, idActionType, paymentTransferPoolID);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse BalanceTransfer(string idCompany, string receiverIdCompany, decimal amount)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).BalanceTransfer(idCompany, receiverIdCompany, amount);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse SetNegativeBalanceLimit(string idCompany, decimal negativeBalanceLimit)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetNegativeBalanceLimit(idCompany, negativeBalanceLimit);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse SetStatus(string idCompany, bool status)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetStatus(idCompany, status);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse SetAutoWithdrawalLimit(string idCompany, decimal autoWithdrawalLimit)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetAutoWithdrawalLimit(idCompany, autoWithdrawalLimit);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse SetAutoTransferLimit(string idCompany, decimal autoTransferLimit)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetAutoTransferLimit(idCompany, autoTransferLimit);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse SetAutoCreditCardLimit(string idCompany, decimal autoCreditCardLimit)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetAutoCreditCardLimit(idCompany, autoCreditCardLimit);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse SetAutoForeignCreditCardLimit(string idCompany, decimal autoForeignCreditCardLimit)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).SetAutoForeignCreditCardLimit(idCompany, autoForeignCreditCardLimit);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

        public GenericResponse NewDealerInsert(NewDealerDto newDealerDto)
        {
            try
            {
                var response = ((ICompanyDAL)_dal).NewDealerInsert(newDealerDto);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = response
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

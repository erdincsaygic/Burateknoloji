using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class CompanyUserManager : BaseBLL<CompanyUser>, ICompanyUserManager
    {
        public CompanyUserManager(ICompanyUserDAL dal) : base(dal)
        {
        }

        public CompanyUser GetUser(string phone, string password)
        {
            return ((ICompanyUserDAL)_dal).GetUser(phone, password);
        }

        public GenericResponse ResetPassword(CompanyUser entity)
        {
            try
            {
                var id = ((ICompanyUserDAL)_dal).ResetPassword(entity);

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

        public GenericResponse SaveMyAccount(CompanyUser entity)
        {
            try
            {
                var id = ((ICompanyUserDAL)_dal).SaveMyAccount(entity);

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

        public GenericResponse SaveLastLogin(string idUser, string ipAddress)
        {
            try
            {
                var id = ((ICompanyUserDAL)_dal).SaveLastLogin(idUser, ipAddress);

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

        public List<CompanyUser> GetAllCompanyUsers(string IDCompany)
        {
            return ((ICompanyUserDAL)_dal).GetAllCompanyUsers(IDCompany);
        }
    }
}

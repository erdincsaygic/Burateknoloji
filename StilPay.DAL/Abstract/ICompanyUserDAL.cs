using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyUserDAL : IBaseDAL<CompanyUser>
    {
        CompanyUser GetUser(string phone, string password);
        string ResetPassword(CompanyUser entity);
        string SaveMyAccount(CompanyUser entity);
        string SaveLastLogin(string idUser, string ipAddress);
        List<CompanyUser> GetAllCompanyUsers(string IDCompany);
    }
}

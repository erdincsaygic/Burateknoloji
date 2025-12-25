using DocumentFormat.OpenXml.Office2010.ExcelAc;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyUserManager : IBaseBLL<CompanyUser>
    {
        CompanyUser GetUser(string phone, string password);
        GenericResponse ResetPassword(CompanyUser entity);
        GenericResponse SaveMyAccount(CompanyUser entity);
        GenericResponse SaveLastLogin(string idUser, string ipAddress);
        List<CompanyUser> GetAllCompanyUsers(string IDCompany);
    }
}

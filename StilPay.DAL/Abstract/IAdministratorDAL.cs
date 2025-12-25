using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface IAdministratorDAL : IBaseDAL<Administrator>
    {
        Administrator GetAdministrator(string phone, string password);
        string SaveLastLogin(string idAdministrator, string ipAddress);
        string RefreshExitDate(string idAdministrator);
        List<Administrator> GetInOuts();
        List<Administrator> GetLogs(string idAdministrator);
    }
}

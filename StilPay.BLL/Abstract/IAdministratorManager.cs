using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface IAdministratorManager : IBaseBLL<Administrator>
    {
        Administrator GetAdministrator(string phone, string password);
        GenericResponse SaveLastLogin(string idAdministrator, string ipAddress);
        GenericResponse RefreshExitDate(string idAdministrator);
        List<Administrator> GetInOuts();
        List<Administrator> GetLogs(string idAdministrator);
    }
}

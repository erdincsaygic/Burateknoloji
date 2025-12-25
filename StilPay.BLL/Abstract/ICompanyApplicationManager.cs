using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyApplicationManager : IBaseBLL<CompanyApplication>
    {
        CompanyApplication GetApplication(string phone, string password);
        GenericResponse SetApplicationStatus(string id, string cUser, bool status);
        GenericResponse SetFileStatus(string id, string file, byte status, string mUser);
    }
}

using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyApplicationDAL : IBaseDAL<CompanyApplication>
    {
        CompanyApplication GetApplication(string phone, string password);
        string SetApplicationStatus(string id, string cUser, bool status);
        string SetFileStatus(string id, string file, byte status, string mUser);
    }
}

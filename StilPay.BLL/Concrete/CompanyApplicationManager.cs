using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;

namespace StilPay.BLL.Concrete
{
    public class CompanyApplicationManager : BaseBLL<CompanyApplication>, ICompanyApplicationManager
    {
        public CompanyApplicationManager(ICompanyApplicationDAL dal) : base(dal)
        {
        }

        public CompanyApplication GetApplication(string phone, string password)
        {
            return ((ICompanyApplicationDAL)_dal).GetApplication(phone, password);
        }

        public GenericResponse SetApplicationStatus(string id, string cUser, bool status)
        {
            try
            {
                ((ICompanyApplicationDAL)_dal).SetApplicationStatus(id, cUser, status);

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

        public GenericResponse SetFileStatus(string id, string file, byte status, string mUser)
        {
            try
            {
                ((ICompanyApplicationDAL)_dal).SetFileStatus(id, file, status, mUser);

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

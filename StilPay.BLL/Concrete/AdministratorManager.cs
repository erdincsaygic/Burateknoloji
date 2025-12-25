using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class AdministratorManager : BaseBLL<Administrator>, IAdministratorManager
    {
        public AdministratorManager(IAdministratorDAL dal) : base(dal)
        {
        }

        public Administrator GetAdministrator(string phone, string password)
        {
            return ((IAdministratorDAL)_dal).GetAdministrator(phone, password);
        }

        public GenericResponse SaveLastLogin(string idAdministrator, string ipAddress)
        {
            try
            {
                var id = ((IAdministratorDAL)_dal).SaveLastLogin(idAdministrator, ipAddress);

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

        public GenericResponse RefreshExitDate(string idAdministrator)
        {
            try
            {
                var id = ((IAdministratorDAL)_dal).RefreshExitDate(idAdministrator);

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

        public List<Administrator> GetInOuts()
        {
            return ((IAdministratorDAL)_dal).GetInOuts();
        }

        public List<Administrator> GetLogs(string idAdministrator)
        {
            return ((IAdministratorDAL)_dal).GetLogs(idAdministrator);
        }
    }
}

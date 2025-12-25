using StilPay.DAL.Abstract;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class CompanyUserDAL : BaseDAL<CompanyUser>, ICompanyUserDAL
    {
        public override string TableName
        {
            get { return "CompanyUsers"; }
        }

        public override CompanyUser GetSingle(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();
                DataSet ds = _connector.GetDataSet(spGetSingle, parameters);

                var entity = ds.Tables[0].Rows.Count > 0
                    ? CreateAndGetObjectFromDataRow(ds.Tables[0].Rows[0])
                    : new CompanyUser();

                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var item = (CompanyUserRole)CreateAndGetObjectFromDataRow(row, typeof(CompanyUserRole));
                    entity.CompanyUserRoles.Add(item);
                }

                return entity;
            }
            catch { }

            return new CompanyUser();
        }

        public CompanyUser GetUser(string phone, string password)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, phone),
                    new FieldParameter("Password", Enums.FieldType.NVarChar, password),
                };

                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetUser", parameters);

                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return null;
        }

        public string ResetPassword(CompanyUser entity)
        {
            try
            {
                List<FieldParameter> param = new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MDate", Enums.FieldType.NVarChar, entity.MDate),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("Password", Enums.FieldType.NVarChar, entity.Password),
                    new FieldParameter("NewPassword", Enums.FieldType.NVarChar, entity.NewPassword)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_ResetPassword", param);
                _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

                return IDMaster;
            }
            catch (Exception ex)
            {
                if (_connector.SqlConn != null)
                    _connector.CommitOrRollBackTransaction(Enums.TransactionType.RollBack);
                throw new Exception(ex.Message);
            }
        }

        public string SaveMyAccount(CompanyUser entity)
        {
            try
            {
                _connector = new tSQLConnector();

                List<FieldParameter> param = new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MDate", Enums.FieldType.NVarChar, entity.MDate),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("Name", Enums.FieldType.NVarChar, entity.Name),
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, entity.Phone),
                    new FieldParameter("Email", Enums.FieldType.NVarChar, entity.Email),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SaveMyAccount", param);
                _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

                return IDMaster;
            }
            catch (Exception ex)
            {
                if (_connector.SqlConn != null)
                    _connector.CommitOrRollBackTransaction(Enums.TransactionType.RollBack);
                throw new Exception(ex.Message);
            }
        }

        public string SaveLastLogin(string idUser, string ipAddress)
        {
            try
            {
                _connector = new tSQLConnector();

                List<FieldParameter> param = new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idUser),
                    new FieldParameter("IPAddress", Enums.FieldType.NVarChar, ipAddress),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SaveLastLogin", param);
                _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

                return IDMaster;
            }
            catch (Exception ex)
            {
                if (_connector.SqlConn != null)
                    _connector.CommitOrRollBackTransaction(Enums.TransactionType.RollBack);
                throw new Exception(ex.Message);
            }
        }

        public List<CompanyUser> GetAllCompanyUsers(string IDCompany)
        {
            try
            {
                _connector = new tSQLConnector();
                var parameters = new List<FieldParameter> {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, IDCompany)
                };
                DataTable dt = _connector.GetDataTable(TableName + "_GetAllCompanyUsers", parameters);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<CompanyUser>();
        }
    }
}




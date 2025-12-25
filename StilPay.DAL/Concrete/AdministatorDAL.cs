using Microsoft.AspNetCore.Mvc;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace StilPay.DAL.Concrete
{
    public class AdministatorDAL : BaseDAL<Administrator>, IAdministratorDAL
    {
        public override string TableName
        {
            get { return "Administrators"; }
        }

        public override Administrator GetSingle(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();
                DataSet ds = _connector.GetDataSet(spGetSingle, parameters);

                var entity = ds.Tables[0].Rows.Count > 0
                    ? CreateAndGetObjectFromDataRow(ds.Tables[0].Rows[0])
                    : new Administrator();

                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var item = (AdministratorRole)CreateAndGetObjectFromDataRow(row, typeof(AdministratorRole));
                    entity.AdministratorRoles.Add(item);
                }

                return entity;
            }
            catch { }

            return new Administrator();
        }

        public Administrator GetAdministrator(string phone, string password)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, phone),
                    new FieldParameter("Password", Enums.FieldType.NVarChar, password),
                };

                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetAdministrator", parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return null;
        }

        public string SaveLastLogin(string idAdministrator, string ipAddress)
        {
            try
            {
                _connector = new tSQLConnector();

                List<FieldParameter> parameters = new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idAdministrator),
                    new FieldParameter("IPAddress", Enums.FieldType.NVarChar, ipAddress),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SaveLastLogin", parameters);
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

        public string RefreshExitDate(string idAdministrator)
        {
            //try
            //{
            //    _connector = new tSQLConnector();

            //    List<FieldParameter> param = new List<FieldParameter>
            //    {
            //        new FieldParameter("ID", Enums.FieldType.NVarChar, idAdministrator)
            //    };

            //    _connector = new tSQLConnector();
            //    _connector.BeginTransaction();
            //    var IDMaster = _connector.RunSqlCommand(TableName + "_RefreshExitDate", param);
            //    _connector.CommitOrRollBackTransaction(Enums.TransactionType.Commit);

            //    return IDMaster;
            //}
            //catch (Exception ex)
            //{
            //    if (_connector != null && _connector.SqlConn != null)
            //        _connector.CommitOrRollBackTransaction(Enums.TransactionType.RollBack);
            //    throw new Exception(ex.Message);
            //}

            return "Ok";
        }

        public List<Administrator> GetInOuts()
        {
            try
            {
                _connector = new tSQLConnector();
                DataTable dt = _connector.GetDataTable(TableName + "_GetInOuts", null);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<Administrator>();
        }

        public List<Administrator> GetLogs(string idAdministrator)
        {
            try
            {
                _connector = new tSQLConnector();

                List<FieldParameter> parameters = new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idAdministrator)
                };

                DataTable dt = _connector.GetDataTable(TableName + "_GetLogs", parameters);
                return CreateAndGetObjectFromDataTable(dt);
            }
            catch { }

            return new List<Administrator>();
        }
    }
}

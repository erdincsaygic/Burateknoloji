using StilPay.DAL.Abstract;
using StilPay.Entities;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ConstrainedExecution;

namespace StilPay.DAL.Concrete
{
    public class CompanyApplicationDAL : BaseDAL<CompanyApplication>, ICompanyApplicationDAL
    {
        public override string TableName
        {
            get { return "CompanyApplications"; }
        }

        public CompanyApplication GetApplication(string phone, string password)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, phone),
                    new FieldParameter("Password", Enums.FieldType.NVarChar, password),
                };

                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetApplication", parameters);

                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return null;
        }

        public override string Update(CompanyApplication entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                new FieldParameter("MDate", Enums.FieldType.DateTime, DateTime.Now),
                new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                new FieldParameter("IdentityFrontSide", Enums.FieldType.VarBinary, entity.IdentityFrontSide),
                new FieldParameter("IdentityBackSide", Enums.FieldType.VarBinary, entity.IdentityBackSide),
                new FieldParameter("TaxPlate", Enums.FieldType.VarBinary, entity.TaxPlate),
                new FieldParameter("SignatureCirculars", Enums.FieldType.VarBinary, entity.SignatureCirculars),
                new FieldParameter("TradeRegistryGazette", Enums.FieldType.VarBinary, entity.TradeRegistryGazette),
                new FieldParameter("Agreement", Enums.FieldType.VarBinary, entity.Agreement),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(spUpdate, parameters);
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

        public string SetApplicationStatus(string id, string cUser, bool status)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id),
                    new FieldParameter("CUser", Enums.FieldType.NVarChar, cUser),
                    new FieldParameter("CDate", Enums.FieldType.DateTime, DateTime.Now),
                    new FieldParameter("Status", Enums.FieldType.Bit, status)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetAplicationStatus", parameters);
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

        public string SetFileStatus(string id, string file, byte status, string mUser)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, id),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, mUser),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, DateTime.Now),
                    new FieldParameter("File", Enums.FieldType.NVarChar, file),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, status)
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetFileStatus", parameters);
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
    }
}

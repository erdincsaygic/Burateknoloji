using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class MemberDAL : BaseDAL<Member>, IMemberDAL
    {
        public override string TableName
        {
            get { return "Members"; }
        }

        public Member GetMember(string phone)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("Phone", Enums.FieldType.NVarChar, phone)
                };

                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(TableName + "_GetMember", parameters);

                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return null;
        }

        public decimal? GetBalance(string idMember)
        {
            var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idMember)
                };

            _connector = new tSQLConnector();
            return _connector.GetDecimal(TableName + "_GetBalance", parameters);
        }

        public string SaveLastLogin(string idMember, string ipAddress)
        {
            try
            {
                _connector = new tSQLConnector();

                List<FieldParameter> param = new List<FieldParameter>
                {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, idMember),
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

        public string SetMemberType(Member entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, entity.MDate),
                    new FieldParameter("IDMemberType", Enums.FieldType.NVarChar, entity.IDMemberType),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetMemberType", parameters);
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

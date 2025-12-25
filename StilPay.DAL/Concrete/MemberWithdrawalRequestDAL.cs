using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System;

namespace StilPay.DAL.Concrete
{
    public class MemberWithdrawalRequestDAL : BaseDAL<MemberWithdrawalRequest>, IMemberWithdrawalRequestDAL
    {
        public override string TableName
        {
            get { return "MemberWithdrawalRequests"; }
        }

        public string SetStatus(MemberWithdrawalRequest entity)
        {
            try
            {
                var parameters = new List<FieldParameter> {
                    new FieldParameter("ID", Enums.FieldType.NVarChar, entity.ID),
                    new FieldParameter("MUser", Enums.FieldType.NVarChar, entity.MUser),
                    new FieldParameter("MDate", Enums.FieldType.DateTime, entity.MDate),
                    new FieldParameter("Status", Enums.FieldType.Tinyint, entity.Status),
                    new FieldParameter("Description", Enums.FieldType.NVarChar, entity.Description),
                };

                _connector = new tSQLConnector();
                _connector.BeginTransaction();
                var IDMaster = _connector.RunSqlCommand(TableName + "_SetStatus", parameters);
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

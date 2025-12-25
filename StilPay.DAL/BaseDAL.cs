using StilPay.Entities;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace StilPay.DAL
{
    public abstract class BaseDAL<TEntity> : IBaseDAL<TEntity> where TEntity : class, IEntity, new()
    {
        public abstract string TableName { get; }
        protected virtual string spInsert { get { return TableName + "_Insert"; } }
        protected virtual string spUpdate { get { return TableName + "_Update"; } }
        protected virtual string spDelete { get { return TableName + "_Delete"; } }
        protected virtual string spGetSingle { get { return TableName + "_GetSingle"; } }
        protected virtual string spGetList { get { return TableName + "_GetList"; } }
        protected virtual string spActiveList { get { return TableName + "_GetActiveList"; } }

        protected tSQLConnector _connector = null;
        protected List<List<IEntity>> _itemList = null;
        protected List<IEntity> _entityList = null;


        protected PropertyInfo[] _itemProperties;
        protected virtual PropertyInfo[] ItemProperties
        {
            get
            {
                _itemProperties = typeof(TEntity).GetProperties();
                return _itemProperties;
            }
        }


        public virtual string Insert(TEntity entity)
        {
            try
            {
                _connector = new tSQLConnector();
                _connector.BeginTransaction();

                List<FieldParameter> param = GetSQLParametersFromObject(Enums.SQLMode.New, entity);
                string IDMaster = _connector.RunSqlCommand(spInsert, param);

                int i, n = _itemList.Count;
                for (i = 0; i < n; i++)
                {
                    List<IEntity> EntityItems = _itemList[i];
                    int j, m = EntityItems.Count, IDChild;
                    for (j = 0; j < m; j++)
                    {
                        IDChild = j + 1;
                        IEntity EntityItem = EntityItems[j];
                        string EntityName = EntityItem.GetType().Name;
                        param = GetItemSQLParametersFromObject(EntityItem, IDMaster, IDChild);
                        _connector.RunSqlCommand(EntityName + "s_Insert", param);
                    }
                }
                int k, t = _entityList.Count;
                for (k = 0; k < t; k++)
                {
                    IEntity EntityItem = (IEntity)_entityList[k];
                    string EntityName = EntityItem.GetType().Name;
                    param = GetItemSQLParametersFromObject(EntityItem, IDMaster);
                    _connector.RunSqlCommand(EntityName + "s_Insert", param);
                }

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

        public virtual string Update(TEntity entity)
        {
            try
            {
                _connector = new tSQLConnector();
                _connector.BeginTransaction();

                List<FieldParameter> param = GetSQLParametersFromObject(Enums.SQLMode.Edit, entity);
                string IDMaster = _connector.RunSqlCommand(spUpdate, param);

                int i, n = _itemList.Count;
                for (i = 0; i < n; i++)
                {
                    List<IEntity> EntityItems = _itemList[i];
                    int j, m = EntityItems.Count, IDChild;
                    for (j = 0; j < m; j++)
                    {
                        IDChild = j + 1;
                        IEntity EntityItem = EntityItems[j];
                        string EntityName = EntityItem.GetType().Name;
                        param = GetItemSQLParametersFromObject(EntityItem, IDMaster, IDChild);
                        _connector.RunSqlCommand(EntityName + "s_Insert", param);
                    }
                }
                int k, t = _entityList.Count;
                for (k = 0; k < t; k++)
                {
                    IEntity EntityItem = (IEntity)_entityList[k];
                    string EntityName = EntityItem.GetType().Name;
                    param = GetItemSQLParametersFromObject(EntityItem, IDMaster);
                    _connector.RunSqlCommand(EntityName + "s_Insert", param);
                }

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

        public virtual string Delete(TEntity entity)
        {
            try
            {
                _connector = new tSQLConnector();
                _connector.BeginTransaction();

                List<FieldParameter> param = GetSQLParametersFromObject(Enums.SQLMode.Drop, entity);
                string IDMaster = _connector.RunSqlCommand(spDelete, param);

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

        public virtual TEntity GetSingle(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();
                DataRow dr = _connector.GetDataRow(spGetSingle, parameters);
                return CreateAndGetObjectFromDataRow(dr);
            }
            catch { }

            return new TEntity();
        }

        public virtual List<TEntity> GetList(List<FieldParameter> parameters)
        {
            try
            {
                DataTable dtList = GetDataTableList(parameters);
                return CreateAndGetObjectFromDataTable(dtList);
            }
            catch { }

            return new List<TEntity>();
        }

        public virtual DataTable GetDataTableList(List<FieldParameter> parameters)
        {
            _connector = new tSQLConnector();
            return _connector.GetDataTable(spGetList, parameters);
        }

        public virtual List<TEntity> GetActiveList(List<FieldParameter> parameters)
        {
            try
            {
                DataTable dtActiveList = GetActiveDataTableList(parameters);
                return CreateAndGetObjectFromDataTable(dtActiveList);
            }
            catch { }

            return new List<TEntity>();
        }

        public virtual DataTable GetActiveDataTableList(List<FieldParameter> parameters)
        {
            _connector = new tSQLConnector();
            return _connector.GetDataTable(spActiveList, parameters);
        }



        protected List<FieldParameter> GetSQLParametersFromObject(Enums.SQLMode sqlMode, TEntity entity)
        {
            _itemList = new List<List<IEntity>>();
            _entityList = new List<IEntity>();
            List<FieldParameter> fieldList = new List<FieldParameter>();
            int i, n = ItemProperties.Length;
            for (i = 0; i < n; i++)
            {
                object[] attribute = ItemProperties[i].GetCustomAttributes(typeof(FieldAttribute), true);
                if (attribute.Length == 0)
                    continue;

                FieldAttribute fa = (FieldAttribute)attribute[0];

                if (sqlMode == Enums.SQLMode.New && (fa.AutoIncrement || fa.Name == "MDate" || fa.Name == "MUser"))
                    continue;

                if (sqlMode == Enums.SQLMode.Edit && (fa.Name == "CDate" || fa.Name == "CUser"))
                    continue;

                if (sqlMode == Enums.SQLMode.Drop && !fa.PK)
                    continue;

                object val = ItemProperties[i].GetValue(entity);

                if (val == null || val.ToString() == "")
                    val = DBNull.Value;

                if (!fa.Nullable && val == DBNull.Value)
                    continue;
                else if (!fa.Nullable && fa.FK)
                    if (val == DBNull.Value || val == null || val.ToString() == "")
                        continue;

                if (fa.FieldType == Enums.FieldType.NVarChar)
                    if (val != null && val != DBNull.Value)
                        val = val.ToString().Replace("'", "’");

                string fieldName = "@" + ItemProperties[i].Name;
                switch (fa.FieldType)
                {
                    case Enums.FieldType.None:
                        break;
                    case Enums.FieldType.Int:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Int, val));
                        break;
                    case Enums.FieldType.SmallInt:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.SmallInt, val));
                        break;
                    case Enums.FieldType.NVarChar:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.NVarChar, val));
                        break;
                    case Enums.FieldType.Decimal:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Decimal, val));
                        break;
                    case Enums.FieldType.Tinyint:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Tinyint, val));
                        break;
                    case Enums.FieldType.Bit:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Bit, val));
                        break;
                    case Enums.FieldType.DateTime:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.DateTime, val));
                        break;
                    case Enums.FieldType.VarBinary:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.VarBinary, val));
                        break;
                    case Enums.FieldType.List:
                        _itemList.Add((val as IEnumerable<IEntity>).ToList());
                        break;
                    case Enums.FieldType.Entity:
                        _entityList.Add((IEntity)val);
                        break;
                }
            }

            return fieldList;
        }

        protected List<FieldParameter> GetItemSQLParametersFromObject(IEntity entity, string IDMaster, int idChild = 0)
        {
            List<FieldParameter> fieldList = new List<FieldParameter>();
            PropertyInfo[] EntityItemProperties = entity.GetType().GetProperties();
            int i, n = EntityItemProperties.Length;
            for (i = 0; i < n; i++)
            {
                object[] attribute = EntityItemProperties[i].GetCustomAttributes(typeof(FieldAttribute), true);
                if (attribute.Length == 0)
                    continue;

                FieldAttribute fa = (FieldAttribute)attribute[0];

                if (fa.AutoIncrement)
                    continue;

                if (fa.Name == "MDate" || fa.Name == "MUser")
                    continue;

                object val = EntityItemProperties[i].GetValue(entity);

                if (!fa.AutoIncrement && !fa.isMaster && fa.PK)
                    val = idChild;
                else if (fa.isMaster)
                    val = IDMaster;

                if (val == null || Convert.ToString(val) == "")
                    val = DBNull.Value;

                string fieldName = "@" + EntityItemProperties[i].Name;
                if (fa.FieldType == Enums.FieldType.NVarChar)
                {
                    if (val != null && val != DBNull.Value)
                        val = val.ToString().Replace("'", "’");
                }

                switch (fa.FieldType)
                {
                    case Enums.FieldType.None:
                        break;
                    case Enums.FieldType.Int:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Int, val));
                        break;
                    case Enums.FieldType.SmallInt:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.SmallInt, val));
                        break;
                    case Enums.FieldType.NVarChar:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.NVarChar, val));
                        break;
                    case Enums.FieldType.Decimal:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Decimal, val));
                        break;
                    case Enums.FieldType.Tinyint:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Tinyint, val));
                        break;
                    case Enums.FieldType.Bit:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.Bit, val));
                        break;
                    case Enums.FieldType.DateTime:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.DateTime, val));
                        break;
                    case Enums.FieldType.VarBinary:
                        fieldList.Add(new FieldParameter(fieldName, Enums.FieldType.VarBinary, val));
                        break;
                }
            }
            return fieldList;
        }

        protected TEntity CreateAndGetObjectFromDataRow(DataRow row)
        {
            if (row == null) return null;
            TEntity Item = new TEntity();
            int i, n = ItemProperties.Length;
            for (i = 0; i < n; i++)
            {
                object[] temp = ItemProperties[i].GetCustomAttributes(typeof(FieldAttribute), true);

                if (temp.Length == 0) continue;

                FieldAttribute fa = (FieldAttribute)temp[0];

                int index = row.Table.Columns.IndexOf(fa.Name);
                object val = null;
                if (index < 0)
                    continue;
                else if (row[index] == DBNull.Value)
                    val = null;
                else
                    val = row[index];

                ItemProperties[i].SetValue(Item, val, null);
            }
            return Item;
        }

        protected IEntity CreateAndGetObjectFromDataRow(DataRow row, Type NT)
        {
            if (row == null) return null;
            Object Item = Activator.CreateInstance(NT);
            PropertyInfo[] ItemProperties = NT.GetProperties();
            int i, n = ItemProperties.Length;
            for (i = 0; i < n; i++)
            {
                object[] temp = ItemProperties[i].GetCustomAttributes(typeof(FieldAttribute), true);

                if (temp.Length == 0) continue;

                FieldAttribute fa = (FieldAttribute)temp[0];

                int index = row.Table.Columns.IndexOf(fa.Name);
                object val = null;
                if (index < 0)
                    continue;
                else if (row[index] == DBNull.Value)
                    val = null;
                else
                    val = row[index];

                ItemProperties[i].SetValue(Item, val, null);
            }
            return (IEntity)Item;
        }

        protected List<TEntity> CreateAndGetObjectFromDataTable(DataTable dt)
        {
            List<TEntity> TList = new List<TEntity>();
            if (dt == null) return TList;
            int j = 0, k = dt.Rows.Count;
            for (j = 0; j < k; j++)
            {
                DataRow row = dt.Rows[j];
                TEntity item = CreateAndGetObjectFromDataRow(row);
                TList.Add(item);
            }
            return TList;
        }

    }
}
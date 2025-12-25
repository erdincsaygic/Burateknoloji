using StilPay.Entities;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL
{
    public interface IBaseDAL<TEntity> where TEntity : class, IEntity, new()
    {
        string Insert(TEntity entity);
        string Update(TEntity entity);
        string Delete(TEntity entity);
        TEntity GetSingle(List<FieldParameter> parameters);
        List<TEntity> GetList(List<FieldParameter> parameters);
        DataTable GetDataTableList(List<FieldParameter> parameters);
        List<TEntity> GetActiveList(List<FieldParameter> parameters);
        DataTable GetActiveDataTableList(List<FieldParameter> parameters);
    }
}

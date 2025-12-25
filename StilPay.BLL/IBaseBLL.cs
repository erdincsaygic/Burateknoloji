using StilPay.Entities;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Data;

namespace StilPay.BLL
{
    public interface IBaseBLL<TEntity> where TEntity : class, IEntity, new()
    {
        GenericResponse Insert(TEntity entity);

        GenericResponse Update(TEntity entity);

        GenericResponse Delete(TEntity entity);

        TEntity GetSingle(List<FieldParameter> parameters);

        List<TEntity> GetList(List<FieldParameter> parameters);

        DataTable GetDataTableList(List<FieldParameter> parameters);

        List<TEntity> GetActiveList(List<FieldParameter> parameters);

        DataTable GetActiveDataTableList(List<FieldParameter> parameters);
    }
}

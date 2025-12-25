
using StilPay.DAL;
using StilPay.Entities;
using StilPay.Utility.Helper;
using System.Collections.Generic;
using System.Data;
using System;

namespace StilPay.BLL
{
    public abstract class BaseBLL<TEntity> : IBaseBLL<TEntity> where TEntity : class, IEntity, new()
    {
        protected readonly IBaseDAL<TEntity> _dal;

        public BaseBLL(IBaseDAL<TEntity> dal)
        {
            _dal = dal;
        }

        public virtual GenericResponse Insert(TEntity entity)
        {
            try
            {
                var id = _dal.Insert(entity);

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

        public virtual GenericResponse Update(TEntity entity)
        {
            try
            {
                var id = _dal.Update(entity);

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

        public virtual GenericResponse Delete(TEntity entity)
        {
            try
            {
                var id = _dal.Delete(entity);

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

        public TEntity GetSingle(List<FieldParameter> parameters)
        {
            return _dal.GetSingle(parameters);
        }

        public virtual DataTable GetDataTableList(List<FieldParameter> parameters)
        {
            return _dal.GetDataTableList(parameters);
        }

        public virtual List<TEntity> GetList(List<FieldParameter> parameters)
        {
            return _dal.GetList(parameters);
        }

        public virtual DataTable GetActiveDataTableList(List<FieldParameter> parameters)
        {
            return _dal.GetActiveDataTableList(parameters);
        }

        public virtual List<TEntity> GetActiveList(List<FieldParameter> parameters)
        {
            return _dal.GetActiveList(parameters);
        }
    }
}

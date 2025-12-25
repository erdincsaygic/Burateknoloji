using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System.Collections.Generic;
using System.Data;

namespace StilPay.DAL.Concrete
{
    public class BlogDAL : BaseDAL<Blog>, IBlogDAL
    {
        public override string TableName
        {
            get { return "Blogs"; }
        }

        public override Blog GetSingle(List<FieldParameter> parameters)
        {
            try
            {
                _connector = new tSQLConnector();
                DataSet ds = _connector.GetDataSet(spGetSingle, parameters);

                var entity = ds.Tables[0].Rows.Count > 0
                    ? CreateAndGetObjectFromDataRow(ds.Tables[0].Rows[0])
                    : new Blog();
                
                entity.BlogCategories = new List<BlogCategory>();

                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var item = (BlogCategory)CreateAndGetObjectFromDataRow(row, typeof(BlogCategory));
                    entity.BlogCategories.Add(item);
                }

                return entity;
            }
            catch { }

            return new Blog();
        }
    }
}

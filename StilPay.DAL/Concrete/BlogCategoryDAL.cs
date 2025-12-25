using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class BlogCategoryDAL : BaseDAL<BlogCategory>, IBlogCategoryDAL
    {
        public override string TableName
        {
            get { return "BlogCategories"; }
        }
    }
}

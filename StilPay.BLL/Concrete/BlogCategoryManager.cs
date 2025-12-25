using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.BLL.Concrete
{
    public class BlogCategoryManager : BaseBLL<BlogCategory>, IBlogCategoryManager
    {
        public BlogCategoryManager(IBlogCategoryDAL dal) : base(dal)
        {
        }
    }
}

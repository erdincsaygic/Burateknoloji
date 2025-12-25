using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class BlogManager : BaseBLL<Blog>, IBlogManager
    {
        public BlogManager(IBlogDAL dal) : base(dal)
        {
        }
    }
}

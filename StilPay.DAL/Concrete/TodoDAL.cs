using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;


namespace StilPay.DAL.Concrete
{
    public class TodoDAL : BaseDAL<Todo>, ITodoDAL
    {
        public override string TableName
        {
            get { return "Todoist"; }
        }
    }
}

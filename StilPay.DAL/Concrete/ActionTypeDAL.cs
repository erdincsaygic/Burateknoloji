using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class ActionTypeDAL : BaseDAL<ActionTypes>, IActionTypeDAL
    {
        public override string TableName
        {
            get { return "ActionTypes"; }
        }
    }
}

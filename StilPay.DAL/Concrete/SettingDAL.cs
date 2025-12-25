using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.DAL.Concrete
{
    public class SettingDAL : BaseDAL<Setting>, ISettingDAL
    {
        public override string TableName
        {
            get { return "Settings"; }
        }
    }
}

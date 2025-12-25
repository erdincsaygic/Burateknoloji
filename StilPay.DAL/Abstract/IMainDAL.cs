using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface IMainDAL : IBaseDAL<Main>
    {
        Main GetNotifyCounts();
    }
}

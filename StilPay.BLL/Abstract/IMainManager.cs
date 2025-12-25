using StilPay.Entities.Concrete;

namespace StilPay.BLL.Abstract
{
    public interface IMainManager : IBaseBLL<Main>
    {
        Main GetNotifyCounts();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.UI.Dealer.Controllers
{
    [Authorize(Roles = "NotificationTransaction")]
    public class NotificationTransactionController : BaseController<NotificationTransaction>
    {
        private readonly INotificationTransactionManager _manager;

        public NotificationTransactionController(INotificationTransactionManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<NotificationTransaction> Manager()
        {
            return _manager;
        }
    }
}

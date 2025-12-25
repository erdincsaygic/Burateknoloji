using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StilPay.BLL;
using StilPay.BLL.Abstract;
using StilPay.Entities.Concrete;

namespace StilPay.UI.Admin.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberTypeController : BaseController<MemberType>
    {
        private readonly IMemberTypeManager _manager;

        public MemberTypeController(IMemberTypeManager manager, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _manager = manager;
        }

        public override IBaseBLL<MemberType> Manager()
        {
            return _manager;
        }
    }
}
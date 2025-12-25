using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;

namespace StilPay.BLL.Abstract
{
    public interface IMemberManager : IBaseBLL<Member>
    {
        Member GetMember(string phone);
        decimal? GetBalance(string idMember);
        GenericResponse SaveLastLogin(string idUser, string ipAddress);
        GenericResponse SetMemberType(Member entity);
    }
}

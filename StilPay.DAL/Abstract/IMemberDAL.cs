using StilPay.Entities.Concrete;

namespace StilPay.DAL.Abstract
{
    public interface IMemberDAL : IBaseDAL<Member>
    {
        decimal? GetBalance(string idMember);
        Member GetMember(string phone);
        string SaveLastLogin(string idUser, string ipAddress);
        string SetMemberType(Member entity);
    }
}

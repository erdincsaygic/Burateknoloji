using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;

namespace StilPay.BLL.Concrete
{
    public class MemberManager : BaseBLL<Member>, IMemberManager
    {
        public MemberManager(IMemberDAL dal) : base(dal)
        {
        }

        public Member GetMember(string phone)
        {
            return ((IMemberDAL)_dal).GetMember(phone);
        }

        public decimal? GetBalance(string idMember)
        {
            return ((IMemberDAL)_dal).GetBalance(idMember);
        }

        public GenericResponse SaveLastLogin(string idUser, string ipAddress)
        {
            try
            {
                var id = ((IMemberDAL)_dal).SaveLastLogin(idUser, ipAddress);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }

        public GenericResponse SetMemberType(Member entity)
        {
            try
            {
                var id = ((IMemberDAL)_dal).SetMemberType(entity);

                return new GenericResponse
                {
                    Status = "OK",
                    Data = id
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}

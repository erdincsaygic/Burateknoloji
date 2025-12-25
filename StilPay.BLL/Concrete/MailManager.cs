using StilPay.BLL.Abstract;
using StilPay.DAL;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.BLL.Concrete
{
    public class MailManager : BaseBLL<Mail>, IMailManager
    {
        public MailManager(IMailDAL dal) : base(dal)
        {
        }
    }
}

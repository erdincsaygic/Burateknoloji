using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class SmsLogDAL : BaseDAL<SmsLog>, ISmsLogDAL
    {
        public override string TableName
        {
            get { return "SmsLogs"; }
        }
    }
}

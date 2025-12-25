using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.DAL.Concrete
{
    public class PublicHolidayDAL : BaseDAL<PublicHoliday>, IPublicHolidayDAL
    {
        public override string TableName
        {
            get { return "PublicHolidays"; }
        }
    }
}

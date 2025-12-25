using DocumentFormat.OpenXml.Office2010.ExcelAc;
using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface IBankDAL : IBaseDAL<Bank>
    {
        List<Bank> GetBanksForIframeSetting();
    }
}

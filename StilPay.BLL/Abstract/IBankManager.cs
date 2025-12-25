using DocumentFormat.OpenXml.Office2010.ExcelAc;
using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface IBankManager : IBaseBLL<Bank>
    {
        List<Bank> GetBanksForIframeSetting();
    }
}

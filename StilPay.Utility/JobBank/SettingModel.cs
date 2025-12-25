using DocumentFormat.OpenXml.Wordprocessing;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.JobBank
{
    public class SettingModel
    {
        public string ParamType { get; set; }

        public string ParamDef { get; set; }

        public string ParamVal { get; set; }

        public bool ActivatedForGeneralUse { get; set; }
    }
}

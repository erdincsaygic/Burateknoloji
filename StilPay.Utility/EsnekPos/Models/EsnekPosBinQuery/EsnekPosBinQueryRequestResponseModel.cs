using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StilPay.Utility.EsnekPos.Models.EsnekPosBinQuery
{
    public class EsnekPosBinQueryRequestResponseModel
    {

        public string Bank_Code { get; set; }


        public string Bank_Name { get; set; }


        public string Bank_Brand { get; set; }

        public string Card_Type { get; set; }

        public string Card_Family { get; set; }

        public string Card_Kind { get; set; }

    }
}

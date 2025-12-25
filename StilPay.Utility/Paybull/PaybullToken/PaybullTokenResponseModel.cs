using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.Paybull.PaybullToken
{
    public class PaybullTokenResponseModel
    {
        public class Data
        {
            public string token { get; set; }
            public int is_3d { get; set; }
            public string expires_at { get; set; }
        }

        public class Root
        {
            public int status_code { get; set; }
            public string status_description { get; set; }
            public Data data { get; set; }
        }
    }
}

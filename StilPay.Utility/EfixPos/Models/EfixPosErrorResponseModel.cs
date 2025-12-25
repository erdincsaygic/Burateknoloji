using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StilPay.Utility.EfixPos.Models
{
    public class EfixPosErrorResponseModel
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

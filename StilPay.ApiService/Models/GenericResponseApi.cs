using Microsoft.AspNetCore.Mvc;

namespace StilPay.ApiService.Models
{
    public class GenericResponseApi 
    {
        public int ResponseStatus { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}

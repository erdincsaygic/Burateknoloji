using StilPay.Entities.Concrete;
using System.Collections.Generic;

namespace StilPay.UI.WebSite.Areas.Panel.Models
{
    public class InvoiceInformationEditViewModel : EditViewModel<MemberInvoiceInformation>
    {
        public List<City> Cities { get; set; }
        public List<Town> Towns { get; set; }

        public InvoiceInformationEditViewModel()
        {
            Cities = new List<City>();
            Towns = new List<Town>();
        }
    }
}

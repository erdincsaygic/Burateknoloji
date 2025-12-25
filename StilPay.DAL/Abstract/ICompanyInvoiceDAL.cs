using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.DAL.Abstract
{
    public interface ICompanyInvoiceDAL : IBaseDAL<CompanyInvoice>
    {
        List<CompanyInvoice> GetProcess(string idCompany, DateTime startDate, DateTime endDate);
        bool CheckInvoiceDateOverlap(string idCompany, DateTime startDate, DateTime endDate);
        GenericResponse SendInvoiceToIntegrator(string invoiceID);
    }
}

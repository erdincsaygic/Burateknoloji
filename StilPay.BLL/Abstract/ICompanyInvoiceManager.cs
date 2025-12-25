using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Abstract
{
    public interface ICompanyInvoiceManager : IBaseBLL<CompanyInvoice>
    {
        List<CompanyInvoice> GetProcess(string idCompany, DateTime startDate, DateTime endDate);
        bool CheckInvoiceDateOverlap(string idCompany, DateTime startDate, DateTime endDate);
        string ExportExcel(string idInvoice);
        string ExportPDF(string idInvoice);

        GenericResponse SendInvoiceToIntegrator (string invoiceID);
    }
}

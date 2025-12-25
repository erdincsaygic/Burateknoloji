using DocumentFormat.OpenXml.Office2010.Excel;
using StilPay.BLL.Abstract;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;

namespace StilPay.BLL.Concrete
{
    public class CompanyInvoiceManager : BaseBLL<CompanyInvoice>, ICompanyInvoiceManager
    {
        public CompanyInvoiceManager(ICompanyInvoiceDAL dal) : base(dal)
        {
        }

        public List<CompanyInvoice> GetProcess(string idCompany, DateTime startDate, DateTime endDate)
        {
            return ((ICompanyInvoiceDAL)_dal).GetProcess(idCompany, startDate, endDate);
        }

        public string ExportExcel(string idInvoice)
        {
            var _companyTransactionDAL = new DAL.Concrete.CompanyTransactionDAL();
            var list = _companyTransactionDAL.GetCompanyInvoiceTransactions(idInvoice);
            if (list.Count > 0)
            {
                string fileName = "Fatura_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                var excel = ExcelHelper.ExportExcel(list, fileName);
                return excel;
            }
            else
                return null;
        }

        public string ExportPDF(string idInvoice)
        {
            try
            {
                var _companyDAL = new DAL.Concrete.CompanyDAL();
                var _companyInvoiceDAL = new DAL.Concrete.CompanyInvoiceDAL();

                var invoice = _companyInvoiceDAL.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, idInvoice) });
                var company = _companyDAL.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, invoice.IDCompany) });
                if (invoice != null)
                {
                    string fileName = "Fatura_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    var pdf = PDFHelper.ExportPDF(fileName, invoice.Company, company.TaxNr, company.Address, company.Phone, company.Email, invoice.InvoiceNumber.ToString(), invoice.CDate.Date.ToString("dd/MM/yyyy"), "TL", invoice.TotalAmount.ToString("n2"), invoice.TotalAmount.ToString("n2"), "");
                    return pdf;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool CheckInvoiceDateOverlap(string idCompany, DateTime startDate, DateTime endDate)
        {
            return ((ICompanyInvoiceDAL)_dal).CheckInvoiceDateOverlap(idCompany, startDate, endDate);
        }

        public GenericResponse SendInvoiceToIntegrator(string invoiceID)
        {
            try
            {
                var response = ((ICompanyInvoiceDAL)_dal).SendInvoiceToIntegrator(invoiceID);

                return new GenericResponse
                {
                    Status = response.Status,
                    Data = invoiceID,
                    Message = response.Message,
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Status = "ERROR",
                    Message = ex.Message
                };
            }
        }
    }
}

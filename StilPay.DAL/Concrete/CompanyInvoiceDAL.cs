using Microsoft.AspNetCore.Http;
using StilPay.DAL.Abstract;
using StilPay.Entities.Concrete;
using StilPay.Utility.Helper;
using StilPay.Utility.Parasut.Models;
using StilPay.Utility.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace StilPay.DAL.Concrete
{
    public class CompanyInvoiceDAL : BaseDAL<CompanyInvoice>, ICompanyInvoiceDAL
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyInvoiceDAL(IHttpContextAccessor httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override string TableName
        {
            get { return "CompanyInvoices"; }
        }

        //public override CompanyInvoice GetSingle(List<FieldParameter> parameters)
        //{
        //    try
        //    {
        //        _connector = new tSQLConnector();
        //        DataSet ds = _connector.GetDataSet(spGetSingle, parameters);

        //        var entity = ds.Tables[0].Rows.Count > 0
        //            ? CreateAndGetObjectFromDataRow(ds.Tables[0].Rows[0])
        //            : new CompanyInvoice();

        //        entity.CompanyTransactions = new List<CompanyTransaction>();

        //        foreach (DataRow row in ds.Tables[1].Rows)
        //        {
        //            var item = (CompanyTransaction)CreateAndGetObjectFromDataRow(row, typeof(CompanyTransaction));
        //            entity.CompanyTransactions.Add(item);
        //        }

        //        return entity;
        //    }
        //    catch { }

        //    return new CompanyInvoice();
        //}

        public List<CompanyInvoice> GetProcess(string idCompany, DateTime startDate, DateTime endDate)
        {
            try
            {
                _connector = new tSQLConnector();
                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var dtList = _connector.GetDataTable(TableName + "_GetProcess", parameters);
                return CreateAndGetObjectFromDataTable(dtList);
            }
            catch { }

            return new List<CompanyInvoice>();
        }

        public bool CheckInvoiceDateOverlap(string idCompany, DateTime startDate, DateTime endDate)
        {
            try
            {
                _connector = new tSQLConnector();
                List<FieldParameter> parameters = new List<FieldParameter>()
                {
                    new FieldParameter("IDCompany", Enums.FieldType.NVarChar, idCompany),
                    new FieldParameter("StartDate", Enums.FieldType.DateTime,startDate),
                    new FieldParameter("EndDate", Enums.FieldType.DateTime, endDate)
                };

                var result = _connector.GetBoolean(TableName + "_CheckInvoiceDateOverlap", parameters);

                return result.HasValue && result.Value;
            }
            catch
            {
                return false;
            }
        }

        public GenericResponse SendInvoiceToIntegrator(string invoiceID)
        {
            var result = new ResponseModel<InvoiceResponseModel>();

            var _settingDAL = new SettingDAL();
            var _companyDAL = new CompanyDAL();

            var invoice = GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, invoiceID) });

            if(invoice != null && invoice.SendStatus)
                return new GenericResponse { Status = "ERROR", Message = "Entegratöre gönderilmiş fatura tekrar gönderilemez"};


            var company = _companyDAL.GetSingle(new List<FieldParameter> { new FieldParameter("ID", Enums.FieldType.NVarChar, invoice.IDCompany) });
            var parasutSettings = _settingDAL.GetList(new List<FieldParameter> { new FieldParameter("ParamType", Enums.FieldType.NVarChar, "PARASUT") });

            AuthModel auth = new AuthModel
            {
                grant_type = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "grant_type").ParamVal,
                client_id = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "client_id").ParamVal,
                client_secret = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "client_secret").ParamVal,
                redirect_uri = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "redirect_uri").ParamVal,
                username = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "username").ParamVal,
                password = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "password").ParamVal
            };

            string baseUrl = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "base_url").ParamVal;
            string companyID = parasutSettings.FirstOrDefault(x => x.ParamType == "PARASUT" && x.ParamDef == "company_id").ParamVal;

            var authentication = Utility.Parasut.Authentication.GetAccessToken(auth, baseUrl);

            if (!authentication.Status)          
                return new GenericResponse { Status = "ERROR", Message = authentication.Message };

            var invoiceModel = new InvoiceModel
            {
                Amount = decimal.ToDouble(invoice.NetAmount),
                ContactID = company.IDParasut,
                Tax = invoice.TaxRate,
                InvoiceDate = invoice.InvoiceEndDateTime,
                Description = "",
                ProductID = "1007225209",
                CurrencyCode = invoice.CurrencyCode == "TRY" ? "TRL" : invoice.CurrencyCode,
                ExchangeRate = decimal.ToDouble(invoice.ExchangeRate)
            };

            if (string.IsNullOrEmpty(invoiceModel.ContactID))
            {
                if (!company.IsAbroad)
                {
                    CustomerModel customer = new CustomerModel
                    {
                        Name = company.InvoiceTitle,
                        Email = company.Email,
                        Address = company.Address,
                        Iban = "",
                        Phone = company.Phone,
                        TaxNumber = company.TaxNr,
                        TaxOffice = company.TaxOffice,
                        City = company.City,
                        District = company.District
                    };
                    var createCustomer = Utility.Parasut.Customer.Create(customer, companyID, baseUrl, authentication.Data.access_token);
                    if (!createCustomer.Status)
                    {
                        return new GenericResponse { Status = "ERROR", Message = createCustomer.Message };
                    }
                    else
                    {
                        invoiceModel.ContactID = createCustomer.Data.data.id;
                        company.IDParasut = invoiceModel.ContactID;
                        _companyDAL.Update(company);
                    }
                }
                else
                {
                    CustomerModel customer = new CustomerModel
                    {
                        Name = company.InvoiceTitle,
                        Email = "",
                        Address = "",
                        Iban = "",
                        Phone = "",
                        TaxNumber = "2222222222",
                        TaxOffice = "",
                        IsAbroad = true
                    };
                    var createCustomer = Utility.Parasut.Customer.Create(customer, companyID, baseUrl, authentication.Data.access_token);
                    if (!createCustomer.Status)
                    {
                        return new GenericResponse { Status = "ERROR", Message = createCustomer.Message };
                    }
                    else
                    {
                        invoiceModel.ContactID = createCustomer.Data.data.id;
                        company.IDParasut = invoiceModel.ContactID;
                        _companyDAL.Update(company);
                    }
                }
            }

            var createInvoice = Utility.Parasut.Invoice.Create(invoiceModel, companyID, baseUrl, authentication.Data.access_token);

            if(createInvoice.Status) 
            {
                invoice.SendStatus = true;
                invoice.IntegratorInvoiceNumber = createInvoice.Data.data.id;
                invoice.ParasutPrintUrl = createInvoice.Data.data.attributes.print_url;
                invoice.MDate = DateTime.Now;
                var claim = _httpContextAccessor.HttpContext.User.FindFirst(f => f.Type == ClaimTypes.Sid);
                invoice.MUser = claim.Value;
                var resp = Update(invoice);

                var bb = resp;

                return new GenericResponse { Status =  "OK", Message = createInvoice.Message , Data = createInvoice };

            }
            else
                return new GenericResponse { Status = "ERROR", Message = createInvoice.Message };

        }
    }
}

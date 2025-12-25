using MigraDoc.DocumentObjectModel;
using System;
using System.IO;

namespace StilPay.Utility.Helper
{
    public class PDFHelper
    {
        public static string ExportPDF(string fileName, string CompanyName, string CompanyTaxNumber, string CompanyAddress, string CompanyPhone, string CompanyEmail, string InvoiceNumber, string InvoiceDate, string InvoiceCurrency, string InvoiceTotal, string InvoiceTotalText, string InvoiceNote)
        {
			try
			{
                var pdf = new Document();
                var section = pdf.AddSection();
                var table = section.AddTable();
                table.Borders.Width = 0.75;
                table.Borders.Color = Colors.Black;
                table.Rows.LeftIndent = 0;

                var column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Center;

                column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Center;

                column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Center;

                var row = table.AddRow();
                row.Cells[0].AddParagraph("Company Name");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph("Company Tax Number");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph("Company Address");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph(CompanyName);
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(CompanyTaxNumber);
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph(CompanyAddress);
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph("Company Phone");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph("Company Email");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph(CompanyPhone);
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(CompanyEmail);
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph("Company Logo");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph("Invoice Number");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph("Invoice Date");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[1].AddParagraph(InvoiceNumber);
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph(InvoiceDate);
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph("Invoice Due Date");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph("Invoice Currency");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph("Invoice Total");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[1].AddParagraph(InvoiceCurrency);
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].AddParagraph(InvoiceTotal);
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph("Invoice Total Text");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph("Invoice Note");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Cells[0].AddParagraph(InvoiceTotalText);
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(InvoiceNote);
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                
                var renderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);
                renderer.Document = pdf;
                renderer.RenderDocument();

                string path = Path.Combine(Directory.GetCurrentDirectory(), "Download\\PDF\\");
                string fullPath = Path.Combine(path, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                renderer.PdfDocument.Save(fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}

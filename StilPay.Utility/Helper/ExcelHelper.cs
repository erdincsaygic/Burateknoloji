using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;

namespace StilPay.Utility.Helper
{
    public class ExcelHelper
    {
        public static string ExportExcel<T>(List<T> list, string fileName)
        {
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Data");
                int currentRow = 1;
                int currentColumn = 1;

                foreach (var prop in typeof(T).GetProperties())
                {
                    worksheet.Cell(currentRow, currentColumn).Value = prop.Name;
                    currentColumn++;
                }

                currentRow++;
                currentColumn = 1;

                foreach (var item in list)
                {
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        var value = prop.GetValue(item, null);
                        worksheet.Cell(currentRow, currentColumn).Value = value?.ToString();
                        currentColumn++;
                    }
                    currentRow++;
                    currentColumn = 1;
                }

                string path = Path.Combine(Directory.GetCurrentDirectory(), "Download\\Excel\\");
                string fullPath = Path.Combine(path, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                workbook.SaveAs(fullPath);
                return fullPath;
            }
            catch
            {
                return null;
            }
        }
    }
}

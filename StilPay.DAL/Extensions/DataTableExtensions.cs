using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace StilPay.DAL.Extensions // Uygun bir namespace kullanın
{
    public static class DataTableExtensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<T> result = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T item = new T();
                foreach (DataColumn column in table.Columns)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.ColumnName);
                    if (prop != null && row[column] != DBNull.Value)
                    {
                        prop.SetValue(item, Convert.ChangeType(row[column], prop.PropertyType), null);
                    }
                }
                result.Add(item);
            }

            return result;
        }
    }
}


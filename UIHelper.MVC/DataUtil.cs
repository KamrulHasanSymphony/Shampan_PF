using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaizaSMS.UIHelper.MVC
{
    public static class DataUtil

    {
        /// <summary>
        /// Convert an ICollection&lt;T&gt; into a DataTable schema
        /// </summary>
        public static DataTable CreateDataTable<T>() where T : class
        {
            Type objType = typeof(T);
            DataTable table = new DataTable(objType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            foreach (PropertyDescriptor property in properties)
            {
                Type propertyType = property.PropertyType;
                if (!CanUseType(propertyType)) continue; //shallow only

                //nullables must use underlying types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                //enums also need special treatment
                if (propertyType.IsEnum)
                    propertyType = Enum.GetUnderlyingType(propertyType); //probably Int32
                //if you have nested application classes, they just get added. Check if this is valid?
                Debug.WriteLine("table.Columns.Add(\"" + property.Name + "\", typeof(" + propertyType.Name + "));");
                table.Columns.Add(property.Name, propertyType);
            }
            return table;
        }

        /// <summary>
        /// Convert an ICollection&lt;T&gt; into a DataTable
        /// </summary>
        /// <example><code>
        /// ICollection&lt;Person&gt; people = new List&lt;Person&gt;
        ///    {
        ///        new Person { Id= 1, DoB = DateTime.Now, Name = "Bob", Sex = Person.Sexes.Male }
        ///    };
        /// DataTable dt = DataUtil.ConvertToDataTable(people);
        /// </code></example>
        public static DataTable ConvertToDataTable<T>(ICollection<T> collection) where T : class
        {
            DataTable table = CreateDataTable<T>();
            Type objType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            //Debug.WriteLine("foreach (" + objType.Name + " item in collection) {");
            foreach (T item in collection)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor property in properties)
                {
                    if (!CanUseType(property.PropertyType)) continue; //shallow only
                    //Debug.WriteLine("row[\"" + property.Name + "\"] = item." + property.Name + "; //.HasValue ? (object)item." + property.Name + ": DBNull.Value;");
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value; //can't use null
                }
                Debug.WriteLine("//===");
                table.Rows.Add(row);
            }
            return table;
        }

        private static bool CanUseType(Type propertyType)
        {
            //only strings and value types
            if (propertyType.IsArray) return false;
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
            return true;
        }

        /// <summary>
        /// Convert DataTable to ICollection&lt;T&gt;. Some column names should match property names- or you'll have a collection of empty T entities.
        /// </summary>
        /// <example><code>
        /// ICollection&lt;Person&gt; people = new List&lt;Person&gt;
        ///    {
        ///        new Person { Id= 1, DoB = DateTime.Now, Name = "Bob", Sex = Person.Sexes.Male }
        ///    };
        /// DataTable dt = DataUtil.ConvertToDataTable(people);
        /// ICollection&lt;Person&gt; people2 = DataUtil.ConvertToCollection&lt;Person&gt;(dt); //round trip
        /// //Note that people2 is a collection of cloned Person objects
        /// </code></example>
        public static ICollection<T> ConvertToCollection<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0) return null;
            ICollection<T> collection = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T obj = ConvertDataRowToEntity<T>(row);
                collection.Add(obj);
            }
            return collection;
        }

        /// <summary>
        /// Convert a single DataRow into an object of type T.
        /// </summary>
        public static T ConvertDataRowToEntity<T>(DataRow row) where T : class, new()
        {
            Type objType = typeof(T);
            T obj = Activator.CreateInstance<T>(); //hence the new() contsraint
            //Debug.WriteLine(objType.Name + " = new " + objType.Name + "();");
            foreach (DataColumn column in row.Table.Columns)
            {
                //may error if no match
                PropertyInfo property =
                    objType.GetProperty(column.ColumnName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null || !property.CanWrite)
                {
                    //Debug.WriteLine("//Property " + column.ColumnName + " not in object");
                    continue; //or throw
                }
                object value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                property.SetValue(obj, value, null);
                Debug.WriteLine("obj." + property.Name + " = row[\"" + column.ColumnName + "\"];");
            }
            return obj;
        }

        public static DataTable ConvertToDataTable<T1>(string p)
        {
            throw new NotImplementedException();
        }

        //public static DataTable ConvertToDataTable<T1>(global::FaizaSMS.ApplicationService.HRM.Payroll.ViewModel.LoanRepaymentViewModel loan)
        //{
        //    throw new NotImplementedException();
        //}

        //public static DataTable ConvertToDataTable<T1>(List<global::FaizaSMS.WebUI.ViewModels.BoothViewModel> list)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

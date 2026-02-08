using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace UIHelper.MVC
{

   public static class SymERPHtmlHelper
    {
       private static bool CanUseType(Type propertyType)
       {
           //only strings and value types
           if (propertyType.IsArray) return false;
           if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
           return true;
       }

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
        public static HtmlString SimpleEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Form[fullName];
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tb.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tb.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(htmlFieldName, metadata));
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString SimpleDropDown(this HtmlHelper helper, string htmlFieldName)
        {
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Params[fullName];
            TagBuilder tb = new TagBuilder("select");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            if (defaultValue != null)
            {
                tb.Attributes.Add("data-selected", defaultValue);
            }
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString SimpleDropDown(this HtmlHelper helper, string htmlFieldName, string dataUrl)
        {
            return SimpleDropDown(helper, htmlFieldName, dataUrl, null);
        }

        public static HtmlString SimpleDropDown(this HtmlHelper helper, string htmlFieldName, string dataUrl, object htmlAttributes)
        {
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Params[fullName];
            TagBuilder tb = new TagBuilder("select");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            tb.Attributes.Add("data-url", dataUrl);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            tb.AddCssClass("Dropdown");
            if (defaultValue != null)
            {
                tb.Attributes.Add("data-selected", defaultValue);
            }
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Form[fullName];
            TagBuilder tb = new TagBuilder("select");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
            {
                tb.Attributes.Add("data-selected", metadata.Model.ToString());
            }
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tb.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tb.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(htmlFieldName, metadata));
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string dataUrl)
        {
            return SimpleDropDownFor(helper, expression, dataUrl, null);
        }

        public static HtmlString SimpleDropDownFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string dataUrl, object htmlAttributes)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Form[fullName];
            TagBuilder tb = new TagBuilder("select");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            tb.Attributes.Add("data-url", dataUrl);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            tb.AddCssClass("Dropdown");
            //tb.AddCssClass("selectDropdown");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model  != null)
            {
                tb.Attributes.Add("data-selected", metadata.Model.ToString());
                tb.Attributes.Add("val", metadata.Model.ToString());
                
            }
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tb.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tb.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(htmlFieldName, metadata));
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString CascadingDropDown(this HtmlHelper helper, string htmlFieldName, string dataUrl, string parent)
        {
            return CascadingDropDown(helper, htmlFieldName, dataUrl, parent, null);
        }

        public static HtmlString CascadingDropDown(this HtmlHelper helper, string htmlFieldName, string dataUrl, string parent, object htmlAttributes)
        {
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Params[fullName];
            TagBuilder tb = new TagBuilder("select");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            tb.Attributes.Add("data-url", dataUrl);
            tb.Attributes.Add("data-parent", parent);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            tb.AddCssClass("Cascading");

            if (defaultValue != null)
            {
                tb.Attributes.Add("data-selected", defaultValue);
            }
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString CascadingDropDownFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string dataUrl, string parent)
        {
            return CascadingDropDownFor(helper, expression, dataUrl, parent, null);
        }

        public static HtmlString CascadingDropDownFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string dataUrl, string parent, object htmlAttributes)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Form[fullName];
            TagBuilder tb = new TagBuilder("select");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            tb.Attributes.Add("data-url", dataUrl);
            tb.Attributes.Add("data-parent", parent);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            tb.AddCssClass("Cascading");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
            {
                tb.Attributes.Add("data-selected", metadata.Model.ToString());
            }
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tb.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }
            tb.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(htmlFieldName, metadata));
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString DatePicker(this HtmlHelper helper, string htmlFieldName)
        {
            return DatePicker(helper, htmlFieldName, null);
        }
        public static HtmlString DatePicker(this HtmlHelper helper, string htmlFieldName, object htmlAttributes)
        {
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Form[fullName];
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes.Add("type", "text");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            tb.AddCssClass("DatePicker");
            if (defaultValue != null)
            {
                try
                {
                    DateTime date = Convert.ToDateTime(defaultValue);
                    if (date != new DateTime())
                    {
                        tb.Attributes.Add("value", date.ToString("dd-MMM-yyyy"));
                    }
                }
                catch (Exception)
                {
                    tb.Attributes.Add("value", "");
                }
            }
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString DatePickerFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            return DatePickerFor(helper, expression, null);
        }

        public static HtmlString DatePickerFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string defaultValue = HttpContext.Current.Request.Form[fullName];
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes.Add("type", "text");
            tb.Attributes.Add("id", fullName);
            tb.Attributes.Add("name", fullName);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            tb.AddCssClass("DatePicker");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if (metadata.Model != null)
            {
                try
                {
                    DateTime date = Convert.ToDateTime(metadata.Model);
                    if (date != new DateTime())
                    {
                        tb.Attributes.Add("value", date.ToString("dd-MMM-yyyy"));
                    }
                }
                catch (Exception)
                {
                    tb.Attributes.Add("value", "");
                }
            }
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tb.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }
            tb.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(htmlFieldName, metadata));
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString YesNoFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string propertyName = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            TagBuilder tb = new TagBuilder("label");
            tb.Attributes.Add("title", propertyName);
            tb.Attributes.Add("id", propertyName);
            if (metadata.Model != null)
            {
                tb.InnerHtml = Convert.ToBoolean(metadata.Model) ? "Yes" : "No";
            }
            return MvcHtmlString.Create(tb.ToString());
        }

        public static HtmlString YesNoFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string propertyName = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            TagBuilder tb = new TagBuilder("input");
            tb.Attributes.Add("type", "text");
            tb.Attributes.Add("title", propertyName);
            tb.Attributes.Add("id", propertyName);
            tb.Attributes.Add("name", propertyName);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tb.MergeAttributes(attributes);
            }
            if (metadata.Model != null)
            {
                tb.Attributes.Add("Value", Convert.ToBoolean(metadata.Model) ? "Yes" : "No");
            }
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString GoBack(this HtmlHelper helper)
        {
            string href = "#";

            if (HttpContext.Current.Request.UrlReferrer != null
                && !HttpContext.Current.Request.UrlReferrer.LocalPath.StartsWith("/Home/Login")
                )
            {
                href = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            TagBuilder tb = new TagBuilder("a");
            tb.Attributes.Add("href", href);
            tb.Attributes.Add("class", "GoBack");
            return MvcHtmlString.Create(tb.ToString());
        }
    }
}

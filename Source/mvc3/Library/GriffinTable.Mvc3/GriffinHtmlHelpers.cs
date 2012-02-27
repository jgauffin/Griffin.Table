using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace GriffinTable.Mvc3
{
	/// <summary>
	/// HTML helpers
	/// </summary>
	public static class GriffinHtmlHelpers
	{
		/// <summary>
		/// Generate a basic form. Use it as you would with <c>BeginForm</c>
		/// </summary>
		/// <param name="htmlHelper">this</param>
		/// <param name="tableName">Name of the table, same as in <see cref="GriffinTable"/></param>
		/// <param name="route">A route object like <c>new { action: "Items" }</c></param>
		/// <param name="htmlAttributes">Additional html attributes</param>
		/// <param name="method">How to post the form</param>
		/// <returns>form container</returns>
		public static MvcForm GriffinTableForm(this HtmlHelper htmlHelper, string tableName, object route = null, object htmlAttributes = null, FormMethod method = FormMethod.Post)
		{
			var routeDictionary = new RouteValueDictionary(route);
			var attributes = htmlAttributes == null ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);


			var tagBuilder = new TagBuilder("form");
			tagBuilder.MergeAttributes(attributes);
			tagBuilder.MergeAttribute("id", tableName + "-form");
			var controllerName = (string)routeDictionary["controller"] ?? htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
			var actionName = (string)routeDictionary["action"] ?? htmlHelper.ViewContext.RouteData.Values["action"].ToString();
			var helper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
			var uri = helper.Action(actionName, controllerName, routeDictionary);
			tagBuilder.MergeAttribute("action", uri);
			tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method));


			htmlHelper.ViewContext.Writer.WriteLine(tagBuilder.ToString(TagRenderMode.StartTag));
			return new MvcForm(htmlHelper.ViewContext);
		}

		/// <summary>
		/// Generate a table
		/// </summary>
		/// <typeparam name="T">Type of entity to list</typeparam>
		/// <param name="helper">this</param>
		/// <param name="tableName">Name of the table</param>
		/// <param name="model">Model used to generate rows</param>
		/// <param name="options">Additional options</param>
		/// <returns>Generated table</returns>
		public static MvcHtmlString GriffinTable<T>(this HtmlHelper helper, string tableName, GriffinTableViewModel<T> model, GriffinTableOptions options = GriffinTableOptions.None)
			where T : class
		{
			var targetType = typeof(T);
			var sb = new StringBuilder();
			sb.AppendFormat(@"<table id=""{0}"" class=""striped"">", tableName);
			sb.AppendLine("\r\n\t<thead>");
			sb.AppendLine("\t\t<tr>");
			var hidden = new List<bool>();
			foreach (var property in targetType.GetProperties())
			{
				if (!property.CanRead)
					continue;


				var displayAttribute = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
				var name = displayAttribute != null ? displayAttribute.Name ?? displayAttribute.ShortName : property.Name;

				if (name == "")
				{
					hidden.Add(true);
					sb.AppendFormat("\t\t\t<th rel=\"{0}\" style=\"display:none\">{1}</th>\r\n", property.Name, name);
				}
				else
				{
					hidden.Add(false);
					sb.AppendFormat("\t\t\t<th rel=\"{0}\">{1}</th>\r\n", property.Name, name);
				}
			}
            sb.AppendLine("\t\t</tr>");
            sb.AppendLine("\t</thead>");
			sb.AppendLine("<tbody>");

			if (model != null)
			{
				foreach (var row in model.Rows)
				{
					sb.AppendLine("\t\t<tr>");
					var index = 0;
					foreach (var property in targetType.GetProperties())
					{
						if (!property.CanRead)
							continue;

						var value = property.GetValue(row, null);
						if (hidden[index++])
							sb.AppendFormat("\t\t\t<td style=\"display:none;\">{0}</td>\r\n", value);
						else
							sb.AppendFormat("\t\t\t<td>{0}</td>\r\n", value);
					}
					sb.AppendLine("\t\t</tr>");
				}
			}

			sb.AppendLine("\t</tbody>");
			sb.AppendLine("</table>");

			if (options == GriffinTableOptions.CreateScript)
			{
				if (model != null)
				{
					var str = @"<script type=""text/javascript"">
$(function(){{
	$('#{0}').griffinTable({{ totalRowCount: {1}, pageSize: {2} }});
}});
</script>";
					sb.AppendLine(string.Format(str, tableName, model.TotalRowCount, model.PageSize));
				}
				else
				{
					var str = @"<script type=""text/javascript"">
$(function(){{
	$('#{0}').griffinTable({{ fetchAtStart: true }});
}});
</script>";
					sb.AppendLine(string.Format(str, tableName));
					
				}
			}

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}
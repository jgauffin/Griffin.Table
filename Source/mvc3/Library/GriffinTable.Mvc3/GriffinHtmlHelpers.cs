using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GriffinTable.Demo.Controllers;

namespace GriffinTable.Mvc3
{
	public static class GriffinHtmlHelpers
	{
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

		private class GriffinForm : IDisposable
		{
			private readonly HttpResponseBase _httpResponseBase;

			public GriffinForm(HttpResponseBase httpResponseBase)
			{
				_httpResponseBase = httpResponseBase;
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
			{
				_httpResponseBase.Write("</form>");
			}
		}

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
			sb.AppendLine("\t\t</thead>");
			sb.AppendLine("\t</tr>");
			sb.AppendLine("\t<tbody>");

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

			sb.AppendLine("\t</tbody>");
			sb.AppendLine("</table>");

			if (options == GriffinTableOptions.CreateScript)
			{
				var str = @"<script type=""text/javascript"">
$(function(){{
	$('#{0}').griffinTable({{ totalRowCount: {1}, pageSize: {2} }});
}});
</script>";
				sb.AppendLine(string.Format(str, tableName, model.TotalRowCount, model.PageSize));
			}

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}
using System.Collections.Generic;
using System.Web.Mvc;

namespace GriffinTable.Mvc3
{
	/// <summary>
	/// Extension methods for the controller
	/// </summary>
	public static class GriffinTableExtensions
	{
		/// <summary>
		/// Return a new table result
		/// </summary>
		/// <typeparam name="T">Type of model to return</typeparam>
		/// <param name="controller">this</param>
		/// <param name="model">Collection of view models</param>
		/// <param name="totalCount">Total number of matching rows (if using paged result)</param>
		/// <returns>JSON result</returns>
		public static ActionResult GriffinTable<T>(this Controller controller, IEnumerable<T> model, int totalCount)
		{
			return new JsonResult
					{
						Data = new GriffinTableAjaxResponse
								{
									TotalRowCount = totalCount,
									Rows = model
								},
						JsonRequestBehavior = JsonRequestBehavior.AllowGet
					};
		}


	}
}
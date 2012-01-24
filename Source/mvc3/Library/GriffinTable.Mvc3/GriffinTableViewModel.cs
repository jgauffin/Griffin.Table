using System.Collections.Generic;

namespace GriffinTable.Mvc3
{
	/// <summary>
	/// Base view model that you can use to work with the table without ajax.
	/// </summary>
	/// <typeparam name="T">Type of entity that the table should list</typeparam>
	public class GriffinTableViewModel<T>
	{
		/// <summary>
		/// Gets total number of rows
		/// </summary>
		/// <remarks>Set to 0 to disable paging</remarks>
		public int TotalRowCount { get; set; }

		/// <summary>
		/// Gets a collection of rows to add to the table
		/// </summary>
		public IEnumerable<T> Rows { get; set; }

		/// <summary>
		/// Gets number of items per page.
		/// </summary>
		public int PageSize { get; set; }
	}
}
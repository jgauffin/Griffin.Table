using System.Collections;

namespace GriffinTable.Mvc3
{
	/// <summary>
	/// Response that should be sent back to the table
	/// </summary>
	public class GriffinTableAjaxResponse
	{
		/// <summary>
		/// Gets or sets total number of rows
		/// </summary>
		public int TotalRowCount { get; set; }

		/// <summary>
		/// Get or sets the actual rows.
		/// </summary>
		public IEnumerable Rows { get; set; }
	}
}
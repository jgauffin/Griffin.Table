namespace GriffinTable.Mvc3
{
	/// <summary>
	/// A request made from the plugin
	/// </summary>
	public class GriffinTableAjaxRequest
	{
		/// <summary>
		/// Gets or sets column that we should sort after
		/// </summary>
		public string SortColumn { get; set; }

		/// <summary>
		/// Gets or sets sort order "asc", "desc" or ""
		/// </summary>
		public string SortOrder { get; set; }

		/// <summary>
		/// Gets number of items per page
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Gets page number (one-based index)
		/// </summary>
		public int PageNumber { get; set; }

		/// <summary>
		/// Convert the sort order into an enum
		/// </summary>
		/// <returns></returns>
		public SortOrder ParseSortOrder()
		{
			switch (SortOrder)
			{
				case "asc":
					return Mvc3.SortOrder.Ascending;
				case "desc":
					return Mvc3.SortOrder.Descending;
				default:
					return Mvc3.SortOrder.None;
			}
		}
	}
}
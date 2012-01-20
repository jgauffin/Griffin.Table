namespace GriffinTable.Demo.Controllers
{
	public class GriffinTableAjaxRequest
	{
		public string SortColumn { get; set; }
		public string SortOrder { get; set; }
		public int PageSize { get; set; }
		public int PageNumber { get; set; }

		public SortOrder ParseSortOrder()
		{
			switch (SortOrder)
			{
				case "asc":
					return Controllers.SortOrder.Ascending;
				case "desc":
					return Controllers.SortOrder.Descending;
				default:
					return Controllers.SortOrder.None;
			}
		}
	}
}
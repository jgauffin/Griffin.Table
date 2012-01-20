using System.Collections.Generic;

namespace GriffinTable.Demo.Controllers
{
	public class GriffinTableViewModel<T>
	{
		public int TotalRowCount { get; set; }
		public IEnumerable<T> Rows { get; set; }
		public int PageSize { get; set; }
	}
}
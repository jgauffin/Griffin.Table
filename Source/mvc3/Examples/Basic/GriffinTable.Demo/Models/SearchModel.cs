using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GriffinTable.Demo.Controllers;

namespace GriffinTable.Demo.Models
{
	public class SearchModel : GriffinTableAjaxRequest
	{
		public string Term { get; set; }
	}
}
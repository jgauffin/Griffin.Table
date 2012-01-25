using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GriffinTable.Demo.Models;
using GriffinTable.Mvc3;

namespace GriffinTable.Demo.Controllers
{
	public class BasicPagedController : UserBaseController
	{
		public ActionResult Index()
		{
			var items = UserRepository.Users.Select(usr => new TableModel(usr, Url)).Take(20);

			return View();
		}

		public ActionResult CustomPager()
		{
			var items = UserRepository.Users.Select(usr => new TableModel(usr, Url)).Take(20);

			return View(new GriffinTableViewModel<TableModel>
			{
				PageSize = 20,
				Rows = items,
				TotalRowCount = UserRepository.Users.Count()
			});
		}


		[OutputCache(NoStore = true, Duration = 0)]
		public ActionResult Items(SearchModel model)
		{
			var result = Filter(model);
			var totalCount = result.Count();
			result = Sort(model, result);
			result = Page(model, result);
			var viewModelResult = result.Select(usr => new TableModel(usr, Url));

			return this.GriffinTable(viewModelResult, totalCount);

		}

		public ActionResult About()
		{
			return View();
		}
	}
}

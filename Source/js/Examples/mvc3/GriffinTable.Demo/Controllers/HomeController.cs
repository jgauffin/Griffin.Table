using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GriffinTable.Demo.Models;

namespace GriffinTable.Demo.Controllers
{
    public class User
    {
        
    }
    public class HomeController : Controller
    {
        private static List<UserModel> _users = new List<UserModel>();

        public HomeController()
        {
            if (_users.Count == 0)
            {
                var fk = new FakeUsers();
                _users = fk.Generate(200);
            }
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Items(SearchRequestModel model)
        {
        	return Json(new SearchResponse
        	            	{
        	            		RowMode = model.PageNumber < 2 ? RowMode.Clear : RowMode.Append,
        	            		TotalRowCount = _users.Count,
        	            		Rows = _users.Skip((model.PageNumber - 1)*model.PageSize).Take(model.PageSize)
        	            	}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public enum RowMode
    {
        Clear = 0,
        Append = 1,
    }

    public class SearchResponse
    {
        /// <summary>
        /// Gets or sets total number of rows
        /// </summary>
        public int TotalRowCount { get; set; }

        /// <summary>
        /// Gets or sets if rows should be appended or if the table should be cleared first.
        /// </summary>
        public RowMode RowMode { get; set; }

        /// <summary>
        /// Get or sets the actual rows.
        /// </summary>
        public IEnumerable Rows { get; set; }
    }

    

    public class SearchRequestModel
    {
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string SearchValue { get; set; }
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


    public enum SortOrder
    {
        None,
        Ascending,
        Descending
    }
}

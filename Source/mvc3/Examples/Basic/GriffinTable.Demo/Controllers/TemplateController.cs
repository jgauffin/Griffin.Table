using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GriffinTable.Demo.Models;
using GriffinTable.Mvc3;

namespace GriffinTable.Demo.Controllers
{
    public class TemplateController : UserBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tmpl()
        {
            return View();
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
    }
}

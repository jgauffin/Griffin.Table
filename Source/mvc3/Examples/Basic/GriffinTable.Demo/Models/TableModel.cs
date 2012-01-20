using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GriffinTable.Demo.Repositories;

namespace GriffinTable.Demo.Models
{
	public class TableModel
	{
		public TableModel(UserEntity usr, UrlHelper url)
		{
			Age = usr.Age;
			Department = usr.Department;
			FirstName = usr.FirstName;
			Id = usr.Id;
			LastName = usr.LastName;
			Options = string.Format(@"<a href=""{0}"">{1}</a>", url.Action("Edit"), "Edit");
			Title = usr.Title;
		}

		[Display(Name = "")]
		public int Id { get; set; }

		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		public string Department { get; set; }
		public string Title { get; set; }
		public int Age { get; set; }

		[Display(Name = "&nbsp;")]
		public string Options { get; set; }
	}
}
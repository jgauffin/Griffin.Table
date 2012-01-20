using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using GriffinTable.Demo.Models;
using GriffinTable.Demo.Repositories;

namespace GriffinTable.Demo.Controllers
{
	public class UserBaseController : Controller
	{
		protected static IQueryable<UserEntity> Page(SearchModel model, IQueryable<UserEntity> result)
		{
			var pagedResult = result
				.Skip((model.PageNumber - 1)*model.PageSize)
				.Take(model.PageSize);
			return pagedResult;
		}

		protected static IQueryable<UserEntity> Sort(SearchModel model, IQueryable<UserEntity> result)
		{
			if (!string.IsNullOrEmpty(model.SortColumn))
			{
				var param = Expression.Parameter(typeof (UserEntity), "t");
				var sortExpression = Expression.Lambda<Func<UserEntity, object>>
					(Expression.Property(param, model.SortColumn), param);

				switch (model.SortOrder)
				{
					case "asc":
						result = result.OrderBy(sortExpression);
						break;
					case "desc":
						result = result.OrderByDescending(sortExpression);
						break;
				}
			}
			return result;
		}

		protected static IQueryable<UserEntity> Filter(SearchModel model)
		{
			var result = string.IsNullOrEmpty(model.Term)
			             	? UserRepository.Users
			             	: UserRepository.Users.Where(t =>
			             	                             t.LastName.StartsWith(model.Term, StringComparison.OrdinalIgnoreCase)
			             	                             || t.FirstName.StartsWith(model.Term, StringComparison.OrdinalIgnoreCase)
			             	                             || t.Title.StartsWith(model.Term, StringComparison.OrdinalIgnoreCase));
			return result;
		}
	}
}
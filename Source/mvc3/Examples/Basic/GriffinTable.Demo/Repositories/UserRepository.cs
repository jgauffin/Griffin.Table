using System.Collections.Generic;
using System.Linq;
using GriffinTable.Demo.Models;
using GriffinTable.Demo.Repositories;

namespace GriffinTable.Demo.Controllers
{
	/// <summary>
	/// Fake repos, hence static
	/// </summary>
	public static class UserRepository
	{
		private readonly static List<UserEntity> _users = new List<UserEntity>();

		static UserRepository()
		{
			var fk = new FakeUsers();
			_users = fk.Generate(200);
		}

		public static IQueryable<UserEntity> Users
		{
			get { return _users.AsQueryable(); }

		}
	}
}
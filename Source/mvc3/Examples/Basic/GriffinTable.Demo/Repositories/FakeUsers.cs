using System;
using System.Collections.Generic;

namespace GriffinTable.Demo.Repositories
{
	public class FakeUsers
	{
		public List<UserEntity> Generate(int count)
		{
			var fn = new FirstNames();
			var ln = new LastNames();
			var d = new Departments();
			var t = new Titles();
			var r = new Random();

			List<UserEntity> users = new List<UserEntity>();
			for (int i = 0; i < count; i++)
			{
				var user = new UserEntity
				           	{
				           		Id = i + 1,
				           		Age = r.Next(1, 100),
				           		Department = d.GetRandom(),
				           		FirstName = fn.GetRandom(),
				           		LastName = ln.GetRandom(),
				           		Title = t.GetRandom()
				           	};
				users.Add(user);
			}

			return users;
		}
	}
}
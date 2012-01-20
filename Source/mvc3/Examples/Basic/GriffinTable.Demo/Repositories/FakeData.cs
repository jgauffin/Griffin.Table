using System;
using System.Collections.Generic;
using System.Reflection;

namespace GriffinTable.Demo.Repositories
{
	public class FakeData
	{
		private Random _random = new Random();
		protected List<string> _names = new List<string>();

		public FakeData()
		{
			foreach (var field in this.GetType().GetFields(BindingFlags.Static|BindingFlags.Public))
			{
				_names.Add(field.GetValue(this).ToString());
			}
		}

		public virtual string GetRandom()
		{
			return _names[_random.Next(_names.Count)];
		}
    
	}
}
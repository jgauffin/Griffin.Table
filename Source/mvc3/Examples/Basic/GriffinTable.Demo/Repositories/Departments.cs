namespace GriffinTable.Demo.Repositories
{
	public class Departments : FakeData
	{
		public Departments()
		{
			_names.AddRange(new[]{"Sales", "Support", "Customer Service", "Desk", "Human resources", "Leets"});
		}
	}
}
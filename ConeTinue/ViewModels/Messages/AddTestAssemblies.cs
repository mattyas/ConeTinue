namespace ConeTinue.ViewModels.Messages
{
	public class AddTestAssemblies
	{
		public string[] Paths { get; private set; }

		public AddTestAssemblies(params string[] paths)
		{
			Paths = paths;
		}
	}
}
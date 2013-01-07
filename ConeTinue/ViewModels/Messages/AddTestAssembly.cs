namespace ConeTinue.ViewModels.Messages
{
	public class AddTestAssembly
	{
		public string Path { get; private set; }

		public AddTestAssembly(string path)
		{
			Path = path;
		}
	}
}
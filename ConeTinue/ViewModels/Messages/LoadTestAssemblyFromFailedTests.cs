namespace ConeTinue.ViewModels.Messages
{
	public class LoadTestAssemblyFromFailedTests
	{
		public string Path { get; private set; }

		public LoadTestAssemblyFromFailedTests(string path)
		{
			Path = path;
		}
	}
}
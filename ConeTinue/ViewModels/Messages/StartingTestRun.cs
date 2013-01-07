namespace ConeTinue.ViewModels.Messages
{
	public class StartingTestRun
	{
		public TestRunType TestRunType { get; private set; }

		public StartingTestRun(TestRunType testRunType)
		{
			TestRunType = testRunType;
		}
	}
}
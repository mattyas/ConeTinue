namespace ConeTinue.ViewModels.Messages
{
	public class TestRunDone
	{
		public TestRunType TestRunType { get; private set; }

		public TestRunDone(TestRunType testRunType)
		{
			TestRunType = testRunType;
		}
	}
}
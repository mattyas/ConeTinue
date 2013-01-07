namespace ConeTinue.ViewModels.Messages
{
	public class UpdateTestRunStatus
	{
		public TestRunStatus TestRunStatus { get; private set; }

		public UpdateTestRunStatus(TestRunStatus testRunStatus)
		{
			TestRunStatus = testRunStatus;
		}
	}
}
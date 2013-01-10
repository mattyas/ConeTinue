using ConeTinue.Domain.CrossDomain;

namespace ConeTinue.ViewModels.Messages
{
	public class ReportFailures
	{
		public TestFailure[] Failures { get; set; }

		public ReportFailures(params TestFailure[] failures)
		{
			Failures = failures;
		}
	}
}
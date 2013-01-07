using ConeTinue.Domain;

namespace ConeTinue.ViewModels.Messages
{
	public class NewTestsLoaded
	{
		public TestItemHolder Tests { get; private set; }

		public NewTestsLoaded(TestItemHolder tests)
		{
			Tests = tests;
		}
	}
}
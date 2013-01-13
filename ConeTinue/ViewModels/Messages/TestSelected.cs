using ConeTinue.Domain;

namespace ConeTinue.ViewModels.Messages
{
	public class TestSelected
	{
		public TestItem Test { get; private set; }

		public TestSelected(TestItem test)
		{
			Test = test;
		}
	}
}
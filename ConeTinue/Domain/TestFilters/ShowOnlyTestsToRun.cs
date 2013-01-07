namespace ConeTinue.Domain.TestFilters
{
	public class ShowOnlyTestsToRun : FilterTests
	{
		public ShowOnlyTestsToRun() : base(FilterType.ShowOnlyTestsToRun)
		{
		}

		public override void Apply(TestItemHolder testItemHolder)
		{
			ApplyFilter(testItemHolder, test => test.IsVisible = test.EffectiveShouldRun);

		}

		public override string ToString()
		{
			return "Show only tests to run";
		}
	}
}
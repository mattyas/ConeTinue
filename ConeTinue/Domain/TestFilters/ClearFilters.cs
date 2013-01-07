namespace ConeTinue.Domain.TestFilters
{
	public class ClearFilters : FilterTests
	{
		public ClearFilters() : base(FilterType.Clear) { }
		public override void Apply(TestItemHolder testItemHolder)
		{
			
		}

		public override string ToString()
		{
			return "Reset filters";
		}
	}
}
using System;

namespace ConeTinue.Domain.TestFilters
{
	public enum FilterType
	{
		Clear,
		FilterOnText,
		ShowOnlyTestsToRun,
		ExludeCategories,
		IncludeCategories
	}

	[Serializable]
	public abstract class FilterTests
	{
		protected FilterTests(FilterType type)
		{
			Type = type;
		}
		public FilterType Type { get; set; }
		public virtual bool CanRemove { get { return true; } }

		public abstract void Apply(TestItemHolder testItemHolder);

		public abstract override string ToString();

		protected void ApplyFilter(TestItemHolder testItemHolder, Action<TestItem> filter)
		{
			var itemsToFilter = testItemHolder.GetAllVisibleItems();
			var testsToFilter = testItemHolder.GetAllVisibleTests();
			foreach (var testItem in itemsToFilter)
				testItem.IsVisible = false;
			foreach (var testItem in testsToFilter)
				filter(testItem);
		}
	}
}
using System.Text.RegularExpressions;

namespace ConeTinue.Domain.TestFilters
{
    public class FilterOnTextWithReplaceUnderscore : FilterTests
    {
        private readonly FilterOnText filterOnText;

		public FilterOnTextWithReplaceUnderscore(FilterOnText filterOnText) : base(FilterType.FilterOnText)
		{
			this.filterOnText = filterOnText;
			if (!string.IsNullOrEmpty(filterOnText.Filter))
				filterOnText.Filter = filterOnText.Filter.Replace("_", " ");
		}

        public string Filter => filterOnText.Filter;

        public override void Apply(TestItemHolder testItemHolder) => filterOnText.Apply(testItemHolder);

        public override string ToString() => filterOnText.ToString();
    }

    public class FilterOnText : FilterTests
	{
		private readonly bool showWhenMatching;

		public FilterOnText(string filter, bool showWhenMatching) : base(FilterType.FilterOnText)
		{
			this.showWhenMatching = showWhenMatching;
			Filter = filter;
		}

		public string Filter { get; set; }

		public override void Apply(TestItemHolder testItemHolder)
		{
			try
			{
				var regex = new Regex(Filter, RegexOptions.IgnoreCase);
				ApplyFilter(testItemHolder, test => test.IsVisible = regex.Match(test.TestKey.FullName).Success == showWhenMatching);
			}
			catch
			{
			}
		}

		public override string ToString()
		{
			try
			{
				new Regex(Filter, RegexOptions.IgnoreCase);
				return string.Format("Tests {0}matching regex: {1}", showWhenMatching ? "":"not ", Filter);
			}
			catch
			{
				return "<Invalid regex>";
			}
			
		}
	}
}
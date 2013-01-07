using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConeTinue.Domain.TestFilters
{
	public class ExcludeCategoriesFilter : FilterTests
	{
		public ExcludeCategoriesFilter(IEnumerable<TestCategory> ignoreCategories)
			: base(FilterType.ExludeCategories)
		{
			this.ignoreCategories = ignoreCategories.ToList();
		}

		private readonly List<TestCategory> ignoreCategories;
		public override void Apply(TestItemHolder testItemHolder)
		{
			ApplyFilter(testItemHolder, test => test.IsVisible = ignoreCategories.All(category => !category.Matches(test)));
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("Exclude categories: ");
			sb.Append(string.Join(" and ", ignoreCategories.Select(x => string.Format("\"{0}\"", x)).ToArray()));
			return sb.ToString();

		}
		public override bool CanRemove
		{
			get
			{
				return ignoreCategories.Count == 0;
			}
		}
	}
}
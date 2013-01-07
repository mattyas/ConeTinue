using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConeTinue.Domain.TestFilters
{
	public class IncludeCategoriesFilter : FilterTests
	{
		public IncludeCategoriesFilter(IEnumerable<TestCategory> includeCategories)
			: base(FilterType.IncludeCategories)
		{
			this.includeCategories = includeCategories.ToList();
		}

		private readonly List<TestCategory> includeCategories;
		public override void Apply(TestItemHolder testItemHolder)
		{
			ApplyFilter(testItemHolder, test => test.IsVisible = includeCategories.Any(category => category.Matches(test)));
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("Include categories: ");
			sb.Append(string.Join(" and ", includeCategories.Select(x => string.Format("\"{0}\"", x)).ToArray()));
			return sb.ToString();

		}
		public override bool CanRemove
		{
			get
			{
				return includeCategories.Count == 0;
			}
		}
	}
}
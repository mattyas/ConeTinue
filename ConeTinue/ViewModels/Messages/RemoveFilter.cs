using ConeTinue.Domain.TestFilters;

namespace ConeTinue.ViewModels.Messages
{
	public class RemoveFilter
	{
		public FilterTests Filter { get; private set; }

		public RemoveFilter(FilterTests filter)
		{
			Filter = filter;
		}
	}
}
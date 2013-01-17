using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ConeTinue.Domain.TestFilters;

namespace ConeTinue.Domain
{
	public class TestItemHolder : TestItem
	{
		public TestItemHolder()
		{
			TestKey = new TestKey {FullName = string.Empty, TestAssembly = new NoTestAssembly()};
		}

		public override string Name
		{
			get
			{
				return string.Format("{0} Tests {1} Visible", allTestsByFullname.Count, allTestsByFullname.Count(x => x.Value.IsVisible));
			}
		}

		public override bool IsVisible
		{
			get
			{
				return true;
			}
		}
		
		public IEnumerable<TestCategory> AllCategories
		{
			get
			{
				if (allTestsByCategory.Count == 0)
					return new[]{TestCategory.None(0)};
				return
					allTestsByCategory.Select(
						x => x.Key == string.Empty ? TestCategory.None(x.Value.Count) : new TestCategory(x.Key, x.Value.Count))
					                  .ToList()
					                  .OrderBy(x => x.ToString());
			}
		}

		private readonly ConcurrentDictionary<TestKey, TestItem> allTestsByFullname = new ConcurrentDictionary<TestKey, TestItem>();
		private readonly ConcurrentDictionary<string, List<TestItem>> allTestsByCategory = new ConcurrentDictionary<string, List<TestItem>>();
		private readonly ConcurrentDictionary<TestKey, TestItem> allItems = new ConcurrentDictionary<TestKey, TestItem>();
		private readonly TestRun currentTestRun = new TestRun();
		public bool TryGetTest(TestKey testKey, out TestItem testItem)
		{
			return allTestsByFullname.TryGetValue(testKey, out testItem);
		}


		public TestRun CurrentTestRun
		{
			get { return currentTestRun; }
		}

		public ObservableCollection<FilterTests> Filters
		{
			get { return filters; }
		}

		private readonly ObservableCollection<FilterTests> filters = new ObservableCollection<FilterTests>();
		private void ApplyFilters()
		{
			foreach (var testItem in allItems.Values)
			{
				testItem.IsVisible = true;
			}
			foreach (var filter in filters)
			{
				filter.Apply(this);
			}
		}

		public void Filter(FilterTests filter)
		{
			switch (filter.Type)
			{
				case FilterType.Clear:
					foreach (var filterToRemove in filters.Where(x => x.CanRemove).ToList())
						filters.Remove(filterToRemove);
					break;
				case FilterType.ShowOnlyTestsToRun:
					foreach (var filterToRemove in filters.Where(x => x.Type == FilterType.ShowOnlyTestsToRun).ToList())
						filters.Remove(filterToRemove);
					filters.Add(filter);
					break;
				case FilterType.IncludeCategories:
				case FilterType.ExludeCategories:
					foreach (var filterToRemove in filters.Where(x => x.Type == FilterType.ExludeCategories || x.Type == FilterType.IncludeCategories).ToList())
						filters.Remove(filterToRemove);
					if (!filter.CanRemove)
						filters.Add(filter);
					break;
				default:
					filters.Add(filter);
					break;
			}			
			ApplyFilters();
		}

		public IEnumerable<TestItem> GetAllVisibleItems()
		{
			return allItems.Values.Where(x => x.IsVisible).ToList();
		}

		public IEnumerable<TestItem> GetAllVisibleTestsFromAssembly(TestAssembly testAssembly)
		{
			return allTestsByFullname.Values.Where(x => x.IsVisible).Where(x => x.TestKey.TestAssembly == testAssembly).ToList();
		}

		public IEnumerable<TestItem> GetAllVisibleTests()
		{
			return allTestsByFullname.Values.Where(x => x.IsVisible).ToList();
		}

		public IEnumerable<TestKey> GetTestToRun(TestAssembly testAssembly)
		{
			return GetAllVisibleTestsFromAssembly(testAssembly)
				.Where(x => x.Status == TestStatus.InQueue)
				.Select(test => test.TestKey)
				.ToList();
		}

		public void MarkTestToRun()
		{
			currentTestRun.Reset();
			foreach (var item in allItems.Values)
			{
				item.IsInLatestRun = false;
				if (item.Status == TestStatus.InQueue)
					item.Status = TestStatus.Unknown;
			}

			foreach (var test in GetAllVisibleTests().Where(test => test.EffectiveShouldRun))
			{
				test.Status = TestStatus.InQueue;
				test.IsInLatestRun = true;
				currentTestRun.AddTest(test);
			}
		}

		public void RemoveFilter(FilterTests filter)
		{
			if (filter.Type == FilterType.ExludeCategories || filter.Type == FilterType.IncludeCategories)
				return;
			filters.Remove(filter);
			ApplyFilters();
		}

		public void ReApplySettings(TestItemHolder oldTestItemHolder)
		{
			foreach (var testItem in oldTestItemHolder.allItems.Values)
			{
				TestItem item;
				if (!allItems.TryGetValue(testItem.TestKey, out item))
					continue;
				item.IsExpanded = testItem.IsExpanded;
				item.IsInLatestRun = testItem.IsInLatestRun;
				item.IsSelected = testItem.IsSelected;
				item.Status = testItem.Status;
				item.RunTime = testItem.RunTime;
				item.ShouldRun = testItem.ShouldRun;
			}
			foreach (var filter in oldTestItemHolder.Filters)
				Filter(filter);
		}

		public void RegisterTest(TestItem test)
		{
			TestRun.AddTest(test);
			allTestsByFullname[test.TestKey] = test;
			allItems[test.TestKey] = test;
			if (test.Categories.Count == 0)
			{
				if (!allTestsByCategory.ContainsKey(""))
					allTestsByCategory[""] = new List<TestItem>();
				allTestsByCategory[""].Add(test);

			}
			foreach (var category in test.Categories)
			{
				if (!allTestsByCategory.ContainsKey(category))
					allTestsByCategory[category] = new List<TestItem>();
				allTestsByCategory[category].Add(test);
			}

		}

		public void RegisterPath(TestItem path)
		{
			allItems[path.TestKey] = path;
		}

		public void SortAllTests()
		{
			foreach (var path in allItems.Values.Where(x => !x.IsTest))
				path.SortTests();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using ConeTinue.Domain.CrossDomain;

namespace ConeTinue.Domain
{
	public class TestItemHolderFactory
	{
		public static TestItemHolder CreateFrom(IList<TestInfoWithAssembly> tests, bool shouldExpandAll)
		{
			var testItemHolder = new TestItemHolder { IsExpanded = shouldExpandAll };
			foreach (var testItem in tests.Select(x => new TestItem
				{
					TestKey = new TestKey { FullName = x.FullName, TestAssembly = x.TestAssembly },
					Categories = x.Categories,
					IsTest = true,
					IsVisible = true,
					Name = x.Name,
				}))
			{
				Transform(testItem, testItemHolder, shouldExpandAll);
			}
			testItemHolder.SortAllTests();
			return testItemHolder;
		}


		private static IEnumerable<string> GetTestPaths(TestItem test)
		{
			var length = test.TestKey.FullName.Length - test.Name.Length;
			return test.TestKey.FullName.Substring(0, length).Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
		}

		private static void Transform(TestItem test, TestItemHolder destination, bool isExpanded)
		{
			TestItem dummy;
			if (destination.TryGetTest(test.TestKey, out dummy))
				return;
			test.IsExpanded = isExpanded;
			TestItem path = destination;
			destination.RegisterTest(test);

			foreach (var part in GetTestPaths(test))
			{
				path = path.SubPath(part);
				path.TestRun.AddTest(test);
				destination.RegisterPath(path);
			}
			test.Parent = path;
			path.AddTest(test);
		}
	}
}
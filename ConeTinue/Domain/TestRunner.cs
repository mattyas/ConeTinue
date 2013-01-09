using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Caliburn.Micro;
using ConeTinue.Domain.CrossDomain;
using ConeTinue.ViewModels;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Domain
{
	public class TestRunner : IDisposable, IHandle<RunTests>, IHandle<ClearTestSession>, IHandle<AddTestAssembly>, IHandle<ReloadTestSession>, IHandle<AbortTestRun>, IHandle<LoadTestAssemblyFromFailedTests>, IHandle<TestRunDone>, IHandle<NewTestsLoaded>
	{
		private readonly IEventAggregator eventAggregator;
		private readonly SettingsStrategy settings;

		public TestRunner(IEventAggregator eventAggregator, Guid myId, SettingsStrategy settings)
		{
			this.eventAggregator = eventAggregator;
			this.settings = settings;
			testRunner = new CrossDomainTestRunner(eventAggregator, myId, settings);
			eventAggregator.Subscribe(this);
		}

		private readonly List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();
		private readonly ObservableCollection<TestAssembly> testAssemblies = new ObservableCollection<TestAssembly>();

		public ObservableCollection<TestAssembly> TestAssemblies
		{
			get
			{
				return testAssemblies;
			}
		}
		
		private readonly CrossDomainTestRunner testRunner;
		private volatile bool isReloading;
		private void ReloadAssemblies()
		{
			Task.Factory.StartNew(() =>
			{
				if (isReloading)
					return;
				isReloading = true;
				eventAggregator.Publish(new StatusMessage("Loading assembly"));
				eventAggregator.Publish(new StartingTestRun(TestRunType.FindTests));
				testRunner.FindTests(TestAssemblies);
				isReloading = false;
				eventAggregator.Publish(new TestRunDone(TestRunType.FindTests));
			});

		}

		public void Dispose()
		{
			testRunner.Dispose();
		}

		public void Handle(RunTests message)
		{
			Task.Factory.StartNew(() =>
			{
				eventAggregator.Publish(new StartingTestRun(TestRunType.RunTests));
				bool success = testRunner.RunTests();
				eventAggregator.Publish(new TestRunDone(success ? TestRunType.RunTests : TestRunType.Aborted));
			});
		}
		
		public void Handle(AbortTestRun message)
		{
			Task.Factory.StartNew(() => testRunner.AbortTests());
		}
		
		public void Handle(ClearTestSession message)
		{
			eventAggregator.Publish(ModifyTests.UnCheckAll);
			TestAssemblies.Clear();
			foreach (var fileSystemWatcher in fileSystemWatchers)
			{
				fileSystemWatcher.Dispose();
			}
			fileSystemWatchers.Clear();
			ReloadAssemblies();
		}

		public void Handle(ReloadTestSession message)
		{
			ReloadAssemblies();
		}

		public void Handle(AddTestAssembly message)
		{
			var newTestAssembly = new TestAssembly(message.Path);
			if (testAssemblies.Any(x => x == newTestAssembly))
				return;
			settings.AddRecent(message.Path);
			testAssemblies.Add(newTestAssembly);
			for (int index = 0; index < testAssemblies.Count; index++)
			{
				var testAssembly = testAssemblies[index];
				testAssembly.Number = index + 1;
			}
			ReloadAssemblies();
			var watcher = new FileSystemWatcher(newTestAssembly.AssemblyDirectory, newTestAssembly.AssemblyFileName);
			watcher.Changed += (sender, args) => { if (settings.ReloadTestAssembliesWhenChanged) ReloadAssemblies(); };
			fileSystemWatchers.Add(watcher);
			watcher.EnableRaisingEvents = true;
		}

		public void Handle(LoadTestAssemblyFromFailedTests message)
		{
			try
			{
				testsToSelectOnNextReload = FindFailedTests.FindFailures(message.Path);
				eventAggregator.Publish(new ClearTestSession());
				eventAggregator.Publish(new TestFilters.ClearFilters());
				foreach (var path in testsToSelectOnNextReload.Select(x => x.TestKey.TestAssembly.AssemblyPath).Distinct())
				{
					Handle(new AddTestAssembly(path));
				}
			}
			catch (Exception ex)
			{
				eventAggregator.Publish(new ErrorMessage(ex.ToString()));
			}

		}

		private TestFailure[] testsToSelectOnNextReload;

			
			public void Handle(TestRunDone message)
			{
				if (testsToSelectOnNextReload == null)
					return;
				if (message.TestRunType != TestRunType.FindTests)
					return;
				foreach (var testFailure in testsToSelectOnNextReload)
				{
					TestItem item;
					if (tests.TryGetTest(testFailure.TestKey, out item))
						item.Status = TestStatus.Failed;
					eventAggregator.Publish(testFailure);	
				}
				eventAggregator.Publish(ModifyTests.CheckFailed);
				eventAggregator.Publish(new TestFilters.ShowOnlyTestsToRun());
				testsToSelectOnNextReload = null;
			}
		
		public class FindFailedTests
		{
			public static TestFailure[] FindFailures(string path)
			{
				var xdoc = XElement.Load(path);
				return xdoc.Elements("test-case")
				.Where(x => x.Attribute("executed").Value == "True")
				.Where(x => x.Attribute("success").Value == "False")
				.Select(x => new TestFailure
				{
					TestKey = new TestKey() { FullName = x.Attribute("context").Value + "." + x.Attribute("name").Value, TestAssembly = new TestAssembly(x.Attribute("assembly").Value)},
					File = x.Element("failure").Attribute("file").Value,
					Line = ToInt(x.Element("failure").Attribute("line").Value),
					Column = ToInt(x.Element("failure").Attribute("column").Value),
					Message = x.Element("failure").Element("message").Value,
					Context = x.Attribute("context").Value,
					TestName = x.Attribute("name").Value
				})
				.ToArray();
			}

			public static int ToInt(string value)
			{
				int tmp;
				if (int.TryParse(value, out tmp))
					return tmp;
				return 0;
			}
		}

		private TestItemHolder tests = new TestItemHolder();
		public void Handle(NewTestsLoaded message)
		{
			tests = message.Tests;
		}
	}
}
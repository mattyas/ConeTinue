using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ConeTinue.Domain.CrossDomain;
using ConeTinue.ViewModels;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Domain
{
	public class TestRunner : IDisposable, IHandle<RunTests>, IHandle<ClearTestSession>, IHandle<AddTestAssemblies>, IHandle<ReloadTestSession>, IHandle<AbortTestRun>
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
				try
				{
					eventAggregator.Publish(new StatusMessage("Loading assembly"));
					eventAggregator.Publish(new StartingTestRun(TestRunType.FindTests));
					testRunner.FindTests(TestAssemblies);
				}
				finally
				{
					isReloading = false;	
				}
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
				bool success = testRunner.RunTests(message.Fast);
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

		public void Handle(AddTestAssemblies message)
		{
			foreach (var path in message.Paths)
			{
				var newTestAssembly = new TestAssembly(path);
				if (testAssemblies.Any(x => x == newTestAssembly))
					return;
				settings.AddRecent(path);
				testAssemblies.Add(newTestAssembly);
				for (int index = 0; index < testAssemblies.Count; index++)
				{
					var testAssembly = testAssemblies[index];
					testAssembly.Number = index + 1;
				}
				var watcher = new FileSystemWatcher(newTestAssembly.AssemblyDirectory, newTestAssembly.AssemblyFileName);
				watcher.Changed += (sender, args) => { if (settings.ReloadTestAssembliesWhenChanged) ReloadAssemblies(); };
				fileSystemWatchers.Add(watcher);
				watcher.EnableRaisingEvents = true;
			}
			ReloadAssemblies();
		}
	}
}
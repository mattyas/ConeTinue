using System;
using System.Collections.Generic;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.Domain.TestFilters;
using ConeTinue.ViewModels.Messages;
using Microsoft.Win32;
using ConeTinue.ViewModels.RibbonBuilders;

namespace ConeTinue.ViewModels
{
	public class RibbonViewModel : Screen, IHandle<StartingTestRun>, IHandle<TestRunDone>
	{
		private readonly IEventAggregator eventAggregator;
		public BindableCollection<RibbonTabViewModel> Tabs { get; private set; }
		public BindableCollection<IRibbonControlViewModel> QuickItems { get; private set; }

		private readonly List<ICanExecute> availableWhenNotRunningTests = new List<ICanExecute>();
		private readonly List<ICanExecute> availableWhenRunningTests = new List<ICanExecute>();

		private ICanExecute AvailableWhenNotRunningTests(ICanExecute button)
		{
			availableWhenNotRunningTests.Add(button);
			return button;
		}
		private RibbonButtonViewModel AvailableWhenRunningTests(RibbonButtonViewModel button)
		{
			availableWhenRunningTests.Add(button);
			return button;
		}

		private int runCount = 0;
		public void Handle(StartingTestRun message)
		{
			runCount++;
			foreach (var buttonViewModel in availableWhenNotRunningTests)
				buttonViewModel.CanExecute = false;
			foreach (var buttonViewModel in availableWhenRunningTests)
				buttonViewModel.CanExecute = message.TestRunType == TestRunType.RunTests;
		}

		public void Handle(TestRunDone message)
		{
			runCount--;
			if (runCount != 0)
				return;

			foreach (var buttonViewModel in availableWhenNotRunningTests)
				buttonViewModel.CanExecute = true;
			foreach (var buttonViewModel in availableWhenRunningTests)
				buttonViewModel.CanExecute = false;
		}

		public RibbonViewModel(IEventAggregator eventAggregator, TestSessionViewModel testSessionViewModel, SettingsStrategy settingsStrategy)
		{
			this.eventAggregator = eventAggregator;

			QuickItems = new BindableCollection<IRibbonControlViewModel>
				{
					AvailableWhenNotRunningTests(new RibbonButtonViewModel("Run", () => eventAggregator.Publish(new RunTests()),
					                                                       Icon.Run, "R", showToolTip: true)),
					AvailableWhenRunningTests(new RibbonButtonViewModel("Abort", () => eventAggregator.Publish(new AbortTestRun()),
					                                                    Icon.AbortTestRun, "A", canExecute: false, showToolTip: true)),
					AvailableWhenNotRunningTests(new RibbonSplitButtonViewModel("Add test assembly", OpenTestAssembly,
					                                                            Icon.AddTestAssembly, "H","O", new RecentHistoryProvider(settingsStrategy, eventAggregator), showToolTip: true)),
					AvailableWhenNotRunningTests(new RibbonButtonViewModel("Select and show only failed", SelectAndShowOnlyFailed, Icon.Error, "T", showToolTip: true)),
					AvailableWhenNotRunningTests(new RibbonButtonViewModel("Clear filter", () => eventAggregator.Publish(new ClearFilters()), Icon.ClearFilters,"X", showToolTip: true))

				};
			Tabs = new BindableCollection<RibbonTabViewModel> {
				new RibbonTabBuilder("Test session", "T").WithRibbonGroups(
					new RibbonGroupBuilder("Run")
					.WithItems(
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Run", () => eventAggregator.Publish(new RunTests()),Icon.Run, "R")),
						AvailableWhenRunningTests(new RibbonButtonViewModel("Abort", () => eventAggregator.Publish(new AbortTestRun()),Icon.AbortTestRun, "A", canExecute: false)),
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Run Fast", () => eventAggregator.Publish(new RunTests(true)),Icon.RunFast, "F"))
						).Build(),
					new RibbonGroupBuilder("Test session")
					.WithItems(
						AvailableWhenNotRunningTests(new RibbonSplitButtonViewModel("Add test assembly", OpenTestAssembly, Icon.AddTestAssembly, "H", "O", new RecentHistoryProvider(settingsStrategy, eventAggregator))),
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Clear test session", () => eventAggregator.Publish(new ClearTestSession()), Icon.ClearTestSession, "C")),
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Reload test session", () => eventAggregator.Publish(new ReloadTestSession()), Icon.ReloadTestSession, "L")),
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Load test session from failed tests", LoadFromFailed, Icon.LoadTestSessionFromFailedTests, "F"))
						).Build(),
					new RibbonGroupBuilder("Current test session")
					.WithItems(testSessionViewModel).Build()
						).Build(),
				new RibbonTabBuilder("Select/Expand", "E").WithRibbonGroups(
					new RibbonGroupBuilder("Select").WithItems(
						new RibbonButtonViewModel("Failed", () => eventAggregator.Publish(ModifyTests.CheckFailed), Icon.CheckFailed,"F"),
						new RibbonButtonViewModel("All", () => eventAggregator.Publish(ModifyTests.CheckAll), Icon.CheckAll,"A"),
						new RibbonButtonViewModel("None", () => eventAggregator.Publish(ModifyTests.UnCheckAll), Icon.UnCheckAll,"N")
						).Build(),
					new RibbonGroupBuilder("Expand").WithItems(
						new RibbonButtonViewModel("All visible", () => eventAggregator.Publish(ModifyTests.ExpandAll), Icon.AllVisible,"V"),
						new RibbonButtonViewModel("Only tests to run", () => eventAggregator.Publish(ModifyTests.ExpandOnlyTestsToRun), Icon.ExpandOnlyTestsToRun,"R")
						).Build(),
					new RibbonGroupBuilder("Collapse").WithItems(new RibbonButtonViewModel("All", () => eventAggregator.Publish(ModifyTests.CollapseAll), Icon.CollapseAll,"X")).Build()
					).Build(),
				new RibbonTabBuilder("Filters","F").WithRibbonGroups(
					new RibbonGroupBuilder("Filters").WithItems(
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Clear filter", () => eventAggregator.Publish(new ClearFilters()), Icon.ClearFilters,"X")),
						AvailableWhenNotRunningTests(new RibbonButtonViewModel("Show only tests to run", () => eventAggregator.Publish(new ShowOnlyTestsToRun()), Icon.ShowOnlyTestsToRun,"R"))
						).Build(),
					new RibbonGroupBuilder("Current filters").WithItems(new FilterRibbonViewModel(eventAggregator)).Build()
					).Build(),
				new RibbonTabBuilder("Categories","C").WithRibbonGroups(
					new RibbonGroupBuilder("Select categories").WithItemsProvider(new CategoryProvider(eventAggregator, settingsStrategy)).Build(),
					new RibbonGroupBuilder("Settings").WithItems(
						new RibbonCheckboxViewModel("Exclude is more importaint then include", settingsStrategy, () => settingsStrategy.ExcludeCategories, "E", Icon.Category)
						).Build()
					).Build(),
				new RibbonTabBuilder("Settings","S").WithRibbonGroups(
					new RibbonGroupBuilder("Settings").WithItems(
						new RibbonCheckboxViewModel("Reload assembly when file is changed", settingsStrategy, () => settingsStrategy.ReloadTestAssembliesWhenChanged, "A", Icon.ReloadTestSession ),
						new RibbonCheckboxViewModel("Run tests on reload", settingsStrategy, () => settingsStrategy.RunTestsOnReload, "R", Icon.Run ),
						new RibbonCheckboxViewModel("All tests expanded when loaded", settingsStrategy, () => settingsStrategy.DefaultAllExpanded, "E", Icon.AllVisible),
						new RibbonCheckboxViewModel("Output debug and error text", settingsStrategy, () => settingsStrategy.OutputDebugAndError, "O", Icon.Error),
						new RibbonCheckboxViewModel("Pin ConeTinue output in Visual Studio", settingsStrategy, () => settingsStrategy.PinOutputInVisualStudio, "P", Icon.VisualStudio)
				
						).Build()
					).Build()
				};
			eventAggregator.Subscribe(this);
			base.DisplayName = "ConeTinue testing";
		}

		private void OpenTestAssembly()
		{
			var openFile = new OpenFileDialog();
			if (openFile.ShowDialog() == false)
			{
				return;
			}
			eventAggregator.Publish(new AddTestAssemblies(openFile.FileName));
		}

		private void LoadFromFailed()
		{
			var openFile = new OpenFileDialog();
			if (openFile.ShowDialog() == false)
			{
				return;
			}
			eventAggregator.Publish(new LoadTestAssemblyFromFailedTests(openFile.FileName));
		}

		private void SelectAndShowOnlyFailed()
		{
			eventAggregator.Publish(ModifyTests.CheckFailed);
			eventAggregator.Publish(new ShowOnlyTestsToRun());
			eventAggregator.Publish(ModifyTests.ExpandOnlyTestsToRun);
		}

		public void Exit()
		{
			eventAggregator.Publish(new Exit());
		}
	}
}

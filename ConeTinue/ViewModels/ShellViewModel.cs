using System;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class ShellViewModel : Screen, IHandle<StatusMessage>, IHandle<ErrorMessage>, IHandle<Exit>, IHandle<UpdateTestRunStatus>
	{
		private readonly Guid myId = Guid.NewGuid();
		private readonly IEventAggregator eventAggregator;
		private readonly TestRunner testRunner;

		public ShellViewModel()
		{
			base.DisplayName = "ConeTinue testing..";
			eventAggregator = new EventAggregator();
			eventAggregator.Subscribe(this);

			var settingsStrategy = new SettingsStrategy();
			testRunner = new TestRunner(eventAggregator, myId, settingsStrategy);
			Ribbon = new RibbonViewModel(eventAggregator, new TestSessionViewModel(testRunner.TestAssemblies), settingsStrategy);
			TestsViewModel = new TestsViewModel(eventAggregator, settingsStrategy);
			TestFailuresViewModel = new TestFailuresViewModel(eventAggregator, settingsStrategy);
			FilterViewModel = new FilterViewModel(eventAggregator);
			CurrentTestRunViewModel = new CurrentTestRunViewModel(eventAggregator);
			eventAggregator.Publish(new NewTestsLoaded(new TestItemHolder()));
		}

		public CurrentTestRunViewModel CurrentTestRunViewModel { get; private set; }
		public RibbonViewModel Ribbon { get; private set; }
		public TestsViewModel TestsViewModel { get; private set; }
		public TestFailuresViewModel TestFailuresViewModel { get; private set; }
		public FilterViewModel FilterViewModel { get; private set; }
		private string status = "Load test assemblies and start testing!";
		public string Status
		{
			get { return status; }
			private set
			{
				if (value == status) return;
				status = value;
				NotifyOfPropertyChange(() => Status);
			}
		}

		public Icon Icon 
		{
			get { return icon; }
			set
			{
				if (value == icon) return;
				icon = value;
				NotifyOfPropertyChange(() => Icon);
			}
		}

		private bool canLoadFile = true;
		private bool canRunTests;


		public bool CanLoadFile
		{
			get { return canLoadFile; }
			set
			{
				if (value.Equals(canLoadFile)) return;
				canLoadFile = value;
				NotifyOfPropertyChange(() => CanLoadFile);
			}
		}


		private bool canClearTestSession;
		private Icon icon = Icon.Info;
		private TestRunStatus testRunStatus = TestRunStatus.NotStarted;


		public bool CanRunTests
		{
			get { return canRunTests; }
			set
			{
				if (value.Equals(canRunTests)) return;
				canRunTests = value;
				NotifyOfPropertyChange(() => CanRunTests);
			}
		}

		protected override void OnDeactivate(bool close)
		{
			if (close)
				testRunner.Dispose();
			base.OnDeactivate(close);
			
		}

		public void Handle(StatusMessage message)
		{
			Status = message.Status;
			Icon = Icon.Info;
		}


		public bool CanClearTestSession
		{
			get { return canClearTestSession; }
			set
			{
				if (value.Equals(canClearTestSession)) return;
				canClearTestSession = value;
				NotifyOfPropertyChange(() => CanClearTestSession);
			}
		}

		public void Handle(Exit message)
		{
			TryClose();
		}

		public void Handle(ErrorMessage message)
		{
			Status = message.Error;
			Icon = Icon.Error;
		}

		public void Handle(UpdateTestRunStatus message)
		{
			TestRunStatus = message.TestRunStatus;
		}

		public TestRunStatus TestRunStatus
		{
			get { return testRunStatus; }
			set
			{
				if (value == testRunStatus) return;
				testRunStatus = value;
				NotifyOfPropertyChange(() => TestRunStatus);
			}
		}
	}
}

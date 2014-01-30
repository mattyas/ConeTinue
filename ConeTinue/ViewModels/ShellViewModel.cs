using System;
using System.IO;
using Caliburn.Micro;
using Cone;
using ConeTinue.Domain;
using ConeTinue.ViewModels.Messages;
using System.Linq;

namespace ConeTinue.ViewModels
{
	public class ShellViewModel : Screen, IHandle<StatusMessage>, IHandle<ErrorMessage>, IHandle<Exit>, IHandle<UpdateTestRunStatus>
	{
		private readonly Guid myId = Guid.NewGuid();
		private readonly IEventAggregator eventAggregator;
		private readonly TestRunner testRunner;
		private readonly LoadTestFailureFromFileHandler loadTestFailureFromFileHandler;

		public ShellViewModel()
		{
			base.DisplayName = "ConeTinue testing..";
			eventAggregator = new EventAggregator();
			eventAggregator.Subscribe(this);

			var settingsStrategy = new SettingsStrategy();
			testRunner = new TestRunner(eventAggregator, myId, settingsStrategy);
			loadTestFailureFromFileHandler = new LoadTestFailureFromFileHandler(eventAggregator);
			Ribbon = new RibbonViewModel(eventAggregator, new TestSessionViewModel(testRunner.TestAssemblies), settingsStrategy);
			TestsViewModel = new TestsViewModel(eventAggregator, settingsStrategy);
			TestFailuresViewModel = new TestFailuresViewModel(eventAggregator, settingsStrategy);
			FilterViewModel = new FilterViewModel(eventAggregator);
			CurrentTestRunViewModel = new CurrentTestRunViewModel(eventAggregator);
			eventAggregator.Publish(new NewTestsLoaded(new TestItemHolder()));
			foreach (var argument in Environment.GetCommandLineArgs().Skip(1))
			{
				TryHandleArgument(argument);
			}
		}

		void TryHandleArgument(string arg)
		{
			if (!arg.StartsWith("--"))
				return;
			var parts = arg.Remove(0,2).Split(new[]{'='},2);
			if (parts.Length != 2)
				return;
			if (parts[0].ToLower() == "assembly")
			{
				if (File.Exists(parts[1]))
					eventAggregator.Publish(new Messages.AddTestAssemblies(parts[1]));
			}
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
				var newValue = string.Format("{0} @{1}", value, DateTime.Now.ToString("HH:mm:ss"));
				if (newValue == status) return;
				status = newValue;
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
			eventAggregator.Publish(new TestRunDone(TestRunType.Aborted));
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

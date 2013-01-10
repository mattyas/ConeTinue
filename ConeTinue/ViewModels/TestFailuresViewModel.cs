using System.Collections.ObjectModel;
using System.Text;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.Domain.CrossDomain;
using ConeTinue.Domain.VisualStudio;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class TestFailuresViewModel : PropertyChangedBase, IHandle<ReportFailures>, IHandle<StartingTestRun>, IHandle<InfoMessage>
	{
		private readonly IEventAggregator eventAggregator;
		private readonly SettingsStrategy settings;
		private readonly StringBuilder info = new StringBuilder();
		private TestFailure selectedFailure;

		public TestFailuresViewModel(IEventAggregator eventAggregator, SettingsStrategy settings)
		{
			this.eventAggregator = eventAggregator;
			this.settings = settings;
			eventAggregator.Subscribe(this);
			Failures = new ObservableCollection<TestFailure>();
		}
		
		public ObservableCollection<TestFailure> Failures { get; private set; }
		public TestFailure SelectedFailure
		{
			get { return selectedFailure; }
			set
			{
				if (Equals(value, selectedFailure)) return;
				selectedFailure = value;
				NotifyOfPropertyChange(() => SelectedFailure);
			}
		}

		public void Handle(ReportFailures message)
		{
			foreach (var failure in message.Failures)
			{
				Failures.Add(failure);
				if (SelectedFailure == null)
					SelectedFailure = failure;
			}
		}

		public void Handle(StartingTestRun message)
		{
			Failures.Clear();
			ClearInfo();
		}
		public string Info
		{
			get { return info.ToString(); }
		}

		public void ClearInfo()
		{
			info.Clear();
			NotifyOfPropertyChange(() => Info);
		}

		public void Handle(InfoMessage message)
		{
			if (message.Info.Length == 0)
				return;
			info.Append(message.Info);
			NotifyOfPropertyChange(() => Info);
		}

		public void OpenInVisualStudio(TestFailure failure)
		{
			VisualStudioInstance instance;
			if (!VisualStudioInstance.TryGetVisualStudioInstance(failure, out instance, settings))
			{
				eventAggregator.Publish(new StatusMessage("Project not found in open instance of Visual Studio! (If you run VS as admin, you need to run ConeTinue as admin)"));
				return;
			}
			instance.SelectLine(failure);
			instance.ShowErrorInVisualStudioOutput(failure);
			instance.SetStatusBar("ConeTinue and fix the error");

		}



	}
}
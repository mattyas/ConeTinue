using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using ConeTinue.Domain;
using ConeTinue.Domain.CrossDomain;
using ConeTinue.Domain.VisualStudio;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.ViewModels
{
	public class TestFailuresViewModel : PropertyChangedBase, IHandle<ReportFailures>, IHandle<StartingTestRun>, IHandle<InfoMessage>, IHandle<BookmarkAllFailuresInVisualStudio>, IHandle<TestSelected>
	{
		private readonly IEventAggregator eventAggregator;
		private readonly SettingsStrategy settings;
		private readonly StringBuilder info = new StringBuilder();
		private TestFailure selectedFailure;
		private TestFailureStack selectedStackFrame;

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
				NotifyOfPropertyChange(() => StackFrames);
				SelectedStackFrame = StackFrames.LastOrDefault(x => x.HasSource);
			}
		}

		public TestFailureStack[] StackFrames
		{
			get
			{
				if (SelectedFailure == null)
				{
					return new TestFailureStack[0];
				}
				return SelectedFailure.StackTrace;
			}
		}

		public TestFailureStack SelectedStackFrame
		{
			get { return selectedStackFrame; }
			set
			{
				if (Equals(value, selectedStackFrame)) return;
				selectedStackFrame = value;
				NotifyOfPropertyChange(() => SelectedStackFrame);
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
			SelectedFailure = failure;
			VisualStudioInstance instance;
			if (!VisualStudioInstance.TryGetVisualStudioInstance(failure, out instance, settings))
			{
				eventAggregator.Publish(new StatusMessage("Project not found in open instance of Visual Studio! (If you run VS as admin, you need to run ConeTinue as admin)"));
				return;
			}
			instance.ShowErrorInVisualStudioOutput(failure);
			instance.SelectLine(failure);
			instance.SetStatusBar("ConeTinue and fix the error");
			instance.FocusVisualStudio();
		}

		public void Handle(BookmarkAllFailuresInVisualStudio message)
		{
			VisualStudioInstance instance;
			var allFailures = Failures.ToArray();
			var failure = Failures.FirstOrDefault(x => x.HasSource);
			if (failure == null)
				return;
			if (!VisualStudioInstance.TryGetVisualStudioInstance(failure, out instance, settings))
			{
				eventAggregator.Publish(new StatusMessage("Project not found in open instance of Visual Studio! (If you run VS as admin, you need to run ConeTinue as admin)"));
				return;
			}
			instance.BookmarkErrorsInVisualStudio(allFailures);
			instance.SetStatusBar("All failures are now bookmarks");
			instance.FocusVisualStudio();
		}

		public void Handle(TestSelected message)
		{
			if (message.Test == null)
				return;
			var failure = Failures.FirstOrDefault(x => x.TestKey == message.Test.TestKey);
			if (failure == null)
				return;
			SelectedFailure = failure;

		}
	}
}
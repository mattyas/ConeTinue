using System;
using System.Diagnostics;
using System.Linq;
using ConeTinue.Domain.CrossDomain;
using ConeTinue.External.DteFix;

namespace ConeTinue.Domain.VisualStudio
{
	public class VisualStudioInstance
	{
		private readonly SettingsStrategy settings;

		public static bool TryGetVisualStudioInstance(TestFailure failure, out VisualStudioInstance visualStudioInstance, SettingsStrategy settings)
		{
			visualStudioInstance = null;
			foreach (var visualStudioVersion in VisualStudioVersion.Versions)
			{
				try
				{
					var dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject(visualStudioVersion.Key);
					if (null == dte) 
						continue;
					var fileName = failure.StackTrace.First(x => x.HasSource).File;
					if (dte.Solution.FindProjectItem(fileName) != null)
					{
						visualStudioInstance = new VisualStudioInstance(visualStudioVersion, dte, settings);
						return true;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
			return false;
		}

		private VisualStudioInstance(VisualStudioVersion visualStudioVersion, EnvDTE.DTE dte, SettingsStrategy settings)
		{
			this.settings = settings;
			VisualStudioVersion = visualStudioVersion;
			Dte = dte;
		}

		public VisualStudioVersion VisualStudioVersion { get; private set; }
		private EnvDTE.DTE Dte { get; set; }
		private bool TryGetDte2(out EnvDTE80.DTE2 dte2)
		{
			dte2 = VisualStudioVersion.Is80Compatible ? (EnvDTE80.DTE2) Dte : null;
			return VisualStudioVersion.Is80Compatible;
		}

		public void SelectLine(TestFailure failure, TestFailureStack stackFrame)
		{
			try
			{
				var fileName = stackFrame.File;
				EnvDTE.ProjectItem ptojItem = Dte.Solution.FindProjectItem(fileName);
				if (null != ptojItem)
				{
					ptojItem.Open(EnvDteConstants.vsViewKindCode).Activate();
					var textSelection = (EnvDTE.TextSelection)Dte.ActiveDocument.Selection;
					textSelection.MoveToLineAndOffset(stackFrame.Line, stackFrame.Column, true);
					textSelection.SelectLine();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void SetStatusBar(string message)
		{
			try
			{
				EnvDTE80.DTE2 dte;
				if (TryGetDte2(out dte))
				{
					dte.StatusBar.Text = message;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void ShowErrorInVisualStudioOutput(TestFailure failure)
		{
			try
			{
				EnvDTE.Window win = Dte.Windows.Item(EnvDteConstants.vsWindowKindOutput);
				EnvDTE.OutputWindow ow = win.Object;
				EnvDTE.OutputWindowPane owPane = null;
				if (settings.PinOutputInVisualStudio)
					ow.Parent.AutoHides = false;
				ow.Parent.Activate();
				for (int i = 1; i <= ow.OutputWindowPanes.Count; i++)
				{
					if (ow.OutputWindowPanes.Item(i).Name == "ConeTinue")
						owPane = ow.OutputWindowPanes.Item(i);
				}
				if (owPane == null)
					owPane = ow.OutputWindowPanes.Add("ConeTinue");
				owPane.Activate();
				owPane.Clear();
				owPane.OutputString("Test: " + failure.TestName + Environment.NewLine);
				owPane.OutputString(failure.Message + Environment.NewLine + Environment.NewLine);
				bool first = true;
				foreach (var stackTrace in failure.StackTrace)
				{
					if (first)
						first = false;
					else
					{
						owPane.OutputString("called by ");
					}
					owPane.OutputString(stackTrace + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void BookmarkErrorsInVisualStudio(TestFailure[] failures)
		{
			try
			{
				foreach (var failure in failures)
				{
					var firstStackFrame = failure.StackTrace.Last(x => x.HasSource);
					var lastStackFrame = failure.StackTrace.First(x => x.HasSource);
					
					SetBookmark(firstStackFrame);
					if (lastStackFrame != firstStackFrame)
						SetBookmark(lastStackFrame);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private void SetBookmark(TestFailureStack stackFrame)
		{
			var fileName = stackFrame.File;
			EnvDTE.ProjectItem ptojItem = Dte.Solution.FindProjectItem(fileName);
			if (null != ptojItem)
			{
				ptojItem.Open(EnvDteConstants.vsViewKindCode).Activate();
				var textSelection = (EnvDTE.TextSelection) Dte.ActiveDocument.Selection;
				textSelection.MoveToLineAndOffset(stackFrame.Line, stackFrame.Column, true);
				textSelection.SelectLine();
				textSelection.ClearBookmark();
				textSelection.SetBookmark();
			}
		}

		public void FocusVisualStudio()
		{
			try
			{
				Dte.MainWindow.Activate();
				Dte.MainWindow.SetFocus();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

	}
}
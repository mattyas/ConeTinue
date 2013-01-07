using System.Linq;
using Caliburn.Micro;

namespace ConeTinue.Domain
{
	public class SettingsStrategy : PropertyChangedBase, IChangeValue
	{
		public bool ReloadTestAssembliesWhenChanged
		{
			get { return Properties.Settings.Default.ReloadTestAssembliesWhenChanged; }
		}

		public bool RunTestsOnReload
		{
			get { return Properties.Settings.Default.RunTestsOnReload; }
		}

		public bool DefaultAllExpanded
		{
			get { return Properties.Settings.Default.DefaultAllExpanded; }
		}


		public string[] Recent
		{
			get { return Properties.Settings.Default.Recent.Cast<string>().ToArray(); }
		}

		public bool ExcludeCategories
		{
			get { return Properties.Settings.Default.ExcludeCategories; }
		}

		public bool OutputDebugAndError
		{
			get { return Properties.Settings.Default.OutputDebugAndError; }
		}

		public bool PinOutputInVisualStudio
		{
			get { return Properties.Settings.Default.PinOutputInVisualStudio; }
		}

		public void AddRecent(string path)
		{
			var recent = Properties.Settings.Default.Recent;
			if (recent.Contains(path))
				recent.Remove(path);
			if (recent.Count > 9)
				recent.RemoveAt(recent.Count - 1);
			recent.Insert(0, path);
			Properties.Settings.Default.Save();
			NotifyOfPropertyChange(() => Recent);
		}
		public bool GetValue(string propertyName)
		{
			return (bool)Properties.Settings.Default[propertyName];
		}

		public void SetValue(string propertyName, bool value)
		{
			Properties.Settings.Default[propertyName] = value;
			Properties.Settings.Default.Save();
			NotifyOfPropertyChange(propertyName);
		}

	}
}
using System.Windows;
using Caliburn.Micro;
using ConeTinue.ViewModels;

namespace ConeTinue
{
	public class AppBootstrapper : Bootstrapper<ShellViewModel>
	{
		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			base.OnStartup(sender, e);
			Application.MainWindow.SizeToContent = SizeToContent.Manual;
			Application.MainWindow.Height = 500;
			Application.MainWindow.Width = 800;
		}
	}
}

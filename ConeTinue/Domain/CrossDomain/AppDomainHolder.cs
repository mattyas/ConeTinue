using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ConeTinue.Domain.CrossDomain
{
	public class AppDomainHolder : IDisposable
	{
		public TestProxy Proxy { get; private set; }
		public AppDomain MyDomain { get; private set; }

		public AppDomainHolder(TestAssembly testAssembly, Guid myId)
		{
			var domainSetup = new AppDomainSetup
				{
					ApplicationBase = testAssembly.AssemblyDirectory,
					ShadowCopyFiles = "True",
				};

			var configPath = Path.GetFullPath(testAssembly.AssemblyPath + ".config");
			if (File.Exists(configPath))
				domainSetup.ConfigurationFile = configPath;

			MyDomain = AppDomain.CreateDomain("ConeTinue.TestDomain",
			                                  null,
			                                  domainSetup,
			                                  new PermissionSet(PermissionState.Unrestricted),
			                                  new StrongName[0]);

			Proxy = (TestProxy) MyDomain.CreateInstanceFrom(typeof (TestProxy).Assembly.Location, typeof (TestProxy).FullName).Unwrap();
			Proxy.Init();
			Proxy.MyId = myId;
			Proxy.AssemblyPath = testAssembly.AssemblyPath;
		}

		public void Dispose()
		{
			if (MyDomain != null)
				AppDomain.Unload(MyDomain);
		}
	}
}
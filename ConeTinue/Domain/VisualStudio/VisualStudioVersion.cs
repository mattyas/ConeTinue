namespace ConeTinue.Domain.VisualStudio
{
	public class VisualStudioVersion
	{
		public string Name { get; set; }
		public string Key { get; set; }
		public bool Is80Compatible { get; set; }

		public static VisualStudioVersion[] Versions
		{
			get
			{
				return new[]
					{
						new VisualStudioVersion {Key = "VisualStudio.DTE.12.0", Name = "Visual Studio 2013", Is80Compatible = true},
						new VisualStudioVersion {Key = "VisualStudio.DTE.11.0", Name = "Visual Studio 2012", Is80Compatible = true},
						new VisualStudioVersion {Key = "VisualStudio.DTE.10.0", Name = "Visual Studio 2010", Is80Compatible = true},
						new VisualStudioVersion {Key = "VisualStudio.DTE.9.0", Name = "Visual Studio 2008", Is80Compatible = true},
						new VisualStudioVersion {Key = "VisualStudio.DTE.8.0", Name = "Visual Studio 2005", Is80Compatible = true},
						new VisualStudioVersion {Key = "VisualStudio.DTE.7.1", Name = "Visual Studio.NET 2003", Is80Compatible = false},
						new VisualStudioVersion {Key = "VisualStudio.DTE.7", Name = "Visual Studio.NET 2002", Is80Compatible = false},
					};
			}
		}
	}
}
using System;
using System.Collections.Generic;

namespace ConeTinue.Domain.CrossDomain
{
	[Serializable]
	public class TestInfo
	{
		public string FullName { get; set; }
		public string Name { get; set; }
		public List<string> Categories { get; set; }
		public TestInfoWithAssembly WithAssembly(TestAssembly assembly)
		{
			return new TestInfoWithAssembly()
				{
					FullName = FullName,
					Name = Name,
					Categories = Categories,
					TestAssembly = assembly
				};
		}
	}
}
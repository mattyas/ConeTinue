using System;
using Cone;

namespace ConeTinue.Domain.CrossDomain
{
	[Serializable]
	public class TestFailure
	{
		public int Column { get; set; }
		public string Context { get; set; }
		public string File { get; set; }
		public int Line { get; set; }
		public string Message { get; set; }
		public string TestName { get; set; }
		public TestKey TestKey { get; set; }

		public TestFailure() { }

		public TestFailure(ConeTestFailure failure, TestKey testKey)
		{
			Column = failure.Column;
			Context = failure.Context;
			File = failure.File;
			Line = failure.Line;
			Message = failure.Message;
			TestName = failure.TestName;
			TestKey = testKey;
		}
	}
}
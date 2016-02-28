using System;
using System.Collections.Generic;
using System.Linq;
using Cone;
using Cone.Core;

namespace ConeTinue.Domain.CrossDomain
{
	[Serializable]
	public class TestFailureStack
	{
		public int Column { get; set; }
		public string File { get; set; }
		public int Line { get; set; }

		public string FullName { get; set; }
		public string ClassName { get; set; }
		public string MethodName { get; set; }
		public bool HasSource { get { return !string.IsNullOrEmpty(File); } }
		public override string ToString()
		{
			return string.Format("{0}.{1}{2}", FullName, MethodName, (HasSource ? string.Format(" : Line {0}", Line) : ""));
		}
	}

	[Serializable]
	public class TestFailure
	{
		public string Context { get; private set; }
		public string Message { get; private set; }
		public string TestName { get; private set; }
		public TestKey TestKey { get; private set; }
		public bool HasSource { get { return Enumerable.Any(StackTrace, x => x.HasSource); } }
		public TestFailureStack[] StackTrace { get; private set; }
		public TestFailure(string context, string message, string testName, TestKey testKey, string file, int line, int column)
		{
			StackTrace = new[]{new TestFailureStack()
				{
					Column = column,
					File = file,
					Line = line,
					FullName = "<unknown class>",
					ClassName = "<unknown class>",
					MethodName = "<unknown method>",
				}};
			Context = context;
			Message = message;
			TestName = testName;
			TestKey = testKey;
			
		}

		public TestFailure(ConeTestFailure failure, TestKey testKey)
		{
			StackTrace = failure.StackFrames.Select(x => new TestFailureStack
				{
					Column = x.Column,
					File = x.File,
					Line = x.Line,
					FullName = x.Method.DeclaringType.FullName,
					ClassName = x.Method.DeclaringType.Name,
					MethodName = string.Format("{0}({1})", x.Method.Name, string.Join(", ", x.Method.GetParameters().Select(y => string.Format("{0} {1}", y.ParameterType.Name, y.Name)))),
				}).ToArray();
			Context = failure.Context;
			Message = failure.Message;
			TestName = failure.TestName;
			TestKey = testKey;
		}
	}
}
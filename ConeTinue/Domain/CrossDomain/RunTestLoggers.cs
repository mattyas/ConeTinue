using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cone;
using Cone.Core;
using Cone.Runners;

namespace ConeTinue.Domain.CrossDomain
{
	internal class RunTestsTestLogger : ITestLogger
	{
		private readonly Action<TestStatus> update;
		private readonly Action<ConeTestFailure> failed;
		private readonly Stopwatch stopwatch;
		private readonly Action<TimeSpan> setTime;
		public RunTestsTestLogger(Action<TestStatus> update, Action<ConeTestFailure> failed, Action<TimeSpan> setTime)
		{
			this.update = update;
			this.failed = failed;
			this.setTime = setTime;
			stopwatch = new Stopwatch();
			stopwatch.Restart();
		}

		private void SetTime()
		{
			stopwatch.Stop();
			setTime(stopwatch.Elapsed);
		}
		public void Failure(ConeTestFailure failure)
		{
			failed(failure);
			update(TestStatus.Failed);
		}

		public void Success()
		{
			update(TestStatus.Success);
		}

		public void Pending(string reason)
		{
			update(TestStatus.Pending);
		}

		public void Skipped()
		{
			update(TestStatus.Skipped);
		}

		public void EndTest()
		{
			SetTime();
		}
	}

	internal class RunTestsLogger : ISuiteLogger, ISessionLogger
	{
		private readonly IUpdateStatus updateStatus;
		private readonly HashSet<string> testToRun;
		private readonly TestAssembly testAssembly;
		private readonly InfoWriter infoWriter;

		public RunTestsLogger(IUpdateStatus updateStatus, HashSet<string> testToRun, TestAssembly testAssembly, InfoWriter infoWriter)
		{
			this.updateStatus = updateStatus;
			this.testToRun = testToRun;
			this.testAssembly = testAssembly;
			this.infoWriter = infoWriter;
		}

		public ITestLogger BeginTest(IConeTest test)
		{
			var testKey = new TestKey {FullName = test.TestName.FullName, TestAssembly = testAssembly};
			if (!testToRun.Contains(test.TestName.FullName))
				return new RunTestsTestLogger(_ => { }, _ => { }, _ => {});

			infoWriter.SetCurrentTest(testKey);
			updateStatus.Update(testKey, TestStatus.Running);
			return new RunTestsTestLogger(status => updateStatus.Update(testKey, status),
			                              failure => updateStatus.Failed(new TestFailure(failure, testKey)),
										  timeSpan => updateStatus.UpdateTestTime(testKey, timeSpan));
		}

		public void EndSuite() { }

		public void Done() {infoWriter.SetCurrentTest(null); }
		public void WriteInfo(Action<TextWriter> output)
		{
			output(infoWriter);
		}

		public void BeginSession() { }
		public ISuiteLogger BeginSuite(IConeSuite suite) { return this; }
		public void EndSession() { }
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cone;
using Cone.Core;
using Cone.Runners;

namespace ConeTinue.Domain.CrossDomain
{
	internal class RunTestsFastTestLogger : ITestLogger
	{
		private readonly Action<TestStatus> update;
		private readonly Action<ConeTestFailure> failed;
		private readonly Stopwatch stopwatch;
		private readonly Action<TimeSpan> setTime;
		public RunTestsFastTestLogger(Action<TestStatus> update, Action<ConeTestFailure> failed, Action<TimeSpan> setTime)
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

	[Serializable]
	public class FastTestReport
	{
		public Dictionary<TestKey, TestStatus> TestStatuses = new Dictionary<TestKey, TestStatus>();
		public List<TestFailure> Failures = new List<TestFailure>();
		public Dictionary<TestKey, TimeSpan> TestTimes = new Dictionary<TestKey, TimeSpan>();
		public string Error;

		public bool IsSuccess { get; set; }

		public bool HasError
		{
			get { return Error != null; }
		}

		public string Output = string.Empty;
	}

	internal class RunTestsFastLogger : ISuiteLogger, ISessionLogger
	{
		private readonly HashSet<string> testToRun;
		private readonly TestAssembly testAssembly;
		private readonly InfoWriter infoWriter;
		private readonly FastTestReport report;
		public FastTestReport Report { get { return report; } }
		public RunTestsFastLogger(HashSet<string> testToRun, TestAssembly testAssembly, InfoWriter infoWriter)
		{
			this.testToRun = testToRun;
			this.testAssembly = testAssembly;
			this.infoWriter = infoWriter;
			report = new FastTestReport();

		}

		public ITestLogger BeginTest(IConeTest test)
		{
			var testKey = new TestKey {FullName = test.TestName.FullName, TestAssembly = testAssembly};
			if (!testToRun.Contains(test.TestName.FullName))
				return new RunTestsTestLogger(_ => { }, _ => { }, _ => {});

			infoWriter.SetCurrentTest(testKey);
			return new RunTestsTestLogger(status => report.TestStatuses[testKey] = status,
			                              failure => report.Failures.Add(new TestFailure(failure, testKey)),
										  timeSpan => report.TestTimes[testKey] = timeSpan);
		}

		public void EndSuite()
		{
			
		}

		public void Done() {infoWriter.SetCurrentTest(null); }
		public void WriteInfo(Action<ISessionWriter> output)
		{
			output(infoWriter);
		}

		public void BeginSession() { }
		public ISuiteLogger BeginSuite(IConeSuite suite) { return this; }
		public void EndSession() { }
	}
}
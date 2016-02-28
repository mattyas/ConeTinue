using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cone;
using Cone.Core;
using Cone.Runners;

namespace ConeTinue.Domain.CrossDomain
{
	public class FindTestsLogger : ITestLogger, ISuiteLogger, ISessionLogger
	{
		public void BeginTest()	{ }
		public void Failure(ConeTestFailure failure) { }
		public void Success() { }
		public void Pending(string reason) { }
		public void Skipped() { }
		public void EndTest() { }

		public ITestLogger BeginTest(IConeTest test)
		{
			var newTestItem = new TestInfo { Name = test.TestName.Name, FullName = test.TestName.FullName, Categories = test.Categories.ToList()};
			tests.Add(newTestItem);
			return this;
		}

		public void EndSuite() { }
		public void Done() { }
		private readonly SynchronizedCollection<TestInfo> tests;

		public FindTestsLogger(SynchronizedCollection<TestInfo> tests)
		{
			this.tests = tests;
		}
        public void WriteInfo(Action<ISessionWriter> output) { output(new ConsoleWriter()); }
		public void BeginSession() { }
		public ISuiteLogger BeginSuite(IConeSuite suite) { return this; }
		public void EndSession() { }
	}
    public class ConsoleWriter : ISessionWriter
    {
        private class Color : IDisposable
        {
            ConsoleColor color;
            public Color(ConsoleColor newColor)
            {
                color = Console.ForegroundColor;
                Console.ForegroundColor = newColor;
            }

            public void Dispose()
            {
                Console.ForegroundColor = color;
            }
        }
        public void Important(string format, params object[] args)
        {
            using (new Color(ConsoleColor.Red))
                Console.WriteLine(format, args);
        }

        public void Info(string format, params object[] args)
        {
            using (new Color(ConsoleColor.Yellow))
                Console.WriteLine(format, args);
        }

        public void NewLine()
        {
            Console.WriteLine();
        }

        public void Write(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
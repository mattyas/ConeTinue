using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cone.Runners;

namespace ConeTinue.Domain.CrossDomain
{
	public class TestProxy : MarshalByRefObject
	{
		private Assembly assembly;
		public string AssemblyPath { get; set; }
		public TestAssembly TestAssembly { get { return new TestAssembly(AssemblyPath); } }
		public Guid MyId { get; set; }
		public override object InitializeLifetimeService()
		{
			return null;
		}

		public IList<TestInfo> FindTests()
		{
			var tests = new SynchronizedCollection<TestInfo>();
			var logger = new FindTestsLogger(tests);
			try
			{
				assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(AssemblyPath));
				new SimpleConeRunner { Workers = Environment.ProcessorCount }
					.RunTests(new TestSession(logger)
					{
						GetResultCollector = fixture => ((test, result) => result.Success())
					}, new[] { assembly });
				return tests.ToList();
			}
			catch (Exception)
			{
				using (var client = new RemotingClient(MyId))
				{
					client.UpdateStatus.ReportError("Failed to load " + AssemblyPath);
				}
				return new List<TestInfo>();
			}
		}

		public bool RunTests(HashSet<string> testsToRun, bool outputErrors)
		{
			ShouldRunTests = true;
			using (var client = new RemotingClient(MyId))
			{
				var output = Console.Out;
				var infoWriter = new InfoWriter(client.UpdateStatus);
				var consoleTraceListener = new TextWriterTraceListener(infoWriter);
				try
				{
					Console.SetOut(infoWriter); 
					var logger = new RunTestsLogger(client.UpdateStatus, testsToRun, TestAssembly, infoWriter);
					
					if (outputErrors)
						Debug.Listeners.Add(consoleTraceListener);
					new SimpleConeRunner { Workers = 1 }
						.RunTests(new TestSession(logger)
							{
								ShouldSkipTest = test =>
									{
										if (!ShouldRunTests) throw new AbortTestsException();
										return !testsToRun.Contains(test.TestName.FullName);
									}
							}, new[] {assembly});
				}
				catch (AbortTestsException)
				{
					return false;
				}
				catch (Exception ex)
				{
					client.UpdateStatus.ReportError(ex.ToString());
					return false;
				}
				finally
				{
					Console.SetOut(output);
					if (outputErrors)
						Debug.Listeners.Remove(consoleTraceListener);
				}
			}
			return true;
		}

		public class AbortTestsException : Exception
		{
		}

		public volatile bool ShouldRunTests = true;

		public void Init()
		{
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) => { };
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => { };
		}
	}
}
using System;
using System.Diagnostics;
using Cone;
using ConeTinue.Domain;

namespace TestSamples
{
	[Feature("Mixed tests", Category = "Bogus")]
	public class MixedTests
	{
		[Context("Many tests", Category = "Funky category")]
		public class ManyTests
		{
			public void is_less_then_300(int value)
			{
				Verify.That(() => value < 300);
			}
			public RowBuilder<ManyTests> Test()
			{
				var tests = new RowBuilder<ManyTests>();
				for (int j = 0; j < 100;j++ )
					for (int i = 0; i < 300; i++)
					{
						var testRun = j;
						var someValue = i;
						tests.Add(string.Format("Test run {0}", testRun), x => x.is_less_then_300(someValue));
					}
				return tests;
			}

			[Context("Random")]
			public class Random
			{
				public void good_with_output()
				{
					Console.WriteLine("good lego");
				}

				public void bad_with_output()
				{
					Console.WriteLine("bad lego");
					Verify.That(() => false);
				}

				[Context("Fail before each")]
				public class FailBeforeEach
				{
					[BeforeEach]
					public void fail()
					{
						Debug.WriteLine("fail before");
						throw new Exception("Bad code here");
					}

					public void test() { }
					public void test2() { }
				}

				[Context("Fail before all")]
				public class FailBeforeAll
				{
					[BeforeAll]
					public void fail()
					{
						Debug.WriteLine("fail before all");
						throw new Exception("Bad code here");
					}

					public void test() { }
					public void test2() { }
				}

				[Context("Fail after each")]
				public class FailAfterEach
				{
					[AfterEach]
					public void fail()
					{
						Debug.WriteLine("fail after each");
						throw new Exception("Bad code here");
					}

					public void test() { }
					public void test2() { }
				}

				[Context("Fail after all")]
				public class FailAfterAll
				{
					[AfterAll]
					public void fail()
					{
						Debug.WriteLine("fail after all");
						throw new Exception("Bad code here");
					}

					public void test() { }
					public void test2() { }
				}
			}

			[Context("Extreme")]
			public class Extreme
			{
				[DisplayAs("{0} is odd")]
				public void is_odd(int value)
				{
					var modolo = value%2;
					Verify.That(() => modolo == 1);
				}

				public RowBuilder<Extreme> Test()
				{
					var tests = new RowBuilder<Extreme>();
						for (int i = 0; i < 20000; i++)
						{
							tests.Add(x => x.is_odd(i));
						}
					return tests;
				}
				 
			}
		}

		[Context("Failing test")]
		public class FailingTest
		{
			public void I_fail_because_my_math_is_bad()
			{
				Verify.That(() => 1 + 2 == 5);
			}

			public void I_fail_because_I_devide_by_zero_in_verify()
			{
				int zero = 0;
				Verify.That(() => 2 / zero == 3);
			}

			[Pending(Reason = "Not done yet")]
			public void I_am_pending()
			{
				Verify.That(() => false);
			}

			public void I_fail_in_my_setup()
			{
				Setup();
				Verify.That(() => true);
			}

			[Pending(Reason = "Supposed to not be done yet")]
			public void I_fail_because_I_work()
			{
				Verify.That(() => true);
			}

			private void Setup()
			{
				int zero = 0;
				int number = 5/zero;
			}
		}
		
	}

	[Describe(typeof(TestItemHolder))]
	public class TestItemHolderSpec
	{
		public void is_always_visible()
		{
			var item = new TestItemHolder {IsVisible = false};
			Verify.That(() => item.IsVisible);
		}
	}
}
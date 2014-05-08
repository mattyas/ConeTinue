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
				Check.That(() => value < 300);
			}
			public RowBuilder<ManyTests> Test()
			{
				var tests = new RowBuilder<ManyTests>();
				Assume.That(() => false);
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
					Check.That(() => false);
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
					Check.That(() => modolo == 1);
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
			[DisplayAs("My name contains.a dot")]
			public void my_name_contains_a_dot()
			{
				Check.That(() => false);
			}
			public void I_fail_because_my_math_is_bad()
			{
				Check.That(() => 1 + 2 == 5);
			}

			public void I_fail_because_I_devide_by_zero_in_verify()
			{
				int zero = 0;
				Check.That(() => 2 / zero == 3);
			}

			[Pending(Reason = "Not done yet")]
			public void I_am_pending()
			{
				Check.That(() => false);
			}

			public void I_fail_in_my_setup()
			{
				Setup();
				Check.That(() => true);
			}

			public void I_fail_in_my_setup_after_long_loop()
			{
				FailAfterAWhile(100);
				Check.That(() => true);
			}

			//public void I_fail_in_my_setup_after_forever_loop()
			//{
			//	I_fail_in_my_setup_after_forever_loop();
			//	Check.That(() => true);
			//}

			private void FailAfterAWhile(int number)
			{
				int j = 100/number;
				if (j == -1)
					return;
				FailAfterAWhile(--number);
			}

			public void I_fail_in_system()
			{
				var x= int.Parse("Sju10två");
				Check.That(() => x == 72);
			}

			[Pending(Reason = "Supposed to not be done yet")]
			public void I_fail_because_I_work()
			{
				Check.That(() => true);
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
			Check.That(() => item.IsVisible);
		}
	}
}
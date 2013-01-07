using System;
using System.Collections.Generic;
using System.Linq;

namespace ConeTinue.Domain
{
	[Flags]
	public enum TestStatus
	{
		NotSet = 0,
		Unknown = 1,
		Running = 2,
		Pending = 4,
		Failed = 8,
		Success = 16,
		Skipped = 32,
		InQueue = 64,
	}

	public static class TestStatusExtensions
	{
		public static TestStatus Sum(this IEnumerable<TestStatus> statuses)
		{
			return statuses.Aggregate(TestStatus.NotSet, (current, status) => current | status);
		}
		public static TestStatus SumStatus(this IEnumerable<TestItem> statuses)
		{
			return statuses.Aggregate(TestStatus.NotSet, (current, status) => current | status.Status);
		}
	}


}
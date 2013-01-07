using System;

namespace ConeTinue.Domain
{
	public class StatusChangedEventArgs : EventArgs
	{
		public TestStatus OldStatus { get; private set; }
		public TestStatus NewStatus { get; private set; }
		public TestItem Test { get; private set; }

		public StatusChangedEventArgs(TestStatus oldStatus, TestStatus newStatus, TestItem test)
		{
			OldStatus = oldStatus;
			NewStatus = newStatus;
			Test = test;
		}
	}
}
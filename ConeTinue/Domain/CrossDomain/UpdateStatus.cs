using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ConeTinue.ViewModels.Messages;

namespace ConeTinue.Domain.CrossDomain
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class UpdateStatus : IUpdateStatus, IDisposable
	{
		private readonly IEventAggregator eventAggregator;
		private readonly ConcurrentDictionary<TestStatus, int> counts = new ConcurrentDictionary<TestStatus, int>(); 
		private readonly ConcurrentQueue<Tuple<TestKey,TestStatus>> statusUpdateQueue = new ConcurrentQueue<Tuple<TestKey, TestStatus>>(); 
		private readonly Thread queueWorker;


		public UpdateStatus(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			queueWorker = new Thread(SendQueue);
			queueWorker.Start();
		}

		private volatile bool work = true;
		private void SendQueue()
		{
			while (work || statusUpdateQueue.Count != 0)
			{
				Tuple<TestKey,TestStatus> workItem;
				if (!statusUpdateQueue.TryDequeue(out workItem))
				{
					if (!work)
						return;
					Thread.Sleep(200);
					continue;
				}
				UpdateCount(workItem.Item2);

				TestItem item;
				if (!tests.TryGetTest(workItem.Item1, out item))
					return;
				item.Status = workItem.Item2;
			}
		}

		private TestItemHolder tests;
		public void SetTests(TestItemHolder testItemHolder)
		{
			tests = testItemHolder;
			counts.Clear();
		}
		
		public void Update(TestKey testKey, TestStatus status)
		{
			AddToQueue(testKey, status);
		}

		private void AddToQueue(TestKey testKey, TestStatus status)
		{
			statusUpdateQueue.Enqueue(new Tuple<TestKey, TestStatus>(testKey, status));
		}

		public void UpdateTestTime(TestKey testKey, TimeSpan time)
		{
			Task.Factory.StartNew(() =>
				{
					TestItem test;
					if (tests.TryGetTest(testKey, out test))
						test.RunTime = time;
				});
		}

		public void Failed(TestFailure failure)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(failure));
		}

		public void SetStatus(string message)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(new StatusMessage(message)));
		}

		public void ReportError(string error)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(new ErrorMessage(error)));
		}

		public void ReportInfo(string info)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(new InfoMessage(info)));
		}

		private void UpdateCount(TestStatus status)
		{
			if (status == TestStatus.Running)
				return;
			
			if (counts.ContainsKey(status))
				counts[status]++;
			else
				counts[status] = 1;
		
			SetStatus(string.Join(" - ", counts.Select(x => string.Format("{0}: {1}", x.Key, x.Value))));
		}

		public void Dispose()
		{
			work = false;
		}
	}
}
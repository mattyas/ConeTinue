﻿using System;
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
		private readonly ConcurrentQueue<Tuple<TestKey, TestStatus>> statusUpdateQueue = new ConcurrentQueue<Tuple<TestKey, TestStatus>>();
		private readonly ConcurrentQueue<Tuple<TestKey, TimeSpan>> timingUpdateQueue = new ConcurrentQueue<Tuple<TestKey, TimeSpan>>();
		private readonly Thread statusQueueWorker;
		private readonly Thread timingQueueWorker;


		public UpdateStatus(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			statusQueueWorker = new Thread(SendStatusQueue) {IsBackground = true};
			statusQueueWorker.Start();
			timingQueueWorker = new Thread(SendTimingQueue) { IsBackground = true };
			timingQueueWorker.Start();
		}

		private volatile bool work = true;
		private void SendStatusQueue()
		{
			while (work || statusUpdateQueue.Count != 0)
			{
                if (!statusUpdateQueue.TryDequeue(out Tuple<TestKey, TestStatus> workItem))
                {
                    if (!work)
                        return;
                    Thread.Sleep(200);
                    continue;
                }
                UpdateCount(workItem.Item2);

                if (!tests.TryGetTest(workItem.Item1, out TestItem item))
                    return;
                item.Status = workItem.Item2;
			}
		}

		private void SendTimingQueue()
		{
			while (work || timingUpdateQueue.Count != 0)
			{
                if (!timingUpdateQueue.TryDequeue(out Tuple<TestKey, TimeSpan> workItem))
                {
                    if (!work)
                        return;
                    Thread.Sleep(200);
                    continue;
                }

                if (tests.TryGetTest(workItem.Item1, out TestItem item))
                    item.RunTime = workItem.Item2;
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
			statusUpdateQueue.Enqueue(new Tuple<TestKey, TestStatus>(testKey, status));
		}

		public void UpdateTestTime(TestKey testKey, TimeSpan time)
		{
			timingUpdateQueue.Enqueue(new Tuple<TestKey, TimeSpan>(testKey, time));
		}

		public void Failed(TestFailure failure)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(new ReportFailures(failure)));
		}

		public void SetStatus(string message)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(new StatusMessage(message)));
		}

		public void ReportError(string error, string details)
		{
			Task.Factory.StartNew(() => eventAggregator.Publish(new ErrorMessage(error, details)));
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
using System;
using System.ServiceModel;
using Caliburn.Micro;

namespace ConeTinue.Domain.CrossDomain
{
	public class RemotingServer : IDisposable
	{
		private readonly ServiceHost host;
		private readonly UpdateStatus updateStatus;

		public RemotingServer(Guid myId, IEventAggregator eventAggregator, TestItemHolder tests) : this(myId,eventAggregator)
		{
			updateStatus.SetTests(tests);
		}

		public RemotingServer(Guid myId, IEventAggregator eventAggregator)
		{
			updateStatus = new UpdateStatus(eventAggregator);
			host = new ServiceHost(updateStatus, new[] { new Uri("net.pipe://localhost/" + myId + "/") });
			host.AddServiceEndpoint(typeof(IUpdateStatus), new NetNamedPipeBinding(), "UpdateStatus");
			host.Open();
		}

		public void Dispose()
		{
			host.Close();
			updateStatus.Dispose();
		}
	}
}
using System;
using System.ServiceModel;

namespace ConeTinue.Domain.CrossDomain
{
	public class RemotingClient : IDisposable
	{
		private readonly ChannelFactory<IUpdateStatus> pipeFactory;
		public IUpdateStatus UpdateStatus { get; private set; }

		public RemotingClient(Guid myId)
		{
			pipeFactory = new ChannelFactory<IUpdateStatus>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/" + myId + "/UpdateStatus"));
			UpdateStatus = pipeFactory.CreateChannel();
		}

		public void Dispose()
		{
			pipeFactory.Close();
		}
	}
}
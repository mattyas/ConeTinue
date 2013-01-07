namespace ConeTinue.ViewModels.Messages
{
	public class StatusMessage
	{
		public string Status { get; private set; }

		public StatusMessage(string status)
		{
			Status = status;
		}
	}
}
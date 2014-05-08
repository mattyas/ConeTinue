namespace ConeTinue.ViewModels.Messages
{
	public class ErrorMessage
	{
		public string Error { get; private set; }
		public string Details { get; private set; }

		public ErrorMessage(string error, string details)
		{
			Error = error;
			Details = details;
		}
	}
}
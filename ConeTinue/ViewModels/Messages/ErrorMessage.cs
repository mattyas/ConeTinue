namespace ConeTinue.ViewModels.Messages
{
	public class ErrorMessage
	{
		public string Error { get; private set; }

		public ErrorMessage(string error)
		{
			Error = error;
		}
	}
}
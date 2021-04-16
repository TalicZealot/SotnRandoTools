namespace SotnRandoTools.Services
{
	public interface INotificationService
	{
		void DisplayMessage(string message);

		void PlayAlert(string uri);
	}
}
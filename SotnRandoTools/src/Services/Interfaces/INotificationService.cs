namespace SotnRandoTools.Services
{
	public interface INotificationService
	{
		double Volume { set; }
		short KhaosMeter { get; set; }
		void AddMessage(string message);
		void PlayAlert(string uri);
	}
}
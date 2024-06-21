namespace SotnRandoTools.Services
{
	internal interface INotificationService
	{
		double Volume { set; }
		void AddMessage(string message);
		void PlayAlert(string uri);
		void StartOverlayServer();
		void StopOverlayServer();
		void UpdateTrackerOverlay(int relics, int items);
	}
}
namespace SotnRandoTools.Services
{
	internal interface INotificationService
	{
		float Volume { set; }
		void AddMessage(string message);
		void PlayAlert();
		void StartOverlayServer();
		void StopOverlayServer();
		void UpdateTrackerOverlay(int relics, int items);
	}
}
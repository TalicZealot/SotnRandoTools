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
		void SetRelicCoordinates(string relic, int mapCol, int mapRow);
		void SetInvertedRelicCoordinates(string relic, int mapCol, int mapRow);
	}
}
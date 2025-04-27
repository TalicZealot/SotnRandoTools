using System.Threading.Tasks;

namespace SotnRandoTools.Services
{
	internal interface INotificationService
	{
		float Volume { set; }
		void AddMessage(string message);
		void PlayAlert();
		void StartOverlayServer();
		Task StopOverlayServer();
		void UpdateOverlayLayout();
		void UpdateTrackerOverlay(int relics, int items, int bosses);
	}
}
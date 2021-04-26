using SotnRandoTools.Khaos.Enums;

namespace SotnRandoTools.Services
{
	public interface INotificationService
	{
		double Volume { set; }
		void DisplayMessage(string message);
		void AddAction(ActionType action);
		void DequeueAction();
		void PlayAlert(string uri);
	}
}
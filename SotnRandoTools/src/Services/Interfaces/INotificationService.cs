using SotnRandoTools.Khaos.Enums;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Services
{
	public interface INotificationService
	{
		double Volume { set; }
		void AddMessage(string message);
		void AddAction(ActionType action);
		void DequeueAction();
		void AddTimer(ActionTimer timer);
		void PlayAlert(string uri);
	}
}
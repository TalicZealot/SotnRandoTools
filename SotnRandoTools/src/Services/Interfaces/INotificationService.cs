using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Services
{
	public interface INotificationService
	{
		double Volume { set; }
		short KhaosMeter { get; set; }
		void AddMessage(string message);
		void PlayAlert(string uri);

		void StartOverlayServer();
		void StopOverlayServer();
		void AddOverlayTimer(string name, int duration);
		void UpdateOverlayQueue(List<QueuedAction> actionQueue);
		void UpdateTrackerOverlay(int relics, int items);
	}
}
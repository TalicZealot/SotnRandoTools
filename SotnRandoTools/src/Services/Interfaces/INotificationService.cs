using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Services
{
	internal interface INotificationService
	{
		double Volume { set; }
		bool MapOpen { get; set; }
		bool InvertedMapOpen { get; set; }
		int VermillionBirds { get; set; }
		int AzureDragons { get; set; }
		int BlackTortoises { get; set; }
		int WhiteTigers { get; set; }
		void AddMessage(string message);
		void PlayAlert(string uri);
		void StartOverlayServer();
		void StopOverlayServer();
		void UpdateOverlayMeter(int meter);
		void AddOverlayTimer(string name, int duration);
		void UpdateOverlayQueue(List<QueuedAction> actionQueue);
		void UpdateTrackerOverlay(int relics, int items);
		void SetRelicCoordinates(string relic, int mapCol, int mapRow);
		void SetInvertedRelicCoordinates(string relic, int mapCol, int mapRow);
	}
}
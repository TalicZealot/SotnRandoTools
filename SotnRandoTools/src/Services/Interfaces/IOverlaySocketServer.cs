using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Services.Interfaces
{
	public interface IOverlaySocketServer
	{
		void StartServer();
		void StopServer();
		void UpdateMeter(int meter);
		void AddTimer(string name, int duration);
		void UpdateQueue(List<QueuedAction> actionQueue);
		void UpdateTracker(int relics, int items);
	}
}

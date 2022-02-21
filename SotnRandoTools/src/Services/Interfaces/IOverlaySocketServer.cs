using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.Services.Interfaces
{
	public interface IOverlaySocketServer
	{
		void StartServer();
		void StopServer();
		void AddTimer(string name, int duration);
		void UpdateQueue(List<QueuedAction> actionQueue);
		void UpdateTracker(int relics, int items);
	}
}

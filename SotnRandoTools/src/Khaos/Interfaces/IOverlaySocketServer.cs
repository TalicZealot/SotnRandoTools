using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IOverlaySocketServer
	{
		void StartServer();
		void StopServer();
		void AddTimer(string name, int duration);
		void UpdateQueue(List<QueuedAction> actionQueue);
	}
}

using System.Collections.Generic;

namespace SotnRandoTools.Services.Interfaces
{
	internal interface IOverlaySocketServer
	{
		void StartServer();
		void StopServer();
		void UpdateTracker(int relics, int items);
	}
}

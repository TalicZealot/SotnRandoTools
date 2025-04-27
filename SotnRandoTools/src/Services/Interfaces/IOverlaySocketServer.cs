using System.Threading.Tasks;

namespace SotnRandoTools.Services.Interfaces
{
	internal interface IOverlaySocketServer
	{
		void StartServer();
		Task StopServer();
		void UpdateTracker(int relics, int items, int bosses); 
	}
}

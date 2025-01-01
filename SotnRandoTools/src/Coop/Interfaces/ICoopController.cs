using SotnRandoTools.Coop.Models;

namespace SotnRandoTools.Coop.Interfaces
{
	internal interface ICoopController
	{
		CoopState CoopState { get; }
		bool SynchRequested { get; set; }
		void Connect(string hostIp, int port);
		void Disconnect();
		void StartServer(int port);
		void StopServer();
		void DisposeAll();
		void SendData(byte[] data);
		bool IsConnected();
	}
}
using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Interfaces
{
	public interface ICoopMessanger
	{
		void Connect(string hostIp, int port);
		void Disconnect();
		void StartServer(int port);
		void StopServer();
		void DisposeAll();
		void SendData(MessageType type, byte[] data);
		bool IsConnected();
	}
}
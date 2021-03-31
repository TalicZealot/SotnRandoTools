using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Interfaces
{
	public interface ICoopMessanger
	{
		bool Connect(string hostIp, int port);
		void Disconnect();
		bool StartServer(int port);
		void StopServer();
		void SendData(MessageType type, byte[] data);
	}
}
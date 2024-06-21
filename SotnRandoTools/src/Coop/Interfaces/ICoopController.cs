namespace SotnRandoTools.Coop.Interfaces
{
	internal interface ICoopController
	{
		void Connect(string hostIp, int port);
		void Disconnect();
		void StartServer(int port);
		void StopServer();
		void DisposeAll();
		void SendData(byte[] data);
		bool IsConnected();
	}
}
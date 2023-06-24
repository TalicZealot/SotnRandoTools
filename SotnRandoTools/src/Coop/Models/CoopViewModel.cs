using System.ComponentModel;
using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Models
{
	public class CoopViewModel : INotifyPropertyChanged, ICoopViewModel
	{
		private ServerStatus serverStarted = ServerStatus.Stopped;
		private ClientStatus clientConnected = ClientStatus.ManuallyDisconnected;
		private string message = "";
		public ServerStatus ServerStatus
		{
			get
			{
				return serverStarted;
			}
			set
			{
				serverStarted = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(ServerStatus)));
				}
			}
		}

		public ClientStatus ClientStatus
		{
			get
			{
				return clientConnected;
			}
			set
			{
				clientConnected = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(ClientStatus)));
				}
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
			set
			{
				message = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(Message)));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

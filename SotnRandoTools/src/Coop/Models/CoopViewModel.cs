using System.ComponentModel;

namespace SotnRandoTools.Coop.Models
{
	public class CoopViewModel : INotifyPropertyChanged, ICoopViewModel
	{
		private bool serverStarted;
		private bool clientConnected;
		private string message;
		public bool ServerStarted
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
					PropertyChanged(this, new PropertyChangedEventArgs("ServerStarted"));
				}
			}
		}
		public bool ClientConnected
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
					PropertyChanged(this, new PropertyChangedEventArgs("ClientConnected"));
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
					PropertyChanged(this, new PropertyChangedEventArgs("Message"));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

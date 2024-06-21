using System.ComponentModel;
using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Models
{
	public class CoopViewModel : INotifyPropertyChanged, ICoopViewModel
	{
		private NetworkStatus status = NetworkStatus.Stopped;
		private string message = "";
		private int ping = -1;
		public NetworkStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
				}
			}
		}
		public int Ping 
		{
			get
			{
				return ping;
			}
			set
			{
				ping = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(Ping)));
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}

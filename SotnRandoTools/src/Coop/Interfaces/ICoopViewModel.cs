using System.ComponentModel;

namespace SotnRandoTools.Coop.Models
{
	public interface ICoopViewModel
	{
		bool ClientConnected { get; set; }
		bool ServerStarted { get; set; }
		string Message { get; set; }

		event PropertyChangedEventHandler PropertyChanged;
	}
}
using System.ComponentModel;
using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Models
{
	public interface ICoopViewModel
	{
		NetworkStatus Status { get; set; }
		int Ping { get; set; }
		event PropertyChangedEventHandler PropertyChanged;
	}
}
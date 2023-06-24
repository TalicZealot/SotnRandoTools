using System.ComponentModel;
using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Models
{
	public interface ICoopViewModel
	{
		ClientStatus ClientStatus { get; set; }
		ServerStatus ServerStatus { get; set; }
		string Message { get; set; }

		event PropertyChangedEventHandler PropertyChanged;
	}
}
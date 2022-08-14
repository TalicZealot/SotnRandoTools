using SotnRandoTools.Khaos.Interfaces;

namespace SotnRandoTools.RandoTracker
{
	public interface ITracker
	{
		IRelicLocationDisplay RelicLocationDisplay { get; set; }
		void DrawRelicsAndItems();
		void Update();
		void SaveReplay();
	}
}

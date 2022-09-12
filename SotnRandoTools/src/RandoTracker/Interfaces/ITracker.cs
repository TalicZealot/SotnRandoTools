using SotnRandoTools.Khaos.Interfaces;

namespace SotnRandoTools.RandoTracker
{
	internal interface ITracker
	{
		IRelicLocationDisplay RelicLocationDisplay { get; set; }
		void DrawRelicsAndItems();
		void Update();
		void SaveReplay();
	}
}

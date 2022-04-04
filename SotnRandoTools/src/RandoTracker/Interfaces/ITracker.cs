using SotnRandoTools.Khaos.Interfaces;

namespace SotnRandoTools.RandoTracker
{
	public interface ITracker
	{
		TrackerGraphicsEngine GraphicsEngine { get; }
		IRelicLocationDisplay RelicLocationDisplay { get; set; }
		void DrawRelicsAndItems();
		void Update();
		void SaveReplay();
	}
}

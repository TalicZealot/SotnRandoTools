namespace SotnRandoTools.RandoTracker
{
	public interface ITracker
	{
		TrackerGraphicsEngine GraphicsEngine { get; }
		void DrawRelicsAndItems();
		void Update();
		void SaveReplay();
	}
}

using SotnRandoTools.RandoTracker.Interfaces;

namespace SotnRandoTools.RandoTracker
{
	public interface ITrackerGraphicsEngine
	{
		void SetProgression();
		void CalculateGrid(int width, int height);
		void DrawSeedInfo(string seedInfo);
		void Render();
		void ChangeGraphics(IGraphics formGraphics);
	}
}
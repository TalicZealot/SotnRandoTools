using System.Collections.Generic;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.RandoTracker
{
	internal interface ITrackerGraphicsEngine
	{
		bool Refreshed { get; set; }
		void SetProgression();
		void CalculateGrid(int width, int height);
		void DrawSeedInfo(string seedInfo);
		void Render();
		void ChangeGraphics(IGraphics formGraphics);
		void InitializeItems(List<Models.Relic> relics, List<Item> progressionItems, List<Item> thrustSwords);
	}
}
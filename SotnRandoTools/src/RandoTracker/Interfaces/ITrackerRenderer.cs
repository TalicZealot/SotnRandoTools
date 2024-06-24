using System.Collections.Generic;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.RandoTracker
{
	internal interface ITrackerRenderer
	{
		bool Refreshed { get; set; }
		string SeedInfo { get; set; }
		void SetProgression();
		void CalculateGrid(int width, int height);
		void Render();
		void ChangeGraphics(IGraphics formGraphics);
		void InitializeItems(Models.TrackerRelic[] relics, Item[] progressionItems, Item[] thrustSwords);
	}
}
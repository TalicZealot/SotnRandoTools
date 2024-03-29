﻿using System.Drawing;

namespace SotnRandoTools.Configuration.Interfaces
{
	public interface IToolConfig
	{
		TrackerConfig? Tracker { get; set; }
		CoopConfig? Coop { get; set; }
		Point Location { get; set; }
		string Version { get; set; }
		void SaveConfig();
	}
}
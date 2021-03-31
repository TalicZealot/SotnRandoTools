using System.Drawing;

namespace SotnRandoTools.Configuration.Interfaces
{
	public interface IToolConfig
	{
		TrackerConfig? Tracker { get; set; }
		KhaosConfig? Khaos { get; set; }
		CoopConfig? Coop { get; set; }
		Point Location { get; set; }
		void SaveConfig();
	}
}
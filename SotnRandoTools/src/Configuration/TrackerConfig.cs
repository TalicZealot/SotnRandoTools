using System.Drawing;

namespace SotnRandoTools.Configuration
{
	public class TrackerConfig
	{
		public TrackerConfig()
		{
			ProgressionRelicsOnly = false;
			GridLayout = true;
			AlwaysOnTop = false;
			Locations = true;
			SaveReplays = true;
			CustomLocationsGuarded = true;
			CustomLocationsEquipment = false;
			CustomLocationsClassic = false;
			Width = 260;
			Height = 465;
		}
		public bool ProgressionRelicsOnly { get; set; }
		public bool GridLayout { get; set; }
		public bool AlwaysOnTop { get; set; }
		public bool Locations { get; set; }
		public bool SaveReplays { get; set; }
		public string Username { get; set; }
		public bool CustomLocationsGuarded { get; set; }
		public bool CustomLocationsEquipment { get; set; }
		public bool CustomLocationsClassic { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public Point Location { get; set; }
	}
}

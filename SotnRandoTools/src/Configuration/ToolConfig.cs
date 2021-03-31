using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Configuration
{
	public class ToolConfig : IToolConfig
	{
		public ToolConfig()
		{
			Tracker = new TrackerConfig();
			Khaos = new KhaosConfig();
			Coop = new CoopConfig();
		}
		public TrackerConfig? Tracker { get; set; }
		public KhaosConfig? Khaos { get; set; }
		public CoopConfig? Coop { get; set; }
		public Point Location { get; set; }
		public void SaveConfig()
		{
			string output = JsonConvert.SerializeObject(this, Formatting.Indented);
			if (File.Exists(Paths.ConfigPath))
			{
				File.WriteAllText(Paths.ConfigPath, output);
			}
			else
			{
				using (StreamWriter sw = File.CreateText(Paths.ConfigPath))
				{
					sw.Write(output);
				}
			}
		}
	}
}

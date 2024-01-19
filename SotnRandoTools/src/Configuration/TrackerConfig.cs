using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace SotnRandoTools.Configuration
{
	public class TrackerConfig
	{
		public TrackerConfig()
		{
			Default();
		}
		public bool ProgressionRelicsOnly { get; set; }
		public bool GridLayout { get; set; }
		public bool AlwaysOnTop { get; set; }
		public bool Locations { get; set; }
		public bool SaveReplays { get; set; }
		public bool EnableAutosplitter { get; set; }
		public bool UseOverlay { get; set; }
		public bool MuteMusic { get; set; }
		public bool Stereo { get; set; }
		public string Username { get; set; }
		public List<List<int>> OverlaySlots { get; set; }
		public bool CustomLocationsGuarded { get; set; }
		public bool CustomLocationsEquipment { get; set; }
		public bool CustomLocationsClassic { get; set; }
		public bool CustomLocationsSpread { get; set; }
		public bool CustomLocationsCustom { get; set; }
		public string CustomExtension { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public Point Location { get; set; }

		public void Default()
		{
			ProgressionRelicsOnly = false;
			GridLayout = true;
			AlwaysOnTop = false;
			Locations = true;
			SaveReplays = true;
			EnableAutosplitter = true;
			UseOverlay = false;
			MuteMusic = false;
			Stereo = true;
			CustomLocationsGuarded = true;
			CustomLocationsEquipment = false;
			CustomLocationsClassic = false;
			CustomLocationsSpread = false;
			CustomLocationsCustom = false;
			CustomExtension = "";
			Width = 260;
			Height = 490;
			Username = "";
			OverlaySlots = new List<List<int>>
			{
				new() {1, 6, 11, 16, 21, 35, 26, 31, 0, 0, 0, 0, 0, 0},
				new() {2, 7, 12, 17, 22, 0, 27, 32, 0, 0, 0, 0, 0, 0},
				new() {3, 8, 13, 18, 23, 0, 28, 33, 0, 0, 0, 0, 0, 0},
				new() {4, 9, 14, 19, 24, 0, 29, 34, 0, 0, 0, 0, 0, 0},
				new() {5, 10, 15, 20, 25, 0, 30, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			};
		}

		public void SaveOverlayLayout(string path)
		{
			string output = JsonConvert.SerializeObject(OverlaySlots, Formatting.Indented);
			if (File.Exists(path))
			{
				File.WriteAllText(path, output);
			}
			else
			{
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.Write(output);
				}
			}
		}

		public void LoadOverlayLayout(string path)
		{
			if (File.Exists(path))
			{
				string layoutJson = File.ReadAllText(path);

				List<List<int>> layout = JsonConvert.DeserializeObject<List<List<int>>>(layoutJson,
					new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, MissingMemberHandling = MissingMemberHandling.Ignore }) ?? new List<List<int>>();

				if (layout.Count > 0)
				{
					OverlaySlots = layout;
				}
			}
		}
	}
}

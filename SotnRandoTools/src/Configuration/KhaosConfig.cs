using System.Collections.Generic;
using System.Drawing;
using SotnRandoTools.Configuration.Models;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Configuration
{
	public class KhaosConfig
	{
		public KhaosConfig()
		{
			Alerts = true;
			ControlPannelQueueActions = true;
			Volume = 2;
			NamesFilePath = Paths.NamesFilePath;
			BotApiKey = "";
			WeakenFactor = 0.5F;
			CrippleFactor = 0.8F;
			HasteFactor = 3.2F;
			ThirstDrainPerSecond = 1;
			PandoraMinItems = 5;
			PandoraMaxItems = 32;
			QueueInterval = new System.TimeSpan(0, 0, 31);
			DynamicInterval = true;
			KeepVladRelics = false;
			Actions = new List<Action>
			{
				new Action{Name="Khaos Status", Enabled = true, Meter = 2, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Equipment", Enabled = true, Meter = 6, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Stats", Enabled = true, Meter = 6, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Relics", Enabled = true, Meter = 6, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Pandora's Box", Enabled = true, Meter = 6, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Gamble", Enabled = true, Meter = 2, AlertPath = Paths.LibrarianThankYouSound},
				new Action{Name="Bankrupt", Enabled = true, Meter = 5, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Weaken", Enabled = true, Meter = 5, AlertPath = Paths.RichterLaughSound},
				new Action{Name="Respawn Bosses", Enabled = true, Meter = 3, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Subweapons Only", Enabled = true, Meter = 4, AlertPath = Paths.RichterLaughSound, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Cripple", Enabled = true, Meter = 5, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Blood Mana", Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Thirst", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Khaos Horde", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.RichterLaughSound},
				new Action{Name="Endurance", Enabled = true, Meter = 4, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Vampire", Enabled = true,  Meter = 2, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Light Help", Enabled = true, Meter = 2, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Medium Help", Enabled = true, Meter = 3, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Heavy Help", Enabled = true, Meter = 5, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Battle Orders", Enabled = true,  Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Magician", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Melty Blood", Enabled = true, Meter = 5, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.MeltySound},
				new Action{Name="Guilty Gear", Enabled = true, Meter = 5, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DragonInstallSound},
				new Action{Name="Four Beasts", Enabled = true,  Meter = 6, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="ZA WARUDO", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.ZaWarudoSound},
				new Action{Name="Haste", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0)}
			};
			LightHelpItemRewards = new string[]
			{
				"Leather shield",
				"Shaman shield",
				"Pot Roast",
				"Sirloin",
				"Turkey",
				"Bat Pentagram",
				"Javelin",
				"Luminus",
				"Jewel sword",
				"Icebrand",
				"Holy rod",
				"Star flail",
				"Chakram",
				"Holbein dagger",
				"Heart Refresh",
				"Antivenom",
				"Uncurse",
				"Life apple",
				"Str. potion",
				"Attack potion",
				"Shield potion",
				"Resist fire",
				"Potion",
				"Alucart shield",
				"Alucart sword",
				"Stone mask",
				"Wizard hat",
				"Platinum mail",
				"Diamond plate",
				"Healing mail",
				"Fire mail",
				"Mirror cuirass",
				"Brilliant mail",
				"Axe Lord armor",
				"Alucart mail",
				"Royal cloak",
				"Blood cloak",
				"Zircon",
				"Aquamarine",
				"Lapis lazuli",
				"Medal",
				"Talisman"
			};
			MediumHelpItemRewards = new string[]
			{
				"Fire shield",
				"Iron shield",
				"Medusa shield",
				"Alucard shield",
				"Alucard shield",
				"Cross shuriken",
				"Shield rod",
				"Buffalo star",
				"Flame star",
				"Zweihander",
				"Obsidian sword",
				"Marsil",
				"Estoc",
				"Zweihander",
				"Obsidian sword",
				"Iron Fist",
				"Elixir",
				"Gram",
				"Holy sword",
				"Dark Blade",
				"Mourneblade",
				"Osafune katana",
				"Topaz circlet",
				"Beryl circlet",
				"Fury plate",
				"Joseph's cloak",
				"Twilight cloak",
				"Library card",
				"Moonstone",
				"Turquoise",
				"Diamond",
				"Onyx",
				"Mystic pendant",
				"Gauntlet",
				"Ring of Feanor",
				"King's stone"
			};
			HeavyHelpItemRewards = new string[]
			{
				"Mablung Sword",
				"Masamune",
				"Manna prism",
				"Fist of Tulkas",
				"Gurthang",
				"Alucard sword",
				"Vorpal blade",
				"Crissaegirm",
				"Yasatsuna",
				"Dragon helm",
				"Holy glasses",
				"Spike Breaker",
				"Dark armor",
				"Dracula tunic",
				"God's Garb",
				"Ring of Ares",
				"Ring of Varda",
				"Duplicator",
				"Covenant stone",
				"Gold Ring",
				"Silver Ring"
			};
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string NamesFilePath { get; set; }
		public string BotApiKey { get; set; }
		public List<Action> Actions { get; set; }
		public float WeakenFactor { get; set; }
		public float CrippleFactor { get; set; }
		public float HasteFactor { get; set; }
		public uint ThirstDrainPerSecond { get; set; }
		public int PandoraMinItems { get; set; }
		public int PandoraMaxItems { get; set; }
		public System.TimeSpan QueueInterval { get; set; }
		public bool DynamicInterval { get; set; }
		public bool KeepVladRelics { get; set; }
		public string[] LightHelpItemRewards { get; set; }
		public string[] MediumHelpItemRewards { get; set; }
		public string[] HeavyHelpItemRewards { get; set; }
	}
}

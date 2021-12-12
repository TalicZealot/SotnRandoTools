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
			WeakenFactor = 0.5F;
			CrippleFactor = 0.8F;
			HasteFactor = 3.2F;
			ThirstDrainPerSecond = 1;
			PandoraTrigger = 1000;
			PandoraMinItems = 5;
			PandoraMaxItems = 32;
			QueueInterval = new System.TimeSpan(0, 0, 21);
			DynamicInterval = true;
			KeepVladRelics = false;
			Actions = new List<Action>
			{
				new Action{Name="Khaos Status", Enabled = true, Meter = 2, AlertPath = Paths.AlucardWhatSound, Cooldown = new System.TimeSpan(0, 0, 0)},
				new Action{Name="Khaos Equipment", Enabled = true, Meter = 7, AlertPath = Paths.AlucardWhatSound, Cooldown = new System.TimeSpan(0, 10, 0)},
				new Action{Name="Khaos Stats", Enabled = true, Meter = 8, AlertPath = Paths.AlucardWhatSound, Cooldown = new System.TimeSpan(0, 10, 0)},
				new Action{Name="Khaos Relics", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound, Cooldown = new System.TimeSpan(0, 25, 0), StartsOnCooldown = true},
				new Action{Name="Pandora's Box", Enabled = true, Meter = 15, AlertPath = Paths.AlucardWhatSound, Cooldown = new System.TimeSpan(0, 35, 0), StartsOnCooldown = true },
				new Action{Name="Gamble", Enabled = true, Meter = 2, AlertPath = Paths.LibrarianThankYouSound, Cooldown = new System.TimeSpan(0, 10, 0)},
				new Action{Name="Khaotic Burst", Enabled = true, Meter = 10, AlertPath = Paths.AlucardWhatSound, Cooldown = new System.TimeSpan(0, 20, 0)},
				new Action{Name="Bankrupt", Enabled = true, Meter = 12, AlertPath = Paths.DeathLaughSound, Cooldown = new System.TimeSpan(0, 30, 0), StartsOnCooldown = true},
				new Action{Name="Weaken", Enabled = true, Meter = 8, AlertPath = Paths.RichterLaughSound, Cooldown = new System.TimeSpan(0, 24, 0)},
				new Action{Name="Respawn Bosses", Enabled = true, Meter = 3, AlertPath = Paths.DeathLaughSound, Cooldown = new System.TimeSpan(0, 10, 0), StartsOnCooldown = true},
				new Action{Name="Subweapons Only", Enabled = true, Meter = 4, AlertPath = Paths.RichterLaughSound, Cooldown = new System.TimeSpan(0, 6, 0), Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Cripple", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 0, 30), AlertPath = Paths.DeathLaughSound, Cooldown = new System.TimeSpan(0, 5, 0)},
				new Action{Name="Blood Mana", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound, Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Thirst", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound, Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Khaos Horde", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 2, 0), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.RichterLaughSound, Cooldown = new System.TimeSpan(0, 10, 0)},
				new Action{Name="Endurance", Enabled = true, Meter = 7, AlertPath = Paths.DeathLaughSound, Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="HnK", Enabled = true, Meter = 7, AlertPath = Paths.DeathLaughSound, Duration = new System.TimeSpan(0, 1, 0), Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Vampire", Enabled = true,  Meter = 2, Duration = new System.TimeSpan(0, 1, 0), Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Light Help", Enabled = true, Meter = 2, AlertPath = Paths.FairyPotionSound, Cooldown = new System.TimeSpan(0, 0, 0)},
				new Action{Name="Medium Help", Enabled = true, Meter = 5, AlertPath = Paths.FairyPotionSound, Cooldown = new System.TimeSpan(0, 2, 0)},
				new Action{Name="Heavy Help", Enabled = true, Meter = 8, AlertPath = Paths.FairyPotionSound, Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Battle Orders", Enabled = true,  Meter = 6, Duration = new System.TimeSpan(0, 1, 0), Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Magician", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Melty Blood", Enabled = true, Meter = 5, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.MeltySound, Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Guilty Gear", Enabled = true, Meter = 5, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DragonInstallSound},
				new Action{Name="Four Beasts", Enabled = true,  Meter = 10, Duration = new System.TimeSpan(0, 1, 0), Cooldown = new System.TimeSpan(0, 15, 0), StartsOnCooldown = true},
				new Action{Name="ZA WARUDO", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 0, 30), AlertPath = Paths.ZaWarudoSound, Cooldown = new System.TimeSpan(0, 6, 0)},
				new Action{Name="Haste", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 0, 30), Cooldown = new System.TimeSpan(0, 5, 0)}
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
		public List<Action> Actions { get; set; }
		public float WeakenFactor { get; set; }
		public float CrippleFactor { get; set; }
		public float HasteFactor { get; set; }
		public uint ThirstDrainPerSecond { get; set; }
		public int PandoraTrigger { get; set; }
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

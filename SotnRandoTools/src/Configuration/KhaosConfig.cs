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
			BotActionsFilePath = Paths.BotActionsFilePath;
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

			Actions = new List<Action>
			{
				new Action{Name="Khaos Status", Enabled = true, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Equipment", Enabled = true, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Stats", Enabled = true, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Relics", Enabled = true, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Pandora's Box", Enabled = true, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Gamble", Enabled = true, AlertPath = Paths.LibrarianThankYouSound},
				new Action{Name="Bankrupt", Enabled = true, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Weaken", Enabled = true, AlertPath = Paths.RichterLaughSound},
				new Action{Name="Respawn Bosses", Enabled = true, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Honest Gamer", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Subweapons Only", Enabled = true, AlertPath = Paths.RichterLaughSound},
				new Action{Name="Cripple", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Blood Mana", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Thirst", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Khaos Horde", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.RichterLaughSound},
				new Action{Name="Endurance", Enabled = true, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Vampire", Enabled = true},
				new Action{Name="Light Help", Enabled = true, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Medium Help", Enabled = true, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Heavy Help", Enabled = true, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Battle Orders", Enabled = true},
				new Action{Name="Magician", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Melty Blood", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.MeltySound},
				new Action{Name="Four Beasts", Enabled = true},
				new Action{Name="ZA WARUDO", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.ZaWarudoSound},
				new Action{Name="Haste", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)}
			};
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string BotActionsFilePath { get; set; }
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
	}
}

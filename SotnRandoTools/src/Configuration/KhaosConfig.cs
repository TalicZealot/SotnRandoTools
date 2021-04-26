using System.Collections.Generic;
using System.Drawing;
using SotnRandoTools.Configuration.Models;

namespace SotnRandoTools.Configuration
{
	public class KhaosConfig
	{
		public KhaosConfig()
		{
			Alerts = true;
			ControlPannelQueueActions = true;
			Volume = 2;
			BotActionsFilePath = "";
			WeakenFactor = 0.5F;
			CrippleFactor = 0.8F;
			HasteFactor = 1.3F;
			ThirstDrainPerSecond = 1;
			PandoraMinItems = 3;
			PandoraMaxItems = 32;
			QueueInterval = new System.TimeSpan(0, 0, 30);

			Actions = new List<Action>
			{
				new Action{Name="Khaos Status", Enabled = true},
				new Action{Name="Khaos Equipment", Enabled = true},
				new Action{Name="Khaos Stats", Enabled = true},
				new Action{Name="Khaos Relics", Enabled = true},
				new Action{Name="Pandora's Box", Enabled = true},
				new Action{Name="Gamble", Enabled = true},
				new Action{Name="Bankrupt", Enabled = true},
				new Action{Name="Weaken", Enabled = true},
				new Action{Name="Respawn Bosses", Enabled = true},
				new Action{Name="Honest Gamer", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Subweapons Only", Enabled = true},
				new Action{Name="Cripple", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Blood Mana", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Thirst", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Khaos Horde", Enabled = true, Duration = new System.TimeSpan(0, 1, 0), Interval = new System.TimeSpan(0, 0, 1)},
				new Action{Name="Endurance", Enabled = true},
				new Action{Name="Vampire", Enabled = true},
				new Action{Name="Light Help", Enabled = true},
				new Action{Name="Medium Help", Enabled = true},
				new Action{Name="Heavy Help", Enabled = true},
				new Action{Name="Battle Orders", Enabled = true},
				new Action{Name="Magician", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Melty Blood", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Four Beasts", Enabled = true},
				new Action{Name="ZA WARUDO", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Haste", Enabled = true, Duration = new System.TimeSpan(0, 1, 0)}
			};
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string BotActionsFilePath { get; set; }
		public string NamesFilePath { get; set; }
		public List<Action> Actions { get; set; }
		public float WeakenFactor { get; set; }
		public float CrippleFactor { get; set; }
		public float HasteFactor { get; set; }
		public uint ThirstDrainPerSecond { get; set; }
		public int PandoraMinItems { get; set; }
		public int PandoraMaxItems { get; set; }
		public System.TimeSpan QueueInterval { get; set; }
	}
}

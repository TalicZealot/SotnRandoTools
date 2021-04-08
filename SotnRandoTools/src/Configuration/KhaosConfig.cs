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
			Volume = 8;
			BotActionsFilePath = "";
			Actions = new List<Action>
			{
				new Action{Name="Khaos Status", Aliases = new string[] {"kstatus"}, Enabled = true, Scaling = false},
				new Action{Name="Khaos Horde", Aliases = new string[] {"horde"}, Enabled = true, Scaling = true, Duration = new System.TimeSpan(0, 1, 0), Interval = new System.TimeSpan(0, 0, 1)}
			};
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string BotActionsFilePath { get; set; }

		public List<Action> Actions { get; set; }
	}
}

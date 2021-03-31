using System.Drawing;

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
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string BotActionsFilePath { get; set; }
	}
}

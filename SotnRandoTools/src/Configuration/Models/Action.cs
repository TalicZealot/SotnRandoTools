using System;

namespace SotnRandoTools.Configuration.Models
{
	public class Action
	{
		public Action()
		{
			this.Enabled = true;
			this.Scaling = false;
			this.ScaleFactor = 2.0f;
			this.EnablesAfter = new TimeSpan(0, 0, 0);
			this.Cooldown = new TimeSpan(0, 0, 0);
			this.Duration = new TimeSpan(0, 1, 0);
			this.Bits = 100;
			this.ChannelPoints = 1000;
			this.Money = 1.0M;
			this.Currency = "USD";
		}
		public string Name { get; set; }
		public string[] Aliases { get; set; }
		public bool Enabled { get; set; }
		public bool Scaling { get; set; }
		public float ScaleFactor { get; set; }
		public TimeSpan EnablesAfter { get; set; }
		public TimeSpan Cooldown { get; set; }
		public TimeSpan Duration { get; set; }
		public TimeSpan Interval { get; set; }
		public int Bits { get; set; }
		public int ChannelPoints { get; set; }
		public decimal Money { get; set; }
		public string Currency { get; set; }
	}
}

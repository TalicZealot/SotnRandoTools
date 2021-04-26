using System;

namespace SotnRandoTools.Configuration.Models
{
	public class Action
	{
		public Action()
		{
			this.Enabled = true;
			this.Duration = new TimeSpan(0, 1, 0);
		}
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public string AlertPath { get; set; }
		public TimeSpan Duration { get; set; }
		public TimeSpan Interval { get; set; }
	}
}

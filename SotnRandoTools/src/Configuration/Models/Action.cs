using System;

namespace SotnRandoTools.Configuration.Models
{
	public class Action
	{
		public Action()
		{
			this.Enabled = true;
		}
		public string Name { get; set; }
		public short Meter { get; set; }
		public bool Enabled { get; set; }
		public string AlertPath { get; set; }
		public TimeSpan Duration { get; set; }
		public TimeSpan Interval { get; set; }
	}
}

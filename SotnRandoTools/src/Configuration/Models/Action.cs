using System;

namespace SotnRandoTools.Configuration.Models
{
	public class Action
	{
		public Action()
		{
			this.Enabled = true;
			this.StartsOnCooldown = false;
		}
		public string Name { get; set; }
		public short Meter { get; set; }
		public bool Enabled { get; set; }
		public bool StartsOnCooldown { get; set; }
		public string AlertPath { get; set; }
		public TimeSpan Duration { get; set; }
		public TimeSpan Interval { get; set; }
		public TimeSpan Cooldown { get; set; }
		public DateTime? LastUsedAt { get; set; }

		public bool IsOnCooldown()
		{
			if (LastUsedAt is null)
			{
				return false;
			}
			else if (DateTime.Now - LastUsedAt > Cooldown)
			{
				return false;
			}

			return true;
		}
	}
}

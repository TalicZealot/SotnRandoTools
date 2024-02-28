namespace SotnRandoTools.RandoTracker.Models
{
	internal sealed class TrackerRelic
	{
		public TrackerRelic()
		{
			Collected = false;
			X = 0;
			Y = 0;
			CollectedAt = 0;
		}
		public string? Name { get; set; }
		public bool Collected { get; set; }
		public bool Progression { get; set; }
		public byte X { get; set; }
		public byte Y { get; set; }
		public ushort CollectedAt { get; set; }
	}
}

namespace SotnRandoTools.RandoTracker.Models
{
	public class Item
	{
		public Item()
		{
			Status = false;
			X = 0;
			Y = 0;
			CollectedAt = 0;
		}
		public string? Name { get; set; }
		public uint Value { get; set; }
		public bool Status { get; set; }
		public bool Collected { get; set; }
		public bool Equipped { get; set; }
		public byte X { get; set; }
		public byte Y { get; set; }
		public ushort CollectedAt { get; set; }
	}
}

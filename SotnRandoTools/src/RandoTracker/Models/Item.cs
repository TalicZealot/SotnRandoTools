namespace SotnRandoTools.RandoTracker.Models
{
	internal struct Item
	{
		public byte Value;
		public byte Index;
		public bool Status;
		public bool Collected;
		public bool Equipped;
		public byte X;
		public byte Y;
		public ushort CollectedAt;
	}
}

namespace SotnRandoTools.RandoTracker.Models
{
	internal struct Item
	{
		public ushort CollectedAt;
		public byte Value;
		public byte Index;
		public byte X;
		public byte Y;
		public bool Status;
		public bool Collected;
		public bool Equipped;
	}
}

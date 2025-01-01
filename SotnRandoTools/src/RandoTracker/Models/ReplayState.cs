namespace SotnRandoTools.RandoTracker.Models
{
	internal struct ReplayState
	{
		public ReplayState()
		{
			Time = 0;
			X = 0;
			Y = 0;
		}
		public byte X = 0;
		public byte Y = 0;
		public ushort Time = 0;
	}
}

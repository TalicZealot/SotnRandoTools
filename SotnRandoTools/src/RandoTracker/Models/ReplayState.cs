namespace SotnRandoTools.RandoTracker.Models
{
	public class ReplayState
	{
		public ReplayState()
		{
			Time = 0;
		}
		public byte X { get; set; }
		public byte Y { get; set; }
		public ushort Time { get; set; }
	}
}

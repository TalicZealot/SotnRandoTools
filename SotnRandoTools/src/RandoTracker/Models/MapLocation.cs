namespace SotnRandoTools.RandoTracker.Models
{
	public class MapLocation
	{
		public MapLocation()
		{
			Time = 0;
		}
		public int X { get; set; }
		public int Y { get; set; }
		public int Time { get; set; }
		public int SecondCastle { get; set; }
		public int Relics { get; set; }
		public int ProgressionItems { get; set; }
	}
}

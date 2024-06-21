using System.Drawing;

namespace SotnRandoTools.Configuration
{
	public class CoopConfig
	{
		public CoopConfig()
		{
			DefaultPort = 46318;
			Volume = 5;
		}

		public Point Location { get; set; }
		public int DefaultPort { get; set; }
		public int Volume { get; set; }
	}
}

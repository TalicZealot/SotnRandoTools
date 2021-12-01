namespace SotnRandoTools.RandoTracker.Models
{
	public class Relic
	{
		public Relic()
		{
			Collected = false;
		}
		public string? Name { get; set; }
		public bool Collected { get; set; }
		public bool Progression { get; set; }
	}
}

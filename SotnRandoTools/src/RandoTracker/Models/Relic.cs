namespace SotnRandoTools.RandoTracker.Models
{
	public class Relic
	{
		public Relic()
		{
			Status = false;
		}
		public string? Name { get; set; }
		public bool Status { get; set; }
		public bool Progression { get; set; }
	}
}

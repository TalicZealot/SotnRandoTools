namespace SotnRandoTools.RandoTracker.Models
{
	public class Item
	{
		public Item()
		{
			Status = false;
		}
		public string? Name { get; set; }
		public uint Value { get; set; }
		public bool Status { get; set; }
	}
}

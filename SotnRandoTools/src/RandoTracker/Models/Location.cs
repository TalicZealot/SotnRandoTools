using System.Collections.Generic;

namespace SotnRandoTools.RandoTracker.Models
{
	public class Location
	{
		public Location()
		{
			Locks = new List<string[]>();
			OutOfLogicLocks = new List<string[]>();
			AvailabilityColor = MapColor.Unavailable;
			SecondCastle = false;
			GuardedExtension = false;
			EquipmentExtension = false;
			Status = false;
		}
		public string? Name { get; set; }
		public bool SecondCastle { get; set; }
		public bool GuardedExtension { get; set; }
		public bool EquipmentExtension { get; set; }
		public MapColor AvailabilityColor { get; set; }
		public bool Status { get; set; }
		public int MapRow { get; set; }
		public int MapCol { get; set; }
		public List<string[]>? Locks { get; set; }
		public List<string[]>? OutOfLogicLocks { get; set; }
		public List<Room>? Rooms { get; set; }
	}
}
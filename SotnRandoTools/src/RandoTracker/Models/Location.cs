using System.Collections.Generic;

namespace SotnRandoTools.RandoTracker.Models
{
	internal sealed class Location
	{
		public Location()
		{
			Locks = new List<string[]>();
			OutOfLogicLocks = new List<string[]>();
			Rooms = new List<Room>();
			WatchIndecies = new List<int>();
			AvailabilityColor = MapColor.Unavailable;
			SecondCastle = false;
			ClassicExtension = false;
			GuardedExtension = false;
			EquipmentExtension = false;
			SpreadExtension = false;
			CustomExtension = false;
			Visited = false;
		}
		public string Name { get; set; }
		public bool SecondCastle { get; set; }
		public bool ClassicExtension { get; set; }
		public bool GuardedExtension { get; set; }
		public bool EquipmentExtension { get; set; }
		public bool SpreadExtension { get; set; }
		public bool CustomExtension { get; set; }
		public MapColor AvailabilityColor { get; set; }
		public bool Visited { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public List<string[]>? Locks { get; set; }
		public List<string[]>? OutOfLogicLocks { get; set; }
		public List<int> WatchIndecies { get; set; }
		public List<Room>? Rooms { get; set; }
	}
}
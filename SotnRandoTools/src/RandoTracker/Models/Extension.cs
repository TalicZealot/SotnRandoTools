using System.Collections.Generic;
using Newtonsoft.Json;

namespace SotnRandoTools.RandoTracker.Models
{
	internal sealed class ExtensionRoom
	{
		[JsonProperty("address")]
		public long Address { get; set; }
		[JsonProperty("values")]
		public int[]? Values { get; set; }
	}
	internal sealed class ExtensionLocation
	{
		[JsonProperty("location")]
		public string Name { get; set; }
		[JsonProperty("x")]
		public int X { get; set; }
		[JsonProperty("y")]
		public int Y { get; set; }
		[JsonProperty("secondCastle")]
		public bool SecondCastle { get; set; }
		[JsonProperty("rooms")]
		public List<ExtensionRoom> Rooms { get; set; } = new List<ExtensionRoom>();
	}
	internal sealed class Extension
	{
		[JsonProperty("extends")]
		public string Extends { get; set; } = "classic";
		[JsonProperty("locations")]
		public List<ExtensionLocation> Locations { get; set; } = new List<ExtensionLocation>();
	}
}

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SotnRandoTools.RandoTracker.Models
{
	internal sealed class ExtensionRoom
	{
		[JsonProperty("address")]
		public string Address { get; set; }
		[JsonProperty("values")]
		public string[] Values { get; set; }
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
		[JsonProperty("customLocations")]
		public List<ExtensionLocation> Locations { get; set; } = new List<ExtensionLocation>();
	}
}

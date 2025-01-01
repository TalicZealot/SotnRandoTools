using System.Collections.Generic;
using Newtonsoft.Json;

namespace SotnRandoTools.RandoTracker.Models
{
	internal sealed class PresetMetadata
	{
		[JsonProperty("id")]
		public string Id { get; set; }
	}
	internal sealed class LockLocation
	{
		[JsonProperty("location")]
		public string Name { get; set; }
		[JsonProperty("locks")]
		public List<string> Locks { get; set; } = new List<string>();
	}
	internal sealed class Alias
	{
		[JsonProperty("relic")]
		public string Relic { get; set; }
		[JsonProperty("alias")]
		public string Replaced { get; set; }
	}

	internal sealed class Preset
	{
		[JsonProperty("metadata")]
		public PresetMetadata Metadata { get; set; }
		[JsonProperty("inherits")]
		public string Inherits { get; set; }
		[JsonProperty("relicLocationsExtension")]
		public string RelicLocationsExtension { get; set; }
		[JsonProperty("alias")]
		public List<Alias> Aliases { get; set; } = new List<Alias>();
		[JsonProperty("lockLocation")]
		public List<LockLocation> LockLocations { get; set; } = new List<LockLocation>();
		[JsonProperty("lockLocationAllowed")]
		public List<LockLocation> LockLocationsAllowed { get; set; } = new List<LockLocation>();
	}
}

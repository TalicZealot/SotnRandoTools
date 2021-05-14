using System;

namespace SotnRandoTools.Coop.Enums
{
	[Flags]
	public enum SettingsFlags
	{
		ShareRelics = 0b_0000_0001,
		ShareWarps = 0b_0000_0010,
		SendItems = 0b_0000_0100,
		SendAssists = 0b_0000_1000,
		ShareLocations = 0b_0001_0000
	}
}

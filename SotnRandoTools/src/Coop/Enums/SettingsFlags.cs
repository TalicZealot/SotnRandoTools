using System;

namespace SotnRandoTools.Coop.Enums
{
	[Flags]
	public enum SettingsFlags
	{
		SendRelics = 0b_0000_0001,
		ShareWarps = 0b_0000_0010,
		ShareShortcuts = 0b_0000_0100,
		SendItems = 0b_0000_1000,
		SendAssists = 0b_0001_0000,
		ShareLocations = 0b_0010_0000
	}
}

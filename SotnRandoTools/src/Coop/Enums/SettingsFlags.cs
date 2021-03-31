using System;

namespace SotnRandoTools.Coop.Enums
{
	[Flags]
	public enum SettingsFlags
	{
		SendRelics		 = 0b_0000_0001,  // 1
		ShareWarps		 = 0b_0000_0010,  // 2
		ShareShortcuts	 = 0b_0000_0100,  // 4
		SendItems		 = 0b_0000_1000,  // 8
		SendAssists		 = 0b_0001_0000   // 16
	}
}

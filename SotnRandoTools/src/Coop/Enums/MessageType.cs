using System;

namespace SotnRandoTools.Coop.Enums
{
	[Flags]
	public enum MessageType : byte
	{
		Relic,
		Item,
		Effect,
		WarpFirstCastle,
		WarpSecondCastle,
		Shortcut,
		Settings,
		Location
	}
}

using System;

namespace SotnRandoTools.Coop.Enums
{
	[Flags]
	public enum MessageType : byte
	{
		Ping,
		Pong,
		Relic,
		Item,
		WarpFirstCastle,
		WarpSecondCastle,
		Shortcut,
		SynchRequest,
		SynchAll,
		Location
	}
}

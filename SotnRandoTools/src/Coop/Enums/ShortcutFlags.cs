namespace SotnRandoTools.Coop.Enums
{
	public enum ShortcutFlags
	{
		OuterWallElevator = 0b_0000_0000_0000_0001,
		AlchemyElevator = 0b_0000_0000_0000_0010,
		EntranceToMarble = 0b_0000_0000_0000_0100,
		ChapelStatue = 0b_0000_0000_0000_1000,
		ColosseumElevator = 0b_0000_0000_0001_0000,
		ColosseumToChapel = 0b_0000_0000_0010_0000,
		MarbleBlueDoor = 0b_0000_0000_0100_0000,
		CavernsSwitchAndBridge = 0b_0000_0000_1000_0000,
		EntranceToCaverns = 0b_0000_0001_0000_0000,
		EntranceWarp = 0b_0000_0010_0000_0000,
		FirstClockRoomDoor = 0b_0000_0100_0000_0000,
		SecondClockRoomDoor = 0b_0000_1000_0000_0000,
		FirstDemonButton = 0b_0001_0000_0000_0000,
		SecondDemonButton = 0b_0010_0000_0000_0000,
		KeepStairs = 0b_0100_0000_0000_0000
	}
}

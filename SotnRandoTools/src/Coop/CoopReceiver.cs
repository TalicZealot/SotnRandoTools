using System;
using System.Collections.Concurrent;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools.Coop
{
	internal sealed class CoopReceiver : ICoopReceiver
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;
		private readonly ICoopController coopController;

		public CoopReceiver(IToolConfig toolConfig, ISotnApi sotnApi, INotificationService notificationService, ICoopController coopController)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));
			this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
			this.coopController = coopController ?? throw new ArgumentNullException(nameof(coopController));
			MessageQueue = new();
		}

		public ConcurrentQueue<byte[]> MessageQueue { get; set; }

		private void ProcessMessage(byte[] data)
		{
			MessageType type = (MessageType) data[0];
			ushort index = BitConverter.ToUInt16(data, 1);
			ushort index2 = BitConverter.ToUInt16(data, 3);
			byte indexByte = data[1];
			byte dataByte = 0;
			if (data.Length > 2)
			{
				dataByte = data[2];
			}
			switch (type)
			{
				case MessageType.Relic:
					if (!sotnApi.AlucardApi.HasRelic((Relic) indexByte))
					{
						sotnApi.AlucardApi.GrantRelic((Relic) indexByte);
						coopController.CoopState.relics[indexByte].status = true;
						notificationService.AddMessage(SotnApi.Constants.Values.Alucard.Equipment.Relics[indexByte]);
						notificationService.PlayAlert();
					}
					break;
				case MessageType.Location:
					sotnApi.GameApi.SetRoomToVisited(SotnApi.Constants.Addresses.Game.MapStart + index);
					coopController.CoopState.locations[index2].status = true;
					sotnApi.AlucardApi.Rooms++;
					//Console.WriteLine($"Received location: {index}");
					break;
				case MessageType.Item:
					sotnApi.AlucardApi.GrantItemByName(Equipment.Items[index]);
					notificationService.AddMessage(Equipment.Items[index]);
					notificationService.PlayAlert();
					//Console.WriteLine($"Received item: {Equipment.Items[index]}");
					break;
				case MessageType.WarpFirstCastle:
					sotnApi.AlucardApi.WarpsFirstCastle |= dataByte;
					coopController.CoopState.WarpsFirstCastle.value = (byte) sotnApi.AlucardApi.WarpsFirstCastle;
					notificationService.AddMessage($"Received warp: {(Warp) index}");
					//Console.WriteLine($"Received warp: {(Warp) index}");
					break;
				case MessageType.WarpSecondCastle:
					sotnApi.AlucardApi.WarpsSecondCastle |= dataByte;
					coopController.CoopState.WarpsSecondCastle.value = (byte) sotnApi.AlucardApi.WarpsSecondCastle;
					notificationService.AddMessage($"Received warp: Inverted {(Warp) index}");
					//Console.WriteLine($"Received warp: Inverted {(Warp) index}");
					break;
				case MessageType.Shortcut:
					if (index > Enum.GetNames(typeof(Shortcut)).Length - 1)
					{
						DecodeShortcuts(index);
					}
					else
					{
						DecodeShortcut((Shortcut) index);
					}
					coopController.CoopState.shortcuts[index].status = true;
					break;
				case MessageType.SynchRequest:
					coopController.SynchRequested = true;
					//Console.WriteLine($"Sending Synch");
					break;
				case MessageType.SynchAll:
					DecodeSynch(data);
					//Console.WriteLine($"Received relics, warps and shortcuts");
					break;
				default:
					break;
			}
		}

		public void Update()
		{
			//avoid marble gallery softlock
			bool insideMarbleGalleryDoorRooms = (sotnApi.GameApi.Room == Various.MarbleGalleryDoorToCavernsRoom || sotnApi.GameApi.Room == Various.MarbleGalleryBlueDoorRoom) && (sotnApi.GameApi.Area == Various.MarbleGalleryArea);
			if (!sotnApi.GameApi.InAlucardMode() || insideMarbleGalleryDoorRooms)
			{
				return;
			}
			if (!MessageQueue.IsEmpty && MessageQueue.TryDequeue(out byte[] data))
			{
				ProcessMessage(data);
			}
		}

		private void DecodeShortcut(Shortcut shortcut)
		{
			switch (shortcut)
			{
				case Shortcut.OuterWallElevator:
					sotnApi.AlucardApi.OuterWallElevator = true;
					break;
				case Shortcut.AlchemyElevator:
					sotnApi.AlucardApi.AlchemyElevator = true;
					break;
				case Shortcut.EntranceToMarble:
					sotnApi.AlucardApi.EntranceToMarble = true;
					break;
				case Shortcut.ChapelStatue:
					sotnApi.AlucardApi.ChapelStatue = true;
					break;
				case Shortcut.ColosseumElevator:
					sotnApi.AlucardApi.ColosseumElevator = true;
					break;
				case Shortcut.ColosseumToChapel:
					sotnApi.AlucardApi.ColosseumToChapel = true;
					break;
				case Shortcut.MarbleBlueDoor:
					sotnApi.AlucardApi.MarbleBlueDoor = true;
					break;
				case Shortcut.CavernsSwitchAndBridge:
					sotnApi.AlucardApi.CavernsSwitchAndBridge = true;
					break;
				case Shortcut.EntranceToCaverns:
					sotnApi.AlucardApi.EntranceToCaverns = true;
					break;
				case Shortcut.EntranceWarp:
					sotnApi.AlucardApi.EntranceWarp = true;
					break;
				case Shortcut.FirstClockRoomDoor:
					sotnApi.AlucardApi.FirstClockRoomDoor = true;
					break;
				case Shortcut.SecondClockRoomDoor:
					sotnApi.AlucardApi.SecondClockRoomDoor = true;
					break;
				case Shortcut.FirstDemonButton:
					sotnApi.AlucardApi.FirstDemonButton = true;
					break;
				case Shortcut.SecondDemonButton:
					sotnApi.AlucardApi.SecondDemonButton = true;
					break;
				case Shortcut.KeepStairs:
					sotnApi.AlucardApi.KeepStairs = true;
					break;
				default:
					//Console.WriteLine($"Shortcut {shortcut} not found!");
					return;
			}
			notificationService.AddMessage(Constants.CoOp.ShortcutNames[(int) shortcut]);
			//Console.WriteLine($"Received shortcut: {shortcut}");
		}

		private void DecodeShortcuts(int flags)
		{
			if ((flags & (int) ShortcutFlags.OuterWallElevator) == (int) ShortcutFlags.OuterWallElevator)
			{
				sotnApi.AlucardApi.OuterWallElevator = true;
				coopController.CoopState.shortcuts[0].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.OuterWallElevator}");
			}
			if ((flags & (int) ShortcutFlags.AlchemyElevator) == (int) ShortcutFlags.AlchemyElevator)
			{
				sotnApi.AlucardApi.AlchemyElevator = true;
				coopController.CoopState.shortcuts[1].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.AlchemyElevator}");
			}
			if ((flags & (int) ShortcutFlags.EntranceToMarble) == (int) ShortcutFlags.EntranceToMarble)
			{
				sotnApi.AlucardApi.EntranceToMarble = true;
				coopController.CoopState.shortcuts[2].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceToMarble}");
			}
			if ((flags & (int) ShortcutFlags.ChapelStatue) == (int) ShortcutFlags.ChapelStatue)
			{
				sotnApi.AlucardApi.ChapelStatue = true;
				coopController.CoopState.shortcuts[3].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.ChapelStatue}");
			}
			if ((flags & (int) ShortcutFlags.ColosseumElevator) == (int) ShortcutFlags.ColosseumElevator)
			{
				sotnApi.AlucardApi.ColosseumElevator = true;
				coopController.CoopState.shortcuts[4].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.ColosseumElevator}");
			}
			if ((flags & (int) ShortcutFlags.ColosseumToChapel) == (int) ShortcutFlags.ColosseumToChapel)
			{
				sotnApi.AlucardApi.ColosseumToChapel = true;
				coopController.CoopState.shortcuts[5].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.ColosseumToChapel}");
			}
			if ((flags & (int) ShortcutFlags.MarbleBlueDoor) == (int) ShortcutFlags.MarbleBlueDoor)
			{
				sotnApi.AlucardApi.MarbleBlueDoor = true;
				coopController.CoopState.shortcuts[6].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.MarbleBlueDoor}");
			}
			if ((flags & (int) ShortcutFlags.CavernsSwitchAndBridge) == (int) ShortcutFlags.CavernsSwitchAndBridge)
			{
				sotnApi.AlucardApi.CavernsSwitchAndBridge = true;
				coopController.CoopState.shortcuts[7].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.CavernsSwitchAndBridge}");
			}
			if ((flags & (int) ShortcutFlags.EntranceToCaverns) == (int) ShortcutFlags.EntranceToCaverns)
			{
				sotnApi.AlucardApi.EntranceToCaverns = true;
				coopController.CoopState.shortcuts[8].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceToCaverns}");
			}
			if ((flags & (int) ShortcutFlags.EntranceWarp) == (int) ShortcutFlags.EntranceWarp)
			{
				sotnApi.AlucardApi.EntranceWarp = true;
				coopController.CoopState.shortcuts[9].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceWarp}");
			}
			if ((flags & (int) ShortcutFlags.FirstClockRoomDoor) == (int) ShortcutFlags.FirstClockRoomDoor)
			{
				sotnApi.AlucardApi.FirstClockRoomDoor = true;
				coopController.CoopState.shortcuts[10].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.FirstClockRoomDoor}");
			}
			if ((flags & (int) ShortcutFlags.SecondClockRoomDoor) == (int) ShortcutFlags.SecondClockRoomDoor)
			{
				sotnApi.AlucardApi.SecondClockRoomDoor = true;
				coopController.CoopState.shortcuts[11].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.SecondClockRoomDoor}");
			}
			if ((flags & (int) ShortcutFlags.FirstDemonButton) == (int) ShortcutFlags.FirstDemonButton)
			{
				sotnApi.AlucardApi.FirstDemonButton = true;
				coopController.CoopState.shortcuts[12].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.FirstDemonButton}");
			}
			if ((flags & (int) ShortcutFlags.SecondDemonButton) == (int) ShortcutFlags.SecondDemonButton)
			{
				sotnApi.AlucardApi.SecondDemonButton = true;
				coopController.CoopState.shortcuts[13].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.SecondDemonButton}");
			}
			if ((flags & (int) ShortcutFlags.KeepStairs) == (int) ShortcutFlags.KeepStairs)
			{
				sotnApi.AlucardApi.KeepStairs = true;
				coopController.CoopState.shortcuts[14].status = true;
				//Console.WriteLine($"Received shortcut: {ShortcutFlags.KeepStairs}");
			}
		}

		private void DecodeSynch(byte[] data)
		{
			sotnApi.AlucardApi.WarpsFirstCastle |= data[1];
			coopController.CoopState.WarpsFirstCastle.value = (byte) sotnApi.AlucardApi.WarpsFirstCastle;
			sotnApi.AlucardApi.WarpsSecondCastle |= data[2];
			coopController.CoopState.WarpsSecondCastle.value = (byte) sotnApi.AlucardApi.WarpsSecondCastle;
			ushort shortcut = BitConverter.ToUInt16(data, 3);
			DecodeShortcuts(shortcut);
			int encodedRelics = BitConverter.ToInt32(data, 5);

			int relicCount = Enum.GetValues(typeof(Relic)).Length;
			for (int i = 0; i < relicCount; i++)
			{
				int flag = (int) Math.Pow(2, i);
				if ((encodedRelics & flag) == flag)
				{
					sotnApi.AlucardApi.GrantRelic((Relic) i);
					coopController.CoopState.relics[i].status = true;
				}
			}
		}
	}
}

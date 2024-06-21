using System;
using System.Collections.Concurrent;
using System.Reflection;
using BizHawk.Emulation.Common;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
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
		private readonly IWatchlistService watchlistService;
		private readonly ICoopController coopController;

		public CoopReceiver(IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, INotificationService notificationService, ICoopController coopController)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));
			this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
			this.watchlistService = watchlistService ?? throw new ArgumentNullException(nameof(watchlistService));
			this.coopController = coopController ?? throw new ArgumentNullException(nameof(coopController));
			MessageQueue = new();
		}

		public ConcurrentQueue<byte[]> MessageQueue { get; set; }

		private void ProcessMessage(byte[] data)
		{
			MessageType type = (MessageType) data[0];
			ushort index = BitConverter.ToUInt16(data, 1);
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
						watchlistService.UpdateWatchlist(watchlistService.CoopRelicWatches);
						watchlistService.CoopRelicWatches.ClearChangeCounts();
						notificationService.AddMessage(SotnApi.Constants.Values.Alucard.Equipment.Relics[indexByte]);
						notificationService.PlayAlert(Paths.ItemPickupSound);
					}
					break;
				case MessageType.Location:
					sotnApi.GameApi.SetRoomValue(watchlistService.CoopLocationWatches[indexByte].Address, dataByte);
					sotnApi.AlucardApi.Rooms++;
					watchlistService.CoopLocationValues[indexByte] |= dataByte;
					Console.WriteLine($"Received location: {watchlistService.CoopLocationWatches[indexByte].Notes} with value {dataByte}");
					break;
				case MessageType.Item:
					sotnApi.AlucardApi.GrantItemByName(Equipment.Items[index]);
					notificationService.AddMessage(Equipment.Items[index]);
					notificationService.PlayAlert(Paths.ItemPickupSound);
					Console.WriteLine($"Received item: {Equipment.Items[index]}");
					break;
				case MessageType.WarpFirstCastle:
					if (indexByte == 0)
					{
						sotnApi.AlucardApi.WarpsFirstCastle |= dataByte;
						watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
						watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
						notificationService.AddMessage($"Received warps and shortcuts.");
						Console.WriteLine($"Received first castle warps with value {dataByte}");
						break;
					}
					sotnApi.AlucardApi.GrantFirstCastleWarp((Warp) index);
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					notificationService.AddMessage($"Received warp: {(Warp) index}");
					Console.WriteLine($"Received warp: {(Warp) index}");
					break;
				case MessageType.WarpSecondCastle:
					if (indexByte == 0)
					{
						sotnApi.AlucardApi.WarpsSecondCastle |= dataByte;
						watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
						watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
						Console.WriteLine($"Received second castle warps with value {dataByte}");
						break;
					}
					sotnApi.AlucardApi.GrantSecondCastleWarp((Warp) index);
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					notificationService.AddMessage($"Received warp: Inverted {(Warp) index}");
					Console.WriteLine($"Received warp: Inverted {(Warp) index}");
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
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					break;
				case MessageType.SynchRequest:
					SendSynchAll();
					Console.WriteLine($"Sending Synch");
					break;
				case MessageType.SynchAll:
					DecodeSynch(data);
					Console.WriteLine($"Received relics, warps and shortcuts");
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
					Console.WriteLine($"Shortcut {shortcut} not found!");
					return;
			}
			notificationService.AddMessage(Constants.CoOp.ShortcutNames[(int) shortcut]);
			Console.WriteLine($"Received shortcut: {shortcut}");
		}

		private void DecodeShortcuts(int flags)
		{
			if ((flags & (int) ShortcutFlags.OuterWallElevator) == (int) ShortcutFlags.OuterWallElevator)
			{
				sotnApi.AlucardApi.OuterWallElevator = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.OuterWallElevator}");
			}
			if ((flags & (int) ShortcutFlags.AlchemyElevator) == (int) ShortcutFlags.AlchemyElevator)
			{
				sotnApi.AlucardApi.AlchemyElevator = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.AlchemyElevator}");
			}
			if ((flags & (int) ShortcutFlags.EntranceToMarble) == (int) ShortcutFlags.EntranceToMarble)
			{
				sotnApi.AlucardApi.EntranceToMarble = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceToMarble}");
			}
			if ((flags & (int) ShortcutFlags.ChapelStatue) == (int) ShortcutFlags.ChapelStatue)
			{
				sotnApi.AlucardApi.ChapelStatue = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.ChapelStatue}");
			}
			if ((flags & (int) ShortcutFlags.ColosseumElevator) == (int) ShortcutFlags.ColosseumElevator)
			{
				sotnApi.AlucardApi.ColosseumElevator = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.ColosseumElevator}");
			}
			if ((flags & (int) ShortcutFlags.ColosseumToChapel) == (int) ShortcutFlags.ColosseumToChapel)
			{
				sotnApi.AlucardApi.ColosseumToChapel = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.ColosseumToChapel}");
			}
			if ((flags & (int) ShortcutFlags.MarbleBlueDoor) == (int) ShortcutFlags.MarbleBlueDoor)
			{
				sotnApi.AlucardApi.MarbleBlueDoor = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.MarbleBlueDoor}");
			}
			if ((flags & (int) ShortcutFlags.CavernsSwitchAndBridge) == (int) ShortcutFlags.CavernsSwitchAndBridge)
			{
				sotnApi.AlucardApi.CavernsSwitchAndBridge = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.CavernsSwitchAndBridge}");
			}
			if ((flags & (int) ShortcutFlags.EntranceToCaverns) == (int) ShortcutFlags.EntranceToCaverns)
			{
				sotnApi.AlucardApi.EntranceToCaverns = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceToCaverns}");
			}
			if ((flags & (int) ShortcutFlags.EntranceWarp) == (int) ShortcutFlags.EntranceWarp)
			{
				sotnApi.AlucardApi.EntranceWarp = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceWarp}");
			}
			if ((flags & (int) ShortcutFlags.FirstClockRoomDoor) == (int) ShortcutFlags.FirstClockRoomDoor)
			{
				sotnApi.AlucardApi.FirstClockRoomDoor = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.FirstClockRoomDoor}");
			}
			if ((flags & (int) ShortcutFlags.SecondClockRoomDoor) == (int) ShortcutFlags.SecondClockRoomDoor)
			{
				sotnApi.AlucardApi.SecondClockRoomDoor = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.SecondClockRoomDoor}");
			}
			if ((flags & (int) ShortcutFlags.FirstDemonButton) == (int) ShortcutFlags.FirstDemonButton)
			{
				sotnApi.AlucardApi.FirstDemonButton = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.FirstDemonButton}");
			}
			if ((flags & (int) ShortcutFlags.SecondDemonButton) == (int) ShortcutFlags.SecondDemonButton)
			{
				sotnApi.AlucardApi.SecondDemonButton = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.SecondDemonButton}");
			}
			if ((flags & (int) ShortcutFlags.KeepStairs) == (int) ShortcutFlags.KeepStairs)
			{
				sotnApi.AlucardApi.KeepStairs = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.KeepStairs}");
			}
		}

		private void SendSynchAll()
		{
			watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
			watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
			byte[] data = new byte[9];
			data[0] = (byte) MessageType.SynchAll;
			data[1] = (byte) watchlistService.WarpsAndShortcutsWatches[0].Value;
			data[2] = (byte) watchlistService.WarpsAndShortcutsWatches[1].Value;
			int shortcuts = 0;
			for (int i = 2; i < watchlistService.WarpsAndShortcutsWatches.Count; i++)
			{
				if (watchlistService.WarpsAndShortcutsWatches[i].Value > 0)
				{
					shortcuts = shortcuts | (int) (ShortcutFlags) Enum.Parse(typeof(ShortcutFlags), watchlistService.WarpsAndShortcutsWatches[i].Notes);
				}
			}
			byte[] shortcutBytes = BitConverter.GetBytes((ushort) shortcuts);
			data[3] = shortcutBytes[0];
			data[4] = shortcutBytes[1];


			int relicsNumber = 0;
			for (int i = 0; i < watchlistService.RelicWatches.Count; i++)
			{
				if (watchlistService.RelicWatches[i].Value > 0)
				{
					relicsNumber |= (int) Math.Pow(2, i);
				}
			}
			byte[] relicsBytes = BitConverter.GetBytes(relicsNumber);
			data[5] = relicsBytes[0];
			data[6] = relicsBytes[1];
			data[7] = relicsBytes[2];
			data[8] = relicsBytes[3];

			coopController.SendData(data);
		}

		private void DecodeSynch(byte[] data)
		{
			sotnApi.AlucardApi.WarpsFirstCastle |= data[1];
			sotnApi.AlucardApi.WarpsSecondCastle |= data[2];
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
				}
			}
			watchlistService.CoopRelicWatches.ClearChangeCounts();
			watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
			watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
		}
	}
}

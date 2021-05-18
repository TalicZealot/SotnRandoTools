using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Services;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace SotnRandoTools.Coop
{
	public class CoopReceiver : ICoopReceiver
	{
		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly INotificationService notificationService;
		private readonly IWatchlistService watchlistService;
		private Queue<MethodInvoker> queuedMessages = new();
		private System.Timers.Timer messageTimer = new();

		public CoopReceiver(IToolConfig toolConfig, IGameApi gameApi, IAlucardApi alucardApi, INotificationService notificationService, IWatchlistService watchlistService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			this.toolConfig = toolConfig;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.notificationService = notificationService;
			this.watchlistService = watchlistService;

			messageTimer.Interval = 1 * 100;
			messageTimer.Start();
		}

		public void EnqueMessage(byte[] data)
		{
			queuedMessages.Enqueue(new MethodInvoker(() => ProcessMessage(data)));
		}

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
					if (!alucardApi.HasRelic((Relic) index))
					{
						alucardApi.GrantRelic((Relic) index);
						watchlistService.UpdateWatchlist(watchlistService.CoopRelicWatches);
						watchlistService.CoopRelicWatches.ClearChangeCounts();
						notificationService.AddMessage(((Relic) index).ToString());
						notificationService.PlayAlert(Paths.ItemPickupSound);
						Console.WriteLine($"Received relic: {(Relic) index}");
					}
					break;
				case MessageType.Location:
					gameApi.SetRoomValue(watchlistService.CoopLocationWatches[indexByte].Address, dataByte);
					watchlistService.CoopLocationValues[indexByte] = dataByte;
					Console.WriteLine($"Received location: {watchlistService.CoopLocationWatches[indexByte].Notes} with value {dataByte}");
					break;
				case MessageType.Item:
					alucardApi.GrantItemByName(Equipment.Items[index]);
					notificationService.AddMessage(Equipment.Items[index]);
					notificationService.PlayAlert(Paths.ItemPickupSound);
					Console.WriteLine($"Received item: {Equipment.Items[index]}");
					break;
				case MessageType.Effect:
					DecodeAssist(Equipment.Items[index]);
					break;
				case MessageType.WarpFirstCastle:
					if (indexByte == 0)
					{
						alucardApi.WarpsFirstCastle = dataByte;
						watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
						watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
						notificationService.AddMessage($"Received warps and shortcuts.");
						Console.WriteLine($"Received first castle warps with value {dataByte}");
						break;
					}
					alucardApi.GrantFirstCastleWarp((Warp) index);
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					notificationService.AddMessage($"Received warp: {(Warp) index}");
					Console.WriteLine($"Received warp: {(Warp) index}");
					break;
				case MessageType.WarpSecondCastle:
					if (indexByte == 0)
					{
						alucardApi.WarpsSecondCastle = dataByte;
						watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
						watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
						Console.WriteLine($"Received second castle warps with value {dataByte}");
						break;
					}
					alucardApi.GrantSecondCastleWarp((Warp) index);
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
				case MessageType.Settings:
					DecodeSettings((int) index);
					Console.WriteLine($"Received co-op settings from host with value {index}.");
					break;
				default:
					break;
			}
		}

		public void ExecuteMessage()
		{
			//avoid marble gallery softlock
			bool insideMarbleGalleryDoorRooms = (gameApi.Room == Various.MarbleGalleryDoorToCavernsRoom || gameApi.Room == Various.MarbleGalleryBlueDoorRoom) && (gameApi.Area == Various.MarbleGalleryArea);
			if (gameApi.InAlucardMode() && !insideMarbleGalleryDoorRooms)
			{
				if (queuedMessages.Count > 0)
				{
					queuedMessages.Dequeue()();
				}
			}
		}

		private void DecodeSettings(int settings)
		{
			if ((settings & (int) SettingsFlags.ShareRelics) == (int) SettingsFlags.ShareRelics)
			{
				toolConfig.Coop.ConnectionShareRelics = true;
			}
			else
			{
				toolConfig.Coop.ConnectionShareRelics = false;
			}
			if ((settings & (int) SettingsFlags.ShareWarps) == (int) SettingsFlags.ShareWarps)
			{
				toolConfig.Coop.ConnectionShareWarps = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionShareWarps = false;
			}
			if ((settings & (int) SettingsFlags.SendItems) == (int) SettingsFlags.SendItems)
			{
				toolConfig.Coop.ConnectionSendItems = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionSendItems = false;
			}
			if ((settings & (int) SettingsFlags.SendAssists) == (int) SettingsFlags.SendAssists)
			{
				toolConfig.Coop.ConnectionSendAssists = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionSendAssists = false;
			}
			if ((settings & (int) SettingsFlags.ShareLocations) == (int) SettingsFlags.ShareLocations)
			{
				toolConfig.Coop.ConnectionShareLocations = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionShareLocations = false;
			}
		}

		private void DecodeShortcut(Shortcut shortcut)
		{
			switch (shortcut)
			{
				case Shortcut.OuterWallElevator:
					alucardApi.OuterWallElevator = true;
					break;
				case Shortcut.AlchemyElevator:
					alucardApi.AlchemyElevator = true;
					break;
				case Shortcut.EntranceToMarble:
					alucardApi.EntranceToMarble = true;
					break;
				case Shortcut.ChapelStatue:
					alucardApi.ChapelStatue = true;
					break;
				case Shortcut.ColosseumElevator:
					alucardApi.ColosseumElevator = true;
					break;
				case Shortcut.ColosseumToChapel:
					alucardApi.ColosseumToChapel = true;
					break;
				case Shortcut.MarbleBlueDoor:
					alucardApi.MarbleBlueDoor = true;
					break;
				case Shortcut.CavernsSwitchAndBridge:
					alucardApi.CavernsSwitchAndBridge = true;
					break;
				case Shortcut.EntranceToCaverns:
					alucardApi.EntranceToCaverns = true;
					break;
				case Shortcut.EntranceWarp:
					alucardApi.EntranceWarp = true;
					break;
				case Shortcut.FirstClockRoomDoor:
					alucardApi.FirstClockRoomDoor = true;
					break;
				case Shortcut.SecondClockRoomDoor:
					alucardApi.SecondClockRoomDoor = true;
					break;
				case Shortcut.FirstDemonButton:
					alucardApi.FirstDemonButton = true;
					break;
				case Shortcut.SecondDemonButton:
					alucardApi.SecondDemonButton = true;
					break;
				case Shortcut.KeepStairs:
					alucardApi.KeepStairs = true;
					break;
				default:
					Console.WriteLine($"Shortcut {shortcut} not found!");
					return;
			}

			notificationService.AddMessage(shortcut.ToString());
			Console.WriteLine($"Received shortcut: {shortcut}");
		}

		private void DecodeShortcuts(int flags)
		{
			if ((flags & (int) ShortcutFlags.OuterWallElevator) == (int) ShortcutFlags.OuterWallElevator)
			{
				alucardApi.OuterWallElevator = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.OuterWallElevator}");
			}
			if ((flags & (int) ShortcutFlags.AlchemyElevator) == (int) ShortcutFlags.AlchemyElevator)
			{
				alucardApi.AlchemyElevator = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.AlchemyElevator}");
			}
			if ((flags & (int) ShortcutFlags.EntranceToMarble) == (int) ShortcutFlags.EntranceToMarble)
			{
				alucardApi.EntranceToMarble = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceToMarble}");
			}
			if ((flags & (int) ShortcutFlags.ChapelStatue) == (int) ShortcutFlags.ChapelStatue)
			{
				alucardApi.ChapelStatue = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.ChapelStatue}");
			}
			if ((flags & (int) ShortcutFlags.ColosseumElevator) == (int) ShortcutFlags.ColosseumElevator)
			{
				alucardApi.ColosseumElevator = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.ColosseumElevator}");
			}
			if ((flags & (int) ShortcutFlags.ColosseumToChapel) == (int) ShortcutFlags.ColosseumToChapel)
			{
				alucardApi.ColosseumToChapel = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.ColosseumToChapel}");
			}
			if ((flags & (int) ShortcutFlags.MarbleBlueDoor) == (int) ShortcutFlags.MarbleBlueDoor)
			{
				alucardApi.MarbleBlueDoor = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.MarbleBlueDoor}");
			}
			if ((flags & (int) ShortcutFlags.CavernsSwitchAndBridge) == (int) ShortcutFlags.CavernsSwitchAndBridge)
			{
				alucardApi.CavernsSwitchAndBridge = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.CavernsSwitchAndBridge}");
			}
			if ((flags & (int) ShortcutFlags.EntranceToCaverns) == (int) ShortcutFlags.EntranceToCaverns)
			{
				alucardApi.EntranceToCaverns = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceToCaverns}");
			}
			if ((flags & (int) ShortcutFlags.EntranceWarp) == (int) ShortcutFlags.EntranceWarp)
			{
				alucardApi.EntranceWarp = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.EntranceWarp}");
			}
			if ((flags & (int) ShortcutFlags.FirstClockRoomDoor) == (int) ShortcutFlags.FirstClockRoomDoor)
			{
				alucardApi.FirstClockRoomDoor = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.FirstClockRoomDoor}");
			}
			if ((flags & (int) ShortcutFlags.SecondClockRoomDoor) == (int) ShortcutFlags.SecondClockRoomDoor)
			{
				alucardApi.SecondClockRoomDoor = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.SecondClockRoomDoor}");
			}
			if ((flags & (int) ShortcutFlags.FirstDemonButton) == (int) ShortcutFlags.FirstDemonButton)
			{
				alucardApi.FirstDemonButton = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.FirstDemonButton}");
			}
			if ((flags & (int) ShortcutFlags.SecondDemonButton) == (int) ShortcutFlags.SecondDemonButton)
			{
				alucardApi.SecondDemonButton = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.SecondDemonButton}");
			}
			if ((flags & (int) ShortcutFlags.KeepStairs) == (int) ShortcutFlags.KeepStairs)
			{
				alucardApi.KeepStairs = true;
				Console.WriteLine($"Received shortcut: {ShortcutFlags.KeepStairs}");
			}
		}

		private void DecodeAssist(string item)
		{
			Potion potion;
			if (Enum.TryParse(Regex.Replace(item, "[ .]", ""), true, out potion))
			{
				alucardApi.ActivatePotion(potion);
			}
			else
			{
				Console.WriteLine($"Assist {item} not found!");
				return;
			}

			Console.WriteLine($"Received assist: {item}");
		}
	}
}

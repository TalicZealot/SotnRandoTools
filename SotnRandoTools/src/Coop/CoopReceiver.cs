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
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;
		private readonly IWatchlistService watchlistService;
		private Queue<MethodInvoker> queuedMessages = new();
		private System.Timers.Timer messageTimer = new();

		public CoopReceiver(IToolConfig toolConfig, ISotnApi sotnApi, INotificationService notificationService, IWatchlistService watchlistService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			this.toolConfig = toolConfig;
			this.sotnApi = sotnApi;
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
					if (!sotnApi.AlucardApi.HasRelic((Relic) index))
					{
						sotnApi.AlucardApi.GrantRelic((Relic) index);
						watchlistService.UpdateWatchlist(watchlistService.CoopRelicWatches);
						watchlistService.CoopRelicWatches.ClearChangeCounts();
						notificationService.AddMessage(((Relic) index).ToString());
						notificationService.PlayAlert(Paths.ItemPickupSound);
						Console.WriteLine($"Received relic: {(Relic) index}");
					}
					break;
				case MessageType.Location:
					sotnApi.GameApi.SetRoomValue(watchlistService.CoopLocationWatches[indexByte].Address, dataByte);
					watchlistService.CoopLocationValues[indexByte] = dataByte;
					Console.WriteLine($"Received location: {watchlistService.CoopLocationWatches[indexByte].Notes} with value {dataByte}");
					break;
				case MessageType.Item:
					sotnApi.AlucardApi.GrantItemByName(Equipment.Items[index]);
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
						sotnApi.AlucardApi.WarpsFirstCastle = dataByte;
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
						sotnApi.AlucardApi.WarpsSecondCastle = dataByte;
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
			bool insideMarbleGalleryDoorRooms = (sotnApi.GameApi.Room == Various.MarbleGalleryDoorToCavernsRoom || sotnApi.GameApi.Room == Various.MarbleGalleryBlueDoorRoom) && (sotnApi.GameApi.Area == Various.MarbleGalleryArea);
			if (sotnApi.GameApi.InAlucardMode() && !insideMarbleGalleryDoorRooms)
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

			notificationService.AddMessage(shortcut.ToString());
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

		private void DecodeAssist(string item)
		{
			Potion potion;
			if (Enum.TryParse(Regex.Replace(item, "[ .]", ""), true, out potion))
			{
				sotnApi.AlucardApi.ActivatePotion(potion);
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

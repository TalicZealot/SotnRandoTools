using System;
using System.Text.RegularExpressions;
using BizHawk.Client.Common;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools.Coop
{
	public class CoopReceiver : ICoopReceiver
	{
		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IGuiApi guiApi;
		private readonly IWatchlistService watchlistService;

		public CoopReceiver(IToolConfig toolConfig, IGameApi gameApi, IAlucardApi alucardApi, IGuiApi guiApi, IWatchlistService watchlistService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (guiApi is null) throw new ArgumentNullException(nameof(guiApi));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			this.toolConfig = toolConfig;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.guiApi = guiApi;
			this.watchlistService = watchlistService;
		}

		public void ProcessMessage(byte[] data)
		{
			MessageType type = (MessageType) data[0];
			ushort index = BitConverter.ToUInt16(data, 1);
			switch (type)
			{
				case MessageType.Relic:
					if (!alucardApi.HasRelic((Relic) index))
					{
						alucardApi.GrantRelic((Relic) index);
						watchlistService.UpdateWatchlist(watchlistService.CoopRelicWatches);
						watchlistService.CoopRelicWatches.ClearChangeCounts();
						guiApi.AddMessage($"Received relic: {(Relic) index}");
						Console.WriteLine($"Received relic: {(Relic) index}");
					}
					break;
				case MessageType.Item:
					alucardApi.GrantItemByName(Equipment.Items[index]);
					guiApi.AddMessage($"Received item: {Equipment.Items[index]}");
					Console.WriteLine($"Received item: {Equipment.Items[index]}");
					break;
				case MessageType.Effect:
					DecodeAssist(Equipment.Items[index]);
					break;
				case MessageType.WarpFirstCastle:
					alucardApi.GrantFirstCastleWarp((Warp) index);
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					guiApi.AddMessage($"Received warp: {(Warp) index}");
					Console.WriteLine($"Received warp: {(Warp) index}");
					break;
				case MessageType.WarpSecondCastle:
					alucardApi.GrantSecondCastleWarp((Warp) index);
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					guiApi.AddMessage($"Received warp: Inverted {(Warp) index}");
					Console.WriteLine($"Received warp: Inverted {(Warp) index}");
					break;
				case MessageType.Shortcut:
					DecodeShortcut((Shortcut) index);
					watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
					watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
					break;
				case MessageType.Settings:
					DecodeSettings((int) index);
					guiApi.AddMessage($"Received co-op settings from host.");
					Console.WriteLine($"Received co-op settings from host.");
					break;
				default:
					break;
			}
		}

		private void DecodeSettings(int settings)
		{
			if ((settings & (int) SettingsFlags.SendRelics) == (int) SettingsFlags.SendRelics)
			{
				toolConfig.Coop.ConnectionSendRelics = true;
			}
			else
			{
				toolConfig.Coop.ConnectionSendRelics = false;
			}
			if ((settings & (int) SettingsFlags.ShareWarps) == (int) SettingsFlags.ShareWarps)
			{
				toolConfig.Coop.ConnectionShareWarps = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionShareWarps = false;
			}
			if ((settings & (int) SettingsFlags.ShareShortcuts) == (int) SettingsFlags.ShareShortcuts)
			{
				toolConfig.Coop.ConnectionShareShortcuts = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionShareShortcuts = false;
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
				toolConfig.Coop.ConnectionAssists = true; ;
			}
			else
			{
				toolConfig.Coop.ConnectionAssists = false;
			}
		}

		private void DecodeShortcut(Shortcut shortcut)
		{
			switch (shortcut)
			{
				case Shortcut.OuterWallElevator:
					if (alucardApi.OuterWallElevator == false)
					{
						alucardApi.OuterWallElevator = true;
					}
					break;
				case Shortcut.AlchemyElevator:
					if (alucardApi.AlchemyElevator == false)
					{
						alucardApi.AlchemyElevator = true;
					}
					break;
				case Shortcut.EntranceToMarble:
					if (alucardApi.EntranceToMarble == false)
					{
						alucardApi.EntranceToMarble = true;
					}
					break;
				case Shortcut.ChapelStatue:
					if (alucardApi.ChapelStatue == false)
					{
						alucardApi.ChapelStatue = true;
					}
					break;
				case Shortcut.ColosseumElevator:
					if (alucardApi.ColosseumElevator == false)
					{
						alucardApi.ColosseumElevator = true;
					}
					break;
				case Shortcut.ColosseumToChapel:
					if (alucardApi.ColosseumToChapel == false)
					{
						alucardApi.ColosseumToChapel = true;
					}
					break;
				case Shortcut.MarbleBlueDoor:
					if (alucardApi.MarbleBlueDoor == false)
					{
						alucardApi.MarbleBlueDoor = true;
					}
					break;
				case Shortcut.CavernsSwitchAndBridge:
					if (alucardApi.CavernsSwitchAndBridge == false)
					{
						alucardApi.CavernsSwitchAndBridge = true;
					}
					break;
				case Shortcut.EntranceToCaverns:
					if (alucardApi.EntranceToCaverns == false)
					{
						alucardApi.EntranceToCaverns = true;
					}
					break;
				case Shortcut.EntranceWarp:
					if (alucardApi.EntranceWarp == false)
					{
						alucardApi.EntranceWarp = true;
					}
					break;
				case Shortcut.FirstClockRoomDoor:
					if (alucardApi.FirstClockRoomDoor == false)
					{
						alucardApi.FirstClockRoomDoor = true;
					}
					break;
				case Shortcut.SecondClockRoomDoor:
					if (alucardApi.SecondClockRoomDoor == false)
					{
						alucardApi.SecondClockRoomDoor = true;
					}
					break;
				case Shortcut.FirstDemonButton:
					if (alucardApi.FirstDemonButton == false)
					{
						alucardApi.FirstDemonButton = true;
					}
					break;
				case Shortcut.SecondDemonButton:
					if (alucardApi.SecondDemonButton == false)
					{
						alucardApi.SecondDemonButton = true;
					}
					break;
				case Shortcut.KeepStairs:
					if (alucardApi.KeepStairs == false)
					{
						alucardApi.KeepStairs = true;
					}
					break;
				default:
					Console.WriteLine($"RShortcut {shortcut} not found!");
					return;
			}

			guiApi.AddMessage($"Received shortcut: {shortcut}");
			Console.WriteLine($"Received shortcut: {shortcut}");
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

			guiApi.AddMessage($"Received assist: {item}");
			Console.WriteLine($"Received assist: {item}");
		}
	}
}

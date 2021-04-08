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
	public class CoopSender
	{
		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IWatchlistService watchlistService;
		private readonly IJoypadApi joypadApi;
		private readonly ICoopMessanger coopMessanger;

		private bool selectPressed = false;
		private bool circlePressed = false;

		public CoopSender(IToolConfig toolConfig, IWatchlistService watchlistService, IGameApi gameApi, IAlucardApi alucardApi, IJoypadApi joypadApi, ICoopMessanger coopMessanger)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (joypadApi is null) throw new ArgumentNullException(nameof(joypadApi));
			if (coopMessanger is null) throw new ArgumentNullException(nameof(coopMessanger));
			this.toolConfig = toolConfig;
			this.watchlistService = watchlistService;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.joypadApi = joypadApi;
			this.coopMessanger = coopMessanger;
		}

		public void Update()
		{
			if (!gameApi.InAlucardMode())
			{
				return;
			}

			if (toolConfig.Coop.ConnectionSendRelics)
			{
				UpdateSendRelic();
			}
			else
			{
				UpdateRelics();
			}

			if (toolConfig.Coop.ConnectionShareWarps)
			{
				UpdateWarpsAndShortcuts();
			}

			if (toolConfig.Coop.ConnectionSendItems)
			{
				UpdateSendItem();
			}

			if (toolConfig.Coop.ConnectionSendAssists)
			{
				UpdateAssist();
			}

			if (toolConfig.Coop.ConnectionShareLocations)
			{
				UpdateLocations();
			}
		}

		private void UpdateSendItem()
		{
			var pressed = joypadApi.Get();
			if (Convert.ToBoolean(pressed["P1 Select"]) == true && selectPressed == false && gameApi.IsInMenu() && gameApi.EquipMenuOpen())
			{
				selectPressed = true;
				string item = alucardApi.GetSelectedItemName();
				if (!item.Contains("empty hand") && !item.Contains("-") && alucardApi.HasItemInInventory(item))
				{
					alucardApi.TakeOneItemByName(item);
					ushort indexData = (ushort) Equipment.Items.IndexOf(item);
					coopMessanger.SendData(MessageType.Item, BitConverter.GetBytes(indexData));
					Console.WriteLine($"Sending item: {item}");
				}
				else
				{
					Console.WriteLine($"Player doesn't have any {item}!");
				}
			}
			else if (Convert.ToBoolean(pressed["P1 Select"]) == false)
			{
				selectPressed = false;
			}
		}

		private void UpdateSendRelic()
		{
			var pressed = joypadApi.Get();
			if (Convert.ToBoolean(pressed["P1 Select"]) == true && selectPressed == false && gameApi.IsInMenu() && gameApi.RelicMenuOpen())
			{
				selectPressed = true;
				Relic relic = alucardApi.GetSelectedRelic();
				ushort indexData = (ushort) relic;
				if (alucardApi.HasRelic(relic))
				{
					alucardApi.TakeRelic(relic);
					coopMessanger.SendData(MessageType.Relic, BitConverter.GetBytes(indexData));
					Console.WriteLine($"Sending relic: {relic}");
				}
				else
				{
					Console.WriteLine($"Player doesn't have {relic}.");
				}
			}
			else if (Convert.ToBoolean(pressed["P1 Select"]) == false)
			{
				selectPressed = false;
			}
		}

		private void UpdateAssist()
		{
			var pressed = joypadApi.Get();
			if (Convert.ToBoolean(pressed["P1 Circle"]) == true && circlePressed == false && gameApi.IsInMenu() && gameApi.EquipMenuOpen())
			{
				circlePressed = true;
				string item = alucardApi.GetSelectedItemName();
				if (!alucardApi.HasItemInInventory(item))
				{
					Console.WriteLine($"Player doesn't have any {item}!");
					return;
				}

				Potion potion;
				if (Enum.TryParse(Regex.Replace(item, "[ .]", ""), true, out potion))
				{
					alucardApi.TakeOneItemByName(item);
					ushort indexData = (ushort) Equipment.Items.IndexOf(item);
					coopMessanger.SendData(MessageType.Effect, BitConverter.GetBytes(indexData));
					Console.WriteLine($"Sending assist: {item}");
				}
				else
				{
					Console.WriteLine($"Item {item} can't be used for an assist.");
				}
			}
			else if (Convert.ToBoolean(pressed["P1 Circle"]) == false)
			{
				circlePressed = false;
			}
		}

		private void UpdateRelics()
		{
			watchlistService.UpdateWatchlist(watchlistService.CoopRelicWatches);
			for (int i = 0; i < watchlistService.CoopRelicWatches.Count; i++)
			{
				if (watchlistService.CoopRelicWatches[i].ChangeCount > 0)
				{
					if (watchlistService.CoopRelicWatches[i].Value > 0)
					{
						coopMessanger.SendData(MessageType.Relic, BitConverter.GetBytes((ushort) i));
						Console.WriteLine($"Sending relic: {watchlistService.CoopRelicWatches[i].Notes}");
					}
				}
			}
			watchlistService.CoopRelicWatches.ClearChangeCounts();
		}

		private void UpdateLocations()
		{
			watchlistService.UpdateWatchlist(watchlistService.CoopLocationWatches);
			for (int i = 0; i < watchlistService.CoopLocationWatches.Count; i++)
			{
				if (watchlistService.CoopLocationWatches[i].ChangeCount > 0)
				{
					if (watchlistService.CoopLocationWatches[i].Value > 0)
					{
						coopMessanger.SendData(MessageType.Location, BitConverter.GetBytes((ushort) i));
						Console.WriteLine($"Sending Location: {watchlistService.CoopLocationWatches[i].Notes}");
					}
				}
			}
			watchlistService.CoopLocationWatches.ClearChangeCounts();
		}

		private void UpdateProgressionItems()
		{
			watchlistService.UpdateWatchlist(watchlistService.ProgressionItemWatches);
			for (int i = 0; i < watchlistService.ProgressionItemWatches.Count; i++)
			{
				if (watchlistService.ProgressionItemWatches[i].ChangeCount > 0)
				{
					if (watchlistService.ProgressionItemWatches[i].Value > 0)
					{
						coopMessanger.SendData(MessageType.Item, new byte[] { 0xFF, 0xFF });
						Console.WriteLine($"Sending item: {watchlistService.ProgressionItemWatches[i].Notes}");
					}
				}
			}
			watchlistService.ProgressionItemWatches.ClearChangeCounts();
		}

		private void UpdateThrustSwords()
		{
			watchlistService.UpdateWatchlist(watchlistService.ThrustSwordWatches);
			for (int i = 0; i < watchlistService.ThrustSwordWatches.Count; i++)
			{
				if (watchlistService.ThrustSwordWatches[i].ChangeCount > 0)
				{
					if (watchlistService.ThrustSwordWatches[i].Value > 0)
					{
						coopMessanger.SendData(MessageType.Item, new byte[] { 0xFF, 0xFF });
						Console.WriteLine($"Sending item: {watchlistService.ThrustSwordWatches[i].Notes}");
					}
				}
			}
			watchlistService.ThrustSwordWatches.ClearChangeCounts();
		}

		private void UpdateWarpsAndShortcuts()
		{
			watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
			for (int i = 0; i < watchlistService.WarpsAndShortcutsWatches.Count; i++)
			{
				if (watchlistService.WarpsAndShortcutsWatches[i].ChangeCount > 0)
				{
					if (watchlistService.WarpsAndShortcutsWatches[i].Value > 0)
					{
						if (watchlistService.WarpsAndShortcutsWatches[i].Notes == "WarpsFirstCastle")
						{
							int difference = watchlistService.WarpsAndShortcutsWatches[i].Previous ^ watchlistService.WarpsAndShortcutsWatches[i].Value;
							coopMessanger.SendData(MessageType.WarpFirstCastle, BitConverter.GetBytes((ushort) difference));
							Console.WriteLine($"Sending warp.");

						}
						else if (watchlistService.WarpsAndShortcutsWatches[i].Notes == "WarpsSecondCastle")
						{
							int difference = watchlistService.WarpsAndShortcutsWatches[i].Previous ^ watchlistService.WarpsAndShortcutsWatches[i].Value;
							coopMessanger.SendData(MessageType.WarpSecondCastle, BitConverter.GetBytes((ushort) difference));
							Console.WriteLine($"Sending warp.");
						}
						else
						{
							byte[] data = BitConverter.GetBytes((ushort) (Shortcut) Enum.Parse(typeof(Shortcut), watchlistService.WarpsAndShortcutsWatches[i].Notes));
							coopMessanger.SendData(MessageType.Shortcut, data);
							Console.WriteLine($"Sending shortcut: {watchlistService.WarpsAndShortcutsWatches[i].Notes}");
						}
					}
				}
			}
			watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
		}
	}
}

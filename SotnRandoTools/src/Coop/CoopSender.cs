using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Services;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace SotnRandoTools.Coop
{
	public class CoopSender
	{
		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IWatchlistService watchlistService;
		private readonly IInputService inputService;
		private readonly ICoopMessanger coopMessanger;
		private Queue<MethodInvoker> queuedMessages = new();

		private bool r3Pressed = false;
		private bool selectPressed = false;
		private bool circlePressed = false;

		public CoopSender(IToolConfig toolConfig, IWatchlistService watchlistService, IInputService inputService, IGameApi gameApi, IAlucardApi alucardApi, ICoopMessanger coopMessanger)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (inputService is null) throw new ArgumentNullException(nameof(inputService));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (coopMessanger is null) throw new ArgumentNullException(nameof(coopMessanger));
			this.toolConfig = toolConfig;
			this.watchlistService = watchlistService;
			this.inputService = inputService;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.coopMessanger = coopMessanger;
		}

		public void Update()
		{
			if (!gameApi.InAlucardMode() || !coopMessanger.IsConnected())
			{
				return;
			}

			if (toolConfig.Coop.ConnectionShareRelics)
			{
				UpdateRelics();
			}

			if (toolConfig.Coop.ConnectionShareWarps)
			{
				UpdateWarpsAndShortcuts();
				UpdateSendAllWarps();
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

			SendMessage();
		}

		private void SendMessage()
		{
			if (queuedMessages.Count > 0)
			{
				queuedMessages.Dequeue()();
			}
		}

		private void UpdateSendItem()
		{
			if (inputService.ButtonPressed(PlaystationInputKeys.R3, Globals.UpdateCooldownFrames) && r3Pressed == false && gameApi.IsInMenu() && gameApi.EquipMenuOpen())
			{
				r3Pressed = true;
				string item = alucardApi.GetSelectedItemName();
				if (!item.Contains("empty hand") && !item.Contains("-") && alucardApi.HasItemInInventory(item))
				{
					alucardApi.TakeOneItemByName(item);
					ushort indexData = (ushort) Equipment.Items.IndexOf(item);
					queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.Item, BitConverter.GetBytes(indexData)); }));
					Console.WriteLine($"Sending item: {item}");
				}
				else
				{
					Console.WriteLine($"Player doesn't have any {item}!");
				}
			}
			else if (!inputService.ButtonPressed(PlaystationInputKeys.R3, Globals.UpdateCooldownFrames))
			{
				r3Pressed = false;
			}
		}

		private void UpdateAssist()
		{
			if (inputService.ButtonPressed(PlaystationInputKeys.Circle, Globals.UpdateCooldownFrames) && circlePressed == false && gameApi.IsInMenu() && gameApi.EquipMenuOpen())
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
			else if (!inputService.ButtonPressed(PlaystationInputKeys.Circle, Globals.UpdateCooldownFrames))
			{
				circlePressed = false;

				if (!gameApi.IsInMenu() && inputService.RegisteredMove(InputKeys.DragonPunch, Globals.UpdateCooldownFrames))
				{
					string item = "Manna prism";
					if (!alucardApi.HasItemInInventory(item))
					{
						Console.WriteLine($"Player doesn't have any {item}!");
						return;
					}
					alucardApi.TakeOneItemByName(item);
					ushort indexData = (ushort) Equipment.Items.IndexOf(item);
					coopMessanger.SendData(MessageType.Effect, BitConverter.GetBytes(indexData));
					alucardApi.ActivatePotion(Potion.Mannaprism);
					Console.WriteLine($"Sending assist: {item}");

				}
				else if (!gameApi.IsInMenu() && inputService.RegisteredMove(InputKeys.HalfCircleForward, Globals.UpdateCooldownFrames))
				{
					string item = "Potion";
					if (!alucardApi.HasItemInInventory(item))
					{
						Console.WriteLine($"Player doesn't have any {item}!");
						return;
					}

					alucardApi.TakeOneItemByName(item);
					ushort indexData = (ushort) Equipment.Items.IndexOf(item);
					coopMessanger.SendData(MessageType.Effect, BitConverter.GetBytes(indexData));
					alucardApi.ActivatePotion(Potion.Potion);
					Console.WriteLine($"Sending assist: {item}");
				}
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
			for (int i = 0; i < watchlistService.CoopLocationWatches.Count; i++)
			{
				if (watchlistService.CoopLocationWatches[i].ChangeCount > 0)
				{
					var data = new byte[] { (byte) i, (byte) watchlistService.CoopLocationValues[i] };
					queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.Location, data); }));
					Console.WriteLine($"Sending Location: {watchlistService.CoopLocationWatches[i].Notes} with value {(byte) watchlistService.CoopLocationValues[i]} at index {i}");
				}
			}
			watchlistService.CoopLocationWatches.ClearChangeCounts();
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
							queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.WarpFirstCastle, BitConverter.GetBytes((ushort) difference)); }));
							Console.WriteLine($"Sending first castle warp {difference}.");

						}
						else if (watchlistService.WarpsAndShortcutsWatches[i].Notes == "WarpsSecondCastle")
						{
							int difference = watchlistService.WarpsAndShortcutsWatches[i].Previous ^ watchlistService.WarpsAndShortcutsWatches[i].Value;
							queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.WarpSecondCastle, BitConverter.GetBytes((ushort) difference)); }));
							Console.WriteLine($"Sending second castle warp {difference}.");
						}
						else
						{
							byte[] data = BitConverter.GetBytes((ushort) (Shortcut) Enum.Parse(typeof(Shortcut), watchlistService.WarpsAndShortcutsWatches[i].Notes));
							queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.Shortcut, data); }));
							Console.WriteLine($"Sending shortcut: {watchlistService.WarpsAndShortcutsWatches[i].Notes}");
						}
					}
				}
			}
			watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
		}

		private void UpdateSendAllWarps()
		{
			if (inputService.ButtonPressed(PlaystationInputKeys.Select, Globals.UpdateCooldownFrames) && selectPressed == false && gameApi.IsInMenu() && gameApi.RelicMenuOpen())
			{
				selectPressed = true;
				var warpsFirstCastle = new byte[] { 0, (byte) watchlistService.WarpsAndShortcutsWatches.Where(w => w.Notes == "WarpsFirstCastle").FirstOrDefault().Value };
				var warpsSecondCastle = new byte[] { 0, (byte) watchlistService.WarpsAndShortcutsWatches.Where(w => w.Notes == "WarpsSecondCastle").FirstOrDefault().Value };


				queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.WarpFirstCastle, warpsFirstCastle); }));
				Console.WriteLine($"Sending first castle warps with value {warpsFirstCastle[1]}");
				queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.WarpSecondCastle, warpsSecondCastle); }));
				Console.WriteLine($"Sending second castle warps with value {warpsSecondCastle[1]}");

				int shortcuts = 0;
				for (int i = 2; i < watchlistService.WarpsAndShortcutsWatches.Count; i++)
				{
					if (watchlistService.WarpsAndShortcutsWatches[i].Value > 0)
					{
						shortcuts = shortcuts | (int) (ShortcutFlags) Enum.Parse(typeof(ShortcutFlags), watchlistService.WarpsAndShortcutsWatches[i].Notes);
					}
				}
				queuedMessages.Enqueue(new MethodInvoker(() => { coopMessanger.SendData(MessageType.Shortcut, BitConverter.GetBytes((ushort) shortcuts)); }));
				Console.WriteLine($"Sending shortcuts with flags {(ushort) shortcuts}");
			}
			else if (!inputService.ButtonPressed(PlaystationInputKeys.Select, Globals.UpdateCooldownFrames))
			{
				selectPressed = false;
			}
		}
	}
}

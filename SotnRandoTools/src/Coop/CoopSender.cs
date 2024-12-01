using System;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools.Coop
{
	internal sealed class CoopSender
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly IWatchlistService watchlistService;
		private readonly ICoopController coopController;
		private bool sendPressedFrame1 = false;
		private bool sendPressedFrame2 = false;
		private bool sendPressed = false;
		private bool inGame = false;
		private bool gameStarted = false;
		private ushort[] sendButton = new ushort[4] { SotnApi.Constants.Values.Game.Controller.Select, SotnApi.Constants.Values.Game.Controller.Triangle, SotnApi.Constants.Values.Game.Controller.L3, SotnApi.Constants.Values.Game.Controller.R3 };

		public CoopSender(IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, ICoopController coopController)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.watchlistService = watchlistService ?? throw new ArgumentNullException(nameof(watchlistService));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi)); ;
			this.coopController = coopController ?? throw new ArgumentNullException(nameof(coopController));
		}

		public void Update()
		{
			if (!gameStarted && sotnApi.GameApi.InAlucardMode())
			{
				gameStarted = true;
				inGame = true;
			}
			if (gameStarted && sotnApi.GameApi.Status == SotnApi.Constants.Values.Game.Status.MainMenu)
			{
				inGame = false;
				return;
			}
			if (gameStarted && !inGame && sotnApi.GameApi.InAlucardMode() && coopController.IsConnected())
			{
				inGame = true;
				SendSynchRequest();
			}
			if (!sotnApi.GameApi.InAlucardMode() || !coopController.IsConnected())
			{
				return;
			}
			CheckSendButton();
			SendRelics();
			SendItem();
			SendLocations();
			SendWarpsAndShortcuts();
			CheckSynchRequest();
		}

		private void CheckSendButton()
		{
			sendPressedFrame1 = sendPressedFrame2;
			if ((sotnApi.GameApi.InputFlags & sendButton[toolConfig.Coop.SendButton]) == sendButton[toolConfig.Coop.SendButton])
			{
				sendPressedFrame2 = true;
			}
			else
			{
				sendPressedFrame2 = false;
			}

			if (sendPressedFrame2 && !sendPressedFrame1)
			{
				sendPressed = true;
			}
			else
			{
				sendPressed = false;
			}
		}

		private void SendItem()
		{
			if (!sotnApi.GameApi.EquipMenuOpen() || !sotnApi.GameApi.IsInMenu() || !sendPressed)
			{
				return;
			}

			sendPressed = true;
			int item = sotnApi.AlucardApi.GetSelectedItem();
			if (item != -1 && sotnApi.AlucardApi.HasItemInInventory(item))
			{
				sotnApi.AlucardApi.TakeOneItem(item);
				byte[] indexData = BitConverter.GetBytes(item);
				byte[] data = new byte[3];
				data[0] = (byte) MessageType.Item;
				data[1] = indexData[0];
				data[2] = indexData[1];

				coopController.SendData(data);
			}
		}

		private void SendRelics()
		{
			watchlistService.UpdateWatchlist(watchlistService.CoopRelicWatches);
			for (int i = 0; i < watchlistService.CoopRelicWatches.Count; i++)
			{
				if (watchlistService.CoopRelicWatches[i].ChangeCount > 0)
				{
					if (watchlistService.CoopRelicWatches[i].Value > 0)
					{
						byte[] data = new byte[2];
						data[0] = (byte) MessageType.Relic;
						data[1] = (byte) i;
						coopController.SendData(data);
					}
				}
			}
			watchlistService.CoopRelicWatches.ClearChangeCounts();
		}

		private void SendLocations()
		{
			for (int i = 0; i < watchlistService.CoopLocationWatches.Count; i++)
			{
				if (watchlistService.CoopLocationWatches[i].ChangeCount > 0)
				{
					var data = new byte[] { (byte) MessageType.Location, (byte) i, (byte) watchlistService.CoopLocationValues[i] };
					coopController.SendData(data);
					Console.WriteLine($"Sending Location: {watchlistService.CoopLocationWatches[i].Notes} with value {(byte) watchlistService.CoopLocationValues[i]} at index {i}");
				}
			}
			watchlistService.CoopLocationWatches.ClearChangeCounts();
		}

		private void SendWarpsAndShortcuts()
		{
			watchlistService.UpdateWatchlist(watchlistService.WarpsAndShortcutsWatches);
			for (int i = 0; i < watchlistService.WarpsAndShortcutsWatches.Count; i++)
			{
				if (watchlistService.WarpsAndShortcutsWatches[i].ChangeCount > 0)
				{
					if (watchlistService.WarpsAndShortcutsWatches[i].Value > 0)
					{
						byte[] data = new byte[3];
						if (i == 0)
						{
							byte[] difference = BitConverter.GetBytes((ushort) (watchlistService.WarpsAndShortcutsWatches[i].Previous ^ watchlistService.WarpsAndShortcutsWatches[i].Value));
							data[0] = (byte) MessageType.WarpFirstCastle;
							data[1] = difference[0];
							data[2] = difference[1];
							coopController.SendData(data);
							Console.WriteLine($"Sending first castle warp {difference}.");

						}
						else if (i == 1)
						{
							byte[] difference = BitConverter.GetBytes((ushort) (watchlistService.WarpsAndShortcutsWatches[i].Previous ^ watchlistService.WarpsAndShortcutsWatches[i].Value));
							data[0] = (byte) MessageType.WarpFirstCastle;
							data[1] = difference[0];
							data[2] = difference[1];
							coopController.SendData(data);
							Console.WriteLine($"Sending second castle warp {difference}.");
						}
						else
						{
							byte[] shortcutData = BitConverter.GetBytes((ushort) (Shortcut) Enum.Parse(typeof(Shortcut), watchlistService.WarpsAndShortcutsWatches[i].Notes));
							data[0] = (byte) MessageType.Shortcut;
							data[1] = shortcutData[0];
							data[2] = shortcutData[1];
							Console.WriteLine($"Sending shortcut: {watchlistService.WarpsAndShortcutsWatches[i].Notes}");
						}
						coopController.SendData(data);
					}
				}
			}
			watchlistService.WarpsAndShortcutsWatches.ClearChangeCounts();
		}

		private void CheckSynchRequest()
		{
			if (!sotnApi.GameApi.RelicMenuOpen() || !sotnApi.GameApi.IsInMenu() || !sendPressed)
			{
				return;
			}

			SendSynchRequest();
		}

		private void SendSynchRequest()
		{
			byte[] data = new byte[1];
			data[0] = (byte) MessageType.SynchRequest;
			coopController.SendData(data);
			Console.WriteLine("Requested synch");
		}
	}
}

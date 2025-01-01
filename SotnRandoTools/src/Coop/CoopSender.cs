using System;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Interfaces;
using SotnApi.Main;
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
		private readonly ICoopController coopController;
		private bool sendPressedFrame1 = false;
		private bool sendPressedFrame2 = false;
		private bool sendPressed = false;
		private bool inGame = false;
		private bool gameStarted = false;
		private ushort[] sendButton = new ushort[4] { SotnApi.Constants.Values.Game.Controller.Select, SotnApi.Constants.Values.Game.Controller.Triangle, SotnApi.Constants.Values.Game.Controller.L3, SotnApi.Constants.Values.Game.Controller.R3 };

		public CoopSender(IToolConfig toolConfig, ISotnApi sotnApi, ICoopController coopController)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
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
			SendWarps();
			SendShortcuts();

			if (coopController.SynchRequested)
			{
				coopController.SynchRequested = false;
				SendSynchAll();
			}

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
			for (int i = 0; i < coopController.CoopState.relics.Length; i++)
			{
				if (coopController.CoopState.relics[i].updated && coopController.CoopState.relics[i].status)
				{
					byte[] data = new byte[2];
					data[0] = (byte) MessageType.Relic;
					data[1] = (byte) i;
					coopController.SendData(data);
				}
			}
		}

		private void SendLocations()
		{
			for (int i = 0; i < coopController.CoopState.locations.Length; i++)
			{
				if (coopController.CoopState.locations[i].updated && coopController.CoopState.locations[i].status)
				{
					byte[] data = new byte[5];
					byte[] roomIndex = BitConverter.GetBytes(coopController.CoopState.locations[i].roomIndex);
					byte[] locationIndex = BitConverter.GetBytes((ushort)i);
					data[0] = (byte) MessageType.Location;
					data[1] = roomIndex[0];
					data[2] = roomIndex[1];
					data[3] = locationIndex[0];
					data[4] = locationIndex[1];
					coopController.SendData(data);
				}
			}
		}

		private void SendWarps()
		{
			byte[] data = new byte[2];
			if (coopController.CoopState.WarpsFirstCastle.updated)
			{
				data[0] = (byte) MessageType.WarpFirstCastle;
				data[1] = coopController.CoopState.WarpsFirstCastle.difference;
				coopController.SendData(data);
				Console.WriteLine($"Sending first castle warp {coopController.CoopState.WarpsFirstCastle.difference}.");
			}
			if (coopController.CoopState.WarpsSecondCastle.updated)
			{
				data[0] = (byte) MessageType.WarpSecondCastle;
				data[1] = coopController.CoopState.WarpsSecondCastle.difference;
				coopController.SendData(data);
				Console.WriteLine($"Sending first castle warp {coopController.CoopState.WarpsSecondCastle.difference}.");
			}
		}

		private void SendShortcuts()
		{
			for (int i = 0; i < coopController.CoopState.shortcuts.Length; i++)
			{
				if (coopController.CoopState.shortcuts[i].updated && coopController.CoopState.shortcuts[i].status)
				{
					byte[] data = new byte[2];
					data[0] = (byte) MessageType.Shortcut;
					data[1] = (byte) i;
					coopController.SendData(data);
				}
			}
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

		private void SendSynchAll()
		{
			byte[] data = new byte[9];
			data[0] = (byte) MessageType.SynchAll;
			data[1] = coopController.CoopState.WarpsFirstCastle.value;
			data[2] = coopController.CoopState.WarpsSecondCastle.value;
			int shortcuts = 0;
			for (int i = 0; i < coopController.CoopState.shortcuts.Length; i++)
			{
				if (coopController.CoopState.shortcuts[i].status)
				{
					shortcuts |= (int) Math.Pow(2, i);
				}
			}
			byte[] shortcutBytes = BitConverter.GetBytes((ushort) shortcuts);
			data[3] = shortcutBytes[0];
			data[4] = shortcutBytes[1];


			int relicsNumber = 0;
			for (int i = 0; i < coopController.CoopState.relics.Length; i++)
			{
				if (coopController.CoopState.relics[i].status)
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
	}
}

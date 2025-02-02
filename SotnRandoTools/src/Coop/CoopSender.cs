using System;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Coop.Enums;
using SotnRandoTools.Coop.Interfaces;

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
		byte[] data2 = new byte[2];
		byte[] data3 = new byte[3];
		byte[] data5 = new byte[5];
		byte[] data9 = new byte[9];
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

		private unsafe void SendItem()
		{
			if (!sotnApi.GameApi.EquipMenuOpen() || !sotnApi.GameApi.IsInMenu() || !sendPressed)
			{
				return;
			}

			sendPressed = true;
			short item = (short) sotnApi.AlucardApi.GetSelectedItem();
			if (item == -1 || !sotnApi.AlucardApi.HasItemInInventory(item))
			{
				return;
			}
			sotnApi.AlucardApi.TakeOneItem(item);
			fixed (byte* buffer = data3)
			{
				buffer[0] = (byte) MessageType.Item;
				*((short*) (buffer + 1)) = item;
			}
			coopController.SendData(data3);
		}

		private void SendRelics()
		{
			for (int i = 0; i < coopController.CoopState.relics.Length; i++)
			{
				if (coopController.CoopState.relics[i].updated && coopController.CoopState.relics[i].status)
				{
					data2[0] = (byte) MessageType.Relic;
					data2[1] = (byte) i;
					coopController.SendData(data2);
				}
			}
		}

		private unsafe void SendLocations()
		{
			for (ushort i = 0; i < coopController.CoopState.locations.Length; i++)
			{
				if (coopController.CoopState.locations[i].updated && coopController.CoopState.locations[i].status)
				{
					ushort roomIndex = coopController.CoopState.locations[i].roomIndex;
					ushort locationIndex = i;
					fixed (byte* buffer = data5)
					{
						buffer[0] = (byte) MessageType.Location;
						*((ushort*) (buffer + 1)) = roomIndex;
						*((ushort*) (buffer + 3)) = locationIndex;
					}
					coopController.SendData(data5);
				}
			}
		}

		private void SendWarps()
		{
			if (coopController.CoopState.WarpsFirstCastle.updated)
			{
				data2[0] = (byte) MessageType.WarpFirstCastle;
				data2[1] = coopController.CoopState.WarpsFirstCastle.difference;
				coopController.SendData(data2);
				//Console.WriteLine($"Sending first castle warp {coopController.CoopState.WarpsFirstCastle.difference}.");
			}
			if (coopController.CoopState.WarpsSecondCastle.updated)
			{
				data2[0] = (byte) MessageType.WarpSecondCastle;
				data2[1] = coopController.CoopState.WarpsSecondCastle.difference;
				coopController.SendData(data2);
				//Console.WriteLine($"Sending first castle warp {coopController.CoopState.WarpsSecondCastle.difference}.");
			}
		}

		private void SendShortcuts()
		{
			for (int i = 0; i < coopController.CoopState.shortcuts.Length; i++)
			{
				if (coopController.CoopState.shortcuts[i].updated && coopController.CoopState.shortcuts[i].status)
				{
					data2[0] = (byte) MessageType.Shortcut;
					data2[1] = (byte) i;
					coopController.SendData(data2);
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
			data2[0] = (byte) MessageType.SynchRequest;
			coopController.SendData(data2);
			Console.WriteLine("Requested synch");
		}

		private unsafe void SendSynchAll()
		{
			data9[0] = (byte) MessageType.SynchAll;
			data9[1] = coopController.CoopState.WarpsFirstCastle.value;
			data9[2] = coopController.CoopState.WarpsSecondCastle.value;
			ushort shortcuts = 0;
			for (ushort i = 0; i < coopController.CoopState.shortcuts.Length; i++)
			{
				if (coopController.CoopState.shortcuts[i].status)
				{
					shortcuts |= (ushort) Math.Pow(2, i);
				}
			}
			int relicsNumber = 0;
			for (int i = 0; i < coopController.CoopState.relics.Length; i++)
			{
				if (coopController.CoopState.relics[i].status)
				{
					relicsNumber |= (int) Math.Pow(2, i);
				}
			}
			fixed (byte* buffer = data9)
			{
				*((ushort*) (buffer + 3)) = shortcuts;
				*((int*) (buffer + 5)) = relicsNumber;
			}

			coopController.SendData(data9);
		}
	}
}

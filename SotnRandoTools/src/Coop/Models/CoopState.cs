using System;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;

namespace SotnRandoTools.Coop.Models
{
	internal struct ObjectState
	{
		public bool status;
		public bool updated;
	}

	internal struct LocationState
	{
		public bool status;
		public ushort roomIndex;
		public bool updated;
	}

	internal struct WarpsState
	{
		public byte value;
		public byte difference;
		public bool updated;
	}

	internal class CoopState
	{
		private readonly ISotnApi sotnApi;
		private readonly ILocationTracker locationTracker;
		private bool locationsInitialized = false;

		public readonly ObjectState[] relics;
		public readonly ObjectState[] shortcuts;
		public LocationState[] locations;
		public WarpsState WarpsFirstCastle;
		public WarpsState WarpsSecondCastle;

		public CoopState(ISotnApi sotnApi, ILocationTracker locationTracker)
		{
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));
			this.locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));
			relics = new ObjectState[Enum.GetValues(typeof(SotnApi.Constants.Values.Alucard.Enums.Relic)).Length];
			shortcuts = new ObjectState[Enum.GetValues(typeof(Enums.Shortcut)).Length];
			locations = new LocationState[1];
		}

		public void Update()
		{
			UpdateRelics();
			UpdateWarps();
			UpdateShortcuts();
			UpdateLocations();
		}

		private void UpdateShortcuts()
		{
			bool currentShortcut;

			currentShortcut = sotnApi.AlucardApi.OuterWallElevator;
			shortcuts[0].updated = currentShortcut != shortcuts[0].status;
			shortcuts[0].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.AlchemyElevator;
			shortcuts[1].updated = currentShortcut != shortcuts[1].status;
			shortcuts[1].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.EntranceToMarble;
			shortcuts[2].updated = currentShortcut != shortcuts[2].status;
			shortcuts[2].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.ChapelStatue;
			shortcuts[3].updated = currentShortcut != shortcuts[3].status;
			shortcuts[3].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.ColosseumElevator;
			shortcuts[4].updated = currentShortcut != shortcuts[4].status;
			shortcuts[4].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.ColosseumToChapel;
			shortcuts[5].updated = currentShortcut != shortcuts[5].status;
			shortcuts[5].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.MarbleBlueDoor;
			shortcuts[6].updated = currentShortcut != shortcuts[6].status;
			shortcuts[6].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.CavernsSwitchAndBridge;
			shortcuts[7].updated = currentShortcut != shortcuts[7].status;
			shortcuts[7].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.EntranceToCaverns;
			shortcuts[8].updated = currentShortcut != shortcuts[8].status;
			shortcuts[8].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.EntranceWarp;
			shortcuts[9].updated = currentShortcut != shortcuts[9].status;
			shortcuts[9].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.FirstClockRoomDoor;
			shortcuts[10].updated = currentShortcut != shortcuts[10].status;
			shortcuts[10].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.SecondClockRoomDoor;
			shortcuts[11].updated = currentShortcut != shortcuts[11].status;
			shortcuts[11].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.FirstDemonButton;
			shortcuts[12].updated = currentShortcut != shortcuts[12].status;
			shortcuts[12].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.SecondDemonButton;
			shortcuts[13].updated = currentShortcut != shortcuts[13].status;
			shortcuts[13].status = currentShortcut;
			currentShortcut = sotnApi.AlucardApi.KeepStairs;
			shortcuts[14].updated = currentShortcut != shortcuts[14].status;
			shortcuts[14].status = currentShortcut;
		}

		private void UpdateLocations()
		{
			if (!locationsInitialized)
			{
				if (!locationTracker.Locations.initialized)
				{
					return;
				}
				locationsInitialized = true;
				locations = new LocationState[locationTracker.Locations.roomCount];
				for (int i = 0; i < locationTracker.Locations.roomCount; i++)
				{
					locations[i].roomIndex = locationTracker.Locations.rooms[i].roomIndex;
				}
				return;
			}

			for (int i = 0; i < locationTracker.Locations.roomCount; i++)
			{
				bool locationStatus = locationTracker.Locations.states[locationTracker.Locations.rooms[i].stateIndex].Visited;
				locations[i].updated = locations[i].status != locationStatus;
				locations[i].status = locationStatus;
			}
		}

		private void UpdateWarps()
		{
			byte currentWarpsFirstCastle = (byte) sotnApi.AlucardApi.WarpsFirstCastle;
			WarpsFirstCastle.updated = WarpsFirstCastle.value != currentWarpsFirstCastle;
			WarpsFirstCastle.difference = (byte) (WarpsFirstCastle.value ^ currentWarpsFirstCastle);
			WarpsFirstCastle.value = currentWarpsFirstCastle;
			byte currentWarpsSecondCastle = (byte) sotnApi.AlucardApi.WarpsSecondCastle;
			WarpsSecondCastle.updated = WarpsSecondCastle.value != currentWarpsSecondCastle;
			WarpsSecondCastle.difference = (byte) (WarpsSecondCastle.value ^ currentWarpsSecondCastle);
			WarpsSecondCastle.value = currentWarpsSecondCastle;
		}

		private void UpdateRelics()
		{
			for (int i = 0; i < relics.Length; i++)
			{
				bool relicState = sotnApi.AlucardApi.HasRelic((Relic) i);
				relics[i].updated = relicState != relics[i].status;
				relics[i].status = relicState;
			}
		}
	}
}

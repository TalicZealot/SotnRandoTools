using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Constants.Values.Game.Enums;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;
using SotnRandoTools.Services;
using static BizHawk.Common.Shell32Imports.BROWSEINFOW;

namespace SotnRandoTools.RandoTracker
{
	internal sealed class Tracker : ILocationTracker
	{
		private const byte ReplayCooldown = 6;
		private const string DefaultSeedInfo = "seed(preset)";
		private const long DraculaEntityAddress = 0x076e98;
		private const int AutosplitterReconnectCooldown = 120;
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

		public readonly Locations locations = new Locations();
		public readonly TrackerRelic[] relics = new TrackerRelic[30];
		public readonly ushort[] relicCollectionTimes = new ushort[30];
		public readonly Item[] progressionItems = new Item[]
		{
			new Item {Value = 72, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Gold Ring")},
			new Item {Value = 73, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Silver Ring")},
			new Item {Value = 14, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Spike Breaker")},
			new Item {Value = 34, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Holy glasses")}
		};
		public readonly Item[] thrustSwords = new Item[]
		{
			new Item {Value = 95, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Estoc")},
			new Item {Value = 98, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Claymore")},
			new Item {Value = 101, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Flamberge")},
			new Item {Value = 103, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("Zweihander")},
			new Item {Value = 107, Index = (byte)SotnApi.Constants.Values.Alucard.Equipment.Items.IndexOf("ObsidianSword")}
		};
		public readonly bool[] timeAttacks = new bool[21];
		public bool allBossesGoal = false;
		private int relicsFlags;
		private int itemsFlags;
		private int bossesFlags;

		private readonly Dictionary<string, ulong> abilityToFlag = new Dictionary<string, ulong>
		{
			{ "Soul of Bat",    0b0000000000000000000000000000000000000000000000000000000000000001},
			{ "Fire of Bat",    0b0000000000000000000000000000000000000000000000000000000000000010},
			{ "Echo of Bat",    0b0000000000000000000000000000000000000000000000000000000000000100},
			{ "Force of Echo",  0b0000000000000000000000000000000000000000000000000000000000001000},
			{ "Soul of Wolf",   0b0000000000000000000000000000000000000000000000000000000000010000},
			{ "Power of Wolf",  0b0000000000000000000000000000000000000000000000000000000000100000},
			{ "Skill of Wolf",  0b0000000000000000000000000000000000000000000000000000000001000000},
			{ "Form of Mist",   0b0000000000000000000000000000000000000000000000000000000010000000},
			{ "Power of Mist",  0b0000000000000000000000000000000000000000000000000000000100000000},
			{ "Gas Cloud",      0b0000000000000000000000000000000000000000000000000000001000000000},
			{ "Cube of Zoe",    0b0000000000000000000000000000000000000000000000000000010000000000},
			{ "Spirit Orb",     0b0000000000000000000000000000000000000000000000000000100000000000},
			{ "Gravity Boots",  0b0000000000000000000000000000000000000000000000000001000000000000},
			{ "Leap Stone",     0b0000000000000000000000000000000000000000000000000010000000000000},
			{ "Holy Symbol",    0b0000000000000000000000000000000000000000000000000100000000000000},
			{ "Faerie Scroll",  0b0000000000000000000000000000000000000000000000001000000000000000},
			{ "Jewel of Open",  0b0000000000000000000000000000000000000000000000010000000000000000},
			{ "Merman Statue",  0b0000000000000000000000000000000000000000000000100000000000000000},
			{ "Bat Card",       0b0000000000000000000000000000000000000000000001000000000000000000},
			{ "Ghost Card",     0b0000000000000000000000000000000000000000000010000000000000000000},
			{ "Faerie Card",    0b0000000000000000000000000000000000000000000100000000000000000000},
			{ "Demon Card",     0b0000000000000000000000000000000000000000001000000000000000000000},
			{ "Sword Card",     0b0000000000000000000000000000000000000000010000000000000000000000},
			{ "Sprite Card",    0b0000000000000000000000000000000000000000100000000000000000000000},
			{ "Nosedevil Card", 0b0000000000000000000000000000000000000001000000000000000000000000},
			{ "Heart of Vlad",  0b0000000000000000000000000000000000000010000000000000000000000000},
			{ "Tooth of Vlad",  0b0000000000000000000000000000000000000100000000000000000000000000},
			{ "Rib of Vlad",    0b0000000000000000000000000000000000001000000000000000000000000000},
			{ "Ring of Vlad",   0b0000000000000000000000000000000000010000000000000000000000000000},
			{ "Eye of Vlad",    0b0000000000000000000000000000000000100000000000000000000000000000},
			{ "Gold ring",      0b0000000000000000000000000000000001000000000000000000000000000000},
			{ "Silver ring",    0b0000000000000000000000000000000010000000000000000000000000000000},
			{ "Spike Breaker",  0b0000000000000000000000000000000100000000000000000000000000000000},
			{ "Holy glasses",   0b0000000000000000000000000000001000000000000000000000000000000000},
			{ "Thrust sword",   0b0000000000000000000000000000010000000000000000000000000000000000}
		};
		private readonly Dictionary<string, byte> abilityToIndex = new Dictionary<string, byte>
		{
			{ "Soul of Bat",    0},
			{ "Fire of Bat",    1},
			{ "Echo of Bat",    2},
			{ "Force of Echo",  3},
			{ "Soul of Wolf",   4},
			{ "Power of Wolf",  5},
			{ "Skill of Wolf",  6},
			{ "Form of Mist",   7},
			{ "Power of Mist",  8},
			{ "Gas Cloud",      9},
			{ "Cube of Zoe",    10},
			{ "Spirit Orb",     11},
			{ "Gravity Boots",  12},
			{ "Leap Stone",     13},
			{ "Holy Symbol",    14},
			{ "Faerie Scroll",  15},
			{ "Jewel of Open",  16},
			{ "Merman Statue",  17},
			{ "Bat Card",       18},
			{ "Ghost Card",     19},
			{ "Faerie Card",    20},
			{ "Demon Card",     21},
			{ "Sword Card",     22},
			{ "Sprite Card",    23},
			{ "Nosedevil Card", 24},
			{ "Heart of Vlad",  25},
			{ "Tooth of Vlad",  26},
			{ "Rib of Vlad",    27},
			{ "Ring of Vlad",   28},
			{ "Eye of Vlad",    29},
			{ "Gold ring",      30},
			{ "Silver ring",    31},
			{ "Spike Breaker",  32},
			{ "Holy glasses",   33},
			{ "Thrust sword",   34}
		};
		private readonly ulong[] abilityFlags = new ulong[]
		{
			0b0000000000000000000000000000000000000000000000000000000000000001,
			0b0000000000000000000000000000000000000000000000000000000000000010,
			0b0000000000000000000000000000000000000000000000000000000000000100,
			0b0000000000000000000000000000000000000000000000000000000000001000,
			0b0000000000000000000000000000000000000000000000000000000000010000,
			0b0000000000000000000000000000000000000000000000000000000000100000,
			0b0000000000000000000000000000000000000000000000000000000001000000,
			0b0000000000000000000000000000000000000000000000000000000010000000,
			0b0000000000000000000000000000000000000000000000000000000100000000,
			0b0000000000000000000000000000000000000000000000000000001000000000,
			0b0000000000000000000000000000000000000000000000000000010000000000,
			0b0000000000000000000000000000000000000000000000000000100000000000,
			0b0000000000000000000000000000000000000000000000000001000000000000,
			0b0000000000000000000000000000000000000000000000000010000000000000,
			0b0000000000000000000000000000000000000000000000000100000000000000,
			0b0000000000000000000000000000000000000000000000001000000000000000,
			0b0000000000000000000000000000000000000000000000010000000000000000,
			0b0000000000000000000000000000000000000000000000100000000000000000,
			0b0000000000000000000000000000000000000000000001000000000000000000,
			0b0000000000000000000000000000000000000000000010000000000000000000,
			0b0000000000000000000000000000000000000000000100000000000000000000,
			0b0000000000000000000000000000000000000000001000000000000000000000,
			0b0000000000000000000000000000000000000000010000000000000000000000,
			0b0000000000000000000000000000000000000000100000000000000000000000,
			0b0000000000000000000000000000000000000001000000000000000000000000,
			0b0000000000000000000000000000000000000010000000000000000000000000,
			0b0000000000000000000000000000000000000100000000000000000000000000,
			0b0000000000000000000000000000000000001000000000000000000000000000,
			0b0000000000000000000000000000000000010000000000000000000000000000,
			0b0000000000000000000000000000000000100000000000000000000000000000,
			0b0000000000000000000000000000000001000000000000000000000000000000,
			0b0000000000000000000000000000000010000000000000000000000000000000,
			0b0000000000000000000000000000000100000000000000000000000000000000,
			0b0000000000000000000000000000001000000000000000000000000000000000,
			0b0000000000000000000000000000010000000000000000000000000000000000
		};
		private readonly Dictionary<string, ushort> locationToIndex = new Dictionary<string, ushort>();

		private string preset = "";
		private string seedName = "";
		private uint roomCount = 2;
		private bool inGame = false;
		private bool gameReset = true;
		private bool secondCastle = false;
		private bool restarted = false;
		private ReplayState[] replay = new ReplayState[20000];
		private int replayLenght = 0;
		private ushort prologueTime = 0;
		private Autosplitter autosplitter;
		private bool autosplitterConnected = false;
		private ushort autosplitterReconnectCounter = 0;
		private bool draculaSpawned = false;
		private byte currentMapX = (byte) 0;
		private byte currentMapY = (byte) 0;
		private byte currentReplayCooldown = 0;
		private bool muted = false;
		private bool started = false;
		private bool finished = false;
		private bool replaySaved = false;
		private TimeSpan finalTime;
		private Stopwatch stopWatch = new Stopwatch();

		public Tracker(IToolConfig toolConfig, ISotnApi sotnApi, INotificationService notificationService)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));
			this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

			if (toolConfig.Tracker.UseOverlay)
			{
				notificationService.StartOverlayServer();
			}
			if (toolConfig.Tracker.EnableAutosplitter)
			{
				autosplitter = new();
			}
			this.SeedInfo = DefaultSeedInfo;
		}

		public string SeedInfo { get; set; }
		public Locations Locations
		{
			get
			{
				return this.locations;
			}
		}

		public void Update()
		{
			UpdateSeedLabel();

			if (toolConfig.Tracker.Stereo && sotnApi.GameApi.Status == Status.MainMenu)
			{
				sotnApi.GameApi.EnableStartWithStereo();
			}

			inGame = sotnApi.GameApi.Status == Status.InGame;
			bool updatedSecondCastle = sotnApi.GameApi.SecondCastle;
			currentReplayCooldown++;
			currentMapX = (byte) sotnApi.AlucardApi.MapX;
			currentMapY = (byte) sotnApi.AlucardApi.MapY;

			if (started && !finished && currentReplayCooldown > ReplayCooldown)
			{
				SaveReplayLine();
				currentReplayCooldown = 0;
			}

			if (sotnApi.GameApi.InAlucardMode() && sotnApi.AlucardApi.HasHitbox())
			{
				restarted = false;
				bool objectChanges = false;

				if (updatedSecondCastle != secondCastle && toolConfig.Tracker.Locations)
				{
					secondCastle = updatedSecondCastle;
					CheckReachability();
					ColorAllLocations();
				}
				else if (updatedSecondCastle != secondCastle)
				{
					secondCastle = updatedSecondCastle;
				}

				objectChanges = objectChanges || UpdateRelics();
				objectChanges = objectChanges || UpdateProgressionItems();
				objectChanges = objectChanges || UpdateThrustSwords();
				objectChanges = objectChanges || UpdateTimeAttacks();
				if (objectChanges)
				{
					UpdateOverlay();
					if (toolConfig.Tracker.Locations)
					{
						CheckReachability();
					}
				}

				if (toolConfig.Tracker.Locations)
				{
					UpdateLocations();
				}
				if (gameReset && toolConfig.Tracker.Locations)
				{
					ColorAllLocations();
					gameReset = false;
				}
				if (!LocationsDrawn() && toolConfig.Tracker.Locations)
				{
					ColorAllLocations();
				}
			}
			else if (!inGame)
			{
				gameReset = true;
			}
			else if (sotnApi.GameApi.InPrologue())
			{
				prologueTime++;
				if (!restarted)
				{
					ResetToDefaults();
					restarted = true;
				}
			}

			if (toolConfig.Tracker.EnableAutosplitter && !started && !autosplitterConnected && autosplitterReconnectCounter == AutosplitterReconnectCooldown)
			{
				autosplitterConnected = autosplitter.AtemptConnect();
				autosplitterReconnectCounter = 0;
			}
			else if (toolConfig.Tracker.EnableAutosplitter && !started && !autosplitterConnected && autosplitterReconnectCounter < 120 && !sotnApi.GameApi.InAlucardMode())
			{
				autosplitterReconnectCounter++;
			}
			else if (toolConfig.Tracker.EnableAutosplitter && !started && autosplitterConnected && !sotnApi.GameApi.InAlucardMode())
			{
				autosplitterConnected = autosplitter.IsConnected();
			}

			CheckStart();
			CheckSplit();
			MuteMusic();
		}

		private void MuteMusic()
		{
			uint currentStatus = sotnApi.GameApi.Status;
			if (!toolConfig.Tracker.MuteMusic || (currentStatus != Status.InGame && currentStatus != Status.MainMenu))
			{
				return;
			}

			bool currentTrackIsSong = sotnApi.GameApi.MusicTrack <= Various.MusicTrackValues.Last();

			if (!muted && currentTrackIsSong)
			{
				sotnApi.GameApi.MuteXA();
				muted = true;
			}
			else if (muted && !currentTrackIsSong)
			{
				sotnApi.GameApi.UnmuteXA();
				muted = false;
			}
		}

		private unsafe bool LoadExtension(string extensionFilePath)
		{
			if (!File.Exists(extensionFilePath))
			{
				return false;
			}

			Extension? extension = JsonConvert.DeserializeObject<Extension>(File.ReadAllText(extensionFilePath));
			if (extension.Extends != String.Empty)
			{
				LoadExtension(Paths.ExtensionPath + extension.Extends + ".json");
			}

			for (int i = 0; i < extension.Locations.Count; i++)
			{
				locationToIndex.Add(extension.Locations[i].Name, (ushort) locations.stateCount);
				locations.AddState(new LocationState { x = (byte) extension.Locations[i].X, y = (byte) extension.Locations[i].Y, SecondCastle = extension.Locations[i].SecondCastle, SmallIndicator = extension.SmallIndicators, availabilityColor = MapColor.Available });

				for (int j = 0; j < extension.Locations[i].Rooms.Count; j++)
				{
					ushort index = (ushort) (Convert.ToInt64(extension.Locations[i].Rooms[j].Address, 16) - SotnApi.Constants.Addresses.Game.MapStart);
					locations.AddRoom(new LocationRoom { roomIndex = index, stateIndex = (ushort) (locations.stateCount - 1), values = Convert.ToByte(extension.Locations[i].Rooms[j].Values, 16) });
				}
			}

			return true;
		}

		private void LoadParentPreset(string presetFilePath, Dictionary<string, LockLocation> uniqueLocks, Dictionary<string, LockLocation> uniqueAllowedLocks)
		{
			if (!File.Exists(presetFilePath))
			{
				return;
			}

			Preset? preset = JsonConvert.DeserializeObject<Preset>(File.ReadAllText(presetFilePath));
			if (preset.Inherits != null)
			{
				LoadParentPreset(Paths.ExtensionPath + preset.Inherits + ".json", uniqueLocks, uniqueAllowedLocks);
			}

			for (int i = 0; i < preset.LockLocations.Count; i++)
			{
				if (!uniqueLocks.ContainsKey(preset.LockLocations[i].Name))
				{
					uniqueLocks.Add(preset.LockLocations[i].Name, preset.LockLocations[i]);
					continue;
				}
				uniqueLocks[preset.LockLocations[i].Name] = preset.LockLocations[i];
			}

			for (int i = 0; i < preset.LockLocationsAllowed.Count; i++)
			{
				if (!uniqueAllowedLocks.ContainsKey(preset.LockLocationsAllowed[i].Name))
				{
					uniqueAllowedLocks.Add(preset.LockLocationsAllowed[i].Name, preset.LockLocationsAllowed[i]);
					continue;
				}
				uniqueAllowedLocks[preset.LockLocationsAllowed[i].Name] = preset.LockLocationsAllowed[i];
			}
		}

		private void LoadPreset(string presetFilePath)
		{
			if (!File.Exists(presetFilePath))
			{
				return;
			}

			Preset? preset = JsonConvert.DeserializeObject<Preset>(File.ReadAllText(presetFilePath));
			Preset? speedrunPreset = JsonConvert.DeserializeObject<Preset>(File.ReadAllText(Paths.SpeedrunPresetPath));

			if (preset.Metadata.Id == "glitch")
			{
				LoadExtension(Paths.ExtensionPath + "equipment.json");
			}

			if (preset.RelicLocationsExtension == "false")
			{
				LoadExtension(Paths.ExtensionPath + "classic.json");
			}
			else if (preset.RelicLocationsExtension == null)
			{
				LoadExtension(Paths.ExtensionPath + "guarded.json");
			}
			else
			{
				LoadExtension(Paths.ExtensionPath + preset.RelicLocationsExtension + ".json");
			}
			Dictionary<string, string> aliases = new Dictionary<string, string>();
			for (int i = 0; i < preset.Aliases.Count; i++)
			{
				aliases.Add(preset.Aliases[i].Replaced, preset.Aliases[i].Relic);
			}

			Dictionary<string, LockLocation> uniqueLocks = new Dictionary<string, LockLocation>();
			Dictionary<string, LockLocation> uniqueAllowedLocks = new Dictionary<string, LockLocation>();
			for (int i = 0; i < speedrunPreset.LockLocations.Count; i++)
			{
				uniqueAllowedLocks.Add(speedrunPreset.LockLocations[i].Name, speedrunPreset.LockLocations[i]);
			}
			if (preset.Inherits != null)
			{
				LoadParentPreset(Paths.PresetPath + preset.Inherits + ".json", uniqueLocks, uniqueAllowedLocks);
			}
			for (int i = 0; i < preset.LockLocations.Count; i++)
			{
				if (!uniqueLocks.ContainsKey(preset.LockLocations[i].Name))
				{
					uniqueLocks.Add(preset.LockLocations[i].Name, preset.LockLocations[i]);
					continue;
				}
				uniqueLocks[preset.LockLocations[i].Name] = preset.LockLocations[i];
			}
			for (int i = 0; i < preset.LockLocationsAllowed.Count; i++)
			{
				if (!uniqueAllowedLocks.ContainsKey(preset.LockLocationsAllowed[i].Name))
				{
					uniqueAllowedLocks.Add(preset.LockLocationsAllowed[i].Name, preset.LockLocationsAllowed[i]);
					continue;
				}
				uniqueAllowedLocks[preset.LockLocationsAllowed[i].Name] = preset.LockLocationsAllowed[i];
			}
			LockLocation[] finalLocks = uniqueLocks.Values.ToArray();
			LockLocation[] finalLocksAllowed = uniqueAllowedLocks.Values.ToArray();

			for (int i = 0; i < finalLocks.Length; i++)
			{
				string locationName = finalLocks[i].Name;
				if (!locationToIndex.ContainsKey(locationName))
				{
					continue;
				}
				if (finalLocks[i].Locks.Count == 0)
				{
					locations.states[locationToIndex[locationName]].availabilityColor = MapColor.Available;
					LocationLock llock = new LocationLock();
					llock.stateIndex = locationToIndex[locationName];
					locations.AddLock(llock);
					continue;
				}
				locations.states[locationToIndex[locationName]].availabilityColor = MapColor.Allowed;
				for (int j = 0; j < finalLocks[i].Locks.Count; j++)
				{
					LocationLock llock = new LocationLock();
					llock.stateIndex = locationToIndex[locationName];
					string[] lockSet = finalLocks[i].Locks[j].Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
					for (int k = 0; k < lockSet.Length; k++)
					{
						string ability;
						if (!aliases.TryGetValue(lockSet[k], out ability))
						{
							ability = lockSet[k];
						}
						byte abilityIndex;
						if (!abilityToIndex.TryGetValue(ability, out abilityIndex))
						{
							continue;
						}
						if (abilityIndex < relics.Length)
						{
							relics[abilityIndex].Progression = true;
						}
						llock.flags |= abilityToFlag[ability];
					}
					locations.AddLock(llock);
				}
			}

			for (int i = 0; i < finalLocksAllowed.Length; i++)
			{
				string locationName = finalLocksAllowed[i].Name;
				if (!locationToIndex.ContainsKey(locationName))
				{
					continue;
				}
				if (finalLocksAllowed[i].Locks.Count == 0)
				{
					locations.states[locationToIndex[locationName]].availabilityColor = MapColor.Allowed;
					LocationLock llock = new LocationLock();
					llock.stateIndex = locationToIndex[locationName];
					locations.AddAllowedLock(llock);
					continue;
				}
				locations.states[locationToIndex[locationName]].availabilityColor = MapColor.Unavailable;
				for (int j = 0; j < finalLocksAllowed[i].Locks.Count; j++)
				{
					LocationLock llock = new LocationLock();
					llock.stateIndex = locationToIndex[locationName];
					string[] lockSet = finalLocksAllowed[i].Locks[j].Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
					for (int k = 0; k < lockSet.Length; k++)
					{
						string ability;
						if (!aliases.TryGetValue(lockSet[k], out ability))
						{
							ability = lockSet[k];
						}
						byte abilityIndex;
						if (!abilityToIndex.TryGetValue(ability, out abilityIndex))
						{
							continue;
						}
						if (abilityIndex < relics.Length)
						{
							relics[abilityIndex].Progression = true;
						}
						llock.flags |= abilityToFlag[ability];
					}
					locations.AddAllowedLock(llock);
				}
			}
		}

		private void ResetToDefaults()
		{
			roomCount = 2;
			for (int i = 0; i < relics.Length; i++)
			{
				relics[i].Collected = false;
			}
			for (int i = 0; i < progressionItems.Length; i++)
			{
				progressionItems[i].Status = false;
			}
			for (int i = 0; i < thrustSwords.Length; i++)
			{
				thrustSwords[i].Status = false;
			}
			locations.Reset();
			if (toolConfig.Tracker.Locations)
			{
				CheckReachability();
			}
		}

		private void UpdateSeedLabel()
		{
			if (SeedInfo == DefaultSeedInfo && sotnApi.GameApi.Status == Status.MainMenu)
			{
				GetSeedData();
				sotnApi.GameApi.RemoveMapRevealCheck();
			}
		}

		private void UpdateLocations()
		{
			uint currentRooms = sotnApi.AlucardApi.Rooms;
			if (currentRooms <= roomCount)
			{
				roomCount = currentRooms;
				return;
			}

			for (int i = 0; i < locations.roomCount; i++)
			{
				if (locations.states[locations.rooms[i].stateIndex].Visited)
				{
					continue;
				}
				byte rmv = sotnApi.GameApi.GetRoomValue(locations.rooms[i].roomIndex);
				if (locations.rooms[i].Checked(sotnApi.GameApi.GetRoomValue(locations.rooms[i].roomIndex)))
				{
					locations.states[locations.rooms[i].stateIndex].Visited = true;
					uint x = locations.states[locations.rooms[i].stateIndex].x;
					uint y = locations.states[locations.rooms[i].stateIndex].y;
					if (x <= 0 || y <= 0)
					{
						return;
					}
					if (secondCastle)
					{
						x = 252 - x;
						y = 199 - y;
					}
					sotnApi.MapApi.ClearMapRoom((byte) x, (byte) y, (byte) locations.states[locations.rooms[i].stateIndex].availabilityColor, (byte) MapColor.Border);
				}
			}

			roomCount = currentRooms;
		}

		private bool UpdateRelics()
		{
			int changes = 0;
			for (int i = 0; i < relics.Length; i++)
			{
				if (sotnApi.AlucardApi.HasRelic((Relic) i))
				{
					if (relicCollectionTimes[i] == 0)
					{
						relics[i].X = currentMapX;
						if (secondCastle)
						{
							relics[i].X += 100;
						}
						relics[i].Y = currentMapY;
						relicCollectionTimes[i] = (ushort) replayLenght;
					}
					if (!relics[i].Collected)
					{
						changes++;
						relics[i].Collected = true;
						relicsFlags |= (1 << i);
					}
				}
				else
				{
					if (relics[i].Collected)
					{
						changes++;
						relics[i].Collected = false;
						relicsFlags &= (byte) ~(1 << i);
					}
				}
			}

			return changes > 0;
		}

		private bool UpdateProgressionItems()
		{
			int changes = 0;
			for (int i = 0; i < progressionItems.Length; i++)
			{
				if (sotnApi.AlucardApi.HasItemInInventory(progressionItems[i].Index))
				{
					if (progressionItems[i].CollectedAt == 0)
					{
						progressionItems[i].X = currentMapX;
						if (secondCastle)
						{
							progressionItems[i].X += 100;
						}
						progressionItems[i].Y = currentMapY;
						progressionItems[i].CollectedAt = (ushort) replayLenght;
					}
					progressionItems[i].Collected = true;
					itemsFlags |= (1 << i);
				}
				else
				{
					progressionItems[i].Collected = false;
					itemsFlags &= (byte) ~(1 << i);
				}
			}

			for (int i = 0; i < progressionItems.Length; i++)
			{
				switch (i)
				{
					case 0:
					case 1:
						progressionItems[i].Equipped = (sotnApi.AlucardApi.Accessory1 == progressionItems[i].Value) || (sotnApi.AlucardApi.Accessory2 == progressionItems[i].Value);
						break;
					case 2:
						progressionItems[i].Equipped = (sotnApi.AlucardApi.Armor == progressionItems[i].Value);
						break;
					case 3:
						progressionItems[i].Equipped = (sotnApi.AlucardApi.Helm == progressionItems[i].Value);
						break;
					default:
						progressionItems[i].Equipped = false;
						break;
				}

				if (!progressionItems[i].Status && (progressionItems[i].Collected || progressionItems[i].Equipped))
				{
					progressionItems[i].Status = true;
					changes++;
				}
				else if (progressionItems[i].Status && !(progressionItems[i].Collected || progressionItems[i].Equipped))
				{
					progressionItems[i].Status = false;
					changes++;
				}
			}

			return changes > 0;
		}

		private bool UpdateThrustSwords()
		{
			int changes = 0;
			for (int i = 0; i < thrustSwords.Length; i++)
			{
				if (sotnApi.AlucardApi.HasItemInInventory(thrustSwords[i].Index))
				{
					if (thrustSwords[i].CollectedAt == 0)
					{
						thrustSwords[i].X = currentMapX;
						if (secondCastle)
						{
							thrustSwords[i].X += 100;
						}
						thrustSwords[i].Y = currentMapY;
						thrustSwords[i].CollectedAt = (ushort) replayLenght;
					}
					thrustSwords[i].Collected = true;

				}
				else
				{
					thrustSwords[i].Collected = false;
				}
			}

			for (int i = 0; i < thrustSwords.Length; i++)
			{
				thrustSwords[i].Equipped = (sotnApi.AlucardApi.RightHand == thrustSwords[i].Value);

				if (!thrustSwords[i].Status && (thrustSwords[i].Collected || thrustSwords[i].Equipped))
				{
					thrustSwords[i].Status = true;
					changes++;
				}
				else if (thrustSwords[i].Status && !(thrustSwords[i].Collected || thrustSwords[i].Equipped))
				{
					thrustSwords[i].Status = false;
					changes++;
				}
			}

			bool hasSword = false;

			for (int i = 0; i < thrustSwords.Length; i++)
			{
				if (thrustSwords[i].Equipped || thrustSwords[i].Collected)
				{
					hasSword = true;
				}
			}

			if (hasSword)
			{
				itemsFlags |= (1 << 4);
			}
			else
			{
				itemsFlags &= unchecked((byte) ~(1 << 4));
			}


			return changes > 0;
		}

		private bool UpdateTimeAttacks()
		{
			if (!allBossesGoal)
			{
				return false;
			}

			int changes = 0;
			for (int i = 0; i < timeAttacks.Length - 2; i++)
			{
				bool state = sotnApi.GameApi.GetTimeAttack((Times) (i + 1)) > 0;
				if ((!timeAttacks[i] && state) || (timeAttacks[i] && !state))
				{
					if (state)
					{
						bossesFlags |= (1 << i);
					}
					else
					{
						bossesFlags &= (byte) ~(1 << i);
					}
					changes++;
				}
				timeAttacks[i] = state;
			}

			return changes > 0;
		}

		private void GetSeedData()
		{
			seedName = sotnApi.GameApi.ReadSeedName();
			preset = sotnApi.GameApi.ReadPresetName();
			if (preset == "tournament" || preset == "")
			{
				preset = "custom";
			}
			if (preset == "custom" || !File.Exists(Paths.PresetPath + preset + ".json"))
			{
				LoadExtension(Paths.ExtensionPath + toolConfig.Tracker.CustomExtension + ".json");
			}
			else
			{
				LoadPreset(Paths.PresetPath + preset + ".json");
			}
			SeedInfo = seedName + "(" + preset + ")";
			SaveSeedInfo(SeedInfo);
			allBossesGoal = sotnApi.GameApi.AllBossesGoal;
		}

		private void ColorAllLocations()
		{
			for (int i = 0; i < locations.stateCount; i++)
			{
				if (!locations.states[i].Visited && locations.states[i].SecondCastle == secondCastle)
				{
					ColorMapRoom(i, (byte) locations.states[i].availabilityColor, locations.states[i].SecondCastle);
				}
			}
		}

		//TODO: portal spell discovery
		private void ColorMapRoom(int locationIndex, byte color, bool secondCastle)
		{
			uint x = (uint) locations.states[locationIndex].x;
			uint y = (uint) locations.states[locationIndex].y;
			if (x <= 0 || y <= 0)
			{
				return;
			}
			if (secondCastle)
			{
				x = 252 - x;
				y = 199 - y;
			}

			if (locations.states[locationIndex].SmallIndicator)
			{
				if (secondCastle)
				{
					x += 2;
					y += 1;
				}
				sotnApi.MapApi.ColorMapLocation((byte) x, (byte) y, color);
			}
			else
			{
				sotnApi.MapApi.ColorMapRoom((byte) x, (byte) y, color, (byte) MapColor.Border);
			}
		}

		private bool LocationsDrawn()
		{
			int uncheckedLocationIndex = -1;

			for (int i = 0; i < locations.stateCount; i++)
			{
				if (!locations.states[i].Visited && locations.states[i].SecondCastle == secondCastle)
				{
					uncheckedLocationIndex = i;
					break;
				}
			}

			if (uncheckedLocationIndex == -1)
			{
				return true;
			}

			uint x = (uint) locations.states[uncheckedLocationIndex].x;
			uint y = (uint) locations.states[uncheckedLocationIndex].y;
			if (secondCastle)
			{
				x = 252 - x;
				y = 199 - y;
				if (locations.states[uncheckedLocationIndex].SmallIndicator)
				{
					x += 2;
					y += 1;
				}
			}
			if (!sotnApi.MapApi.RoomIsRendered((byte) x, (byte) y, (byte) locations.states[uncheckedLocationIndex].availabilityColor))
			{
				return false;
			}

			return true;
		}

		private void CheckReachability()
		{
			for (int i = 0; i < locations.stateCount; i++)
			{
				locations.states[i].ReachabilityChanged = false;
			}
			for (int i = 0; i < locations.lockCount; i++)
			{
				bool unlock = true;
				if (locations.states[locations.locks[i].stateIndex].Visited || locations.states[locations.locks[i].stateIndex].ReachabilityChanged)
				{
					continue;
				}
				locations.states[locations.locks[i].stateIndex].availabilityColor = MapColor.Unavailable;
				for (int j = 0; j < abilityFlags.Length; j++)
				{
					if ((locations.locks[i].flags & abilityFlags[j]) == 0)
					{
						continue;
					}
					if (j < relics.Length)
					{
						if (!relics[j].Collected)
						{
							unlock = false;
							break;
						}
					}
					else if (j < (relics.Length + progressionItems.Length))
					{
						if (!(progressionItems[j - relics.Length].Collected || progressionItems[j - relics.Length].Equipped))
						{
							unlock = false;
							break;
						}
					}
					else
					{
						unlock = false;
						for (int k = 0; k < thrustSwords.Length; k++)
						{
							if (thrustSwords[k].Collected || thrustSwords[k].Equipped)
							{
								unlock = true;
								break;
							}
						}
					}
				}
				if (unlock)
				{
					locations.states[locations.locks[i].stateIndex].ReachabilityChanged = true;
					locations.states[locations.locks[i].stateIndex].availabilityColor = MapColor.Available;
				}
			}

			for (int i = 0; i < locations.stateCount; i++)
			{
				locations.states[i].ReachabilityChanged = false;
			}
			for (int i = 0; i < locations.allowedLockCount; i++)
			{
				bool unlock = true;
				if (locations.states[locations.allowedLocks[i].stateIndex].Visited
					|| locations.states[locations.allowedLocks[i].stateIndex].ReachabilityChanged
					|| locations.states[locations.allowedLocks[i].stateIndex].availabilityColor != MapColor.Unavailable)
				{
					continue;
				}
				for (int j = 0; j < abilityFlags.Length; j++)
				{
					if ((locations.allowedLocks[i].flags & abilityFlags[j]) == 0)
					{
						continue;
					}
					if (j < relics.Length)
					{
						if (!relics[j].Collected)
						{
							unlock = false;
							break;
						}
					}
					else if (j < (relics.Length + progressionItems.Length))
					{
						if (!(progressionItems[j - relics.Length].Collected || progressionItems[j - relics.Length].Equipped))
						{
							unlock = false;
							break;
						}
					}
					else
					{
						unlock = false;
						for (int k = 0; k < thrustSwords.Length; k++)
						{
							if (thrustSwords[k].Collected || thrustSwords[k].Equipped)
							{
								unlock = true;
								break;
							}
						}
					}
				}
				if (unlock)
				{
					locations.states[locations.allowedLocks[i].stateIndex].availabilityColor = MapColor.Allowed;
				}
			}

			ColorAllLocations();
		}

		private void UpdateOverlay()
		{
			if (toolConfig.Tracker.UseOverlay)
			{
				notificationService.UpdateTrackerOverlay(relicsFlags, itemsFlags, bossesFlags);
			}
		}

		private void SaveReplayLine()
		{
			byte replayX = currentMapX;
			byte replayY = currentMapY;

			if (currentMapX < 5 && currentMapY < 4)
			{
				if (sotnApi.GameApi.StageId == (uint) SotnApi.Constants.Values.Game.Enums.Stage.Prologue)
				{
					replayX += 2;
					replayY += 32;
				}
				else if (sotnApi.GameApi.StageId == (uint) SotnApi.Constants.Values.Game.Enums.Stage.Nightmare)
				{
					replayX += 46;
					replayY += 33;
				}
			}

			if ((inGame && (replayX > 1 && replayY > 0) && (replayX < 200 && replayY < 200) && !(replayY == 44 && replayX < 19 && !secondCastle)) && (replayLenght == 0 || (replay[replayLenght - 1].X != replayX || replay[replayLenght - 1].Y != replayY)))
			{
				replayLenght++;
				replay[replayLenght - 1].X = (byte) (replayX + (secondCastle ? 100 : 0));
				replay[replayLenght - 1].Y = replayY;
			}

			replay[replayLenght - 1].Time++;
		}

		public void SaveReplay()
		{
			if (replaySaved || replayLenght < 30)
			{
				return;
			}

			if (!finished)
			{
				stopWatch.Stop();
				finished = true;
				finalTime = stopWatch.Elapsed;
			}

			string username = toolConfig.Tracker.Username;

			if (username is not null && username.Length > 18)
			{
				username = username.Substring(0, 18);
			}

			byte version = 2;
			string baseReplayPath = seedName + preset.ToUpper() + "-" + username;
			string replayPath = baseReplayPath;
			while (File.Exists(Paths.ReplaysPath + replayPath + ".sotnr"))
			{
				replayPath = version + baseReplayPath;
				version++;
				if (version > 10)
				{
					return;
				}
			}
			replayPath = Paths.ReplaysPath + replayPath + ".sotnr";

			byte[] replayBytes = new byte[2 + (replayLenght * 4) + ((relics.Length + progressionItems.Length + 1) * 4)];

			int replayIndex = 0;
			byte[] finalTimeSecondsBytes = BitConverter.GetBytes((ushort) finalTime.TotalSeconds);
			replayBytes[replayIndex] = finalTimeSecondsBytes[0];
			replayIndex++;
			replayBytes[replayIndex] = finalTimeSecondsBytes[1];
			replayIndex++;

			for (int i = 0; i < relics.Length; i++)
			{
				replayBytes[replayIndex] = relics[i].X;
				replayIndex++;
				replayBytes[replayIndex] = relics[i].Y;
				replayIndex++;

				byte[] colletedBytes = BitConverter.GetBytes(relicCollectionTimes[i]);
				replayBytes[replayIndex] = colletedBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = colletedBytes[1];
				replayIndex++;
			}

			for (int i = 0; i < progressionItems.Length; i++)
			{
				replayBytes[replayIndex] = progressionItems[i].X;
				replayIndex++;
				replayBytes[replayIndex] = progressionItems[i].Y;
				replayIndex++;

				byte[] colletedBytes = BitConverter.GetBytes(progressionItems[i].CollectedAt);
				replayBytes[replayIndex] = colletedBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = colletedBytes[1];
				replayIndex++;
			}

			int foundIndex = -1;
			for (int i = 0; i < thrustSwords.Length; i++)
			{
				if (thrustSwords[i].CollectedAt > 0)
				{
					foundIndex = i;
				}
			}

			if (foundIndex > -1)
			{
				replayBytes[replayIndex] = thrustSwords[foundIndex].X;
				replayIndex++;
				replayBytes[replayIndex] = thrustSwords[foundIndex].Y;
				replayIndex++;

				byte[] colletedBytes = BitConverter.GetBytes(thrustSwords[foundIndex].CollectedAt);
				replayBytes[replayIndex] = colletedBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = colletedBytes[1];
				replayIndex++;
			}
			else
			{
				replayBytes[replayIndex] = 0;
				replayIndex++;
				replayBytes[replayIndex] = 0;
				replayIndex++;

				replayBytes[replayIndex] = 0;
				replayIndex++;
				replayBytes[replayIndex] = 0;
				replayIndex++;
			}

			for (int i = 0; i < replayLenght; i++)
			{
				ReplayState state = replay[i];
				replayBytes[replayIndex] = state.X;
				replayIndex++;
				replayBytes[replayIndex] = state.Y;
				replayIndex++;

				byte[] timeBytes = BitConverter.GetBytes(state.Time);
				replayBytes[replayIndex] = timeBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = timeBytes[1];
				replayIndex++;
			}

			File.WriteAllBytes(replayPath, replayBytes);
			replaySaved = true;
		}

		private static void SaveSeedInfo(string info)
		{
			if (File.Exists(Paths.SeedInfoPath))
			{
				File.WriteAllText(Paths.SeedInfoPath, info);
			}
			else
			{
				using (StreamWriter sw = File.CreateText(Paths.SeedInfoPath))
				{
					sw.Write(info);
				}
			}
		}

		private void CheckStart()
		{
			if (sotnApi.GameApi.Hours == 0 && sotnApi.GameApi.Minutes == 0 && sotnApi.GameApi.Seconds == 3 && inGame)
			{
				if (toolConfig.Tracker.EnableAutosplitter)
				{
					autosplitter.StartTImer();
				}
				if (!started)
				{
					started = true;
					stopWatch.Start();
				}
			}
		}

		private void CheckSplit()
		{
			if (sotnApi.AlucardApi.MapX == 31 && sotnApi.AlucardApi.MapY == 30 && sotnApi.GameApi.Status == Status.InGame)
			{
				var boss = sotnApi.EntityApi.GetLiveEntity(DraculaEntityAddress);
				if (boss.Hp > 13 && boss.Hp < 10000 && boss.UpdateFunctionAddress != 0)
				{
					draculaSpawned = true;
				}
				else if (draculaSpawned && boss.Hp < 1 && boss.UpdateFunctionAddress != 0)
				{
					if (toolConfig.Tracker.EnableAutosplitter)
					{
						autosplitter.Split();
					}
					stopWatch.Stop();
					finished = true;
					finalTime = stopWatch.Elapsed;
					if (toolConfig.Tracker.SaveReplays)
					{
						SaveReplay();
					}
				}
			}
			else
			{
				draculaSpawned = false;
			}
		}

		public void CloseAutosplitter()
		{
			if (toolConfig.Tracker.EnableAutosplitter)
			{
				autosplitter.Disconnect();
			}
		}
	}
}

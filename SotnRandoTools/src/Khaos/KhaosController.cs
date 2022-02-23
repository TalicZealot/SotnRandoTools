using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Constants.Addresses;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Services.Models;
using MapLocation = SotnRandoTools.RandoTracker.Models.MapLocation;

namespace SotnRandoTools.Khaos
{
	public class KhaosController : IKhaosController
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly ICheatCollectionAdapter cheats;
		private readonly INotificationService notificationService;
		private readonly IInputService inputService;
		private readonly IKhaosActionsInfoDisplay statusInfoDisplay;
		private Random rng = new();

		private List<QueuedAction> queuedActions = new();
		private Queue<MethodInvoker> queuedFastActions = new();
		#region Timers
		private System.Windows.Forms.Timer actionTimer = new();
		private System.Windows.Forms.Timer fastActionTimer = new();
		private System.Windows.Forms.Timer khaosTrackTimer = new();
		private System.Windows.Forms.Timer subweaponsOnlyTimer = new();
		private System.Windows.Forms.Timer slowTimer = new();
		private System.Windows.Forms.Timer bloodManaTimer = new();
		private System.Windows.Forms.Timer thirstTimer = new();
		private System.Windows.Forms.Timer thirstTickTimer = new();
		private System.Windows.Forms.Timer hordeTimer = new();
		private System.Windows.Forms.Timer hordeSpawnTimer = new();
		private System.Windows.Forms.Timer lordTimer = new();
		private System.Windows.Forms.Timer lordSpawnTimer = new();
		private System.Windows.Forms.Timer enduranceSpawnTimer = new();
		private System.Windows.Forms.Timer hnkTimer = new();
		private System.Windows.Forms.Timer vampireTimer = new();
		private System.Windows.Forms.Timer magicianTimer = new();
		private System.Windows.Forms.Timer meltyTimer = new();
		private System.Windows.Forms.Timer fourBeastsTimer = new();
		private System.Windows.Forms.Timer zawarudoTimer = new();
		private System.Windows.Forms.Timer zawarudoCheckTimer = new();
		private System.Windows.Forms.Timer hasteTimer = new();
		private System.Windows.Forms.Timer hasteOverdriveTimer = new();
		private System.Windows.Forms.Timer hasteOverdriveOffTimer = new();
		private System.Windows.Forms.Timer bloodManaDeathTimer = new();
		private System.Windows.Forms.Timer battleOrdersTimer = new();
		private System.Windows.Forms.Timer azureDragonTimer = new();
		private System.Windows.Forms.Timer whiteTigerBallTimer = new();
		#endregion
		#region Cheats
		private Cheat faerieScroll;
		private Cheat darkMetamorphasisCheat;
		private Cheat underwaterPhysics;
		private Cheat hearts;
		private Cheat curse;
		private Cheat manaCheat;
		private Cheat attackPotionCheat;
		private Cheat defencePotionCheat;
		private Cheat stopwatchTimer;
		private Cheat hitboxWidth;
		private Cheat hitboxHeight;
		private Cheat hitbox2Width;
		private Cheat hitbox2Height;
		private Cheat invincibilityCheat;
		private Cheat shineCheat;
		private Cheat visualEffectPaletteCheat;
		private Cheat visualEffectTimerCheat;
		private Cheat savePalette;
		private Cheat contactDamage;
		private Cheat music;
		#endregion

		private int totalMeterGained = 0;
		private bool pandoraUsed = false;
		private uint alucardMapX = 0;
		private uint alucardMapY = 0;
		private bool alucardSecondCastle = false;
		private bool inMainMenu = false;

		private uint hordeZone = 0;
		private uint hordeZone2 = 0;
		private uint hordeTriggerRoomX = 0;
		private uint hordeTriggerRoomY = 0;
		private List<Actor> hordeEnemies = new();

		private uint lordZone = 0;
		private uint lordZone2 = 0;
		private uint lordTriggerRoomX = 0;
		private uint lordTriggerRoomY = 0;
		private List<Actor> lordEnemies = new();

		private int enduranceCount = 0;
		private uint enduranceRoomX = 0;
		private uint enduranceRoomY = 0;

		private bool zaWarudoActive = false;
		private uint zaWarudoZone = 0;
		private uint zaWarudoZone2 = 0;

		private uint storedMana = 0;
		private int spentMana = 0;

		private int fireballCooldown = 0;
		private List<LiveActor> fireballs = new();
		private List<LiveActor> fireballsUp = new();
		private List<LiveActor> fireballsDown = new();

		private bool speedLocked = false;
		private bool manaLocked = false;
		private bool invincibilityLocked = false;
		private bool bloodManaActive = false;
		private bool hasteActive = false;
		private bool hasteSpeedOn = false;
		private bool overdriveOn = false;
		private bool subweaponsOnlyActive = false;
		private bool gasCloudTaken = false;
		private bool spawnActive = false;
		private bool slowActive = false;
		private bool slowPaused = false;

		private bool azureDragonActive = false;
		private bool vermilionBirdActive = false;
		private bool whiteTigerActive = false;
		private bool blackTortoiseActive = false;

		private bool azureSpiritActive = false;
		private bool whiteTigerBallActive = false;

		private bool azureDragonUsed = false;
		private bool vermilionBirdUsed = false;
		private bool whiteTigerUsed = false;
		private bool blackTortoiseUsed = false;
		private bool darkMetamorphosisCasted = true;
		private bool hellfireCasted = false;

		private bool battleOrdersActive = false;
		private uint battleOrdersBonusHp = 0;
		private uint battleOrdersBonusMp = 0;

		private int slowInterval;
		private int normalInterval;
		private int fastInterval;
		private int autoKhaosDifficulty;

		private bool shaftHpSet = false;
		private bool galamothStatsSet = false;

		private bool superThirst = false;
		private bool superHorde = false;
		private bool superEndurance = false;
		private bool superMelty = false;
		private bool superHaste = false;

		private int bankruptLevel = 1;

		public KhaosController(IToolConfig toolConfig, ISotnApi sotnApi, ICheatCollectionAdapter cheats, INotificationService notificationService, IInputService inputService, IKhaosActionsInfoDisplay statusInfoDisplay)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			if (inputService is null) throw new ArgumentNullException(nameof(inputService));
			if (statusInfoDisplay is null) throw new ArgumentNullException(nameof(statusInfoDisplay));
			this.toolConfig = toolConfig;
			this.sotnApi = sotnApi;
			this.cheats = cheats;
			this.notificationService = notificationService;
			this.inputService = inputService;
			this.statusInfoDisplay = statusInfoDisplay;

			InitializeTimers();
			GetCheats();
			statusInfoDisplay.ActionQueue = queuedActions;
			normalInterval = (int) toolConfig.Khaos.QueueInterval.TotalMilliseconds;
			slowInterval = (int) normalInterval * 2;
			fastInterval = (int) normalInterval / 2;

			switch (toolConfig.Khaos.AutoKhaosDifficulty)
			{
				case "Easy":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyEasy;
					break;
				case "Normal":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyNormal;
					break;
				case "Hard":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyHard;
					break;
				default:
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyNormal;
					break;
			}

			Console.WriteLine($"Intervals set. \n normal: {normalInterval / 1000}s, slow:{slowInterval / 1000}s, fast:{fastInterval / 1000}s");
		}

		public bool AutoKhaosOn { get; set; }

		public void StartKhaos()
		{
			InitializeTimerIntervals();
			actionTimer.Start();
			fastActionTimer.Start();
			StartCheats();
			DateTime startedAt = DateTime.Now;
			Parallel.ForEach(toolConfig.Khaos.Actions, (action) =>
			{
				if (action.StartsOnCooldown)
				{
					action.LastUsedAt = startedAt;
				}
				else
				{
					action.LastUsedAt = null;
				}
			});

			notificationService.StartOverlayServer();
			notificationService.AddMessage($"Khaos started");
			Console.WriteLine("Khaos started");
		}
		public void StopKhaos()
		{
			StopTimers();
			faerieScroll.Disable();
			notificationService.AddMessage($"Khaos stopped");
			Console.WriteLine("Khaos stopped");
		}
		public void OverwriteBossNames(string[] subscribers)
		{
			subscribers = subscribers.OrderBy(x => rng.Next()).ToArray();
			IOrderedEnumerable<KeyValuePair<string, long>>? randomizedBosses = Strings.BossNameAddresses.OrderBy(x => rng.Next());
			int i = 0;
			foreach (KeyValuePair<string, long> boss in randomizedBosses)
			{
				if (i == subscribers.Length)
				{
					break;
				}
				sotnApi.GameApi.OverwriteString(boss.Value, subscribers[i]);
				Console.WriteLine($"{boss.Key} renamed to {subscribers[i]}");
				i++;
			}
		}

		#region Khaotic Effects
		//TODO: random action
		public void KhaosStatus(string user = "Khaos")
		{
			bool entranceCutscene = IsInRoomList(Constants.Khaos.EntranceCutsceneRooms);
			bool succubusRoom = IsInRoomList(Constants.Khaos.SuccubusRoom);
			bool alucardIsImmuneToCurse = sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad)
				|| Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Coral circlet";
			bool alucardIsImmuneToStone = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Mirror cuirass"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Medusa shield"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Medusa shield";
			bool alucardIsImmuneToPoison = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Topaz circlet";
			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp - 15;

			int result = RollStatus(entranceCutscene, succubusRoom, alucardIsImmuneToCurse, alucardIsImmuneToStone, alucardIsImmuneToPoison, highHp);

			while (result == 0)
			{
				result = RollStatus(entranceCutscene, succubusRoom, alucardIsImmuneToCurse, alucardIsImmuneToStone, alucardIsImmuneToPoison, highHp);
			}

			switch (result)
			{
				case 1:
					SpawnPoisonHitbox();
					notificationService.AddMessage($"{user} poisoned you");
					break;
				case 2:
					SpawnCurseHitbox();
					notificationService.AddMessage($"{user} cursed you");
					break;
				case 3:
					SpawnStoneHitbox();
					notificationService.AddMessage($"{user} petrified you");
					break;
				case 4:
					SpawnSlamHitbox();
					notificationService.AddMessage($"{user} slammed you");
					break;
				case 5:
					sotnApi.AlucardApi.ActivatePotion(Potion.Antivenom);
					notificationService.AddMessage($"{user} used an antivenom");
					break;
				case 6:
					sotnApi.AlucardApi.ActivatePotion(Potion.StrPotion);
					notificationService.AddMessage($"{user} gave you strength");
					break;
				case 7:
					sotnApi.AlucardApi.Heal(15);
					notificationService.AddMessage($"{user} used a minor heal");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defence");
					break;
				case 9:
					sotnApi.AlucardApi.CurrentMp -= 15;
					notificationService.AddMessage($"{user} used Mana Burn");
					break;
				default:
					break;
			}

			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStatus]);
		}
		public void KhaosEquipment(string user = "Khaos")
		{
			RandomizeEquipmentSlots();
			notificationService.AddMessage($"{user} used Khaos Equipment");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosEquipment]);
		}
		public void KhaosStats(string user = "Khaos")
		{
			RandomizeStatsActivate();
			notificationService.AddMessage($"{user} used Khaos Stats");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStats]);
		}
		public void KhaosRelics(string user = "Khaos")
		{
			RandomizeRelicsActivate(false);
			sotnApi.AlucardApi.GrantItemByName("Library card");
			sotnApi.AlucardApi.GrantItemByName("Library card");
			notificationService.AddMessage($"{user} used Khaos Relics");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosRelics]);
		}
		public void PandorasBox(string user = "Khaos")
		{
			RandomizeGold();
			RandomizeStatsActivate();
			RandomizeEquipmentSlots();
			RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			RandomizeInventory();
			RandomizeSubweapon();
			sotnApi.GameApi.RespawnBosses();
			sotnApi.GameApi.RespawnItems();
			notificationService.AddMessage($"{user} opened Pandora's Box");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.PandorasBox]);
		}
		public void Gamble(string user = "Khaos")
		{
			double goldPercent = rng.NextDouble();
			uint newGold = (uint) ((double) sotnApi.AlucardApi.Gold * goldPercent);
			uint goldSpent = sotnApi.AlucardApi.Gold - newGold;
			sotnApi.AlucardApi.Gold = newGold;
			string item = Equipment.Items[rng.Next(1, Equipment.Items.Count)];
			while (item.Contains("empty hand") || item.Contains("-"))
			{
				item = Equipment.Items[rng.Next(1, Equipment.Items.Count)];
			}
			sotnApi.AlucardApi.GrantItemByName(item);


			notificationService.AddMessage($"{user} gambled {goldSpent} gold for {item}");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Gamble]);
		}
		public void KhaoticBurst(string user = "Khaos")
		{
			notificationService.KhaosMeter += 100;
			notificationService.AddMessage($"{user} used Khaotic Burst");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaoticBurst]);
		}
		public void KhaosTrack(string track, string user = "Khaos")
		{
			uint trackIndex = 0;
			string foundTrack = Constants.Khaos.AcceptedMusicTrackTitles.Where(t => t.ToLower().Trim() == track.ToLower().Trim()).FirstOrDefault();
			bool alternateTitle = Constants.Khaos.AlternateTrackTitles.ContainsKey(track.ToLower().Trim());

			if (foundTrack is not null)
			{
				trackIndex = SotnApi.Constants.Values.Game.Various.MusicTracks[foundTrack];
			}
			else if (alternateTitle)
			{
				foundTrack = Constants.Khaos.AlternateTrackTitles[track.ToLower().Trim()];
				trackIndex = SotnApi.Constants.Values.Game.Various.MusicTracks[foundTrack];
			}
			else
			{
				int roll = rng.Next(0, Constants.Khaos.AcceptedMusicTrackTitles.Length);
				trackIndex = SotnApi.Constants.Values.Game.Various.MusicTracks[Constants.Khaos.AcceptedMusicTrackTitles[roll]];
			}
			music.PokeValue((int) trackIndex);
			music.Enable();
			khaosTrackTimer.Start();
			notificationService.AddMessage($"{user} queued {track}");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack]);
		}
		#endregion
		#region Debuffs
		public void Bankrupt(string user = "Khaos")
		{
			notificationService.AddMessage($"{user} used Bankrupt level {bankruptLevel}");
			BankruptActivate();
			sotnApi.AlucardApi.GrantItemByName("Library card");
			sotnApi.AlucardApi.GrantItemByName("Library card");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Bankrupt]);
		}
		public void Weaken(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			float enhancedFactor = 1;
			if (meterFull)
			{
				enhancedFactor = Constants.Khaos.SuperWeakenFactor;
				SpendKhaosMeter();
			}

			if (battleOrdersActive)
			{
				sotnApi.AlucardApi.MaxtHp -= (uint) battleOrdersBonusHp;
				sotnApi.AlucardApi.MaxtMp -= (uint) battleOrdersBonusMp;
			}

			sotnApi.AlucardApi.CurrentHp = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.CurrentMp = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.CurrentHearts = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.MaxtHp = (uint) (sotnApi.AlucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.MaxtMp = (uint) (sotnApi.AlucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.MaxtHearts = (uint) (sotnApi.AlucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.Str = (uint) (sotnApi.AlucardApi.Str * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.Con = (uint) (sotnApi.AlucardApi.Con * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.Int = (uint) (sotnApi.AlucardApi.Int * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.Lck = (uint) (sotnApi.AlucardApi.Lck * toolConfig.Khaos.WeakenFactor * enhancedFactor);

			if (battleOrdersActive)
			{
				battleOrdersBonusHp = sotnApi.AlucardApi.MaxtHp;
				battleOrdersBonusMp = sotnApi.AlucardApi.MaxtMp;
				sotnApi.AlucardApi.MaxtHp += battleOrdersBonusHp;
				sotnApi.AlucardApi.MaxtMp += battleOrdersBonusMp;
			}

			uint newLevel = (uint) (sotnApi.AlucardApi.Level * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			sotnApi.AlucardApi.Level = newLevel;
			uint newExperience = 0;
			if (newLevel <= StatsValues.ExperienceValues.Length && newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[(int) newLevel - 1];
			}
			else if (newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[StatsValues.ExperienceValues.Length - 1];
			}
			if (newLevel > 1)
			{
				sotnApi.AlucardApi.Level = newLevel;
				sotnApi.AlucardApi.Experiecne = newExperience;
			}

			string message = meterFull ? $"{user} used Super Weaken" : $"{user} used Weaken";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Weaken]);
		}
		public void RespawnBosses(string user = "Khaos")
		{
			sotnApi.GameApi.RespawnBosses();
			notificationService.AddMessage($"{user} used Respawn Bosses");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.RespawnBosses]);
		}
		public void SubweaponsOnly(string user = "Khaos")
		{
			int roll = rng.Next(1, 10);
			while (roll == 6)
			{
				roll = rng.Next(1, 10);
			}
			sotnApi.AlucardApi.Subweapon = (Subweapon) roll;
			sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
			sotnApi.AlucardApi.GrantRelic(Relic.CubeOfZoe);
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			hearts.Enable();
			curse.Enable();
			manaCheat.PokeValue(7);
			manaCheat.Enable();
			manaLocked = true;
			subweaponsOnlyActive = true;
			subweaponsOnlyTimer.Start();
			notificationService.AddMessage($"{user} used Subweapons Only");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly]);
		}
		public void Slow(string user = "Khaos")
		{
			/*bool meterFull = KhaosMeterFull();
			float enhancedFactor = 1;
			if (meterFull)
			{
				enhancedFactor = Constants.Khaos.SuperCrippleFactor;
				SpendKhaosMeter();
			}
			
			speedLocked = true;
			SetSpeed(toolConfig.Khaos.CrippleFactor * enhancedFactor);*/

			if (IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				queuedActions.Add(new QueuedAction { Name = toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Name, LocksSpeed = true, Invoker = new MethodInvoker(() => Slow(user)) });
				return;
			}

			underwaterPhysics.Enable();
			slowTimer.Start();
			slowActive = true;

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Duration
			};

			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			//string message = meterFull ? $"{user} used Super Cripple" : $"{user} used Cripple";
			string message = $"{user} used {toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Name}";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Slow]);
		}
		public void BloodMana(string user = "Khaos")
		{
			storedMana = sotnApi.AlucardApi.CurrentMp;
			bloodManaActive = true;
			bloodManaTimer.Start();
			manaLocked = true;
			notificationService.AddMessage($"{user} used Blood Mana");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana]);
		}
		public void Thirst(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superThirst = true;
				SpendKhaosMeter();
			}

			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();
			thirstTimer.Start();
			thirstTickTimer.Start();

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			string message = meterFull ? $"{user} used Super Thirst" : $"{user} used Thirst";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Thirst]);
		}
		public void Horde(string user = "Khaos")
		{
			hordeTriggerRoomX = sotnApi.GameApi.MapXPos;
			hordeTriggerRoomY = sotnApi.GameApi.MapYPos;
			spawnActive = true;
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superHorde = true;
				SpendKhaosMeter();
			}

			hordeTimer.Start();
			hordeSpawnTimer.Start();
			string message = meterFull ? $"{user} summoned the Super Horde" : $"{user} summoned the Horde";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde]);
		}
		public void Endurance(string user = "Khaos")
		{
			enduranceRoomX = sotnApi.GameApi.MapXPos;
			enduranceRoomY = sotnApi.GameApi.MapYPos;
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superEndurance = true;
				SpendKhaosMeter();
			}

			enduranceCount++;
			enduranceSpawnTimer.Start();
			string message = meterFull ? $"{user} used Super Endurance" : $"{user} used Endurance";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Endurance]);
		}
		public void HnK(string user = "Khaos")
		{
			invincibilityCheat.PokeValue(0);
			invincibilityCheat.Enable();
			defencePotionCheat.PokeValue(1);
			defencePotionCheat.Enable();
			invincibilityLocked = true;
			hnkTimer.Start();

			ActionTimer timer = new()
			{
				Name = "HnK",
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.HnK].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			notificationService.AddMessage($"{user} used HnK");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.HnK]);
		}
		#endregion
		#region Buffs
		public void Vampire(string user = "Khaos")
		{
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			vampireTimer.Start();
			notificationService.AddMessage(user + " used Vampire");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Vampire]);
		}
		public void LightHelp(string user = "Khaos")
		{
			string item = toolConfig.Khaos.LightHelpItemRewards[rng.Next(0, toolConfig.Khaos.LightHelpItemRewards.Length)];
			int rolls = 0;
			while (sotnApi.AlucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.LightHelpItemRewards[rng.Next(0, toolConfig.Khaos.LightHelpItemRewards.Length)];
				rolls++;
			}

			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;
			bool highMp = sotnApi.AlucardApi.CurrentMp > sotnApi.AlucardApi.MaxtMp * 0.6;

			int roll = rng.Next(1, 4);

			if (highHp && roll == 2)
			{
				roll = 3;
			}
			if ((highMp || manaLocked) && roll == 3)
			{
				roll = 2;
			}
			if ((highHp && highMp) || zaWarudoActive)
			{
				roll = 1;
			}

			switch (roll)
			{
				case 1:
					sotnApi.AlucardApi.GrantItemByName(item);
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Potion);
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					sotnApi.AlucardApi.CurrentMp += 20;
					notificationService.AddMessage($"{user} gave you mana");
					break;
				default:
					break;
			}
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.LightHelp]);
		}
		public void MediumHelp(string user = "Khaos")
		{
			string item = toolConfig.Khaos.MediumHelpItemRewards[rng.Next(0, toolConfig.Khaos.MediumHelpItemRewards.Length)];
			int rolls = 0;
			while (sotnApi.AlucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.MediumHelpItemRewards[rng.Next(0, toolConfig.Khaos.MediumHelpItemRewards.Length)];
				rolls++;
			}

			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;
			bool highMp = sotnApi.AlucardApi.CurrentMp > sotnApi.AlucardApi.MaxtMp * 0.6;

			int roll = rng.Next(1, manaLocked ? 3 : 4);

			if (highHp && roll == 2)
			{
				roll = 3;
			}
			if ((highMp && roll == 3) || manaLocked)
			{
				roll = 2;
			}
			if ((highHp && highMp) || zaWarudoActive)
			{
				roll = 1;
			}

			switch (roll)
			{
				case 1:
					sotnApi.AlucardApi.GrantItemByName(item);
					Console.WriteLine($"Medium help rolled: {item}");
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Elixir);
					Console.WriteLine($"Medium help rolled activate Elixir.");
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					sotnApi.AlucardApi.ActivatePotion(Potion.Mannaprism);
					Console.WriteLine($"Medium help rolled activate Manna prism.");
					notificationService.AddMessage($"{user} used a Mana Prism");
					break;
				default:
					break;
			}
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.MediumHelp]);
		}
		public void HeavytHelp(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				SpendKhaosMeter();
			}

			string item;
			int relic;
			int roll;
			RollRewards(out item, out relic, out roll);
			GiveRewards(user, item, relic, roll);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.HeavyHelp]);

			if (meterFull)
			{
				RollRewards(out item, out relic, out roll);
				GiveRewards(user, item, relic, roll);
			}

			void RollRewards(out string item, out int relic, out int roll)
			{
				item = toolConfig.Khaos.HeavyHelpItemRewards[rng.Next(0, toolConfig.Khaos.HeavyHelpItemRewards.Length)];
				int rolls = 0;
				while (sotnApi.AlucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
				{
					item = toolConfig.Khaos.HeavyHelpItemRewards[rng.Next(0, toolConfig.Khaos.HeavyHelpItemRewards.Length)];
					rolls++;
				}

				relic = rng.Next(0, Constants.Khaos.ProgressionRelics.Length);

				roll = rng.Next(1, 3);
				for (int i = 0; i < 11; i++)
				{
					if (!sotnApi.AlucardApi.HasRelic(Constants.Khaos.ProgressionRelics[relic]))
					{
						break;
					}
					if (i == 10)
					{
						roll = 1;
						break;
					}
					relic = rng.Next(0, Constants.Khaos.ProgressionRelics.Length);
				}
			}

			void GiveRewards(string user, string item, int relic, int roll)
			{
				switch (roll)
				{
					case 1:
						Console.WriteLine($"Heavy help rolled: {item}");
						sotnApi.AlucardApi.GrantItemByName(item);
						notificationService.AddMessage($"{user} gave you a {item}");
						break;
					case 2:
						Console.WriteLine($"Heavy help rolled: {Constants.Khaos.ProgressionRelics[relic]}");
						SetRelicLocationDisplay(Constants.Khaos.ProgressionRelics[relic], false);
						sotnApi.AlucardApi.GrantRelic(Constants.Khaos.ProgressionRelics[relic]);
						notificationService.AddMessage($"{user} gave you {Constants.Khaos.ProgressionRelics[relic]}");
						break;
					default:
						break;
				}
			}
		}

		public void BattleOrders(string user = "Khaos")
		{
			float currentHpPercentage = (float) sotnApi.AlucardApi.CurrentHp / (float) sotnApi.AlucardApi.MaxtHp;
			float currentMpPercentage = (float) sotnApi.AlucardApi.CurrentMp / (float) sotnApi.AlucardApi.MaxtMp;

			battleOrdersActive = true;
			battleOrdersBonusHp = sotnApi.AlucardApi.MaxtHp;
			battleOrdersBonusMp = sotnApi.AlucardApi.MaxtMp;

			sotnApi.AlucardApi.MaxtHp += battleOrdersBonusHp;
			sotnApi.AlucardApi.MaxtMp += battleOrdersBonusMp;

			sotnApi.AlucardApi.CurrentHp = (uint) (sotnApi.AlucardApi.MaxtHp * currentHpPercentage);
			sotnApi.AlucardApi.CurrentMp = (uint) (sotnApi.AlucardApi.MaxtMp * currentMpPercentage);

			notificationService.AddMessage($"{user} used Battle Orders");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders]);

			battleOrdersTimer.Start();
			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Duration
			};
			statusInfoDisplay.AddTimer(timer);
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
		}
		public void Magician(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				SpendKhaosMeter();
				SetRelicLocationDisplay(Relic.SoulOfBat, false);
				SetRelicLocationDisplay(Relic.FormOfMist, false);
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfBat);
				sotnApi.AlucardApi.GrantRelic(Relic.FireOfBat);
				sotnApi.AlucardApi.GrantRelic(Relic.EchoOfBat);
				sotnApi.AlucardApi.GrantRelic(Relic.ForceOfEcho);
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfWolf);
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfWolf);
				sotnApi.AlucardApi.GrantRelic(Relic.SkillOfWolf);
				sotnApi.AlucardApi.GrantRelic(Relic.FormOfMist);
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfMist);
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud);
			}

			sotnApi.AlucardApi.GrantItemByName("Wizard hat");
			sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
			manaCheat.PokeValue((int) sotnApi.AlucardApi.MaxtMp);
			manaCheat.Enable();
			manaLocked = true;
			magicianTimer.Start();

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Duration
			};
			statusInfoDisplay.AddTimer(timer);
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);

			string message = meterFull ? $"{user} activated Shapeshifter" : $"{user} activated Magician";
			notificationService.AddMessage(message);

			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Magician]);
		}
		public void MeltyBlood(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superMelty = true;
				SetHasteStaticSpeeds(true);
				ToggleHasteDynamicSpeeds(2);
				sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
				sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
				sotnApi.AlucardApi.ActivatePotion(Potion.StrPotion);
				sotnApi.AlucardApi.AttackPotionTimer = Constants.Khaos.GuiltyGearAttack;
				sotnApi.AlucardApi.DarkMetamorphasisTimer = Constants.Khaos.GuiltyGearDarkMetamorphosis;
				sotnApi.AlucardApi.DefencePotionTimer = Constants.Khaos.GuiltyGearDefence;
				sotnApi.AlucardApi.InvincibilityTimer = Constants.Khaos.GuiltyGearInvincibility;
				SpendKhaosMeter();
			}

			hitboxWidth.Enable();
			hitboxHeight.Enable();
			hitbox2Width.Enable();
			hitbox2Height.Enable();
			SetRelicLocationDisplay(Relic.LeapStone, false);
			sotnApi.AlucardApi.GrantRelic(Relic.LeapStone);
			meltyTimer.Start();
			string message = meterFull ? $"{user} activated GUILTY GEAR" : $"{user} activated Melty Blood";
			notificationService.AddMessage(message);
			if (meterFull)
			{
				Alert(toolConfig.Khaos.Actions[(int) Enums.Action.GuiltyGear]);

				ActionTimer timer = new()
				{
					Name = toolConfig.Khaos.Actions[(int) Enums.Action.GuiltyGear].Name,
					Duration = toolConfig.Khaos.Actions[(int) Enums.Action.GuiltyGear].Duration
				};
				notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
				statusInfoDisplay.AddTimer(timer);
			}
			else
			{
				Alert(toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood]);

				ActionTimer timer = new()
				{
					Name = toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood].Name,
					Duration = toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood].Duration
				};
				notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
				statusInfoDisplay.AddTimer(timer);
			}
		}
		public void FourBeasts(string user = "Khaos")
		{
			int beast = 0;
			if (azureDragonUsed && whiteTigerUsed && vermilionBirdUsed && blackTortoiseUsed)
			{
				beast = 5;
			}

			while (beast == 0)
			{
				beast = RollBeast();
			}

			switch (beast)
			{
				case 1:
					AzureDragon(user);
					break;
				case 2:
					WhiteTiger(user);
					break;
				case 3:
					VermilionBird(user);
					break;
				case 4:
					BlackTortoise(user);
					break;
				case 5:
					FourHolyBeasts(user);
					break;
				default:
					break;
			}
		}
		public void ZaWarudo(string user = "Khaos")
		{
			sotnApi.AlucardApi.ActivateStopwatch();
			zaWarudoZone = sotnApi.GameApi.Zone;
			zaWarudoZone2 = sotnApi.GameApi.Zone2;
			zaWarudoActive = true;

			if (!subweaponsOnlyActive)
			{
				sotnApi.AlucardApi.Subweapon = Subweapon.Stopwatch;
			}

			stopwatchTimer.Enable();
			stopwatchTimer.PokeValue(1);
			zawarudoTimer.Start();
			zawarudoCheckTimer.Start();

			notificationService.AddMessage(user + " used ZA WARUDO");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO]);
		}
		public void Haste(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();

			if (meterFull)
			{
				SpendKhaosMeter();
				superHaste = true;
			}

			SetHasteStaticSpeeds(meterFull);
			hasteTimer.Start();
			hasteActive = true;
			speedLocked = true;
			Console.WriteLine(user + " used " + toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Name);

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			string message = meterFull ? $"{user} activated Super Haste" : $"{user} activated Haste";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Haste]);
		}
		public void Lord(string user = "Khaos")
		{
			lordTriggerRoomX = sotnApi.GameApi.MapXPos;
			lordTriggerRoomY = sotnApi.GameApi.MapYPos;
			spawnActive = true;

			lordTimer.Start();
			lordSpawnTimer.Start();
			string message = user + " activated Lord of this Castle";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Lord]);
		}
		//TODO: Turn Undead
		#endregion

		public void Update()
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.AlucardApi.HasHitbox() || sotnApi.AlucardApi.CurrentHp < 1
				|| sotnApi.GameApi.InTransition || sotnApi.GameApi.IsLoading)
			{
				return;
			}

			CheckDashInput();
			CheckVermillionBirdFireballs();

			if (azureDragonActive && !azureSpiritActive)
			{
				var spiritAddress = sotnApi.ActorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.SpiritActor }, false);
				if (spiritAddress > 0)
				{
					LiveActor liveSpirit = sotnApi.ActorApi.GetLiveActor(spiritAddress);
					if (liveSpirit.LockOn == SotnApi.Constants.Values.Game.Actors.LockedOn)
					{
						liveSpirit.Palette = Constants.Khaos.SpiritPalette;
						liveSpirit.InvincibilityFrames = 4;
						azureSpiritActive = true;
						cheats.AddCheat(spiritAddress + SotnApi.Constants.Values.Game.Actors.LockOnOffset, SotnApi.Constants.Values.Game.Actors.LockedOn, Constants.Khaos.SpiritLockOnName, WatchSize.Byte);
						azureDragonTimer.Start();
					}
				}
			}

			if (blackTortoiseActive && !darkMetamorphosisCasted && sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.DarkMetamorphosis)
			{
				sotnApi.AlucardApi.ActivatePotion(Potion.HighPotion);
				darkMetamorphosisCasted = true;
			}

			if (blackTortoiseActive && darkMetamorphosisCasted && sotnApi.AlucardApi.State != SotnApi.Constants.Values.Alucard.States.DarkMetamorphosis)
			{
				darkMetamorphosisCasted = false;
			}

			if (whiteTigerActive && !whiteTigerBallActive && !hellfireCasted && sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.Hellfire)
			{
				Actor fireball = new Actor(Constants.Khaos.DarkFireballActorBytes);
				bool alucardFacing = sotnApi.AlucardApi.FacingLeft;
				int offsetX = alucardFacing ? -20 : 20;
				fireball.Xpos = (ushort) (sotnApi.AlucardApi.ScreenX + offsetX);
				fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY - 10);
				fireball.SpeedHorizontal = alucardFacing ? (ushort) 0xFFFF : (ushort) 0;

				long address = sotnApi.ActorApi.SpawnActor(fireball, false);
				LiveActor liveFireball = sotnApi.ActorApi.GetLiveActor(address);
				hellfireCasted = true;
				whiteTigerBallActive = true;
				cheats.AddCheat(address + SotnApi.Constants.Values.Game.Actors.SpeedWholeOffset, alucardFacing ? (ushort) Constants.Khaos.WhiteTigerBallSpeedLeft : (ushort) Constants.Khaos.WhiteTigerBallSpeedRight, Constants.Khaos.WhiteTigerBallSpeedName, WatchSize.Word);
				whiteTigerBallTimer.Start();
			}

			if (whiteTigerActive && hellfireCasted && sotnApi.AlucardApi.State != SotnApi.Constants.Values.Alucard.States.Hellfire)
			{
				hellfireCasted = false;
			}

			if (bloodManaActive)
			{
				CheckManaUsage();
			}

			if (slowActive && !slowPaused && sotnApi.GameApi.CanWarp())
			{
				slowPaused = true;
				underwaterPhysics.Disable();
			}

			if (slowActive && slowPaused && !sotnApi.GameApi.CanWarp())
			{
				slowPaused = false;
				underwaterPhysics.Enable();
			}

			if (subweaponsOnlyActive)
			{
				if (sotnApi.AlucardApi.RightHand == 0)
				{
					sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("Orange");
					if (sotnApi.AlucardApi.HasItemInInventory("Orange"))
					{
						sotnApi.AlucardApi.TakeOneItemByName("Orange");
					}
				}
				if (sotnApi.AlucardApi.LeftHand == 0)
				{
					sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Orange");
					if (sotnApi.AlucardApi.HasItemInInventory("Orange"))
					{
						sotnApi.AlucardApi.TakeOneItemByName("Orange");
					}
				}
			}
			//can unstun for HnK here alternatively
		}
		public void EnqueueAction(EventAddAction eventData)
		{
			if (eventData.ActionIndex < 0 || eventData.ActionIndex > toolConfig.Khaos.Actions.Count) throw new ArgumentOutOfRangeException(nameof(eventData.ActionIndex));
			if (eventData.UserName is null) throw new ArgumentNullException(nameof(eventData.UserName));
			if (eventData.UserName == "") throw new ArgumentException($"Parameter {nameof(eventData.UserName)} is empty!");
			string user = eventData.UserName;
			int action = eventData.ActionIndex;

			SotnRandoTools.Configuration.Models.Action? commandAction;
			switch (action)
			{
				#region Khaotic commands
				case (int) Enums.Action.KhaosStatus:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStatus];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaosStatus(user)));
					}
					break;
				case (int) Enums.Action.KhaosEquipment:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosEquipment];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => KhaosEquipment(user)) });
					}
					break;
				case (int) Enums.Action.KhaosStats:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStats];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => KhaosStats(user)) });
					}
					break;
				case (int) Enums.Action.KhaosRelics:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosRelics];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => KhaosRelics(user)) });
					}
					break;
				case (int) Enums.Action.PandorasBox:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.PandorasBox];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => PandorasBox(user)) });
					}
					break;
				case (int) Enums.Action.Gamble:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Gamble];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Gamble(user)));
					}
					break;
				case (int) Enums.Action.KhaoticBurst:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaoticBurst];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaoticBurst(user)));
					}
					break;
				case (int) Enums.Action.KhaosTrack:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaosTrack(eventData.Data, user)));
					}
					break;
				#endregion
				#region Debuffs
				case (int) Enums.Action.Bankrupt:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Bankrupt];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Bankrupt(user)) });
					}
					break;
				case (int) Enums.Action.Weaken:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Weaken];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Weaken(user)) });
					}
					break;
				case (int) Enums.Action.RespawnBosses:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.RespawnBosses];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => RespawnBosses(user)));
					}
					break;
				case (int) Enums.Action.SubweaponsOnly:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => SubweaponsOnly(user)) });
					}
					break;
				case (int) Enums.Action.Slow:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Slow];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpeed = true, Invoker = new MethodInvoker(() => Slow(user)) });
					}
					break;
				case (int) Enums.Action.BloodMana:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => BloodMana(user)) });
					}
					break;
				case (int) Enums.Action.Thirst:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Thirst(user)) });
					}
					break;
				case (int) Enums.Action.KhaosHorde:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpawning = true, Invoker = new MethodInvoker(() => Horde(user)) });
					}
					break;
				case (int) Enums.Action.Endurance:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Endurance];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Endurance(user)));
					}
					break;
				case (int) Enums.Action.HnK:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.HnK];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HnK(user)));
					}
					break;
				#endregion
				#region Buffs
				case (int) Enums.Action.Vampire:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Vampire];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Vampire(user)));
					}
					break;
				case (int) Enums.Action.LightHelp:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.LightHelp];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => LightHelp(user)));
					}
					break;
				case (int) Enums.Action.MediumHelp:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.MediumHelp];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MediumHelp(user)));
					}
					break;
				case (int) Enums.Action.HeavyHelp:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.HeavyHelp];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HeavytHelp(user)));
					}
					break;
				case (int) Enums.Action.BattleOrders:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => BattleOrders(user)) });
					}
					break;
				case (int) Enums.Action.Magician:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Magician];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => Magician(user)) });
					}
					break;
				case (int) Enums.Action.MeltyBlood:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => MeltyBlood(user)) });
					}
					break;
				case (int) Enums.Action.FourBeasts:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksInvincibility = true, Invoker = new MethodInvoker(() => FourBeasts(user)) });
					}
					break;
				case (int) Enums.Action.ZAWARUDO:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ZaWarudo(user)));
					}
					break;
				case (int) Enums.Action.Haste:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Haste];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpeed = true, Invoker = new MethodInvoker(() => Haste(user)) });
					}
					break;
				case (int) Enums.Action.Lord:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Lord];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpawning = true, Invoker = new MethodInvoker(() => Lord(user)) });
					}
					break;
				default:
					commandAction = null;
					break;
					#endregion
			}
			if (commandAction is not null)
			{
				GainKhaosMeter(commandAction.Meter);
				commandAction.LastUsedAt = DateTime.Now;
			}

			notificationService.UpdateOverlayQueue(queuedActions);
		}
		private void InitializeTimers()
		{
			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 2 * (1 * 1000);

			bloodManaDeathTimer.Tick += KillAlucard;
			bloodManaDeathTimer.Interval = 1 * (1 * 1500);

			khaosTrackTimer.Tick += KhaosTrackOff;
			khaosTrackTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack].Duration.TotalMilliseconds;
			subweaponsOnlyTimer.Tick += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly].Duration.TotalMilliseconds;
			slowTimer.Tick += SlowOff;
			slowTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Duration.TotalMilliseconds;
			bloodManaTimer.Tick += BloodManaOff;
			bloodManaTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana].Duration.TotalMilliseconds;
			thirstTimer.Tick += ThirstOff;
			thirstTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Duration.TotalMilliseconds;
			thirstTickTimer.Tick += ThirstDrain;
			thirstTickTimer.Interval = 1000;
			hordeTimer.Tick += HordeOff;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeSpawnTimer.Tick += HordeSpawn;
			hordeSpawnTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Interval.TotalMilliseconds;
			enduranceSpawnTimer.Tick += EnduranceSpawn;
			enduranceSpawnTimer.Interval = 2 * (1000);
			hnkTimer.Tick += HnkOff;
			hnkTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.HnK].Duration.TotalMilliseconds;

			vampireTimer.Tick += VampireOff;
			vampireTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Duration.TotalMilliseconds;
			magicianTimer.Tick += MagicianOff;
			magicianTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Duration.TotalMilliseconds;
			battleOrdersTimer.Tick += BattleOrdersOff;
			battleOrdersTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Duration.TotalMilliseconds;
			meltyTimer.Tick += MeltyBloodOff;
			meltyTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood].Duration.TotalMilliseconds;
			fourBeastsTimer.Tick += FourBeastsOff;
			fourBeastsTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration.TotalMilliseconds;
			azureDragonTimer.Tick += AzureDragonOff;
			azureDragonTimer.Interval = 10 * 1000;
			whiteTigerBallTimer.Tick += WhiteTigerOff;
			whiteTigerBallTimer.Interval = 2 * 1000;
			zawarudoTimer.Tick += ZawarudoOff;
			zawarudoTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO].Duration.TotalMilliseconds;
			zawarudoCheckTimer.Tick += ZaWarudoAreaCheck;
			zawarudoCheckTimer.Interval += 2 * 1000;
			hasteTimer.Tick += HasteOff;
			hasteTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Duration.TotalMilliseconds;
			hasteOverdriveTimer.Tick += OverdriveOn;
			hasteOverdriveTimer.Interval = (2 * 1000);
			hasteOverdriveOffTimer.Tick += OverdriveOff;
			hasteOverdriveOffTimer.Interval = (2 * 1000);
			lordTimer.Tick += LordOff;
			lordTimer.Interval = 5 * (60 * 1000);
			lordSpawnTimer.Tick += LordSpawn;
			lordSpawnTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Interval.TotalMilliseconds;
		}
		private void InitializeTimerIntervals()
		{
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Interval = 2 * (1 * 1000);
			bloodManaDeathTimer.Interval = 1 * (1 * 1500);
			khaosTrackTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack].Duration.TotalMilliseconds;
			subweaponsOnlyTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly].Duration.TotalMilliseconds;
			slowTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Duration.TotalMilliseconds;
			bloodManaTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana].Duration.TotalMilliseconds;
			thirstTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Duration.TotalMilliseconds;
			thirstTickTimer.Interval = 1000;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeSpawnTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Interval.TotalMilliseconds;
			enduranceSpawnTimer.Interval = 2 * (1000);
			hnkTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.HnK].Duration.TotalMilliseconds;
			vampireTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Duration.TotalMilliseconds;
			magicianTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Duration.TotalMilliseconds;
			battleOrdersTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Duration.TotalMilliseconds;
			meltyTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood].Duration.TotalMilliseconds;
			fourBeastsTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration.TotalMilliseconds;
			azureDragonTimer.Interval = 10 * 1000;
			whiteTigerBallTimer.Interval = 2 * 1000;
			zawarudoTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO].Duration.TotalMilliseconds;
			zawarudoCheckTimer.Interval += 2 * 1000;
			hasteTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Duration.TotalMilliseconds;
			hasteOverdriveTimer.Interval = (2 * 1000);
			hasteOverdriveOffTimer.Interval = (2 * 1000);
			lordTimer.Interval = 5 * (60 * 1000);
			lordSpawnTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Interval.TotalMilliseconds;
		}
		private void StopTimers()
		{
			fastActionTimer.Stop();
			actionTimer.Stop();

			subweaponsOnlyTimer.Interval = 1;
			slowTimer.Interval = 1;
			bloodManaTimer.Interval = 1;
			thirstTimer.Interval = 1;
			thirstTickTimer.Stop();
			hordeTimer.Interval = 1;
			hordeSpawnTimer.Stop();
			enduranceSpawnTimer.Stop();
			hnkTimer.Interval = 1;

			vampireTimer.Interval = 1;
			magicianTimer.Interval = 1;
			meltyTimer.Interval = 1;
			fourBeastsTimer.Interval = 1;
			azureDragonTimer.Interval = 1;
			zawarudoTimer.Interval = 1;
			zawarudoCheckTimer.Stop();
			hasteTimer.Interval = 1;
			hasteOverdriveTimer.Stop();
			hasteOverdriveOffTimer.Stop();
			lordSpawnTimer.Stop();
			lordTimer.Stop();
			battleOrdersTimer.Interval = 1;
		}
		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (queuedActions.Count > 0)
			{
				alucardMapX = sotnApi.AlucardApi.MapX;
				alucardMapY = sotnApi.AlucardApi.MapY;

				if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave() && !IsInRoomList(Constants.Khaos.RichterRooms) && !IsInRoomList(Constants.Khaos.LoadingRooms))
				{
					int index = 0;
					bool actionUnlocked = true;

					for (int i = 0; i < queuedActions.Count; i++)
					{
						index = i;
						actionUnlocked = true;
						if ((queuedActions[i].LocksSpeed && speedLocked) ||
							(queuedActions[i].LocksMana && manaLocked) ||
							(queuedActions[i].LocksInvincibility && invincibilityLocked) ||
							(queuedActions[i].LocksSpawning && spawnActive))
						{
							actionUnlocked = false;
							continue;
						}
						break;
					}

					if (actionUnlocked)
					{
						queuedActions[index].Invoker();
						queuedActions.RemoveAt(index);
						notificationService.UpdateOverlayQueue(queuedActions);
						SetDynamicInterval();
					}
					else
					{
						Console.WriteLine($"All actions locked. speed: {speedLocked}, invincibility: {invincibilityLocked}, mana: {manaLocked}");
					}
				}
			}
			else
			{
				actionTimer.Interval = 2000;
			}
		}
		private void SetDynamicInterval()
		{
			if (toolConfig.Khaos.DynamicInterval && queuedActions.Count < Constants.Khaos.SlowQueueIntervalEnd)
			{
				actionTimer.Interval = slowInterval;
				Console.WriteLine($"Interval set to {slowInterval / 1000}s, {actionTimer.Interval}");
			}
			else if (toolConfig.Khaos.DynamicInterval && queuedActions.Count >= Constants.Khaos.FastQueueIntervalStart)
			{
				actionTimer.Interval = fastInterval;
				Console.WriteLine($"Interval set to {fastInterval / 1000}s, {actionTimer.Interval}");
			}
			else
			{
				actionTimer.Interval = normalInterval;
				Console.WriteLine($"Interval set to {normalInterval / 1000}s, {actionTimer.Interval}");
			}
		}
		private void ExecuteFastAction(Object sender, EventArgs e)
		{
			alucardMapX = sotnApi.AlucardApi.MapX;
			alucardMapY = sotnApi.AlucardApi.MapY;
			CheckCastleChanged();
			CheckMainMenu();

			bool keepRichterRoom = IsInRoomList(Constants.Khaos.RichterRooms);
			bool galamothRoom = IsInRoomList(Constants.Khaos.GalamothRooms);
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& !keepRichterRoom && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading && !sotnApi.AlucardApi.IsInvincible() && !IsInRoomList(Constants.Khaos.LoadingRooms) && alucardMapX < 99)
			{
				shaftHpSet = false;

				if (queuedFastActions.Count > 0)
				{
					queuedFastActions.Dequeue()();
				}
			}
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& keepRichterRoom && !shaftHpSet && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading)
			{
				SetShaftHp();
			}
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& galamothRoom && !galamothStatsSet && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading)
			{
				SetGalamothtStats();
			}
			if (!galamothRoom)
			{
				galamothStatsSet = false;
			}

			if (!pandoraUsed && totalMeterGained >= toolConfig.Khaos.PandoraTrigger)
			{
				EnqueueAction(new EventAddAction { UserName = "Khaos", ActionIndex = 4 });
				pandoraUsed = true;
			}

			if (AutoKhaosOn)
			{
				AutoKhaosAction();
			}
		}
		private void AutoKhaosAction()
		{
			int roll = rng.Next(0, 101);
			if (roll > autoKhaosDifficulty)
			{
				int index = rng.Next(0, toolConfig.Khaos.Actions.Count);
				EventAddAction? actionEvent = new() { UserName = "Auto Khaos", ActionIndex = index, Data = "random" };

				if (toolConfig.Khaos.Actions[index].Name != "Guilty Gear" && toolConfig.Khaos.Actions[index].Name != "Pandora's Box" && !toolConfig.Khaos.Actions[index].IsOnCooldown())
				{
					EnqueueAction(actionEvent);
				}
			}
		}

		#region Khaotic events
		private int RollStatus(bool entranceCutscene, bool succubusRoom, bool alucardIsImmuneToCurse, bool alucardIsImmuneToStone, bool alucardIsImmuneToPoison, bool highHp)
		{
			int min = 1;
			int max = 10;
			int result = rng.Next(min, max);

			switch (result)
			{
				case 1:
					if (alucardIsImmuneToPoison)
					{
						return 0;
					}
					break;
				case 2:
					if (alucardIsImmuneToCurse)
					{
						return 0;
					}
					break;
				case 3:
					if (alucardIsImmuneToStone || succubusRoom)
					{
						return 0;
					}
					break;
				case 4:
					if (succubusRoom || entranceCutscene)
					{
						return 0;
					}
					break;
				case 5:
					if (zaWarudoActive)
					{
						return 0;
					}
					break;
				case 6:
					if (zaWarudoActive)
					{
						return 0;
					}
					break;
				case 7:
					if (highHp)
					{
						return 0;
					}
					break;
				case 8:
					if (zaWarudoActive)
					{
						return 0;
					}
					break;
				case 9:
					if (sotnApi.AlucardApi.CurrentMp < 15 || manaLocked)
					{
						return 0;
					}
					break;
				default:
					break;
			}

			return result;
		}
		private void KhaosTrackOff(Object sender, EventArgs e)
		{
			music.Disable();
			khaosTrackTimer.Stop();
		}
		private void RandomizeGold()
		{
			uint gold = (uint) rng.Next(0, 5000);
			uint roll = (uint) rng.Next(0, 21);
			if (roll > 16 && roll < 20)
			{
				gold *= (uint) rng.Next(1, 11);
			}
			else if (roll > 19)
			{
				gold *= (uint) rng.Next(10, 81);
			}
			sotnApi.AlucardApi.Gold = gold;
		}
		private void RandomizeStatsActivate()
		{
			if (battleOrdersActive)
			{
				sotnApi.AlucardApi.MaxtHp -= (uint) battleOrdersBonusHp;
				sotnApi.AlucardApi.MaxtMp -= (uint) battleOrdersBonusMp;
			}

			uint maxHp = sotnApi.AlucardApi.MaxtHp;
			uint currentHp = sotnApi.AlucardApi.CurrentHp;
			uint maxMana = sotnApi.AlucardApi.MaxtMp;
			uint currentMana = sotnApi.AlucardApi.CurrentMp;
			uint str = sotnApi.AlucardApi.Str;
			uint con = sotnApi.AlucardApi.Con;
			uint intel = sotnApi.AlucardApi.Int;
			uint lck = sotnApi.AlucardApi.Lck;

			uint statPool = str + con + intel + lck > 24 ? str + con + intel + lck - 24 : str + con + intel + lck;
			uint offset = (uint) ((rng.NextDouble() / 2) * statPool);

			int statPoolRoll = rng.Next(1, 4);
			if (statPoolRoll == 2)
			{
				statPool += +offset;
			}
			else if (statPoolRoll == 3)
			{
				statPool -= offset;
			}

			double a = rng.NextDouble();
			double b = rng.NextDouble();
			double c = rng.NextDouble();
			double d = rng.NextDouble();
			double sum = a + b + c + d;
			double percentageStr = (a / sum);
			double percentageCon = (b / sum);
			double percentageInt = (c / sum);
			double percentageLck = (d / sum);
			uint newStr = (uint) Math.Round(statPool * percentageStr);
			uint newCon = (uint) Math.Round(statPool * percentageCon);
			uint newInt = (uint) Math.Round(statPool * percentageInt);
			uint newLck = (uint) Math.Round(statPool * percentageLck);

			sotnApi.AlucardApi.Str = (6 + newStr);
			sotnApi.AlucardApi.Con = (6 + newCon);
			sotnApi.AlucardApi.Int = (6 + newInt);
			sotnApi.AlucardApi.Lck = (6 + newLck);

			uint pointsPool = maxHp + maxMana > 110 ? maxHp + maxMana - 110 : maxHp + maxMana;
			if (maxHp + maxMana < 110)
			{
				pointsPool = 110;
			}
			offset = (uint) ((rng.NextDouble() / 2) * pointsPool);

			int pointsRoll = rng.Next(1, 4);
			if (pointsRoll == 2)
			{
				pointsPool += offset;
			}
			else if (pointsRoll == 3)
			{
				pointsPool -= offset;
			}

			double hpPercent = rng.NextDouble();
			uint pointsHp = 80 + (uint) Math.Round(hpPercent * pointsPool);
			uint pointsMp = 30 + pointsPool - (uint) Math.Round(hpPercent * pointsPool);

			if (currentHp > pointsHp)
			{
				sotnApi.AlucardApi.CurrentHp = pointsHp;
			}
			if (currentMana > pointsMp)
			{
				sotnApi.AlucardApi.CurrentMp = pointsMp;
			}

			sotnApi.AlucardApi.MaxtHp = pointsHp;
			sotnApi.AlucardApi.MaxtMp = pointsMp;

			if (battleOrdersActive)
			{
				battleOrdersBonusHp = sotnApi.AlucardApi.MaxtHp;
				battleOrdersBonusMp = sotnApi.AlucardApi.MaxtMp;
				sotnApi.AlucardApi.MaxtHp += battleOrdersBonusHp;
				sotnApi.AlucardApi.MaxtMp += battleOrdersBonusMp;
			}
		}
		private void RandomizeInventory()
		{
			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");

			sotnApi.AlucardApi.ClearInventory();

			int itemCount = rng.Next(toolConfig.Khaos.PandoraMinItems, toolConfig.Khaos.PandoraMaxItems + 1);

			for (int i = 0; i < itemCount; i++)
			{
				int result = rng.Next(0, Equipment.Items.Count);
				sotnApi.AlucardApi.GrantItemByName(Equipment.Items[result]);
			}

			sotnApi.AlucardApi.HandCursor = 0;
			sotnApi.AlucardApi.HelmCursor = 0;
			sotnApi.AlucardApi.ArmorCursor = 0;
			sotnApi.AlucardApi.CloakCursor = 0;
			sotnApi.AlucardApi.AccessoryCursor = 0;

			sotnApi.AlucardApi.GrantItemByName("Library card");
			sotnApi.AlucardApi.GrantItemByName("Library card");
			if (hasHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (hasSpikeBreaker)
			{
				sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
			}
			if (hasGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (hasSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
			}
		}
		private void RandomizeSubweapon()
		{
			Array? subweapons = Enum.GetValues(typeof(Subweapon));
			sotnApi.AlucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
		}
		private void RandomizeRelicsActivate(bool randomizeVladRelics = true)
		{
			Array? relics = Enum.GetValues(typeof(Relic));
			foreach (object? relic in relics)
			{
				/*if ((int) relic < 25)
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic);
				}*/
				int roll = rng.Next(0, 2);
				if (roll > 0)
				{
					if ((int) relic < 25)
					{
						SetRelicLocationDisplay((Relic) relic, false);
						sotnApi.AlucardApi.GrantRelic((Relic) relic);
					}
				}
				else
				{
					if ((int) relic < 25)
					{
						SetRelicLocationDisplay((Relic) relic, true);
						sotnApi.AlucardApi.TakeRelic((Relic) relic);
					}
					else if (randomizeVladRelics)
					{
						sotnApi.AlucardApi.TakeRelic((Relic) relic);
					}
				}
			}

			if (alucardSecondCastle)
			{
				int roll = rng.Next(0, Constants.Khaos.FlightRelics.Count);
				foreach (Relic relic in Constants.Khaos.FlightRelics[roll])
				{
					SetRelicLocationDisplay((Relic) relic, false);
					sotnApi.AlucardApi.GrantRelic((Relic) relic);
				}
			}

			if (IsInRoomList(Constants.Khaos.SwitchRoom))
			{
				SetRelicLocationDisplay(Relic.JewelOfOpen, false);
				sotnApi.AlucardApi.GrantRelic(Relic.JewelOfOpen);
			}
		}
		private void RandomizeEquipmentSlots()
		{
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";

			uint newRightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newLeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newArmor = (uint) rng.Next(0, Equipment.ArmorCount + 1);
			uint newHelm = Equipment.HelmStart + (uint) rng.Next(0, Equipment.HelmCount + 1);
			uint newCloak = Equipment.CloakStart + (uint) rng.Next(0, Equipment.CloakCount + 1);
			uint newAccessory1 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			uint newAccessory2 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);

			sotnApi.AlucardApi.RightHand = 0;
			sotnApi.AlucardApi.LeftHand = 0;
			sotnApi.AlucardApi.GrantItemByName(Equipment.Items[(int) newRightHand]);
			sotnApi.AlucardApi.GrantItemByName(Equipment.Items[(int) newLeftHand]);
			sotnApi.AlucardApi.Armor = newArmor;
			sotnApi.AlucardApi.Helm = newHelm;
			sotnApi.AlucardApi.Cloak = newCloak;
			sotnApi.AlucardApi.Accessory1 = newAccessory1;
			sotnApi.AlucardApi.Accessory2 = newAccessory2;

			RandomizeSubweapon();
			if (subweaponsOnlyActive)
			{
				while (sotnApi.AlucardApi.Subweapon == Subweapon.Empty || sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch)
				{
					RandomizeSubweapon();
				}
			}

			Console.WriteLine($"Equipment randomized. " +
				$"\n Right Hand: {Equipment.Items[(int) newRightHand]} " +
				$"\n Left Hand: {Equipment.Items[(int) newLeftHand]} " +
				$"\n Armor: {Equipment.Items[(int) (newArmor + Equipment.HandCount + 1)]} " +
				$"\n Helm: {Equipment.Items[(int) (newHelm + Equipment.HandCount + 1)]} " +
				$"\n Cloak: {Equipment.Items[(int) (newCloak + Equipment.HandCount + 1)]} " +
				$"\n Accessory1: {Equipment.Items[(int) (newAccessory1 + Equipment.HandCount + 1)]} " +
				$"\n Accessory2: {Equipment.Items[(int) (newAccessory2 + Equipment.HandCount + 1)]}");

			sotnApi.AlucardApi.GrantItemByName("Library card");
			sotnApi.AlucardApi.GrantItemByName("Library card");
			if (equippedHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker)
			{
				sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
			}
		}
		#endregion
		#region Debuff events
		private void BloodManaUpdate()
		{
			if (spentMana > 0)
			{
				uint currentHp = sotnApi.AlucardApi.CurrentHp;
				if (currentHp > spentMana)
				{
					sotnApi.AlucardApi.CurrentHp -= (uint) spentMana;
					sotnApi.AlucardApi.CurrentMp += (uint) spentMana;
				}
				else
				{
					sotnApi.AlucardApi.CurrentMp = 0;
					sotnApi.AlucardApi.CurrentHp = 0;
					sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("Orange");
					sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Orange");
					bloodManaDeathTimer.Start();
				}
			}
		}
		private void KillAlucard(Object sender, EventArgs e)
		{
			Actor hitbox = new Actor();
			uint offsetPosX = sotnApi.AlucardApi.ScreenX - 255;
			uint offsetPosY = sotnApi.AlucardApi.ScreenY - 255;

			hitbox.Xpos = 0;
			hitbox.Ypos = 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.DamageTypeA = (uint) Actors.Slam;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 999;
			sotnApi.ActorApi.SpawnActor(hitbox);
			sotnApi.AlucardApi.InvincibilityTimer = 0;
			bloodManaDeathTimer.Stop();
		}
		private void BloodManaOff(Object sender, EventArgs e)
		{
			manaLocked = false;
			bloodManaActive = false;
			bloodManaTimer.Stop();
		}
		private void ThirstDrain(Object sender, EventArgs e)
		{
			uint superDrain = superThirst ? Constants.Khaos.SuperThirstExtraDrain : 0u;
			if (sotnApi.AlucardApi.CurrentHp > toolConfig.Khaos.ThirstDrainPerSecond + 1 + +superDrain)
			{
				sotnApi.AlucardApi.CurrentHp -= (toolConfig.Khaos.ThirstDrainPerSecond + superDrain);
			}
			else
			{
				sotnApi.AlucardApi.CurrentHp = 1;
			}
		}
		private void ThirstOff(Object sender, EventArgs e)
		{
			darkMetamorphasisCheat.Disable();
			thirstTimer.Stop();
			thirstTickTimer.Stop();
			superThirst = false;
		}
		private void HordeOff(Object sender, EventArgs e)
		{
			superHorde = false;
			spawnActive = false;
			hordeEnemies.RemoveRange(0, hordeEnemies.Count);
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeTimer.Stop();
			hordeSpawnTimer.Stop();
		}
		private void HordeSpawn(Object sender, EventArgs e)
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5 || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone))
			{
				return;
			}

			uint zone = sotnApi.GameApi.Zone;
			uint zone2 = sotnApi.GameApi.Zone2;

			if (hordeZone != zone || hordeZone2 != zone2 || hordeEnemies.Count == 0)
			{
				hordeEnemies.RemoveRange(0, hordeEnemies.Count);
				FindHordeEnemy();
				hordeZone = zone;
				hordeZone2 = zone2;
			}
			else if (hordeEnemies.Count > 0)
			{
				FindHordeEnemy();
				int enemyIndex = rng.Next(0, hordeEnemies.Count);
				if (hordeTimer.Interval == 5 * (60 * 1000))
				{
					hordeTimer.Stop();
					hordeTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Duration.TotalMilliseconds;

					ActionTimer timer = new()
					{
						Name = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Name,
						Duration = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Duration
					};
					statusInfoDisplay.AddTimer(timer);
					notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
					hordeTimer.Start();
				}
				hordeEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				sotnApi.ActorApi.SpawnActor(hordeEnemies[enemyIndex]);
			}
		}
		private bool FindHordeEnemy()
		{
			uint roomX = sotnApi.GameApi.MapXPos;
			uint roomY = sotnApi.GameApi.MapYPos;

			if ((roomX == hordeTriggerRoomX && roomY == hordeTriggerRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu())
			{
				return false;
			}

			long enemy = sotnApi.ActorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackHordeEnemies : Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? hordeEnemy = new(sotnApi.ActorApi.GetActor(enemy));

				if (hordeEnemy is not null && !hordeEnemies.Where(e => e.Sprite == hordeEnemy.Sprite).Any())
				{
					if (superHorde)
					{
						hordeEnemy.Hp *= 2;
						hordeEnemy.Damage *= 2;

						int damageTypeRoll = rng.Next(0, 4);

						switch (damageTypeRoll)
						{
							case 1:
								hordeEnemy.DamageTypeA = (uint) Actors.Poison;
								break;
							case 2:
								hordeEnemy.DamageTypeB = (uint) Actors.Curse;
								break;
							case 3:
								hordeEnemy.DamageTypeA = (uint) Actors.Stone;
								hordeEnemy.DamageTypeB = (uint) Actors.Stone;
								break;
							default:
								break;
						}
					}
					hordeEnemies.Add(hordeEnemy);
					Console.WriteLine($"Added horde enemy with hp: {hordeEnemy.Hp} sprite: {hordeEnemy.Sprite} damage: {hordeEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		private void SubweaponsOnlyOff(object sender, EventArgs e)
		{
			curse.Disable();
			manaLocked = false;
			manaCheat.Disable();
			hearts.Disable();
			if (gasCloudTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud);
				gasCloudTaken = false;
			}
			subweaponsOnlyTimer.Stop();
			subweaponsOnlyActive = false;
			sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
		}
		private void SlowOff(Object sender, EventArgs e)
		{
			//SetSpeed();
			underwaterPhysics.Disable();
			sotnApi.GameApi.UnderwaterPhysicsEnabled = false;
			slowTimer.Stop();
			slowActive = false;
			slowPaused = false;
			//speedLocked = false;
		}
		private void EnduranceSpawn(Object sender, EventArgs e)
		{
			uint roomX = sotnApi.GameApi.MapXPos;
			uint roomY = sotnApi.GameApi.MapYPos;
			float healthMultiplier = 3.5F;

			if ((roomX == enduranceRoomX && roomY == enduranceRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? bossCopy = null;

			long enemy = sotnApi.ActorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceRomhackBosses : Constants.Khaos.EnduranceBosses);
			if (enemy > 0)
			{
				LiveActor boss = sotnApi.ActorApi.GetLiveActor(enemy);
				bossCopy = new Actor(sotnApi.ActorApi.GetActor(enemy));
				string name = Constants.Khaos.EnduranceRomhackBosses.Where(e => e.Sprite == bossCopy.Sprite).FirstOrDefault().Name;
				Console.WriteLine($"Endurance boss found namne: {name} hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.Sprite}");

				bool right = rng.Next(0, 2) > 0;
				bossCopy.Xpos = right ? (ushort) (bossCopy.Xpos + rng.Next(40, 80)) : (ushort) (bossCopy.Xpos + rng.Next(-80, -40));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
				int newhp = (int) Math.Round(healthMultiplier * bossCopy.Hp);
				if (newhp > Int16.MaxValue)
				{
					newhp = Int16.MaxValue - 200;
				}
				bossCopy.Hp = (ushort) newhp;
				sotnApi.ActorApi.SpawnActor(bossCopy);

				boss.Hp = (ushort) newhp;

				if (superEndurance)
				{
					superEndurance = false;

					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					sotnApi.ActorApi.SpawnActor(bossCopy);
					notificationService.AddMessage($"Super Endurance {name}");
				}
				else
				{
					notificationService.AddMessage($"Endurance {name}");
				}

				enduranceCount--;
				enduranceRoomX = roomX;
				enduranceRoomY = roomY;
				if (enduranceCount == 0)
				{
					enduranceSpawnTimer.Stop();
				}
			}
			else
			{
				enemy = sotnApi.ActorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceAlternateRomhackBosses : Constants.Khaos.EnduranceAlternateBosses);
				if (enemy > 0)
				{
					LiveActor boss = sotnApi.ActorApi.GetLiveActor(enemy);
					string name = Constants.Khaos.EnduranceAlternateBosses.Where(e => e.Sprite == boss.Sprite).FirstOrDefault().Name;
					Console.WriteLine($"Endurance alternate boss found namne: {name}");

					boss.Palette = (ushort) (boss.Palette + rng.Next(1, 10));

					if (superEndurance)
					{
						int newhp = (int) Math.Round((healthMultiplier * 2.3) * boss.Hp);
						if (newhp > Int16.MaxValue)
						{
							newhp = Int16.MaxValue - 200;
						}
						boss.Hp = (ushort) newhp;
						superEndurance = false;
						notificationService.AddMessage($"Super Endurance {name}");
					}
					else
					{
						int newhp = (int) Math.Round((healthMultiplier * 1.3) * boss.Hp);
						if (newhp > Int16.MaxValue)
						{
							newhp = Int16.MaxValue - 200;
						}
						boss.Hp = (ushort) newhp;
						notificationService.AddMessage($"Endurance {name}");
					}

					enduranceCount--;
					enduranceRoomX = roomX;
					enduranceRoomY = roomY;
					if (enduranceCount == 0)
					{
						enduranceSpawnTimer.Stop();
					}
				}
				else
				{
					return;
				}
			}
		}
		private void SpawnPoisonHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Actors.Poison;
			sotnApi.ActorApi.SpawnActor(hitbox);
		}
		private void SpawnCurseHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeB = (uint) Actors.Curse;
			sotnApi.ActorApi.SpawnActor(hitbox);
		}
		private void SpawnStoneHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Actors.Stone;
			hitbox.DamageTypeB = (uint) Actors.Stone;
			sotnApi.ActorApi.SpawnActor(hitbox);
		}
		private void SpawnSlamHitbox()
		{
			//bool alucardIsPoisoned = sotnApi.AlucardApi.PoisonTimer > 0;
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Actors.Slam;
			sotnApi.ActorApi.SpawnActor(hitbox);
		}
		private void BankruptActivate()
		{
			bool clearRightHand = false;
			bool clearLeftHand = false;
			bool clearHelm = false;
			bool clearArmor = false;
			bool clearCloak = false;
			bool clearAccessory1 = false;
			bool clearAccessory2 = false;

			float goldPercentage = 0;
			int clearedSlots = 0;

			switch (bankruptLevel)
			{
				case 1:
					goldPercentage = 0.5f;
					clearedSlots = 2;
					break;
				case 2:
					goldPercentage = 0.2f;
					clearedSlots = 4;
					break;
				case 3:
					goldPercentage = 0;
					clearedSlots = 7;
					break;

				default:
					break;
			}

			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";


			sotnApi.AlucardApi.Gold = goldPercentage == 0 ? 0 : (uint) Math.Round(sotnApi.AlucardApi.Gold * goldPercentage);
			sotnApi.AlucardApi.ClearInventory();

			if (clearedSlots == 7)
			{
				clearRightHand = true;
				clearLeftHand = true;
				clearHelm = true;
				clearArmor = true;
				clearCloak = true;
				clearAccessory1 = true;
				clearAccessory2 = true;
			}
			else
			{
				int[] slots = new int[clearedSlots + 1];
				int slotsIndex = 0;
				for (int i = 0; i <= clearedSlots; i++)
				{
					int result = rng.Next(0, 8);
					while (slots.Contains(result))
					{
						result = rng.Next(0, 8);
					}
					slots[slotsIndex] = result;
					slotsIndex++;
				}

				for (int i = 0; i < slots.Length; i++)
				{
					switch (slots[i])
					{
						case 1:
							clearRightHand = true;
							break;
						case 2:
							clearLeftHand = true;
							break;
						case 3:
							clearHelm = true;
							break;
						case 4:
							clearArmor = true;
							break;
						case 5:
							clearCloak = true;
							break;
						case 6:
							clearAccessory1 = true;
							break;
						case 7:
							clearAccessory1 = true;
							break;
						default:
							break;
					}
				}
			}

			if (clearRightHand)
			{
				sotnApi.AlucardApi.RightHand = 0;
			}
			if (clearLeftHand)
			{
				sotnApi.AlucardApi.LeftHand = 0;
			}
			if (!equippedHolyGlasses && clearHelm)
			{
				sotnApi.AlucardApi.Helm = Equipment.HelmStart;
			}
			if (!equippedSpikeBreaker && clearArmor)
			{
				sotnApi.AlucardApi.Armor = 0;
			}
			if (clearCloak)
			{
				sotnApi.AlucardApi.Cloak = Equipment.CloakStart;
			}
			if (clearAccessory1)
			{
				sotnApi.AlucardApi.Accessory1 = Equipment.AccessoryStart;
			}
			if (clearAccessory2)
			{
				sotnApi.AlucardApi.Accessory2 = Equipment.AccessoryStart;
			}

			sotnApi.GameApi.RespawnItems();

			sotnApi.AlucardApi.HandCursor = 0;
			sotnApi.AlucardApi.HelmCursor = 0;
			sotnApi.AlucardApi.ArmorCursor = 0;
			sotnApi.AlucardApi.CloakCursor = 0;
			sotnApi.AlucardApi.AccessoryCursor = 0;

			if (hasHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (hasSpikeBreaker)
			{
				sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing || hasGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing || hasSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
			}
			if (bankruptLevel < 3)
			{
				bankruptLevel++;
			}
		}
		private void HnkOff(object sender, EventArgs e)
		{
			invincibilityCheat.Disable();
			defencePotionCheat.Disable();
			hnkTimer.Stop();
			invincibilityLocked = false;
		}
		#endregion
		#region Buff events
		private void VampireOff(Object sender, EventArgs e)
		{
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Disable();
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Disable();
			vampireTimer.Stop();
		}
		private void MagicianOff(Object sender, EventArgs e)
		{
			manaCheat.Disable();
			manaLocked = false;
			magicianTimer.Stop();
		}
		private void BattleOrdersOff(Object sender, EventArgs e)
		{
			sotnApi.AlucardApi.MaxtHp -= (uint) battleOrdersBonusHp;
			sotnApi.AlucardApi.MaxtMp -= (uint) battleOrdersBonusMp;
			battleOrdersActive = false;
			battleOrdersBonusHp = 0;
			battleOrdersBonusMp = 0;
			battleOrdersTimer.Stop();
		}
		private void MeltyBloodOff(Object sender, EventArgs e)
		{
			hitboxWidth.Disable();
			hitboxHeight.Disable();
			hitbox2Width.Disable();
			hitbox2Height.Disable();

			if (superMelty)
			{
				superMelty = false;
				SetSpeed();
			}

			meltyTimer.Stop();
		}
		private void FourBeastsOff(Object sender, EventArgs e)
		{
			invincibilityCheat.Disable();
			invincibilityLocked = false;
			vermilionBirdActive = false;
			attackPotionCheat.Disable();
			shineCheat.Disable();
			contactDamage.Disable();
			sotnApi.AlucardApi.ContactDamage = 0;
			azureDragonActive = false;
			whiteTigerActive = false;
			vermilionBirdActive = false;
			blackTortoiseActive = false;
			fourBeastsTimer.Stop();
		}
		private void ZawarudoOff(Object sender, EventArgs e)
		{
			stopwatchTimer.Disable();
			zawarudoTimer.Stop();
			zawarudoCheckTimer.Stop();
			zaWarudoActive = false;
		}
		private void ZaWarudoAreaCheck(Object sender, EventArgs e)
		{
			uint zone = sotnApi.GameApi.Zone;
			uint zone2 = sotnApi.GameApi.Zone2;

			if (zaWarudoZone != zone || zaWarudoZone2 != zone2)
			{
				zaWarudoZone = zone;
				zaWarudoZone2 = zone2;
				sotnApi.AlucardApi.ActivateStopwatch();
			}
		}
		private void HasteOff(Object sender, EventArgs e)
		{
			hasteTimer.Stop();
			SetSpeed();
			superHaste = false;
			hasteActive = false;
			speedLocked = false;
			hasteOverdriveOffTimer.Start();
		}
		private void SetHasteStaticSpeeds(bool super = false)
		{
			float superFactor = super ? 2F : 1F;
			float superWingsmashFactor = super ? 1.5F : 1F;
			float factor = toolConfig.Khaos.HasteFactor;
			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5));
			sotnApi.AlucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2));
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2));
			Console.WriteLine("Set speeds:");
			Console.WriteLine($"Wingsmash: {(uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5))}");
			Console.WriteLine($"Wolf dash right: {(sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2))}");
			Console.WriteLine($"Wolf dash left: {(sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2))}");
		}
		private void ToggleHasteDynamicSpeeds(float factor = 1)
		{
			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

			sotnApi.AlucardApi.WalkingWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.WalkingFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalWhole);
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.FallingHorizontalFractSpeed = horizontalFract;
		}
		private void OverdriveOn(Object sender, EventArgs e)
		{
			visualEffectPaletteCheat.PokeValue(33126);
			visualEffectPaletteCheat.Enable();
			visualEffectTimerCheat.PokeValue(30);
			visualEffectTimerCheat.Enable();
			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			hasteOverdriveTimer.Stop();
		}
		private void OverdriveOff(Object sender, EventArgs e)
		{
			visualEffectPaletteCheat.Disable();
			visualEffectTimerCheat.Disable();
			if (hasteActive)
			{
				SetHasteStaticSpeeds(superHaste);
			}
			else
			{
				sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal);
			}
			overdriveOn = false;
			hasteOverdriveOffTimer.Stop();
		}
		private void LordOff(Object sender, EventArgs e)
		{
			spawnActive = false;
			lordEnemies.RemoveRange(0, hordeEnemies.Count);
			lordTimer.Interval = 5 * (60 * 1000);
			lordTimer.Stop();
			lordSpawnTimer.Stop();
		}
		private void LordSpawn(Object sender, EventArgs e)
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5 || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone))
			{
				return;
			}

			uint zone = sotnApi.GameApi.Zone;
			uint zone2 = sotnApi.GameApi.Zone2;

			if (lordZone != zone || lordZone2 != zone2 || lordEnemies.Count == 0)
			{
				lordEnemies.RemoveRange(0, lordEnemies.Count);
				FindLordEnemy();
				lordZone = zone;
				lordZone2 = zone2;
			}
			else if (lordEnemies.Count > 0)
			{
				FindLordEnemy();
				int enemyIndex = rng.Next(0, lordEnemies.Count);
				if (lordTimer.Interval == 5 * (60 * 1000))
				{
					lordTimer.Stop();
					lordTimer.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Duration.TotalMilliseconds;

					ActionTimer timer = new()
					{
						Name = toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Name,
						Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Duration
					};
					statusInfoDisplay.AddTimer(timer);
					notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
					lordTimer.Start();
				}
				lordEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				lordEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				lordEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				sotnApi.ActorApi.SpawnActor(lordEnemies[enemyIndex], false);
			}
		}
		private bool FindLordEnemy()
		{
			uint roomX = sotnApi.GameApi.MapXPos;
			uint roomY = sotnApi.GameApi.MapYPos;

			if ((roomX == lordTriggerRoomX && roomY == lordTriggerRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu())
			{
				return false;
			}

			long enemy = sotnApi.ActorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackHordeEnemies : Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? lordEnemy = new Actor(sotnApi.ActorApi.GetActor(enemy));

				if (lordEnemy is not null && !lordEnemies.Where(e => e.Sprite == lordEnemy.Sprite).Any())
				{
					lordEnemies.Add(lordEnemy);
					Console.WriteLine($"Added Lord enemy with hp: {lordEnemy.Hp} sprite: {lordEnemy.Sprite} damage: {lordEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		private int RollBeast()
		{
			int result = rng.Next(1, 5);

			switch (result)
			{
				case 1:
					if (azureDragonUsed)
					{
						return 0;
					}
					break;
				case 2:
					if (whiteTigerUsed)
					{
						return 0;
					}
					break;
				case 3:
					if (vermilionBirdUsed)
					{
						return 0;
					}
					break;
				case 4:
					if (blackTortoiseUsed)
					{
						return 0;
					}
					break;
				default:
					return 0;
			}

			return result;
		}
		private void AzureDragon(string user)
		{
			fourBeastsTimer.Start();
			azureDragonActive = true;
			azureDragonUsed = true;

			notificationService.AddMessage(user + " used Azure Dragon");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void VermilionBird(string user)
		{
			fourBeastsTimer.Start();
			vermilionBirdActive = true;
			vermilionBirdUsed = true;

			notificationService.AddMessage(user + " used Vermilion Bird");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void WhiteTiger(string user)
		{
			fourBeastsTimer.Start();
			whiteTigerActive = true;
			whiteTigerUsed = true;

			notificationService.AddMessage(user + " used White Tiger");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void BlackTortoise(string user)
		{
			fourBeastsTimer.Start();
			blackTortoiseActive = true;
			blackTortoiseUsed = true;

			notificationService.AddMessage(user + " used Black Tortoise");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void FourHolyBeasts(string user)
		{
			invincibilityCheat.PokeValue(1);
			invincibilityCheat.Enable();
			invincibilityLocked = true;
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			shineCheat.PokeValue(1);
			shineCheat.Enable();
			fourBeastsTimer.Start();
			contactDamage.PokeValue(4);
			contactDamage.Enable();
			azureDragonActive = true;
			whiteTigerActive = true;
			vermilionBirdActive = true;
			blackTortoiseActive = true;
			azureDragonUsed = false;
			whiteTigerUsed = false;
			vermilionBirdUsed = false;
			blackTortoiseUsed = false;

			notificationService.AddMessage(user + " used Four Beasts");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void CheckVermillionBirdFireballs()
		{
			if (vermilionBirdActive && inputService.ButtonPressed(PlaystationInputKeys.Square, 10) && fireballCooldown == 0)
			{
				Actor fireball = new Actor(Constants.Khaos.FireballActorBytes);
				bool alucardFacing = sotnApi.AlucardApi.FacingLeft;
				int offsetX = alucardFacing ? -20 : 20;
				fireball.Xpos = (ushort) (sotnApi.AlucardApi.ScreenX + offsetX);
				fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY - 10);

				long address = sotnApi.ActorApi.SpawnActor(fireball, false);
				LiveActor liveFireball = sotnApi.ActorApi.GetLiveActor(address);
				if (inputService.ButtonPressed(PlaystationInputKeys.Down, 10))
				{
					fireballsDown.Add(liveFireball);
				}
				else if (inputService.ButtonPressed(PlaystationInputKeys.Up, 10))
				{
					fireballsUp.Add(liveFireball);
				}
				else
				{
					fireballs.Add(liveFireball);
				}
				fireballCooldown = 8;
			}

			fireballs.RemoveAll(f => f.Damage == 0 || f.Damage == 80);
			fireballsUp.RemoveAll(f => f.Damage == 0 || f.Damage == 80);
			fireballsDown.RemoveAll(f => f.Damage == 0 || f.Damage == 80);
			foreach (var fball in fireballs)
			{
				fball.Damage = 40;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 4;
				}
				else
				{
					fball.SpeedHorizontal = -5;
				}
			}
			foreach (var fball in fireballsUp)
			{
				fball.Damage = 40;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 4;
				}
				else
				{
					fball.SpeedHorizontal = -5;
				}
				fball.SpeedVertical = -1;
			}
			foreach (var fball in fireballsDown)
			{
				fball.Damage = 40;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 4;
				}
				else
				{
					fball.SpeedHorizontal = -5;
				}
				fball.SpeedVertical = 2;
			}

			if (fireballCooldown > 0)
			{
				fireballCooldown--;
			}
		}
		private void AzureDragonOff(Object sender, EventArgs e)
		{
			var lockOnCheat = cheats.GetCheatByName(Constants.Khaos.SpiritLockOnName);
			lockOnCheat.Disable();
			cheats.RemoveCheat(lockOnCheat);
			azureSpiritActive = false;
			azureDragonTimer.Stop();
		}
		private void WhiteTigerOff(Object sender, EventArgs e)
		{
			var whiteTigerBallCheat = cheats.GetCheatByName(Constants.Khaos.WhiteTigerBallSpeedName);
			whiteTigerBallCheat.Disable();
			cheats.RemoveCheat(whiteTigerBallCheat);
			whiteTigerBallActive = false;
			whiteTigerBallTimer.Stop();
		}
		#endregion

		private void StartCheats()
		{
			faerieScroll.Enable();
			Cheat batCardXp = cheats.GetCheatByName("BatCardXp");
			batCardXp.PokeValue(10000);
			batCardXp.Enable();
			Cheat ghostCardXp = cheats.GetCheatByName("GhostCardXp");
			ghostCardXp.PokeValue(10000);
			ghostCardXp.Enable();
			Cheat faerieCardXp = cheats.GetCheatByName("FaerieCardXp");
			faerieCardXp.Enable();
			Cheat demonCardXp = cheats.GetCheatByName("DemonCardXp");
			demonCardXp.Enable();
			Cheat swordCardXp = cheats.GetCheatByName("SwordCardXp");
			swordCardXp.PokeValue(7000);
			swordCardXp.Enable();
			Cheat spriteCardXp = cheats.GetCheatByName("SpriteCardXp");
			spriteCardXp.Enable();
			Cheat noseDevilCardXp = cheats.GetCheatByName("NoseDevilCardXp");
			noseDevilCardXp.Enable();
			savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			savePalette.Enable();
		}
		private void SetSaveColorPalette()
		{
			if (alucardSecondCastle)
			{
				savePalette.PokeValue(Constants.Khaos.SaveIcosahedronSecondCastle);
			}
			else
			{
				savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			}
		}
		private void CheckMainMenu()
		{
			if (inMainMenu != (sotnApi.GameApi.Status == SotnApi.Constants.Values.Game.Status.MainMenu))
			{
				if (inMainMenu && (sotnApi.GameApi.Status != SotnApi.Constants.Values.Game.Status.InGame))
				{
					return;
				}
				inMainMenu = sotnApi.GameApi.Status == SotnApi.Constants.Values.Game.Status.MainMenu;
				if (inMainMenu)
				{
					GainKhaosMeter((short) toolConfig.Khaos.MeterOnReset);
					battleOrdersBonusHp = 0;
					battleOrdersBonusMp = 0;
				}
			}
		}
		private void CheckCastleChanged()
		{
			if (alucardSecondCastle != sotnApi.GameApi.SecondCastle)
			{
				alucardSecondCastle = sotnApi.GameApi.SecondCastle;
				SetSaveColorPalette();
			}
		}
		public void GetCheats()
		{
			faerieScroll = cheats.GetCheatByName("FaerieScroll");
			darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			underwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			hearts = cheats.GetCheatByName("Hearts");
			curse = cheats.GetCheatByName("CurseTimer");
			manaCheat = cheats.GetCheatByName("Mana");
			attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			defencePotionCheat = cheats.GetCheatByName("DefencePotion");
			stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			hitboxWidth = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			hitboxHeight = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			hitbox2Width = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			hitbox2Height = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			invincibilityCheat = cheats.GetCheatByName("Invincibility");
			shineCheat = cheats.GetCheatByName("Shine");
			visualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			visualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			savePalette = cheats.GetCheatByName("SavePalette");
			contactDamage = cheats.GetCheatByName("ContactDamage");
			music = cheats.GetCheatByName("Music");
		}
		private void SetRelicLocationDisplay(Relic relic, bool take)
		{
			switch (relic)
			{
				case Relic.SoulOfBat:
					if (take)
					{
						if (statusInfoDisplay.BatLocation == "Khaos")
						{
							statusInfoDisplay.BatLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.BatLocation == String.Empty)
						{
							statusInfoDisplay.BatLocation = "Khaos";
						}
					}
					break;
				case Relic.FormOfMist:
					if (take)
					{
						if (statusInfoDisplay.MistLocation == "Khaos")
						{
							statusInfoDisplay.MistLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.MistLocation == String.Empty)
						{
							statusInfoDisplay.MistLocation = "Khaos";
						}
					}
					break;
				case Relic.GravityBoots:
					if (take)
					{
						if (statusInfoDisplay.GravityBootsLocation == "Khaos")
						{
							statusInfoDisplay.GravityBootsLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.GravityBootsLocation == String.Empty)
						{
							statusInfoDisplay.GravityBootsLocation = "Khaos";
						}
					}
					break;
				case Relic.LeapStone:
					if (take)
					{
						if (statusInfoDisplay.LepastoneLocation == "Khaos")
						{
							statusInfoDisplay.LepastoneLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.LepastoneLocation == String.Empty)
						{
							statusInfoDisplay.LepastoneLocation = "Khaos";
						}
					}
					break;
				case Relic.JewelOfOpen:
					if (take)
					{
						if (statusInfoDisplay.JewelOfOpenLocation == "Khaos")
						{
							statusInfoDisplay.JewelOfOpenLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.JewelOfOpenLocation == String.Empty)
						{
							statusInfoDisplay.JewelOfOpenLocation = "Khaos";
						}
					}
					break;
				case Relic.MermanStatue:
					if (take)
					{
						if (statusInfoDisplay.MermanLocation == "Khaos")
						{
							statusInfoDisplay.MermanLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.MermanLocation == String.Empty)
						{
							statusInfoDisplay.MermanLocation = "Khaos";
						}
					}
					break;
				default:
					break;
			}
		}
		private bool IsInRoomList(List<MapLocation> rooms)
		{
			return rooms.Where(r => r.X == alucardMapX && r.Y == alucardMapY && Convert.ToBoolean(r.SecondCastle) == alucardSecondCastle).FirstOrDefault() is not null;
		}
		private void SetSpeed(float factor = 1)
		{
			bool slow = factor < 1;
			bool fast = factor > 1;

			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * factor);
			sotnApi.AlucardApi.WalkingWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.WalkingFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalWhole);
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.FallingHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * factor);
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * factor);
			sotnApi.AlucardApi.BackdashDecel = slow == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
			Console.WriteLine($"Set all speeds with factor {factor}");
		}
		private void SetShaftHp()
		{
			long shaftAddress = sotnApi.ActorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.ShaftOrbActor });
			if (shaftAddress > 0)
			{
				LiveActor shaft = sotnApi.ActorApi.GetLiveActor(shaftAddress);
				shaft.Hp = (int) Constants.Khaos.ShaftKhaosHp;
				shaftHpSet = true;
				Console.WriteLine("Found Shaft actor and set HP to 25.");
			}
			else
			{
				return;
			}
		}
		private void SetGalamothtStats()
		{
			long galamothTorsoAddress = sotnApi.ActorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothTorsoActor });
			if (galamothTorsoAddress > 0)
			{
				LiveActor galamothTorso = sotnApi.ActorApi.GetLiveActor(galamothTorsoAddress);
				galamothTorso.Hp = (int) Constants.Khaos.GalamothKhaosHp;
				galamothTorso.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
				Console.WriteLine($"gala def: {galamothTorso.Def}");
				//galamothTorso.Def = 0; Removes XP gained

				if (enduranceCount > 0)
				{
					enduranceCount--;
					enduranceRoomX = sotnApi.GameApi.MapXPos;
					enduranceRoomY = sotnApi.GameApi.MapYPos;
					if (enduranceCount == 0)
					{
						enduranceSpawnTimer.Stop();
					}
					if (superEndurance)
					{
						galamothTorso.Hp = (ushort) Math.Round(3.5 * Constants.Khaos.GalamothKhaosHp);
						notificationService.AddMessage($"Super Endurance Galamoth");
					}
					else
					{
						galamothTorso.Hp = (ushort) Math.Round(2.3 * Constants.Khaos.GalamothKhaosHp);
						notificationService.AddMessage($"Endurance Galamoth");
					}
				}

				long galamothHeadAddress = sotnApi.ActorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothHeadActor });
				LiveActor galamothHead = sotnApi.ActorApi.GetLiveActor(galamothHeadAddress);
				galamothHead.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;

				List<long> galamothParts = sotnApi.ActorApi.GetAllActors(new List<SearchableActor> { Constants.Khaos.GalamothPartsActors });
				foreach (long actor in galamothParts)
				{
					LiveActor galamothAnchor = sotnApi.ActorApi.GetLiveActor(actor);
					galamothAnchor.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
					galamothAnchor.Def = 0;
				}

				galamothStatsSet = true;
				Console.WriteLine("Found Galamoth actor and set stats.");
			}
			else
			{
				return;
			}
		}
		private void CheckManaUsage()
		{
			uint currentMana = sotnApi.AlucardApi.CurrentMp;
			spentMana = 0;
			if (currentMana < storedMana)
			{
				spentMana = (int) storedMana - (int) currentMana;
			}
			storedMana = currentMana;
			BloodManaUpdate();
		}
		private void CheckDashInput()
		{
			if (inputService.RegisteredMove(InputKeys.Dash, Globals.UpdateCooldownFrames) && !hasteSpeedOn && hasteActive)
			{
				ToggleHasteDynamicSpeeds(superHaste ? toolConfig.Khaos.HasteFactor * Constants.Khaos.HasteDashFactor : toolConfig.Khaos.HasteFactor);
				hasteSpeedOn = true;
				hasteOverdriveTimer.Start();
			}
			else if (!inputService.ButtonHeld(InputKeys.Forward) && hasteSpeedOn)
			{
				ToggleHasteDynamicSpeeds();
				hasteSpeedOn = false;
				hasteOverdriveTimer.Stop();
				if (overdriveOn)
				{
					hasteOverdriveOffTimer.Start();
				}
			}
		}
		private bool KhaosMeterFull()
		{
			return notificationService.KhaosMeter >= 100;
		}
		private void GainKhaosMeter(short meter)
		{
			notificationService.KhaosMeter += meter;
			totalMeterGained += meter;
		}
		private void SpendKhaosMeter()
		{
			notificationService.KhaosMeter -= 100;
		}
		private void Alert(Configuration.Models.Action action)
		{
			if (!toolConfig.Khaos.Alerts)
			{
				return;
			}

			if (action is not null && action.AlertPath is not null && action.AlertPath != String.Empty)
			{
				notificationService.PlayAlert(action.AlertPath);
			}
		}
	}
}

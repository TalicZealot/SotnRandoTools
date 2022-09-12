using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Main;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Models;
using MapLocation = SotnRandoTools.RandoTracker.Models.MapLocation;

namespace SotnRandoTools.Khaos
{
	internal sealed class KhaosController : IKhaosController
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly ICheatsController cheatsController;
		private readonly INotificationService notificationService;
		private readonly IInputService inputService;
		private readonly IKhaosActionsInfoDisplay statusInfoDisplay;
		private readonly IKhaosEventScheduler eventScheduler;

		private readonly Random rng = new();
		private int khaosMeter = 0;
		private int pandoraProgress = 0;
		private bool alucardSecondCastle = false;
		private bool inMainMenu = false;

		private uint goalVampireKills = 0;
		private uint currentVampireKills = 0;
		private uint currentKills = 0;
		private uint vampureSwordLevel = 0;

		private uint dracMusicCounter = 0;

		private float thirstLevel = 0;

		private uint hordeZone = 0;
		private uint hordeTriggerRoomX = 0;
		private uint hordeTriggerRoomY = 0;
		private List<Entity> hordeEnemies = new();

		private uint lordZone = 0;
		private uint lordTriggerRoomX = 0;
		private uint lordTriggerRoomY = 0;
		private List<Entity> lordEnemies = new();

		private int enduranceCount = 0;
		private int superEnduranceCount = 0;
		private uint enduranceRoomX = 0;
		private uint enduranceRoomY = 0;

		private bool zaWarudoActive = false;
		private uint zaWarudoZone = 0;

		private uint storedMana = 0;
		private uint storedMaxMana = 0;
		private int spentMana = 0;

		private int fireballCooldown = 0;
		private List<LiveEntity> fireballs = new();

		private bool bloodManaActive = false;
		private bool vampireActive = false;
		private bool hasteActive = false;
		private bool hasteSpeedOn = false;
		private bool overdriveOn = false;
		private bool subweaponsOnlyActive = false;
		private bool gasCloudTaken = false;
		private bool thirstActive = false;
		private bool slowActive = false;
		private bool slowPaused = false;
		private bool forwardDashActive = false;

		private bool vermilionBirdPollong = false;

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

		private bool superThirst = false;
		private bool superHorde = false;
		private bool superMelty = false;
		private bool superHaste = false;

		private bool hnkOn = false;
		private int hnkToggled = 0;

		private int bankruptLevel = 1;
		private short mainMenuCounter = 0;

		public KhaosController(IToolConfig toolConfig, ISotnApi sotnApi, ICheatsController cheatsController, INotificationService notificationService, IInputService inputService, IKhaosActionsInfoDisplay statusInfoDisplay, IKhaosEventScheduler eventScheduler)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (cheatsController is null) throw new ArgumentNullException(nameof(cheatsController));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			if (inputService is null) throw new ArgumentNullException(nameof(inputService));
			if (statusInfoDisplay is null) throw new ArgumentNullException(nameof(statusInfoDisplay));
			if (eventScheduler is null) throw new ArgumentNullException(nameof(eventScheduler));
			this.toolConfig = toolConfig;
			this.sotnApi = sotnApi;
			this.cheatsController = cheatsController;
			this.notificationService = notificationService;
			this.inputService = inputService;
			this.statusInfoDisplay = statusInfoDisplay;
			this.eventScheduler = eventScheduler;

			InitializeTimers();
		}

		public bool AutoKhaosOn { get; set; }
		public bool SpeedLocked { get; set; }
		public bool ManaLocked { get; set; }
		public bool InvincibilityLocked { get; set; }
		public bool SpawnActive { get; set; }
		public bool ShaftHpSet { get; set; }
		public bool GalamothStatsSet { get; set; }
		public bool PandoraUsed { get; set; }
		public int TotalMeterGained { get; set; }
		public uint AlucardMapX { get; set; }
		public uint AlucardMapY { get; set; }

		public void StartKhaos()
		{
			cheatsController.StartCheats();
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
			sotnApi.GameApi.OverwriteString(SotnApi.Constants.Addresses.Strings.ItemNameAddresses["Library card"], "Khaotic card", true);
			Console.WriteLine("Khaos started");
		}
		public void StopKhaos()
		{
			StopTimers();
			cheatsController.FaerieScroll.Disable();
			notificationService.AddMessage($"Khaos stopped");
			Console.WriteLine("Khaos stopped");
		}
		public void GainKhaosMeter(short meter)
		{
			khaosMeter += meter;
			TotalMeterGained += meter;

			notificationService.UpdateOverlayMeter(khaosMeter);

			if (!PandoraUsed && pandoraProgress < 6 && TotalMeterGained >= (toolConfig.Khaos.PandoraTrigger / 7) * (pandoraProgress + 1))
			{
				string label = "PANDORA";
				notificationService.AddMessage(label.Substring(0, pandoraProgress + 1));
				pandoraProgress++;
			}
		}
		public bool IsInRoomList(List<MapLocation> rooms)
		{
			return rooms.Where(r => r.X == AlucardMapX && r.Y == AlucardMapY && Convert.ToBoolean(r.SecondCastle) == alucardSecondCastle).Any();
		}
		public void SetShaftHp()
		{
			long shaftAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.ShaftOrbActor });
			if (shaftAddress > 0)
			{
				LiveEntity shaft = sotnApi.EntityApi.GetLiveEntity(shaftAddress);
				shaft.Hp = (int) Constants.Khaos.ShaftKhaosHp;
				ShaftHpSet = true;
				Console.WriteLine("Found Shaft actor and set HP to 25.");
			}
		}
		public void SetGalamothtStats()
		{
			long galamothTorsoAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.GalamothTorsoActor });
			if (galamothTorsoAddress > 0)
			{
				LiveEntity galamothTorso = sotnApi.EntityApi.GetLiveEntity(galamothTorsoAddress);
				galamothTorso.Hp = (int) Constants.Khaos.GalamothKhaosHp;
				galamothTorso.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
				Console.WriteLine($"gala def: {galamothTorso.Def}");
				//galamothTorso.Def = 0; Removes XP gained

				if (enduranceCount > 0)
				{
					enduranceCount--;
					enduranceRoomX = sotnApi.GameApi.RoomX;
					enduranceRoomY = sotnApi.GameApi.RoomY;
					if (enduranceCount == 0)
					{
						eventScheduler.EnduranceSpawnTimer = false;
					}
					if (superEnduranceCount > 0)
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

				long galamothHeadAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.GalamothHeadActor });
				LiveEntity galamothHead = sotnApi.EntityApi.GetLiveEntity(galamothHeadAddress);
				galamothHead.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;

				List<long> galamothParts = sotnApi.EntityApi.GetAllActors(new List<SearchableActor> { Constants.Khaos.GalamothPartsActors });
				foreach (long actor in galamothParts)
				{
					LiveEntity galamothAnchor = sotnApi.EntityApi.GetLiveEntity(actor);
					galamothAnchor.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
					galamothAnchor.Def = 0;
				}

				GalamothStatsSet = true;
				Console.WriteLine("Found Galamoth actor and set stats.");
			}
		}
		public void CheckMainMenu()
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
					GainKhaosMeter((short) (toolConfig.Khaos.MeterOnReset * mainMenuCounter));
					mainMenuCounter++;
					battleOrdersBonusHp = 0;
					battleOrdersBonusMp = 0;

					thirstLevel = 0;
					cheatsController.VisualEffectPalette.Disable();
					cheatsController.VisualEffectTimer.Disable();
				}
			}
		}
		public void CheckCastleChanged()
		{
			if (alucardSecondCastle != sotnApi.GameApi.SecondCastle)
			{
				alucardSecondCastle = sotnApi.GameApi.SecondCastle;
				SetSaveColorPalette();
			}
		}

		#region Khaotic Effects
		//TODO: random action
		//TODO: Banish force khaotic card
		public void KhaosStatus(string user = Constants.Khaos.KhaosName)
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

			int index = rng.Next(0, SotnApi.Constants.Values.Game.Various.SafeLibraryCardZones.Count);
			TeleportZone zone = SotnApi.Constants.Values.Game.Various.SafeLibraryCardZones[index];
			sotnApi.GameApi.SetLibraryCardDestination(zone.Zone, zone.Xpos, zone.Ypos, zone.Room);

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
					ActivateDizzy();
					notificationService.AddMessage($"{user} confused you");
					break;
				case 6:
					sotnApi.AlucardApi.CurrentMp -= 15;
					notificationService.AddMessage($"{user} used Mana Burn");
					break;
				case 7:
					sotnApi.AlucardApi.ActivatePotion(Potion.Antivenom);
					notificationService.AddMessage($"{user} used an antivenom");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.StrPotion);
					notificationService.AddMessage($"{user} gave you strength");
					break;
				case 9:
					sotnApi.AlucardApi.Heal(15);
					notificationService.AddMessage($"{user} used a minor heal");
					break;
				case 10:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defence");
					break;
				case 11:
					ActivateGuardianSpirits();
					notificationService.AddMessage($"{user} used Guardian Spirits");
					break;
				default:
					break;
			}

			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStatus]);
		}
		public void KhaosEquipment(string user = Constants.Khaos.KhaosName)
		{
			RandomizeEquipmentSlots();
			notificationService.AddMessage($"{user} used Khaos Equipment");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosEquipment]);
		}
		public void KhaosStats(string user = Constants.Khaos.KhaosName)
		{
			RandomizeStatsActivate();
			notificationService.AddMessage($"{user} used Khaos Stats");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStats]);
		}
		public void KhaosRelics(string user = Constants.Khaos.KhaosName)
		{
			RandomizeRelicsActivate(false);
			sotnApi.AlucardApi.GrantItemByName("Library card");
			sotnApi.AlucardApi.GrantItemByName("Library card");
			notificationService.AddMessage($"{user} used Khaos Relics");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosRelics]);
		}
		public void PandorasBox(string user = Constants.Khaos.KhaosName)
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
		public void Gamble(string user = Constants.Khaos.KhaosName)
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
		public void KhaoticBurst(string user = Constants.Khaos.KhaosName)
		{
			GainKhaosMeter(100);
			notificationService.AddMessage($"{user} used Khaotic Burst");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaoticBurst]);
		}
		public void KhaosTrack(string track, string user = Constants.Khaos.KhaosName)
		{
			int trackIndex = Array.IndexOf(Constants.Khaos.AcceptedMusicTrackTitles, track.ToLower().Trim());
			bool alternateTitle = Constants.Khaos.AlternateTrackTitles.ContainsKey(track.ToLower().Trim());

			if (trackIndex >= 0)
			{
				trackIndex = (int) Various.MusicTracks[Constants.Khaos.AcceptedMusicTrackTitles[trackIndex]];
			}
			else if (alternateTitle)
			{
				string foundTrack = Constants.Khaos.AlternateTrackTitles[track.ToLower().Trim()];
				trackIndex = (int) Various.MusicTracks[foundTrack];
			}
			else
			{
				int roll = rng.Next(0, Constants.Khaos.AcceptedMusicTrackTitles.Length - 1);
				trackIndex = (int) Various.MusicTracks[Constants.Khaos.AcceptedMusicTrackTitles[roll]];
			}
			cheatsController.Music.PokeValue(trackIndex);
			cheatsController.Music.Enable();
			eventScheduler.KhaosTrackTimer = true;
			notificationService.AddMessage($"{user} queued {track}");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack]);
		}
		#endregion
		#region Debuffs
		public void Bankrupt(string user = Constants.Khaos.KhaosName)
		{
			notificationService.AddMessage($"{user} used Bankrupt level {bankruptLevel}");
			BankruptActivate();
			sotnApi.AlucardApi.GrantItemByName("Library card");
			sotnApi.AlucardApi.GrantItemByName("Library card");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Bankrupt]);
		}
		public void Weaken(string user = Constants.Khaos.KhaosName)
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

			uint newCurrentHp = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newCurrentMp = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newCurrentHearts = (uint) (sotnApi.AlucardApi.CurrentHearts * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newMaxtHp = (uint) (sotnApi.AlucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newMaxtMp = (uint) (sotnApi.AlucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newMaxtHearts = (uint) (sotnApi.AlucardApi.MaxtHearts * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newStr = (uint) (sotnApi.AlucardApi.Str * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newCon = (uint) (sotnApi.AlucardApi.Con * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newInt = (uint) (sotnApi.AlucardApi.Int * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newLck = (uint) (sotnApi.AlucardApi.Lck * toolConfig.Khaos.WeakenFactor * enhancedFactor);

			sotnApi.AlucardApi.CurrentHp = newCurrentHp >= Constants.Khaos.MinimumHp ? newCurrentHp : Constants.Khaos.MinimumHp;
			sotnApi.AlucardApi.CurrentMp = newCurrentMp >= Constants.Khaos.MinimumMp ? newCurrentHp : Constants.Khaos.MinimumMp;
			sotnApi.AlucardApi.CurrentHearts = newCurrentHearts >= Constants.Khaos.MinimumHearts ? newCurrentHearts : Constants.Khaos.MinimumHearts;
			sotnApi.AlucardApi.MaxtHp = newMaxtHp >= Constants.Khaos.MinimumHp ? newMaxtHp : Constants.Khaos.MinimumHp;
			sotnApi.AlucardApi.MaxtMp = newMaxtMp >= Constants.Khaos.MinimumMp ? newMaxtMp : Constants.Khaos.MinimumMp;
			sotnApi.AlucardApi.MaxtHearts = newMaxtHearts >= Constants.Khaos.MinimumHearts ? newMaxtHearts : Constants.Khaos.MinimumHearts;
			sotnApi.AlucardApi.Str = newStr >= Constants.Khaos.MinimumStat ? newStr : Constants.Khaos.MinimumStat;
			sotnApi.AlucardApi.Con = newCon >= Constants.Khaos.MinimumStat ? newCon : Constants.Khaos.MinimumStat;
			sotnApi.AlucardApi.Int = newInt >= Constants.Khaos.MinimumStat ? newInt : Constants.Khaos.MinimumStat;
			sotnApi.AlucardApi.Lck = newLck >= Constants.Khaos.MinimumStat ? newLck : Constants.Khaos.MinimumStat;

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
		public void RespawnBosses(string user = Constants.Khaos.KhaosName)
		{
			sotnApi.GameApi.RespawnBosses();
			notificationService.AddMessage($"{user} used Respawn Bosses");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.RespawnBosses]);
		}
		public void SubweaponsOnly(string user = Constants.Khaos.KhaosName)
		{
			int roll = rng.Next(1, 10);
			while (roll == 6)
			{
				roll = rng.Next(1, 10);
			}
			sotnApi.AlucardApi.Subweapon = (Subweapon) roll;
			sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
			sotnApi.AlucardApi.GrantRelic(Relic.CubeOfZoe, true);
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			cheatsController.Hearts.Enable();
			cheatsController.Curse.Enable();
			cheatsController.Mana.PokeValue(7);
			cheatsController.Mana.Enable();
			ManaLocked = true;
			subweaponsOnlyActive = true;
			eventScheduler.SubweaponsOnlyTimer = true;
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
		public void Slow(string user = Constants.Khaos.KhaosName)
		{
			cheatsController.UnderwaterPhysics.Enable();
			eventScheduler.SlowTimer = true;
			slowActive = true;

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Duration
			};

			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			string message = $"{user} used {toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Name}";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Slow]);
		}
		public void BloodMana(string user = Constants.Khaos.KhaosName)
		{
			storedMana = sotnApi.AlucardApi.CurrentMp;
			storedMaxMana = sotnApi.AlucardApi.MaxtMp;
			bloodManaActive = true;
			eventScheduler.BloodManaTimer = true;
			ManaLocked = true;
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
		public void Thirst(string user = Constants.Khaos.KhaosName)
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superThirst = true;
				SpendKhaosMeter();
			}

			cheatsController.DarkMetamorphasis.PokeValue(1);
			cheatsController.DarkMetamorphasis.Enable();

			eventScheduler.ThirstTimer = true;
			eventScheduler.ThirstTickTimer = true;

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			if (meterFull)
			{
				timer.Name = "Super " + timer.Name;
			}
			statusInfoDisplay.AddTimer(timer);

			string message = meterFull ? $"{user} used Super Thirst" : $"{user} used Thirst";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Thirst]);
			thirstActive = true;
		}
		public void Horde(string user = Constants.Khaos.KhaosName)
		{
			hordeTriggerRoomX = sotnApi.GameApi.RoomX;
			hordeTriggerRoomY = sotnApi.GameApi.RoomY;
			SpawnActive = true;
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superHorde = true;
				SpendKhaosMeter();
			}

			eventScheduler.HordeTimer = true;
			eventScheduler.HordeSpawnTimer = true;
			hordeEnemies.Clear();
			string message = meterFull ? $"{user} summoned the Super Horde" : $"{user} summoned the Horde";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde]);
		}
		public void Endurance(string user = Constants.Khaos.KhaosName)
		{
			enduranceRoomX = sotnApi.GameApi.RoomX;
			enduranceRoomY = sotnApi.GameApi.RoomY;
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superEnduranceCount++;
				SpendKhaosMeter();
			}

			enduranceCount++;
			eventScheduler.EnduranceSpawnTimer = true;
			string message = meterFull ? $"{user} used Super Endurance" : $"{user} used Endurance";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Endurance]);
		}
		public void HnK(string user = Constants.Khaos.KhaosName)
		{
			hnkOn = true;
			cheatsController.DefencePotion.PokeValue(1);
			cheatsController.DefencePotion.Enable();
			InvincibilityLocked = true;
			eventScheduler.HnkTimer = true;
			forwardDashActive = true;
			FlipBackdash();

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
		//TODO: Add Quad Damage, str buff on successful kill streak
		public void Vampire(string user = Constants.Khaos.KhaosName)
		{
			cheatsController.DarkMetamorphasis.PokeValue(1);
			cheatsController.DarkMetamorphasis.Enable();
			eventScheduler.VampireTimer = true;
			notificationService.AddMessage(user + " used Vampire");
			sotnApi.GameApi.OverwriteString(SotnApi.Constants.Addresses.Strings.ItemNameAddresses["Gurthang"], "CravenEdge", false);
			sotnApi.AlucardApi.GrantItemByName("Gurthang");
			vampireActive = true;

			if (goalVampireKills == 0)
			{
				goalVampireKills = 10 + (vampureSwordLevel * vampureSwordLevel);
			}

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Vampire]);
		}
		public void LightHelp(string user = Constants.Khaos.KhaosName)
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
			if ((highMp || ManaLocked) && roll == 3)
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
		public void MediumHelp(string user = Constants.Khaos.KhaosName)
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

			int roll = rng.Next(1, ManaLocked ? 3 : 4);

			if (highHp && roll == 2)
			{
				roll = 3;
			}
			if ((highMp && roll == 3) || ManaLocked)
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
		public void HeavytHelp(string user = Constants.Khaos.KhaosName)
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
						sotnApi.AlucardApi.GrantRelic(Constants.Khaos.ProgressionRelics[relic], true);
						notificationService.AddMessage($"{user} gave you {Constants.Khaos.ProgressionRelics[relic]}");
						break;
					default:
						break;
				}
			}
		}
		//TODO: Detect save rooms and turn off temporarily
		public void BattleOrders(string user = Constants.Khaos.KhaosName)
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

			eventScheduler.BattleOrdersTimer = true;
			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Duration
			};
			statusInfoDisplay.AddTimer(timer);
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
		}
		public void Magician(string user = Constants.Khaos.KhaosName)
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				SpendKhaosMeter();
				SetRelicLocationDisplay(Relic.SoulOfBat, false);
				SetRelicLocationDisplay(Relic.FormOfMist, false);
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfBat, true);
				sotnApi.AlucardApi.GrantRelic(Relic.FireOfBat, true);
				sotnApi.AlucardApi.GrantRelic(Relic.EchoOfBat, true);
				sotnApi.AlucardApi.GrantRelic(Relic.ForceOfEcho, true);
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfWolf, true);
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfWolf, true);
				sotnApi.AlucardApi.GrantRelic(Relic.SkillOfWolf, true);
				sotnApi.AlucardApi.GrantRelic(Relic.FormOfMist, true);
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfMist, true);
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud, true);
			}

			sotnApi.AlucardApi.GrantItemByName("Wizard hat");
			sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
			cheatsController.Mana.PokeValue((int) sotnApi.AlucardApi.MaxtMp);
			cheatsController.Mana.Enable();
			ManaLocked = true;
			eventScheduler.MagicianTimer = true;

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			if (meterFull)
			{
				timer.Name = "Shapeshifter";
			}
			statusInfoDisplay.AddTimer(timer);

			string message = meterFull ? $"{user} activated Shapeshifter" : $"{user} activated Magician";
			notificationService.AddMessage(message);

			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Magician]);
		}
		public void MeltyBlood(string user = Constants.Khaos.KhaosName)
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superMelty = true;
				SetHasteStaticSpeeds(true);
				sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
				sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
				sotnApi.AlucardApi.ActivatePotion(Potion.StrPotion);
				sotnApi.AlucardApi.AttackPotionTimer = Constants.Khaos.GuiltyGearAttack;
				sotnApi.AlucardApi.DarkMetamorphasisTimer = Constants.Khaos.GuiltyGearDarkMetamorphosis;
				sotnApi.AlucardApi.DefencePotionTimer = Constants.Khaos.GuiltyGearDefence;
				sotnApi.AlucardApi.InvincibilityTimer = Constants.Khaos.GuiltyGearInvincibility;
				SpendKhaosMeter();
			}

			cheatsController.HitboxWidth.Enable();
			cheatsController.HitboxHeight.Enable();
			cheatsController.Hitbox2Width.Enable();
			cheatsController.Hitbox2Height.Enable();
			FlipBackdash();
			forwardDashActive = true;
			SetRelicLocationDisplay(Relic.LeapStone, false);
			sotnApi.AlucardApi.GrantRelic(Relic.LeapStone, true);
			eventScheduler.MeltyTimer = true;
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
		public void FourBeasts(string user = Constants.Khaos.KhaosName)
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
		public void ZaWarudo(string user = Constants.Khaos.KhaosName)
		{
			if (sotnApi.AlucardApi.SubweaponTimer == 0)
			{
				sotnApi.AlucardApi.ActivateStopwatch();
				cheatsController.SubweaponTimer.Enable();
				cheatsController.SubweaponTimer.PokeValue(1);
				zaWarudoZone = sotnApi.GameApi.Zone2;
				zaWarudoActive = true;
			}

			if (!subweaponsOnlyActive)
			{
				sotnApi.AlucardApi.Subweapon = Subweapon.Stopwatch;
			}

			eventScheduler.ZawarudoTimer = true;
			eventScheduler.ZawarudoCheckTimer = true;

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
		public void Haste(string user = Constants.Khaos.KhaosName)
		{
			bool meterFull = KhaosMeterFull();

			if (meterFull)
			{
				SpendKhaosMeter();
				superHaste = true;
			}
			hasteActive = true;
			SpeedLocked = true;
			SetHasteStaticSpeeds(meterFull);
			inputService.Polling++;
			inputService.ReadDash = true;
			eventScheduler.HasteTimer = true;
			Console.WriteLine(user + " used " + toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Name);

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Name,
				Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			if (meterFull)
			{
				timer.Name = "Super " + timer.Name;
			}
			statusInfoDisplay.AddTimer(timer);
			string message = meterFull ? $"{user} activated Super Haste" : $"{user} activated Haste";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Haste]);
		}
		public void Lord(string user = Constants.Khaos.KhaosName)
		{
			lordTriggerRoomX = sotnApi.GameApi.RoomX;
			lordTriggerRoomY = sotnApi.GameApi.RoomY;
			SpawnActive = true;

			eventScheduler.LordTimer = true;
			eventScheduler.LordSpawnTimer = true;
			lordEnemies.Clear();
			string message = user + " activated Lord of this Castle";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.Lord]);
		}
		#endregion

		public void Update()
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.AlucardApi.HasHitbox() || sotnApi.AlucardApi.CurrentHp < 1
				|| sotnApi.GameApi.InTransition || sotnApi.GameApi.IsLoading || inMainMenu)
			{
				return;
			}

			if (sotnApi.GameApi.MapOpen)
			{
				if (sotnApi.GameApi.SecondCastle)
				{
					notificationService.InvertedMapOpen = true;
				}
				else
				{
					notificationService.MapOpen = true;
				}
			}
			else
			{
				notificationService.MapOpen = false;
				notificationService.InvertedMapOpen = false;
			}

			//TODO: disable / adjust actions in the end game
			CheckDracularoomMusic();
			CheckDashInput();
			CheckThirstKill();
			CheckVampireKill();
			CheckVermillionBirdFireballs();
			CheckAzureDragon();
			CheckBlackTortoise();
			CheckWhiteDragon();

			CheckManaUsage();
			FixEntranceSlow();
			FixSlowWarps();
			CheckSubweaponsOnlyEquipment();

			HandleHnkInvincibility();
		}
		private void InitializeTimers()
		{
			eventScheduler.Dizzy.Invoker = new MethodInvoker(() => DizzyOff());

			eventScheduler.BloodManaDeath.Invoker = new MethodInvoker(() => KillAlucard());
			eventScheduler.KhaosTrack.Invoker = new MethodInvoker(() => KhaosTrackOff());
			eventScheduler.SubweaponsOnly.Invoker = new MethodInvoker(() => SubweaponsOnlyOff());
			eventScheduler.Slow.Invoker = new MethodInvoker(() => SlowOff());
			eventScheduler.BloodMana.Invoker = new MethodInvoker(() => BloodManaOff());
			eventScheduler.Thirst.Invoker = new MethodInvoker(() => ThirstOff());
			eventScheduler.ThirstTick.Invoker = new MethodInvoker(() => ThirstDrain());
			eventScheduler.Horde.Invoker = new MethodInvoker(() => HordeOff());
			eventScheduler.HordeSpawn.Invoker = new MethodInvoker(() => HordeSpawn());
			eventScheduler.EnduranceSpawn.Invoker = new MethodInvoker(() => EnduranceSpawn());
			eventScheduler.Hnk.Invoker = new MethodInvoker(() => HnkOff());

			eventScheduler.Vampire.Invoker = new MethodInvoker(() => VampireOff());
			eventScheduler.Magician.Invoker = new MethodInvoker(() => MagicianOff());
			eventScheduler.BattleOrders.Invoker = new MethodInvoker(() => BattleOrdersOff());
			eventScheduler.Melty.Invoker = new MethodInvoker(() => MeltyBloodOff());
			eventScheduler.FourBeasts.Invoker = new MethodInvoker(() => FourBeastsOff());
			eventScheduler.AzureDragon.Invoker = new MethodInvoker(() => AzureDragonOff());
			eventScheduler.WhiteTigerBall.Invoker = new MethodInvoker(() => WhiteTigerOff());
			eventScheduler.Zawarudo.Invoker = new MethodInvoker(() => ZawarudoOff());
			eventScheduler.ZawarudoCheck.Invoker = new MethodInvoker(() => ZaWarudoAreaCheck());
			eventScheduler.Haste.Invoker = new MethodInvoker(() => HasteOff());
			eventScheduler.HasteOverdrive.Invoker = new MethodInvoker(() => OverdriveOn());
			eventScheduler.HasteOverdriveOff.Invoker = new MethodInvoker(() => OverdriveOff());
			eventScheduler.Lord.Invoker = new MethodInvoker(() => LordOff());
			eventScheduler.LordSpawn.Invoker = new MethodInvoker(() => LordSpawn());
			eventScheduler.GuardianSpirits.Invoker = new MethodInvoker(() => GuardianSpiritsOff());
		}
		private void StopTimers()
		{
			eventScheduler.StopTimers();
		}

		#region Khaotic events
		private int RollStatus(bool entranceCutscene, bool succubusRoom, bool alucardIsImmuneToCurse, bool alucardIsImmuneToStone, bool alucardIsImmuneToPoison, bool highHp)
		{
			int min = 1;
			int max = 12;
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
				case 6:
					if (sotnApi.AlucardApi.CurrentMp < 15 || ManaLocked)
					{
						return 0;
					}
					break;
				case 7:
					if (zaWarudoActive)
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
					if (highHp)
					{
						return 0;
					}
					break;
				case 10:
					if (zaWarudoActive)
					{
						return 0;
					}
					break;
				case 11:
					if (zaWarudoActive || sotnApi.AlucardApi.SubweaponTimer > 0)
					{
						return 0;
					}
					break;
				default:
					break;
			}

			return result;
		}
		private void KhaosTrackOff()
		{
			cheatsController.Music.Disable();
			eventScheduler.KhaosTrackTimer = false;
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
			uint totalMinStats = Constants.Khaos.MinimumStat * 4;
			uint totalMinPool = sotnApi.AlucardApi.CurrentHp + sotnApi.AlucardApi.CurrentMp;

			uint statPool = str + con + intel + lck > totalMinStats ? str + con + intel + lck - totalMinStats : str + con + intel + lck;
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

			sotnApi.AlucardApi.Str = Constants.Khaos.MinimumStat + newStr;
			sotnApi.AlucardApi.Con = Constants.Khaos.MinimumStat + newCon;
			sotnApi.AlucardApi.Int = Constants.Khaos.MinimumStat + newInt;
			sotnApi.AlucardApi.Lck = Constants.Khaos.MinimumStat + newLck;

			uint pointsPool = maxHp + maxMana > totalMinPool ? maxHp + maxMana - totalMinPool : maxHp + maxMana;
			if (maxHp + maxMana < totalMinPool)
			{
				pointsPool = totalMinPool;
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
			uint pointsHp = Constants.Khaos.MinimumHp + (uint) Math.Round(hpPercent * pointsPool);
			uint pointsMp = Constants.Khaos.MinimumMp + pointsPool - (uint) Math.Round(hpPercent * pointsPool);

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
				int roll = rng.Next(0, 2);
				if (roll > 0)
				{
					if ((int) relic < 25 && !sotnApi.AlucardApi.HasRelic((Relic) relic))
					{
						SetRelicLocationDisplay((Relic) relic, false);
						sotnApi.AlucardApi.GrantRelic((Relic) relic, true);
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
					sotnApi.AlucardApi.GrantRelic((Relic) relic, true);
				}
			}

			if (IsInRoomList(Constants.Khaos.SwitchRoom))
			{
				SetRelicLocationDisplay(Relic.JewelOfOpen, false);
				sotnApi.AlucardApi.GrantRelic(Relic.JewelOfOpen, true);
			}
		}
		//TODO: Try clearing entities for forced eqip
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
			uint currentMaxMana = sotnApi.AlucardApi.MaxtMp;

			if (currentMaxMana < storedMaxMana)
			{
				storedMaxMana = currentMaxMana;
				return;
			}

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
					eventScheduler.BloodManaDeathTimer = true;
				}
			}
		}
		private void KillAlucard()
		{
			Entity hitbox = new Entity();
			uint offsetPosX = sotnApi.AlucardApi.ScreenX - 255;
			uint offsetPosY = sotnApi.AlucardApi.ScreenY - 255;

			hitbox.Xpos = 0;
			hitbox.Ypos = 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.DamageTypeA = (uint) Entities.Slam;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 999;
			sotnApi.EntityApi.SpawnEntity(hitbox);
			sotnApi.AlucardApi.InvincibilityTimer = 0;
			eventScheduler.BloodManaDeathTimer = false;
		}
		private void BloodManaOff()
		{
			ManaLocked = false;
			bloodManaActive = false;
			eventScheduler.BloodManaTimer = false;
		}
		private void CheckThirstKill()
		{
			if (!thirstActive)
			{
				return;
			}

			uint updatedCurrentKills = sotnApi.AlucardApi.Kills;

			if (updatedCurrentKills < currentKills)
			{
				currentKills = updatedCurrentKills;
			}

			if (updatedCurrentKills > currentKills || sotnApi.GameApi.CanSave())
			{
				currentKills = updatedCurrentKills;
				thirstLevel = 0;
				cheatsController.VisualEffectPalette.Disable();
				cheatsController.VisualEffectTimer.Disable();
			}
			else if (thirstLevel < 2.8)
			{
				thirstLevel += Constants.Khaos.ThirstLevelIncreaseRate;
				if (thirstLevel > 2.3F)
				{
					cheatsController.VisualEffectPalette.PokeValue(Constants.Khaos.BloodthirstColorPalette);
					cheatsController.VisualEffectPalette.Enable();
					cheatsController.VisualEffectTimer.PokeValue(30);
					cheatsController.VisualEffectTimer.Enable();
				}
			}
		}
		private void ThirstDrain()
		{
			if (thirstLevel < 1)
			{
				return;
			}

			uint superDrain = superThirst ? Constants.Khaos.SuperThirstExtraDrain : 0u;

			uint drainAmount = (uint) Math.Round((toolConfig.Khaos.ThirstDrainPerSecond + superDrain) * thirstLevel);

			if (sotnApi.AlucardApi.CurrentHp > drainAmount + 1)
			{
				sotnApi.AlucardApi.CurrentHp -= drainAmount;
			}
			else
			{
				sotnApi.AlucardApi.CurrentHp = 1;
			}
		}
		private void ThirstOff()
		{
			cheatsController.VisualEffectPalette.Disable();
			cheatsController.VisualEffectTimer.Disable();
			cheatsController.DarkMetamorphasis.Disable();
			eventScheduler.ThirstTimer = false;
			eventScheduler.ThirstTickTimer = false;
			superThirst = false;
			thirstActive = false;
		}
		private void HordeOff()
		{
			superHorde = false;
			SpawnActive = false;
			hordeEnemies.RemoveRange(0, hordeEnemies.Count);
			eventScheduler.Horde.Interval = 5 * (60 * 1000);
			eventScheduler.HordeTimer = false;
			eventScheduler.HordeSpawnTimer = false;
		}
		private void HordeSpawn()
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5 || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone))
			{
				return;
			}

			uint zone2 = sotnApi.GameApi.Zone2;

			if (hordeZone != zone2)
			{
				hordeEnemies.Clear();
				hordeZone = zone2;
			}

			FindHordeEnemy();

			if (hordeEnemies.Count > 0)
			{
				int enemyIndex = rng.Next(0, hordeEnemies.Count);
				if (eventScheduler.Horde.Interval == 5 * (60 * 1000))
				{
					eventScheduler.HordeTimer = false;
					eventScheduler.Horde.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Duration.TotalMilliseconds;

					ActionTimer timer = new()
					{
						Name = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Name,
						Duration = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Duration
					};
					notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
					if (superHorde)
					{
						timer.Name = "Super " + timer.Name;
					}
					statusInfoDisplay.AddTimer(timer);
					eventScheduler.HordeTimer = true;
				}
				hordeEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);

				if (superHorde)
				{
					int damageTypeRoll = rng.Next(0, 5);

					switch (damageTypeRoll)
					{
						case 1:
							hordeEnemies[enemyIndex].DamageTypeA = (uint) Entities.Poison;
							break;
						case 2:
							hordeEnemies[enemyIndex].DamageTypeB = (uint) Entities.Curse;
							break;
						case 3:
							hordeEnemies[enemyIndex].DamageTypeA = (uint) Entities.Stone;
							hordeEnemies[enemyIndex].DamageTypeB = (uint) Entities.Stone;
							break;
						case 4:
							hordeEnemies[enemyIndex].DamageTypeA = (uint) Entities.Slam;
							break;
						default:
							break;
					}
				}

				sotnApi.EntityApi.SpawnEntity(hordeEnemies[enemyIndex]);
			}
		}
		private bool FindHordeEnemy()
		{
			uint roomX = sotnApi.GameApi.RoomX;
			uint roomY = sotnApi.GameApi.RoomY;

			if ((roomX == hordeTriggerRoomX && roomY == hordeTriggerRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu())
			{
				return false;
			}

			long enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackHordeEnemies : Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Entity? hordeEnemy = new(sotnApi.EntityApi.GetEntity(enemy));

				if (hordeEnemy is not null && !hordeEnemies.Where(e => e.AiId == hordeEnemy.AiId).Any())
				{
					if (superHorde)
					{
						hordeEnemy.Hp *= 2;
						hordeEnemy.Damage *= 2;
					}
					hordeEnemies.Add(hordeEnemy);
					Console.WriteLine($"Added horde enemy with hp: {hordeEnemy.Hp} sprite: {hordeEnemy.AiId} damage: {hordeEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		private void SubweaponsOnlyOff()
		{
			cheatsController.Curse.Disable();
			ManaLocked = false;
			cheatsController.Mana.Disable();
			cheatsController.Hearts.Disable();
			if (gasCloudTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud, true);
				gasCloudTaken = false;
			}
			eventScheduler.SubweaponsOnlyTimer = false;
			subweaponsOnlyActive = false;
			sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
		}
		private void SlowOff()
		{
			//SetSpeed();
			cheatsController.UnderwaterPhysics.Disable();
			sotnApi.GameApi.UnderwaterPhysicsEnabled = false;
			eventScheduler.SlowTimer = false;
			slowActive = false;
			slowPaused = false;
		}
		private void EnduranceSpawn()
		{
			uint roomX = sotnApi.GameApi.RoomX;
			uint roomY = sotnApi.GameApi.RoomY;
			float healthMultiplier = 3.5F;

			if ((roomX == enduranceRoomX && roomY == enduranceRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5)
			{
				return;
			}

			Entity? bossCopy = null;

			long enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceRomhackBosses : Constants.Khaos.EnduranceBosses);
			if (enemy > 0)
			{
				LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(enemy);
				bossCopy = new Entity(sotnApi.EntityApi.GetEntity(enemy));
				string name = Constants.Khaos.EnduranceRomhackBosses.Where(e => e.AiId == bossCopy.AiId).FirstOrDefault().Name;
				Console.WriteLine($"Endurance boss found namne: {name} hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.AiId}");

				bool right = rng.Next(0, 2) > 0;
				bossCopy.Xpos = right ? (ushort) (bossCopy.Xpos + rng.Next(40, 80)) : (ushort) (bossCopy.Xpos + rng.Next(-80, -40));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
				int newhp = (int) Math.Round(healthMultiplier * bossCopy.Hp);
				if (newhp > Int16.MaxValue)
				{
					newhp = Int16.MaxValue - 200;
				}
				bossCopy.Hp = (ushort) newhp;
				sotnApi.EntityApi.SpawnEntity(bossCopy);

				boss.Hp = (ushort) newhp;

				if (superEnduranceCount > 0)
				{
					superEnduranceCount--;

					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					sotnApi.EntityApi.SpawnEntity(bossCopy);
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
					eventScheduler.EnduranceSpawnTimer = false;
				}
			}
			else
			{
				enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceAlternateRomhackBosses : Constants.Khaos.EnduranceAlternateBosses);
				if (enemy > 0)
				{
					LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(enemy);
					string name = Constants.Khaos.EnduranceAlternateBosses.Where(e => e.AiId == boss.AiId).FirstOrDefault().Name;
					Console.WriteLine($"Endurance alternate boss found namne: {name}");

					boss.Palette = (ushort) (boss.Palette + rng.Next(1, 10));

					if (superEnduranceCount > 0)
					{
						int newhp = (int) Math.Round((healthMultiplier * 2.3) * boss.Hp);
						if (newhp > Int16.MaxValue)
						{
							newhp = Int16.MaxValue - 200;
						}
						boss.Hp = (ushort) newhp;
						superEnduranceCount--;
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
						eventScheduler.EnduranceSpawnTimer = false;
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
			Entity hitbox = new Entity();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Entities.Poison;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void SpawnCurseHitbox()
		{
			Entity hitbox = new Entity();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeB = (uint) Entities.Curse;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void SpawnStoneHitbox()
		{
			Entity hitbox = new Entity();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Entities.Stone;
			hitbox.DamageTypeB = (uint) Entities.Stone;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void SpawnSlamHitbox()
		{
			//bool alucardIsPoisoned = sotnApi.AlucardApi.PoisonTimer > 0;
			Entity hitbox = new Entity();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Entities.Slam;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void ActivateDizzy()
		{
			sotnApi.GameApi.SetMovementSpeedDirection(true);
			eventScheduler.DizzyTimer = true;
		}
		private void DizzyOff()
		{
			sotnApi.GameApi.SetMovementSpeedDirection(false);
			eventScheduler.DizzyTimer = false;
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
		private void HnkOff()
		{
			hnkOn = false;
			forwardDashActive = false;
			ResetBackdash();
			cheatsController.DefencePotion.Disable();
			eventScheduler.HnkTimer = false;
			InvincibilityLocked = false;
		}
		#endregion
		#region Buff events
		private void VampireOff()
		{
			vampireActive = false;
			cheatsController.DarkMetamorphasis.PokeValue(1);
			cheatsController.DarkMetamorphasis.Disable();

			eventScheduler.VampireTimer = false;
		}
		private void MagicianOff()
		{
			cheatsController.Mana.Disable();
			ManaLocked = false;
			eventScheduler.MagicianTimer = false;
		}
		private void BattleOrdersOff()
		{
			sotnApi.AlucardApi.MaxtHp -= (uint) battleOrdersBonusHp;
			sotnApi.AlucardApi.MaxtMp -= (uint) battleOrdersBonusMp;
			battleOrdersActive = false;
			battleOrdersBonusHp = 0;
			battleOrdersBonusMp = 0;
			eventScheduler.BattleOrdersTimer = false;
		}
		private void MeltyBloodOff()
		{
			cheatsController.HitboxWidth.Disable();
			cheatsController.HitboxHeight.Disable();
			cheatsController.Hitbox2Width.Disable();
			cheatsController.Hitbox2Height.Disable();
			ResetBackdash();

			if (superMelty)
			{
				superMelty = false;
			}

			eventScheduler.MeltyTimer = false;
			forwardDashActive = false;
		}
		private void FourBeastsOff()
		{
			cheatsController.InvincibilityCheat.Disable();
			InvincibilityLocked = false;
			cheatsController.AttackPotion.Disable();
			cheatsController.ShineCheat.Disable();
			cheatsController.ContactDamage.Disable();
			sotnApi.AlucardApi.ContactDamage = 0;
			eventScheduler.FourBeastsTimer = false;
		}
		private void ZawarudoOff()
		{
			cheatsController.SubweaponTimer.Disable();
			eventScheduler.ZawarudoTimer = false;
			eventScheduler.ZawarudoCheckTimer = false;
			zaWarudoActive = false;
		}
		private void ZaWarudoAreaCheck()
		{
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading
				&& sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && !sotnApi.AlucardApi.IsInvincible() && AlucardMapX < 99)
			{
				if (!zaWarudoActive && sotnApi.AlucardApi.SubweaponTimer == 0)
				{
					sotnApi.AlucardApi.ActivateStopwatch();
					cheatsController.SubweaponTimer.Enable();
					cheatsController.SubweaponTimer.PokeValue(1);
					zaWarudoZone = sotnApi.GameApi.Zone2;
					zaWarudoActive = true;
					return;
				}

				uint zone2 = sotnApi.GameApi.Zone2;
				if (zaWarudoZone != zone2)
				{
					zaWarudoZone = zone2;
					sotnApi.AlucardApi.ActivateStopwatch();
				}
			}
		}
		private void HasteOff()
		{
			eventScheduler.HasteTimer = false;
			inputService.Polling--;
			inputService.ReadDash = false;
			SetSpeed();
			superHaste = false;
			hasteActive = false;
			SpeedLocked = false;
			eventScheduler.HasteOverdriveOffTimer = true;
			if (forwardDashActive)
			{
				FlipBackdash();
			}
		}
		private void SetHasteStaticSpeeds(bool super = false)
		{
			float superFactor = super ? 2F : 1F;
			float superWingsmashFactor = super ? 1.5F : 1F;
			float factor = toolConfig.Khaos.HasteFactor;

			uint wolfDashTopLeft = DefaultSpeeds.WolfDashTopLeft;

			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5));
			sotnApi.AlucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2));
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) wolfDashTopLeft * ((factor * superFactor) / 2));
			if (forwardDashActive)
			{
				FlipBackdash();
			}
			else
			{
				sotnApi.AlucardApi.BackdashWholeSpeed = (int) (DefaultSpeeds.BackdashWhole - 1);
			}
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
		private void OverdriveOn()
		{
			cheatsController.VisualEffectPalette.PokeValue(Constants.Khaos.OverdriveColorPalette);
			cheatsController.VisualEffectPalette.Enable();
			cheatsController.VisualEffectTimer.PokeValue(30);
			cheatsController.VisualEffectTimer.Enable();
			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			eventScheduler.HasteOverdriveTimer = false;
		}
		private void OverdriveOff()
		{
			cheatsController.VisualEffectPalette.Disable();
			cheatsController.VisualEffectTimer.Disable();
			if (hasteActive)
			{
				SetHasteStaticSpeeds(superHaste);
			}
			else
			{
				sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal);
			}
			overdriveOn = false;
			eventScheduler.HasteOverdriveOffTimer = false;
		}
		private void LordOff()
		{
			SpawnActive = false;
			lordEnemies.RemoveRange(0, hordeEnemies.Count);

			eventScheduler.Lord.Interval = 5 * (60 * 1000);
			eventScheduler.LordTimer = false;
			eventScheduler.LordSpawnTimer = false;
		}
		private void LordSpawn()
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5 || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone))
			{
				return;
			}

			uint zone2 = sotnApi.GameApi.Zone2;

			if (lordZone != zone2)
			{
				lordEnemies.Clear();
				lordZone = zone2;
			}

			FindLordEnemy();

			if (lordEnemies.Count > 0)
			{
				int enemyIndex = rng.Next(0, lordEnemies.Count);
				if (eventScheduler.Lord.Interval == 5 * (60 * 1000))
				{
					eventScheduler.LordTimer = false;
					eventScheduler.Lord.Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Duration.TotalMilliseconds;

					ActionTimer timer = new()
					{
						Name = toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Name,
						Duration = toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Duration
					};
					statusInfoDisplay.AddTimer(timer);
					notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
					eventScheduler.LordTimer = true;
				}
				lordEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				lordEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				lordEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				sotnApi.EntityApi.SpawnEntity(lordEnemies[enemyIndex], false);
			}
		}
		private bool FindLordEnemy()
		{
			uint roomX = sotnApi.GameApi.RoomX;
			uint roomY = sotnApi.GameApi.RoomY;

			if ((roomX == lordTriggerRoomX && roomY == lordTriggerRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu())
			{
				return false;
			}

			long enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackHordeEnemies : Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Entity? lordEnemy = new Entity(sotnApi.EntityApi.GetEntity(enemy));

				if (lordEnemy is not null && !lordEnemies.Where(e => e.AiId == lordEnemy.AiId).Any())
				{
					lordEnemies.Add(lordEnemy);
					Console.WriteLine($"Added Lord enemy with hp: {lordEnemy.Hp} sprite: {lordEnemy.AiId} damage: {lordEnemy.Damage}");
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
			azureDragonUsed = true;
			notificationService.AzureDragons += 1;

			notificationService.AddMessage(user + " gave you 1 Azure Dragon Spirit");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void VermilionBird(string user)
		{
			vermilionBirdUsed = true;
			notificationService.VermillionBirds += 5;

			notificationService.AddMessage(user + " gave you 5 Vermilion Bird Fireballs");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void WhiteTiger(string user)
		{
			whiteTigerUsed = true;
			notificationService.WhiteTigers += 2;

			notificationService.AddMessage(user + " gave you 2 White Tiger Hellfires");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void BlackTortoise(string user)
		{
			blackTortoiseUsed = true;
			notificationService.BlackTortoises += 2;

			notificationService.AddMessage(user + " gave you 2 Black Tortoise Dark Metamorphasis");
			Alert(toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts]);
		}
		private void FourHolyBeasts(string user)
		{
			cheatsController.InvincibilityCheat.PokeValue(1);
			cheatsController.InvincibilityCheat.Enable();
			InvincibilityLocked = true;
			cheatsController.AttackPotion.PokeValue(1);
			cheatsController.AttackPotion.Enable();
			cheatsController.ShineCheat.PokeValue(1);
			cheatsController.ShineCheat.Enable();
			eventScheduler.FourBeastsTimer = true;
			cheatsController.ContactDamage.PokeValue(4);
			cheatsController.ContactDamage.Enable();
			azureDragonUsed = false;
			whiteTigerUsed = false;
			vermilionBirdUsed = false;
			blackTortoiseUsed = false;

			notificationService.AzureDragons += 2;
			notificationService.VermillionBirds += 10;
			notificationService.WhiteTigers += 3;
			notificationService.BlackTortoises += 3;

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
			if (notificationService.VermillionBirds > 0 && !vermilionBirdPollong)
			{
				vermilionBirdPollong = true;
				inputService.ReadQuarterCircle = true;
				inputService.Polling++;
			}
			else if (vermilionBirdPollong && notificationService.VermillionBirds < 1)
			{
				vermilionBirdPollong = false;
				inputService.ReadQuarterCircle = false;
				inputService.Polling--;
			}

			if (notificationService.VermillionBirds > 0 && fireballCooldown == 0
				&& inputService.RegisteredMove(InputKeys.QuarterCircleForward, Globals.UpdateCooldownFrames))
			{
				notificationService.VermillionBirds--;
				Entity fireball = new Entity(Constants.Khaos.FireballEntityBytes);
				bool alucardFacing = sotnApi.AlucardApi.FacingLeft;
				int offsetX = alucardFacing ? -20 : 20;
				fireball.Xpos = (ushort) (sotnApi.AlucardApi.ScreenX + offsetX);
				fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY - 10);

				long address = sotnApi.EntityApi.SpawnEntity(fireball, false);
				LiveEntity liveFireball = sotnApi.EntityApi.GetLiveEntity(address);
				fireballs.Add(liveFireball);

				fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY - 20);
				address = sotnApi.EntityApi.SpawnEntity(fireball, false);
				LiveEntity liveFireball2 = sotnApi.EntityApi.GetLiveEntity(address);
				fireballs.Add(liveFireball2);

				fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY);
				address = sotnApi.EntityApi.SpawnEntity(fireball, false);
				LiveEntity liveFireball3 = sotnApi.EntityApi.GetLiveEntity(address);
				fireballs.Add(liveFireball3);

				fireballCooldown = 20;
			}

			fireballs.RemoveAll(f => f.Damage == 0 || f.Damage == 80);
			foreach (var fball in fireballs)
			{
				fball.Damage = 40;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 5;
				}
				else
				{
					fball.SpeedHorizontal = -6;
				}
				if (inputService.ButtonPressed(PlaystationInputKeys.Down, 10))
				{
					fball.SpeedVertical = 3;
				}
				else if (inputService.ButtonPressed(PlaystationInputKeys.Up, 10))
				{
					fball.SpeedVertical = -2;
				}
			}

			if (fireballCooldown > 0)
			{
				fireballCooldown--;
			}
		}
		private void CheckVampireKill()
		{
			if (!vampireActive)
			{
				return;
			}

			uint updatedCurrentKills = sotnApi.AlucardApi.Kills;

			if (Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] != "Gurthang" && Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] != "Gurthang"
				&& (sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.Bat || sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.Wolf
				|| sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.Mist || sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.UntransformBat
				|| sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.UntransformMist || sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.UntransformWolf))
			{
				currentKills = updatedCurrentKills;
				return;
			}

			if (updatedCurrentKills > currentKills)
			{
				currentKills = updatedCurrentKills;
				currentVampireKills++;
			}

			if (currentVampireKills >= goalVampireKills)
			{
				currentVampireKills = 0;
				sotnApi.AlucardApi.Str += 2;
				vampureSwordLevel++;
				goalVampireKills = 10 + (vampureSwordLevel * vampureSwordLevel);
				return;
			}
		}
		private void AzureDragonOff()
		{
			var lockOnCheat = cheatsController.Cheats.GetCheatByName(Constants.Khaos.SpiritLockOnName);
			lockOnCheat.Disable();
			cheatsController.Cheats.RemoveCheat(lockOnCheat);
			azureSpiritActive = false;
			eventScheduler.AzureDragonTimer = false;
		}
		private void WhiteTigerOff()
		{
			var whiteTigerBallCheat = cheatsController.Cheats.GetCheatByName(Constants.Khaos.WhiteTigerBallSpeedName);
			whiteTigerBallCheat.Disable();
			cheatsController.Cheats.RemoveCheat(whiteTigerBallCheat);
			whiteTigerBallActive = false;
			eventScheduler.WhiteTigerBallTimer = false;
		}
		private void ActivateGuardianSpirits()
		{
			cheatsController.Activator.PokeValue((int)SotnApi.Constants.Values.Alucard.Effects.AutoSummonSpirit);
			cheatsController.Activator.Enable();
			eventScheduler.GuardianSpiritsTimer = true;
		}
		private void GuardianSpiritsOff()
		{
			cheatsController.Activator.Disable();
			eventScheduler.GuardianSpiritsTimer = false;
		}
		#endregion

		private void FlipBackdash()
		{
			sotnApi.AlucardApi.BackdashWholeSpeed = (int) ((DefaultSpeeds.BackdashWhole * -1) - 1);
			if (hasteActive)
			{
				sotnApi.AlucardApi.BackdashWholeSpeed = (int) ((DefaultSpeeds.BackdashWhole * -1));
			}
		}
		private void ResetBackdash()
		{
			sotnApi.AlucardApi.BackdashWholeSpeed = DefaultSpeeds.BackdashWhole;
			if (hasteActive)
			{
				sotnApi.AlucardApi.BackdashWholeSpeed = (int) (DefaultSpeeds.BackdashWhole - 1);
			}
		}
		private void SetSaveColorPalette()
		{
			int offset = rng.Next(0, 15);
			if (alucardSecondCastle)
			{
				cheatsController.SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronSecondCastle + offset);
			}
			else
			{
				cheatsController.SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle + offset);
			}
		}
		private void SetRelicLocationDisplay(Relic relic, bool take)
		{
			switch (relic)
			{
				case Relic.SoulOfBat:
					if (take)
					{
						if (statusInfoDisplay.BatLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.BatLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.BatLocation == String.Empty)
						{
							statusInfoDisplay.BatLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.SoulOfWolf:
					if (take)
					{
						if (statusInfoDisplay.WolfLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.WolfLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.WolfLocation == String.Empty)
						{
							statusInfoDisplay.WolfLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.FormOfMist:
					if (take)
					{
						if (statusInfoDisplay.MistLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.MistLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.MistLocation == String.Empty)
						{
							statusInfoDisplay.MistLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.PowerOfMist:
					if (take)
					{
						if (statusInfoDisplay.PowerOfMistLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.PowerOfMistLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.PowerOfMistLocation == String.Empty)
						{
							statusInfoDisplay.PowerOfMistLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.GravityBoots:
					if (take)
					{
						if (statusInfoDisplay.GravityBootsLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.GravityBootsLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.GravityBootsLocation == String.Empty)
						{
							statusInfoDisplay.GravityBootsLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.LeapStone:
					if (take)
					{
						if (statusInfoDisplay.LepastoneLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.LepastoneLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.LepastoneLocation == String.Empty)
						{
							statusInfoDisplay.LepastoneLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.JewelOfOpen:
					if (take)
					{
						if (statusInfoDisplay.JewelOfOpenLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.JewelOfOpenLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.JewelOfOpenLocation == String.Empty)
						{
							statusInfoDisplay.JewelOfOpenLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				case Relic.MermanStatue:
					if (take)
					{
						if (statusInfoDisplay.MermanLocation == Constants.Khaos.KhaosName)
						{
							statusInfoDisplay.MermanLocation = String.Empty;
						}
					}
					else
					{
						if (statusInfoDisplay.MermanLocation == String.Empty)
						{
							statusInfoDisplay.MermanLocation = Constants.Khaos.KhaosName;
						}
					}
					break;
				default:
					break;
			}
		}
		private void SetSpeed(float factor = 1)
		{
			bool slow = factor < 1;
			bool fast = factor > 1;

			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);
			uint wolfDashTopLeft = DefaultSpeeds.WolfDashTopLeft;

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
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) wolfDashTopLeft * factor);
			sotnApi.AlucardApi.BackdashWholeSpeed = (int) (DefaultSpeeds.BackdashWhole * factor);
			sotnApi.AlucardApi.BackdashDecel = slow == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
			Console.WriteLine($"Set all speeds with factor {factor}");
		}
		private void CheckSubweaponsOnlyEquipment()
		{
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
		}
		private void CheckAzureDragon()
		{
			if (notificationService.AzureDragons > 0 && !azureSpiritActive)
			{
				var spiritAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.SpiritActor }, false);
				if (spiritAddress > 0)
				{
					LiveEntity liveSpirit = sotnApi.EntityApi.GetLiveEntity(spiritAddress);
					if (liveSpirit.LockOn == Entities.LockedOn)
					{
						notificationService.AzureDragons--;
						liveSpirit.Palette = Constants.Khaos.SpiritPalette;
						liveSpirit.InvincibilityFrames = 4;
						azureSpiritActive = true;
						cheatsController.Cheats.AddCheat(spiritAddress + Entities.LockOnOffset, Entities.LockedOn, Constants.Khaos.SpiritLockOnName, WatchSize.Byte);
						eventScheduler.AzureDragonTimer = true;
					}
				}
			}
		}
		private void CheckBlackTortoise()
		{
			if (notificationService.BlackTortoises > 0 && !darkMetamorphosisCasted && sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.DarkMetamorphosis)
			{
				notificationService.BlackTortoises--;
				sotnApi.AlucardApi.ActivatePotion(Potion.HighPotion);
				darkMetamorphosisCasted = true;
			}

			if (notificationService.BlackTortoises > 0 && darkMetamorphosisCasted && sotnApi.AlucardApi.State != SotnApi.Constants.Values.Alucard.States.DarkMetamorphosis)
			{
				darkMetamorphosisCasted = false;
			}
		}
		private void CheckWhiteDragon()
		{
			if (notificationService.WhiteTigers > 0 && !whiteTigerBallActive && !hellfireCasted && sotnApi.AlucardApi.State == SotnApi.Constants.Values.Alucard.States.Hellfire)
			{
				notificationService.WhiteTigers--;
				Entity fireball = new Entity(Constants.Khaos.DarkFireballEntityBytes);
				bool alucardFacing = sotnApi.AlucardApi.FacingLeft;
				int offsetX = alucardFacing ? -20 : 20;
				fireball.Xpos = (ushort) (sotnApi.AlucardApi.ScreenX + offsetX);
				fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY - 10);
				fireball.SpeedHorizontal = alucardFacing ? (ushort) 0xFFFF : (ushort) 0;

				long address = sotnApi.EntityApi.SpawnEntity(fireball, false);
				LiveEntity liveFireball = sotnApi.EntityApi.GetLiveEntity(address);
				hellfireCasted = true;
				whiteTigerBallActive = true;
				cheatsController.Cheats.AddCheat(address + SotnApi.Constants.Values.Game.Entities.SpeedWholeOffset, alucardFacing ? (ushort) Constants.Khaos.WhiteTigerBallSpeedLeft : (ushort) Constants.Khaos.WhiteTigerBallSpeedRight, Constants.Khaos.WhiteTigerBallSpeedName, WatchSize.Word);
				eventScheduler.WhiteTigerBallTimer = true;
			}

			if (notificationService.WhiteTigers > 0 && hellfireCasted && sotnApi.AlucardApi.State != SotnApi.Constants.Values.Alucard.States.Hellfire)
			{
				hellfireCasted = false;
			}
		}
		private void FixEntranceSlow()
		{
			if (slowActive && !slowPaused && IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				slowPaused = true;
				cheatsController.UnderwaterPhysics.Disable();
			}

			if (slowActive && slowPaused && !IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				slowPaused = false;
				cheatsController.UnderwaterPhysics.Enable();
			}
		}
		private void FixSlowWarps()
		{
			if (slowActive && !slowPaused && sotnApi.GameApi.CanWarp())
			{
				slowPaused = true;
				cheatsController.UnderwaterPhysics.Disable();
			}

			if (slowActive && slowPaused && !sotnApi.GameApi.CanWarp())
			{
				slowPaused = false;
				cheatsController.UnderwaterPhysics.Enable();
			}
		}
		private void HandleHnkInvincibility()
		{
			if (hnkOn && hnkToggled == 1)
			{
				hnkToggled = 0;
				sotnApi.AlucardApi.InvincibilityTimer = 0;
				sotnApi.AlucardApi.PotionInvincibilityTimer = 0;
				sotnApi.AlucardApi.KnockbackInvincibilityTimer = 0;
				sotnApi.AlucardApi.FreezeInvincibilityTimer = 0;
			}
			else if (hnkOn && hnkToggled < 1)
			{
				hnkToggled++;
			}
		}
		private void CheckManaUsage()
		{
			if (!bloodManaActive)
			{
				return;
			}

			uint currentMana = sotnApi.AlucardApi.CurrentMp;

			spentMana = 0;
			if (currentMana < storedMana)
			{
				spentMana = (int) storedMana - (int) currentMana;
			}

			storedMana = currentMana;
			BloodManaUpdate();
		}
		private void CheckDracularoomMusic()
		{
			if (IsInRoomList(Constants.Khaos.DraculaRoom))
			{
				if (dracMusicCounter == 0)
				{
					cheatsController.Music.PokeValue(rng.Next(0, 256));
					cheatsController.Music.Enable();
					dracMusicCounter = 30;
				}
				else
				{
					dracMusicCounter--;
				}
			}
		}
		private void CheckDashInput()
		{
			if (hasteActive && !hasteSpeedOn && inputService.RegisteredMove(InputKeys.Dash, Globals.UpdateCooldownFrames))
			{
				ToggleHasteDynamicSpeeds(superHaste ? toolConfig.Khaos.HasteFactor * Constants.Khaos.HasteDashFactor : toolConfig.Khaos.HasteFactor);
				hasteSpeedOn = true;
				eventScheduler.HasteOverdriveTimer = true;
			}
			else if (hasteSpeedOn && !inputService.DirectionHeld(InputKeys.Forward))
			{
				ToggleHasteDynamicSpeeds();
				hasteSpeedOn = false;
				eventScheduler.HasteOverdriveTimer = false;
				if (overdriveOn)
				{
					eventScheduler.HasteOverdriveOffTimer = true;
				}
			}
		}
		private bool KhaosMeterFull()
		{
			return khaosMeter >= 100;
		}
		private void SpendKhaosMeter()
		{
			khaosMeter -= 100;
			notificationService.UpdateOverlayMeter(khaosMeter);
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

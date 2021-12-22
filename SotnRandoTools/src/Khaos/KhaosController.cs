using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BizHawk.Client.Common;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Addresses;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Enums;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Services.Models;
using SotnRandoTools.Utils;
using WatsonWebsocket;

namespace SotnRandoTools.Khaos
{
	public class KhaosController
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly ICheatCollectionAdapter cheats;
		private readonly INotificationService notificationService;
		private readonly IInputService inputService;
		private readonly IKhaosActionsInfoDisplay statusInfoDisplay;
		private Random rng = new Random();

		private string[]? subscribers = {};

		private List<QueuedAction> queuedActions = new();
		private Queue<MethodInvoker> queuedFastActions = new();
		private Timer actionTimer = new Timer();
		private Timer fastActionTimer = new Timer();
		private int totalMeterGained = 0;
		private bool pandoraUsed = false;
		private uint alucardMapX = 0;
		private uint alucardMapY = 0;
		private bool alucardSecondCastle = false;
		private bool inMainMenu = false;

		#region Timers
		private System.Timers.Timer subweaponsOnlyTimer = new();
		private System.Timers.Timer crippleTimer = new();
		private System.Timers.Timer bloodManaTimer = new();
		private System.Timers.Timer thirstTimer = new();
		private System.Timers.Timer thirstTickTimer = new();
		private System.Timers.Timer hordeTimer = new();
		private System.Timers.Timer hordeSpawnTimer = new();
		private System.Timers.Timer lordTimer = new();
		private System.Timers.Timer lordSpawnTimer = new();
		private System.Timers.Timer enduranceSpawnTimer = new();
		private System.Timers.Timer hnkTimer = new();
		private System.Timers.Timer vampireTimer = new();
		private System.Timers.Timer magicianTimer = new();
		private System.Timers.Timer meltyTimer = new();
		private System.Timers.Timer fourBeastsTimer = new();
		private System.Timers.Timer zawarudoTimer = new();
		private System.Timers.Timer zawarudoCheckTimer = new();
		private System.Timers.Timer hasteTimer = new();
		private System.Timers.Timer hasteOverdriveTimer = new();
		private System.Timers.Timer hasteOverdriveOffTimer = new();
		#endregion
		#region Cheats
		Cheat faerieScroll;
		Cheat darkMetamorphasisCheat;
		Cheat underwaterPhysics;
		Cheat hearts;
		Cheat curse;
		Cheat manaCheat;
		Cheat attackPotionCheat;
		Cheat defencePotionCheat;
		Cheat stopwatchTimer;
		Cheat hitboxWidth;
		Cheat hitboxHeight;
		Cheat hitbox2Width;
		Cheat hitbox2Height;
		Cheat invincibilityCheat;
		Cheat shineCheat;
		Cheat VisualEffectPaletteCheat;
		Cheat VisualEffectTimerCheat;
		Cheat SavePalette;
		#endregion

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

		private int slowInterval;
		private int normalInterval;
		private int fastInterval;

		private bool shaftHpSet = false;
		private bool galamothStatsSet = false;

		private bool superThirst = false;
		private bool superHorde = false;
		private bool superEndurance = false;
		private bool superMelty = false;
		private bool superHaste = false;

		public KhaosController(IToolConfig toolConfig, ISotnApi sotnApi, ICheatCollectionAdapter cheats, INotificationService notificationService, IInputService inputService, IKhaosActionsInfoDisplay statusInfoDisplay)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			if (notificationService == null) throw new ArgumentNullException(nameof(notificationService));
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
			Console.WriteLine($"Intervals set. \n normal: {normalInterval / 1000}s, slow:{slowInterval / 1000}s, fast:{fastInterval / 1000}s");
		}

		public bool AutoKhaosOn { get; set; }

		public void StartKhaos()
		{
			if (File.Exists(toolConfig.Khaos.NamesFilePath))
			{
				subscribers = FileExtensions.GetLines(toolConfig.Khaos.NamesFilePath);
			}
			actionTimer.Start();
			fastActionTimer.Start();
			if (subscribers is not null && subscribers.Length > 0)
			{
				OverwriteBossNames(subscribers);
			}
			StartCheats();
			DateTime startedAt = DateTime.Now;
			foreach (var action in toolConfig.Khaos.Actions)
			{
				if (action.StartsOnCooldown)
				{
					action.LastUsedAt = startedAt;
				}
				else
				{
					action.LastUsedAt = null;
				}
			}
			notificationService.StartOverlayServer();
			notificationService.AddMessage($"Khaos started");
			Console.WriteLine("Khaos started");
		}
		public void StopKhaos()
		{
			StopTimers();
			faerieScroll.Disable();
			notificationService.StopOverlayServer();
			notificationService.AddMessage($"Khaos stopped");
			Console.WriteLine("Khaos stopped");
		}

		#region Khaotic Effects
		//TODO: random action
		public void KhaosStatus(string user = "Khaos")
		{
			bool entranceCutscene = IsInEntranceCutscene();
			bool succubusRoom = IsInSuccubusRoom();
			int min = 1;
			int max = 9;

			if (zaWarudoActive)
			{
				max = 5;
			}

			if (true)
			{

			}

			int result = rng.Next(min, max);
			bool alucardIsImmuneToCurse = sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad)
				|| Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Coral circlet";
			bool alucardIsImmuneToStone = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Mirror cuirass"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Medusa shield"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Medusa shield";
			bool alucardIsImmuneToPoison = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Topaz circlet";

			if (succubusRoom && result == 3)
			{
				while (result == 3)
				{
					result = rng.Next(1, max);
				}
			}

			if ((succubusRoom || entranceCutscene) && result == 4)
			{
				while (result == 4)
				{
					result = rng.Next(1, max);
				}
			}

			if (alucardIsImmuneToCurse && result == 2)
			{
				while (result == 2)
				{
					result = rng.Next(1, max);
				}
			}

			if (alucardIsImmuneToStone && result == 3)
			{
				while (result == 3)
				{
					result = rng.Next(1, max);
				}
			}

			if (alucardIsImmuneToPoison && result == 1)
			{
				while (result == 1)
				{
					result = rng.Next(1, max);
				}
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
					sotnApi.AlucardApi.ActivatePotion(Potion.LuckPotion);
					notificationService.AddMessage($"{user} gave you luck");
					break;
				case 6:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					notificationService.AddMessage($"{user} gave you resistance to fire");
					break;
				case 7:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave you resistance to dark");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defence");
					break;
				default:
					break;
			}

			Alert(toolConfig.Khaos.Actions[0]);
		}
		public void KhaosEquipment(string user = "Khaos")
		{
			RandomizeEquipmentSlots();
			notificationService.AddMessage($"{user} used Khaos Equipment");
			Alert(toolConfig.Khaos.Actions[1]);
		}
		public void KhaosStats(string user = "Khaos")
		{
			RandomizeStatsActivate();
			notificationService.AddMessage($"{user} used Khaos Stats");
			Alert(toolConfig.Khaos.Actions[2]);
		}
		public void KhaosRelics(string user = "Khaos")
		{
			RandomizeRelicsActivate(false);
			notificationService.AddMessage($"{user} used Khaos Relics");
			Alert(toolConfig.Khaos.Actions[3]);
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
			Alert(toolConfig.Khaos.Actions[4]);
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
			Alert(toolConfig.Khaos.Actions[5]);
		}
		public void KhaoticBurst(string user = "Khaos")
		{
			notificationService.KhaosMeter += 100;
			notificationService.AddMessage($"{user} used Khaotic Burst");
			Alert(toolConfig.Khaos.Actions[6]);
		}
		#endregion
		#region Debuffs
		public void Bankrupt(string user = "Khaos")
		{
			BankruptActivate();
			notificationService.AddMessage($"{user} used Bankrupt");
			Alert(toolConfig.Khaos.Actions[7]);
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
			Alert(toolConfig.Khaos.Actions[8]);
		}
		public void RespawnBosses(string user = "Khaos")
		{
			sotnApi.GameApi.RespawnBosses();
			notificationService.AddMessage($"{user} used Respawn Bosses");
			Alert(toolConfig.Khaos.Actions[9]);
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

			ActionTimer timer = new ()
			{
				Name = toolConfig.Khaos.Actions[10].Name,
				Duration = toolConfig.Khaos.Actions[10].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[10]);
		}
		public void Cripple(string user = "Khaos")
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

			if (IsInEntranceCutscene())
			{
				queuedActions.Add(new QueuedAction { Name = toolConfig.Khaos.Actions[11].Name, LocksSpeed = true, Invoker = new MethodInvoker(() => Cripple(user)) });
			}

			underwaterPhysics.Enable();
			crippleTimer.Start();

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[11].Name,
				Duration = toolConfig.Khaos.Actions[11].Duration
			};

			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			//string message = meterFull ? $"{user} used Super Cripple" : $"{user} used Cripple";
			string message = $"{user} used Cripple";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[11]);
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
				Name = toolConfig.Khaos.Actions[12].Name,
				Duration = toolConfig.Khaos.Actions[12].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[12]);
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
				Name = toolConfig.Khaos.Actions[13].Name,
				Duration = toolConfig.Khaos.Actions[13].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int)timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			string message = meterFull ? $"{user} used Super Thirst" : $"{user} used Thirst";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[13]);
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
			Alert(toolConfig.Khaos.Actions[14]);
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
			Alert(toolConfig.Khaos.Actions[15]);
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
				Duration = toolConfig.Khaos.Actions[16].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);

			notificationService.AddMessage($"{user} used HnK");
			Alert(toolConfig.Khaos.Actions[16]);
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
			notificationService.AddMessage($"{user} used Vampire");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[17].Name,
				Duration = toolConfig.Khaos.Actions[17].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[17]);
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

			bool heighHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;

			int roll = rng.Next(1, 4);
			if (heighHp && roll == 2)
			{
				roll = 3;
			}

			if (zaWarudoActive)
			{
				roll = 1;
			}

			switch (roll)
			{
				case 1:
					sotnApi.AlucardApi.GrantItemByName(item);
					Console.WriteLine($"Light help rolled: {item}");
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Potion);
					Console.WriteLine($"Light help rolled activate potion.");
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					Console.WriteLine($"Light help rolled activate shield potion.");
					notificationService.AddMessage($"{user} used a Shield Potion");
					break;
				default:
					break;
			}
			Alert(toolConfig.Khaos.Actions[18]);
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
			if (highMp && roll == 3)
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
			Alert(toolConfig.Khaos.Actions[19]);
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
			Alert(toolConfig.Khaos.Actions[20]);

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
			sotnApi.AlucardApi.CurrentHp = (uint) (sotnApi.AlucardApi.MaxtHp * Constants.Khaos.BattleOrdersHpMultiplier);
			sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
			sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
			notificationService.AddMessage($"{user} used Battle Orders");
			Alert(toolConfig.Khaos.Actions[21]);
		}
		public void Magician(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				SpendKhaosMeter();
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfBat);
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
			manaCheat.PokeValue((int)sotnApi.AlucardApi.MaxtMp);
			manaCheat.Enable();
			manaLocked = true;
			magicianTimer.Start();

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[22].Name,
				Duration = toolConfig.Khaos.Actions[22].Duration
			};
			statusInfoDisplay.AddTimer(timer);
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);

			string message = meterFull ? $"{user} activated Shapeshifter" : $"{user} activated Magician";
			notificationService.AddMessage(message);

			Alert(toolConfig.Khaos.Actions[22]);
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
			sotnApi.AlucardApi.GrantRelic(Relic.LeapStone);
			meltyTimer.Start();
			string message = meterFull ? $"{user} activated GUILTY GEAR" : $"{user} activated Melty Blood";
			notificationService.AddMessage(message);
			if (meterFull)
			{
				Alert(toolConfig.Khaos.Actions[24]);

				ActionTimer timer = new()
				{
					Name = toolConfig.Khaos.Actions[24].Name,
					Duration = toolConfig.Khaos.Actions[24].Duration
				};
				notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
				statusInfoDisplay.AddTimer(timer);
			}
			else
			{
				Alert(toolConfig.Khaos.Actions[23]);

				ActionTimer timer = new()
				{
					Name = toolConfig.Khaos.Actions[23].Name,
					Duration = toolConfig.Khaos.Actions[23].Duration
				};
				notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
				statusInfoDisplay.AddTimer(timer);
			}
		}
		//TODO: Split into a random effect of four, 4B on super
		//TODO: Add contact damage
		public void FourBeasts(string user = "Khaos")
		{
			invincibilityCheat.PokeValue(1);
			invincibilityCheat.Enable();
			invincibilityLocked = true;
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			shineCheat.PokeValue(1);
			shineCheat.Enable();
			fourBeastsTimer.Start();

			notificationService.AddMessage($"{user} used Four Beasts");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[25].Name,
				Duration = toolConfig.Khaos.Actions[25].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[25]);
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

			notificationService.AddMessage($"{user} used ZA WARUDO");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[26].Name,
				Duration = toolConfig.Khaos.Actions[26].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			Alert(toolConfig.Khaos.Actions[26]);
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
			Console.WriteLine($"{user} used {toolConfig.Khaos.Actions[27].Name}");

			ActionTimer timer = new()
			{
				Name = toolConfig.Khaos.Actions[27].Name,
				Duration = toolConfig.Khaos.Actions[27].Duration
			};
			notificationService.AddOverlayTimer(timer.Name, (int) timer.Duration.TotalMilliseconds);
			statusInfoDisplay.AddTimer(timer);
			string message = meterFull ? $"{user} activated Super Haste" : $"{user} activated Haste";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[27]);
		}
		public void Lord(string user = "Khaos")
		{
			lordTriggerRoomX = sotnApi.GameApi.MapXPos;
			lordTriggerRoomY = sotnApi.GameApi.MapYPos;

			lordTimer.Start();
			lordSpawnTimer.Start();
			string message = $"{user} activated Lord of this Castle";
			notificationService.AddMessage(message);
			Alert(toolConfig.Khaos.Actions[28]);
		}
		//TODO: Necromancer
		//TODO: Turn Undead
		#endregion

		public void Update()
		{
			if (!sotnApi.GameApi.InAlucardMode())
			{
				return;
			}
			CheckDashInput();
			if (bloodManaActive)
			{
				CheckManaUsage();
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
				case 0:
					commandAction = toolConfig.Khaos.Actions[0];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaosStatus(user)));
					}
					break;
				case 1:
					commandAction = toolConfig.Khaos.Actions[1];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => KhaosEquipment(user)) });
					}
					break;
				case 2:
					commandAction = toolConfig.Khaos.Actions[2];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => KhaosStats(user)) });
					}
					break;
				case 3:
					commandAction = toolConfig.Khaos.Actions[3];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => KhaosRelics(user)) });
					}
					break;
				case 4:
					commandAction = toolConfig.Khaos.Actions[4];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => PandorasBox(user)) });
					}
					break;
				case 5:
					commandAction = toolConfig.Khaos.Actions[5];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Gamble(user)));
					}
					break;
				case 6:
					commandAction = toolConfig.Khaos.Actions[6];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaoticBurst(user)));
					}
					break;
				#endregion
				#region Debuffs
				case 7:
					commandAction = toolConfig.Khaos.Actions[7];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Bankrupt(user)) });
					}
					break;
				case 8:
					commandAction = toolConfig.Khaos.Actions[8];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Weaken(user)) });
					}
					break;
				case 9:
					commandAction = toolConfig.Khaos.Actions[9];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => RespawnBosses(user)));
					}
					break;
				case 10:
					commandAction = toolConfig.Khaos.Actions[10];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => SubweaponsOnly(user)) });
					}
					break;
				case 11:
					commandAction = toolConfig.Khaos.Actions[11];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpeed = true, Invoker = new MethodInvoker(() => Cripple(user)) });
					}
					break;
				case 12:
					commandAction = toolConfig.Khaos.Actions[12];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => BloodMana(user)) });
					}
					break;
				case 13:
					commandAction = toolConfig.Khaos.Actions[13];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Thirst(user)) });
					}
					break;
				case 14:
					commandAction = toolConfig.Khaos.Actions[14];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => Horde(user)) });
					}
					break;
				case 15:
					commandAction = toolConfig.Khaos.Actions[15];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Endurance(user)));
					}
					break;
				case 16:
					commandAction = toolConfig.Khaos.Actions[16];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HnK(user)));
					}
					break;
				#endregion
				#region Buffs
				case 17:
					commandAction = toolConfig.Khaos.Actions[17];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Vampire(user)));
					}
					break;
				case 18:
					commandAction = toolConfig.Khaos.Actions[18];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => LightHelp(user)));
					}
					break;
				case 19:
					commandAction = toolConfig.Khaos.Actions[19];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MediumHelp(user)));
					}
					break;
				case 20:
					commandAction = toolConfig.Khaos.Actions[20];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HeavytHelp(user)));
					}
					break;
				case 21:
					commandAction = toolConfig.Khaos.Actions[21];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => BattleOrders(user)) });
					}
					break;
				case 22:
					commandAction = toolConfig.Khaos.Actions[22];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => Magician(user)) });
					}
					break;
				case 23:
					commandAction = toolConfig.Khaos.Actions[23];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => MeltyBlood(user)) });
					}
					break;
				case 25:
					commandAction = toolConfig.Khaos.Actions[25];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksInvincibility = true, Invoker = new MethodInvoker(() => FourBeasts(user)) });
					}
					break;
				case 26:
					commandAction = toolConfig.Khaos.Actions[26];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ZaWarudo(user)));
					}
					break;
				case 27:
					commandAction = toolConfig.Khaos.Actions[27];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpeed = true, Invoker = new MethodInvoker(() => Haste(user)) });
					}
					break;
				case 28:
					commandAction = toolConfig.Khaos.Actions[28];
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

			subweaponsOnlyTimer.Elapsed += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly].Duration.TotalMilliseconds;
			crippleTimer.Elapsed += CrippleOff;
			crippleTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.Cripple].Duration.TotalMilliseconds;
			bloodManaTimer.Elapsed += BloodManaOff;
			bloodManaTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana].Duration.TotalMilliseconds;
			thirstTimer.Elapsed += ThirstOff;
			thirstTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Duration.TotalMilliseconds;
			thirstTickTimer.Elapsed += ThirstDrain;
			thirstTickTimer.Interval = 1000;
			hordeTimer.Elapsed += HordeOff;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeSpawnTimer.Elapsed += HordeSpawn;
			hordeSpawnTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Interval.TotalMilliseconds;
			enduranceSpawnTimer.Elapsed += EnduranceSpawn;
			enduranceSpawnTimer.Interval = 2 * (1000);
			hnkTimer.Elapsed += HnkOff;
			hnkTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.HnK].Duration.TotalMilliseconds;

			vampireTimer.Elapsed += VampireOff;
			vampireTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Duration.TotalMilliseconds;
			magicianTimer.Elapsed += MagicianOff;
			magicianTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Duration.TotalMilliseconds;
			meltyTimer.Elapsed += MeltyBloodOff;
			meltyTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood].Duration.TotalMilliseconds;
			fourBeastsTimer.Elapsed += FourBeastsOff;
			fourBeastsTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration.TotalMilliseconds;
			zawarudoTimer.Elapsed += ZawarudoOff;
			zawarudoTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO].Duration.TotalMilliseconds;
			zawarudoCheckTimer.Elapsed += ZaWarudoAreaCheck;
			zawarudoCheckTimer.Interval += 2 * 1000;
			hasteTimer.Elapsed += HasteOff;
			hasteTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Duration.TotalMilliseconds;
			hasteOverdriveTimer.Elapsed += OverdriveOn;
			hasteOverdriveTimer.Interval = (2 * 1000);
			hasteOverdriveOffTimer.Elapsed += OverdriveOff;
			hasteOverdriveOffTimer.Interval = (2 * 1000);
			lordTimer.Elapsed += LordOff;
			lordTimer.Interval = 5 * (60 * 1000);
			lordSpawnTimer.Elapsed += LordSpawn;
			lordSpawnTimer.Interval = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Interval.TotalMilliseconds;
		}
		private void StopTimers()
		{
			fastActionTimer.Stop();
			actionTimer.Stop();

			subweaponsOnlyTimer.Interval = 1;
			crippleTimer.Interval = 1;
			bloodManaTimer.Interval = 1;
			thirstTimer.Interval = 1;
			thirstTickTimer.Stop();
			hordeTimer.Interval = 1;
			hordeSpawnTimer.Stop();
			enduranceSpawnTimer.Stop();

			vampireTimer.Interval = 1;
			magicianTimer.Interval = 1;
			meltyTimer.Interval = 1;
			fourBeastsTimer.Interval = 1;
			zawarudoTimer.Interval = 1;
			zawarudoCheckTimer.Stop();
			hasteTimer.Interval = 1;
			hasteOverdriveTimer.Stop();
			hasteOverdriveOffTimer.Stop();
		}
		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (queuedActions.Count > 0)
			{
				alucardMapX = sotnApi.AlucardApi.MapX;
				alucardMapY = sotnApi.AlucardApi.MapY;

				if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave() && !IsInKeepRichterRoom() && !IsInLoadingRoom())
				{
					int index = 0;
					bool actionUnlocked = true;

					for (int i = 0; i < queuedActions.Count; i++)
					{
						index = i;
						actionUnlocked = true;
						if (queuedActions[i].LocksSpeed && speedLocked)
						{
							actionUnlocked = false;
							continue;
						}
						if (queuedActions[i].LocksMana && manaLocked)
						{
							actionUnlocked = false;
							continue;
						}
						if (queuedActions[i].LocksInvincibility && invincibilityLocked)
						{
							actionUnlocked = false;
							continue;
						}
						if (queuedActions[i].LocksSpawning && spawnActive)
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

			bool keepRichterRoom = IsInKeepRichterRoom();
			bool galamothRoom = IsInGalamothRoom();
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& !keepRichterRoom && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading && !sotnApi.AlucardApi.IsInvincible() && !IsInLoadingRoom() && alucardMapX < 99)
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
			if (roll > 50)
			{
				int index = rng.Next(0, toolConfig.Khaos.Actions.Count);
				var actionEvent = new EventAddAction { UserName = "Auto Khaos", ActionIndex = index };

				if (toolConfig.Khaos.Actions[index].Name != "Guilty Gear" && toolConfig.Khaos.Actions[index].Name != "Pandora's Box" && !toolConfig.Khaos.Actions[index].IsOnCooldown())
				{
					EnqueueAction(actionEvent);
				}
			}
		}

		#region Khaotic events
		private void RandomizeGold()
		{
			uint gold = (uint) rng.Next(0, 5000);
			uint roll = (uint) rng.Next(0, 21);
			if (roll > 16 && roll < 20)
			{
				gold = gold * (uint) rng.Next(1, 11);
			}
			else if (roll > 19)
			{
				gold = gold * (uint) rng.Next(10, 81);
			}
			sotnApi.AlucardApi.Gold = gold;
		}
		private void RandomizeStatsActivate()
		{
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
				statPool = statPool + offset;
			}
			else if (statPoolRoll == 3)
			{
				statPool = statPool - offset;
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
				pointsPool = pointsPool + offset;
			}
			else if (pointsRoll == 3)
			{
				pointsPool = pointsPool - offset;
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
			var subweapons = Enum.GetValues(typeof(Subweapon));
			sotnApi.AlucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
		}
		private void RandomizeRelicsActivate(bool randomizeVladRelics = true)
		{
			var relics = Enum.GetValues(typeof(Relic));
			foreach (var relic in relics)
			{
				if ((int) relic < 25)
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic);
				}
				int roll = rng.Next(0, 2);
				if (roll > 0)
				{
					if ((int) relic < 25)
					{
						sotnApi.AlucardApi.GrantRelic((Relic) relic);
					}
				}
				else
				{
					if ((int) relic < 25)
					{
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
				foreach (var relic in Constants.Khaos.FlightRelics[roll])
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic);
				}
			}

			if (IsInSwitchRoom())
			{
				sotnApi.AlucardApi.GrantRelic(Relic.JewelOfOpen);
			}
		}
		private void RandomizeEquipmentSlots()
		{
			bool equippedBuggyQuickSwapWeaponRight = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)]);
			bool equippedBuggyQuickSwapWeaponLeft = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)]);
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

			if (equippedBuggyQuickSwapWeaponRight || equippedBuggyQuickSwapWeaponLeft)
			{
				while (Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) newRightHand]))
				{
					newRightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
				}
				while (Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) newLeftHand]))
				{
					newLeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
				}
			}

			sotnApi.AlucardApi.RightHand = newRightHand;
			sotnApi.AlucardApi.LeftHand = newLeftHand;
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
					sotnApi.AlucardApi.State = States.Standing;
					queuedFastActions.Enqueue(new MethodInvoker(() => KillAlucard()));
				}
			}
		}
		private void KillAlucard()
		{
			sotnApi.AlucardApi.State = States.Death;
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
			if (sotnApi.AlucardApi.CurrentHp > toolConfig.Khaos.ThirstDrainPerSecond + 1 + + superDrain)
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
			uint mapX = sotnApi.AlucardApi.MapX;
			uint mapY = sotnApi.AlucardApi.MapY;
			bool keepRichterRoom = ((mapX >= 31 && mapX <= 34) && mapY == 8);
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5 || sotnApi.GameApi.CanSave() || keepRichterRoom)
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
					hordeTimer.Interval = toolConfig.Khaos.Actions[14].Duration.TotalMilliseconds;

					ActionTimer timer = new()
					{
						Name = toolConfig.Khaos.Actions[14].Name,
						Duration = toolConfig.Khaos.Actions[14].Duration
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

			long enemy = sotnApi.ActorApi.FindActorFrom(Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? hordeEnemy = new Actor(sotnApi.ActorApi.GetActor(enemy));

				if (hordeEnemy is not null && hordeEnemies.Where(e => e.Sprite == hordeEnemy.Sprite).Count() < 1)
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
		private void CrippleOff(Object sender, EventArgs e)
		{
			//SetSpeed();
			underwaterPhysics.Disable();
			sotnApi.GameApi.UnderwaterPhysicsEnabled = false;
			crippleTimer.Stop();
			speedLocked = false;
		}
		private void EnduranceSpawn(Object sender, EventArgs e)
		{
			uint roomX = sotnApi.GameApi.MapXPos;
			uint roomY = sotnApi.GameApi.MapYPos;
			float healthMultiplier = 2.5F;

			if ((roomX == enduranceRoomX && roomY == enduranceRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? bossCopy = null;

			long enemy = sotnApi.ActorApi.FindActorFrom(Constants.Khaos.EnduranceBosses);
			if (enemy > 0)
			{
				LiveActor boss = sotnApi.ActorApi.GetLiveActor(enemy);
				bossCopy = new Actor(sotnApi.ActorApi.GetActor(enemy));
				Console.WriteLine($"Endurance boss found hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.Sprite}");

				bossCopy.Xpos = (ushort) (bossCopy.Xpos + rng.Next(-70, 70));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
				bossCopy.Hp = (ushort) (healthMultiplier * bossCopy.Hp);
				sotnApi.ActorApi.SpawnActor(bossCopy);

				boss.Hp = (ushort) (healthMultiplier * boss.Hp);

				if (superEndurance)
				{
					superEndurance = false;

					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					sotnApi.ActorApi.SpawnActor(bossCopy);
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
		private void SpawnPoisonHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + 1);
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
			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + 1);
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
			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + 1);
			hitbox.DamageTypeA = (uint) Actors.Stone;
			hitbox.DamageTypeB = (uint) Actors.Stone;
			sotnApi.ActorApi.SpawnActor(hitbox);
		}
		private void SpawnSlamHitbox()
		{
			bool alucardIsPoisoned = sotnApi.AlucardApi.PoisonTimer > 0;
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + (alucardIsPoisoned ? 3 : 5));
			hitbox.DamageTypeA = (uint) Actors.Slam;
			sotnApi.ActorApi.SpawnActor(hitbox);
		}
		private void BankruptActivate()
		{
			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";


			sotnApi.AlucardApi.Gold = 0;
			sotnApi.AlucardApi.ClearInventory();
			sotnApi.AlucardApi.RightHand = 0;
			sotnApi.AlucardApi.LeftHand = 0;
			sotnApi.AlucardApi.Helm = Equipment.HelmStart;
			sotnApi.AlucardApi.Armor = 0;
			sotnApi.AlucardApi.Cloak = Equipment.CloakStart;
			sotnApi.AlucardApi.Accessory1 = Equipment.AccessoryStart;
			sotnApi.AlucardApi.Accessory2 = Equipment.AccessoryStart;
			sotnApi.GameApi.RespawnItems();

			if (equippedHolyGlasses || hasHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker || hasSpikeBreaker)
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
		private void VampireOff(object sender, System.Timers.ElapsedEventArgs e)
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
		private void FourBeastsOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			invincibilityCheat.Disable();
			invincibilityLocked = false;
			attackPotionCheat.Disable();
			shineCheat.Disable();
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
		private void OverdriveOn(object sender, System.Timers.ElapsedEventArgs e)
		{
			VisualEffectPaletteCheat.PokeValue(33126);
			VisualEffectPaletteCheat.Enable();
			VisualEffectTimerCheat.PokeValue(30);
			VisualEffectTimerCheat.Enable();
			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			hasteOverdriveTimer.Stop();
		}
		private void OverdriveOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			VisualEffectPaletteCheat.Disable();
			VisualEffectTimerCheat.Disable();
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
			uint mapX = sotnApi.AlucardApi.MapX;
			uint mapY = sotnApi.AlucardApi.MapY;
			bool keepRichterRoom = ((mapX >= 31 && mapX <= 34) && mapY == 8);
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < 5 || sotnApi.GameApi.CanSave() || keepRichterRoom)
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
					lordTimer.Interval = toolConfig.Khaos.Actions[14].Duration.TotalMilliseconds;

					ActionTimer timer = new()
					{
						Name = toolConfig.Khaos.Actions[28].Name,
						Duration = toolConfig.Khaos.Actions[28].Duration
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

			long enemy = sotnApi.ActorApi.FindActorFrom(Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? lordEnemy = new Actor(sotnApi.ActorApi.GetActor(enemy));

				if (lordEnemy is not null && lordEnemies.Where(e => e.Sprite == lordEnemy.Sprite).Count() < 1)
				{
					lordEnemies.Add(lordEnemy);
					Console.WriteLine($"Added Lord enemy with hp: {lordEnemy.Hp} sprite: {lordEnemy.Sprite} damage: {lordEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		#endregion

		private void StartCheats()
		{
			faerieScroll.Enable();
			Cheat batCardXp = cheats.GetCheatByName("BatCardXp");
			batCardXp.Enable();
			Cheat ghostCardXp = cheats.GetCheatByName("GhostCardXp");
			ghostCardXp.Enable();
			Cheat faerieCardXp = cheats.GetCheatByName("FaerieCardXp");
			faerieCardXp.Enable();
			Cheat demonCardXp = cheats.GetCheatByName("DemonCardXp");
			demonCardXp.Enable();
			Cheat swordCardXp = cheats.GetCheatByName("SwordCardXp");
			swordCardXp.Enable();
			Cheat spriteCardXp = cheats.GetCheatByName("SpriteCardXp");
			spriteCardXp.Enable();
			Cheat noseDevilCardXp = cheats.GetCheatByName("NoseDevilCardXp");
			noseDevilCardXp.Enable();
			SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			SavePalette.Enable();
		}
		private void SetSaveColorPalette()
		{
			if (alucardSecondCastle)
			{
				SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronSecondCastle);
			}
			else
			{
				SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
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
		private void GetCheats()
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
			VisualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			VisualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			SavePalette = cheats.GetCheatByName("SavePalette");
		}
		private void OverwriteBossNames(string[] subscribers)
		{
			subscribers = subscribers.OrderBy(x => rng.Next()).ToArray();
			var randomizedBosses = Strings.BossNameAddresses.OrderBy(x => rng.Next());
			int i = 0;
			foreach (var boss in randomizedBosses)
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
		private bool IsInGalamothRoom()
		{
			return alucardMapX >= Constants.Khaos.GalamothRoomMapMinX && alucardMapX <= Constants.Khaos.GalamothRoomMapMaxX 
				&& alucardMapY >= Constants.Khaos.GalamothRoomMapMinY && alucardMapY <= Constants.Khaos.GalamothRoomMapMaxY;
		}
		private bool IsInKeepRichterRoom()
		{
			return alucardMapX >= Constants.Khaos.RichterRoomMapMinX && alucardMapX <= Constants.Khaos.RichterRoomMapMaxX && alucardMapY == Constants.Khaos.RichterRoomMapY;
		}
		private bool IsInSuccubusRoom()
		{
			return alucardMapX == Constants.Khaos.SuccubusMapX && alucardMapY == Constants.Khaos.SuccubusMapY;
		}
		private bool IsInEntranceCutscene()
		{
			return alucardMapX >= Constants.Khaos.EntranceCutsceneMapMinX && alucardMapX <= Constants.Khaos.EntranceCutsceneMapMaxX && alucardMapY == Constants.Khaos.EntranceCutsceneMapY;
		}
		private bool IsInSwitchRoom()
		{
			return alucardMapX == 46 && alucardMapY == 24;
		}
		private bool IsInLoadingRoom()
		{
			return Constants.Khaos.LoadingRooms.Where(r => r.X == alucardMapX && r.Y == alucardMapY && Convert.ToBoolean(r.SecondCastle) == alucardSecondCastle).FirstOrDefault() is not null;
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
			long shaftAddress = sotnApi.ActorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.ShaftActor });
			if (shaftAddress > 0)
			{
				LiveActor shaft = sotnApi.ActorApi.GetLiveActor(shaftAddress);
				shaft.Hp = Constants.Khaos.ShaftKhaosHp;
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
				galamothTorso.Hp = Constants.Khaos.GalamothKhaosHp;
				galamothTorso.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
				Console.WriteLine($"gala def: {galamothTorso.Def}");
				//galamothTorso.Def = 0; Removes XP gained

				long galamothHeadAddress = sotnApi.ActorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothHeadActor });
				LiveActor galamothHead = sotnApi.ActorApi.GetLiveActor(galamothHeadAddress);
				galamothHead.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;

				List<long> galamothParts = sotnApi.ActorApi.GetAllActors(new List<SearchableActor> { Constants.Khaos.GalamothPartsActors });
				foreach (var actor in galamothParts)
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
		private void CheckExperience()
		{
			//uint currentExperiecne = sotnApi.AlucardApi.Experiecne;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
		}
		private void CheckWingsmashActive()
		{
			//bool wingsmashActive = sotnApi.AlucardApi.Action == SotnApi.Constants.Values.Alucard.States.Bat;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
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

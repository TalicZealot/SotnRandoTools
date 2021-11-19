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
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Utils;
using WatsonWebsocket;

namespace SotnRandoTools.Khaos
{
	public class KhaosController
	{
		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IActorApi actorApi;
		private readonly ICheatCollectionAdapter cheats;
		private readonly INotificationService notificationService;
		private readonly IInputService inputService;
		private WatsonWsClient socketClient;
		//private WatsonWsServer socketServer;
		private Random rng = new Random();

		private string[]? subscribers =
		{
		};

		private List<QueuedAction> queuedActions = new();
		private Queue<MethodInvoker> queuedFastActions = new();
		private Timer actionTimer = new Timer();
		private Timer fastActionTimer = new Timer();

		#region Timers
		private System.Timers.Timer subweaponsOnlyTimer = new();
		private System.Timers.Timer crippleTimer = new();
		private System.Timers.Timer bloodManaTimer = new();
		private System.Timers.Timer thirstTimer = new();
		private System.Timers.Timer thirstTickTimer = new();
		private System.Timers.Timer hordeTimer = new();
		private System.Timers.Timer hordeSpawnTimer = new();
		private System.Timers.Timer enduranceSpawnTimer = new();
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

		private uint hordeZone = 0;
		private uint hordeZone2 = 0;
		private uint hordeTriggerRoomX = 0;
		private uint hordeTriggerRoomY = 0;
		private List<Actor> hordeEnemies = new();

		private int enduranceCount = 0;
		private uint enduranceRoomX = 0;
		private uint enduranceRoomY = 0;

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

		public KhaosController(IToolConfig toolConfig, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, ICheatCollectionAdapter cheats, INotificationService notificationService, IInputService inputService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (actorApi is null) throw new ArgumentNullException(nameof(actorApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			if (notificationService == null) throw new ArgumentNullException(nameof(notificationService));
			if (inputService is null) throw new ArgumentNullException(nameof(inputService));
			this.toolConfig = toolConfig;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.actorApi = actorApi;
			this.cheats = cheats;
			this.notificationService = notificationService;
			this.inputService = inputService;

			InitializeTimers();
			notificationService.ActionQueue = queuedActions;
			normalInterval = (int) toolConfig.Khaos.QueueInterval.TotalMilliseconds;
			slowInterval = (int) normalInterval * 2;
			fastInterval = (int) normalInterval / 2;
			Console.WriteLine($"Intervals set. \n normal: {normalInterval / 1000}s, slow:{slowInterval / 1000}s, fast:{fastInterval / 1000}s");

			socketClient = new WatsonWsClient(new Uri(Globals.StreamlabsSocketAddress));
			socketClient.ServerConnected += BotConnected;
			socketClient.ServerDisconnected += BotDisconnected;
			socketClient.MessageReceived += BotMessageReceived;
		}

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
			socketClient.Start();

			notificationService.AddMessage($"Khaos started");
			Console.WriteLine("Khaos started");
		}
		public void StopKhaos()
		{
			StopTimers();
			Cheat faerieScroll = cheats.GetCheatByName("FaerieScroll");
			faerieScroll.Disable();
			if (socketClient.Connected)
			{
				socketClient.Stop();
			}
			notificationService.AddMessage($"Khaos stopped");
			Console.WriteLine("Khaos stopped");
		}
		public void OverwriteBossNames(string[] subscribers)
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
				gameApi.OverwriteString(boss.Value, subscribers[i]);
				Console.WriteLine($"{boss.Key} renamed to {subscribers[i]}");
				i++;
			}
		}

		#region Khaotic Effects
		public void KhaosStatus(string user = "Khaos")
		{
			bool entranceCutscene = IsInEntranceCutscene();
			bool succubusRoom = IsInSuccubusRoom();
			int max = 9;
			int result = rng.Next(1, max);
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
					if (succubusRoom)
					{
						SpawnPoisonHitbox();
						notificationService.AddMessage($"{user} poisoned you");
						break;
					}
					SpawnStoneHitbox();
					notificationService.AddMessage($"{user} petrified you");
					break;
				case 4:
					if (succubusRoom || entranceCutscene)
					{
						SpawnPoisonHitbox();
						notificationService.AddMessage($"{user} poisoned you");
						break;
					}
					SpawnSlamHitbox();
					notificationService.AddMessage($"{user} slammed you");
					break;
				case 5:
					alucardApi.ActivatePotion(Potion.LuckPotion);
					notificationService.AddMessage($"{user} gave you luck");
					break;
				case 6:
					alucardApi.ActivatePotion(Potion.ResistFire);
					notificationService.AddMessage($"{user} gave you resistance to fire");
					break;
				case 7:
					alucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave you resistance to dark");
					break;
				case 8:
					alucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defence");
					break;
				default:
					break;
			}

			Alert(KhaosActionNames.KhaosStatus);
		}
		public void KhaosEquipment(string user = "Khaos")
		{
			RandomizeEquipmentSlots();
			notificationService.AddMessage($"{user} used Khaos Equipment");
			Alert(KhaosActionNames.KhaosEquipment);
		}
		public void KhaosStats(string user = "Khaos")
		{
			RandomizeStatsActivate();
			notificationService.AddMessage($"{user} used Khaos Stats");
			Alert(KhaosActionNames.KhaosStats);
		}
		public void KhaosRelics(string user = "Khaos")
		{
			RandomizeRelicsActivate();
			notificationService.AddMessage($"{user} used Khaos Relics");
			Alert(KhaosActionNames.KhaosRelics);
		}
		public void PandorasBox(string user = "Khaos")
		{
			RandomizeGold();
			RandomizeStatsActivate();
			RandomizeEquipmentSlots();
			RandomizeRelicsActivate();
			RandomizeInventory();
			RandomizeSubweapon();
			gameApi.RespawnBosses();
			gameApi.RespawnItems();
			notificationService.AddMessage($"{user} opened Pandora's Box");
			Alert(KhaosActionNames.PandorasBox);
		}
		public void Gamble(string user = "Khaos")
		{
			double goldPercent = rng.NextDouble();
			uint newGold = (uint) ((double) alucardApi.Gold * goldPercent);
			uint goldSpent = alucardApi.Gold - newGold;
			alucardApi.Gold = newGold;
			string item = Equipment.Items[rng.Next(1, Equipment.Items.Count)];
			while (item.Contains("empty hand") || item.Contains("-"))
			{
				item = Equipment.Items[rng.Next(1, Equipment.Items.Count)];
			}
			alucardApi.GrantItemByName(item);


			notificationService.AddMessage($"{user} gambled {goldSpent} gold for {item}");
			Alert(KhaosActionNames.Gamble);
		}
		#endregion
		#region Debuffs
		public void Thirst(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;
			if (meterFull)
			{
				superThirst = true;
				notificationService.KhaosMeter -= 100;
			}

			Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();
			thirstTimer.Start();
			thirstTickTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Thirst,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} used Super Thirst" : $"{user} used Thirst";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Thirst);
		}
		public void Weaken(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;
			float enhancedFactor = 1;
			if (meterFull)
			{
				enhancedFactor = Constants.Khaos.SuperWeakenFactor;
				notificationService.KhaosMeter -= 100;
			}

			alucardApi.CurrentHp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.CurrentMp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.CurrentHearts = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.MaxtHp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.MaxtMp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.MaxtHearts = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Str = (uint) (alucardApi.Str * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Con = (uint) (alucardApi.Con * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Int = (uint) (alucardApi.Int * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Lck = (uint) (alucardApi.Lck * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			uint newLevel = (uint) (alucardApi.Level * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Level = newLevel;
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
				alucardApi.Level = newLevel;
				alucardApi.Experiecne = newExperience;
			}

			string message = meterFull ? $"{user} used Super Weaken" : $"{user} used Weaken";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Weaken);
		}
		public void Cripple(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;
			float enhancedFactor = 1;
			if (meterFull)
			{
				enhancedFactor = Constants.Khaos.SuperCrippleFactor;
				notificationService.KhaosMeter -= 100;
			}

			speedLocked = true;
			SetSpeed(toolConfig.Khaos.CrippleFactor * enhancedFactor);
			Cheat underwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			underwaterPhysics.Enable();
			crippleTimer.Start();
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Cripple,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} used Super Cripple" : $"{user} used Cripple";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Cripple);
		}
		public void BloodMana(string user = "Khaos")
		{
			storedMana = alucardApi.CurrentMp;
			bloodManaActive = true;
			bloodManaTimer.Start();
			manaLocked = true;
			notificationService.AddMessage($"{user} used Blood Mana");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.BloodMana,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.BloodMana);
		}
		public void SubweaponsOnly(string user = "Khaos")
		{
			int roll = rng.Next(1, 10);
			while (roll == 6)
			{
				roll = rng.Next(1, 10);
			}
			alucardApi.Subweapon = (Subweapon) roll;
			alucardApi.ActivatePotion(Potion.SmartPotion);
			alucardApi.GrantRelic(Relic.CubeOfZoe);
			if (alucardApi.HasRelic(Relic.GasCloud))
			{
				alucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			Cheat hearts = cheats.GetCheatByName("Hearts");
			hearts.Enable();
			Cheat curse = cheats.GetCheatByName("CurseTimer");
			curse.Enable();
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(7);
			manaCheat.Enable();
			manaLocked = true;
			subweaponsOnlyActive = true;
			subweaponsOnlyTimer.Start();
			notificationService.AddMessage($"{user} used Subweapons Only");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.SubweaponsOnly,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.SubweaponsOnly);
		}
		public void Bankrupt(string user = "Khaos")
		{
			BankruptActivate();
			notificationService.AddMessage($"{user} used Bankrupt");
			Alert(KhaosActionNames.Bankrupt);
		}
		public void RespawnBosses(string user = "Khaos")
		{
			gameApi.RespawnBosses();
			notificationService.AddMessage($"{user} used Respawn Bosses");
			Alert(KhaosActionNames.RespawnBosses);
		}
		public void Horde(string user = "Khaos")
		{
			hordeTriggerRoomX = gameApi.MapXPos;
			hordeTriggerRoomY = gameApi.MapYPos;
			bool meterFull = notificationService.KhaosMeter >= 100;
			if (meterFull)
			{
				superHorde = true;
				notificationService.KhaosMeter -= 100;
			}

			hordeTimer.Start();
			hordeSpawnTimer.Start();
			string message = meterFull ? $"{user} summoned the Super Horde" : $"{user} summoned the Horde";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.KhaosHorde);
		}
		public void Endurance(string user = "Khaos")
		{
			enduranceRoomX = gameApi.MapXPos;
			enduranceRoomY = gameApi.MapYPos;
			bool meterFull = notificationService.KhaosMeter >= 100;
			if (meterFull)
			{
				superEndurance = true;
				notificationService.KhaosMeter -= 100;
			}

			enduranceCount++;
			enduranceSpawnTimer.Start();
			string message = meterFull ? $"{user} used Super Endurance" : $"{user} used Endurance";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Endurance);
		}
		#endregion
		#region Buffs
		public void LightHelp(string user = "Khaos")
		{
			string item = toolConfig.Khaos.LightHelpItemRewards[rng.Next(0, toolConfig.Khaos.LightHelpItemRewards.Length)];
			int rolls = 0;
			while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.LightHelpItemRewards[rng.Next(0, toolConfig.Khaos.LightHelpItemRewards.Length)];
				rolls++;
			}

			int roll = rng.Next(1, 4);
			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					Console.WriteLine($"Light help rolled: {item}");
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Potion);
					Console.WriteLine($"Light help rolled activate potion.");
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.ShieldPotion);
					Console.WriteLine($"Light help rolled activate shield potion.");
					notificationService.AddMessage($"{user} used a Shield Potion");
					break;
				default:
					break;
			}
			Alert(KhaosActionNames.LightHelp);
		}
		public void MediumHelp(string user = "Khaos")
		{
			string item = toolConfig.Khaos.MediumHelpItemRewards[rng.Next(0, toolConfig.Khaos.MediumHelpItemRewards.Length)];
			int rolls = 0;
			while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.MediumHelpItemRewards[rng.Next(0, toolConfig.Khaos.MediumHelpItemRewards.Length)];
				rolls++;
			}

			int roll = rng.Next(1, 4);
			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					Console.WriteLine($"Medium help rolled: {item}");
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Elixir);
					Console.WriteLine($"Medium help rolled activate Elixir.");
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.Mannaprism);
					Console.WriteLine($"Medium help rolled activate Manna prism.");
					notificationService.AddMessage($"{user} used a Mana Prism");
					break;
				default:
					break;
			}
			Alert(KhaosActionNames.MediumHelp);
		}
		public void HeavytHelp(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;
			if (meterFull)
			{
				notificationService.KhaosMeter -= 100;
			}

			string item;
			int relic;
			int roll;
			RollRewards(out item, out relic, out roll);
			GiveRewards(user, item, relic, roll);
			Alert(KhaosActionNames.HeavyHelp);

			if (meterFull)
			{
				RollRewards(out item, out relic, out roll);
				GiveRewards(user, item, relic, roll);
			}

			void RollRewards(out string item, out int relic, out int roll)
			{
				item = toolConfig.Khaos.HeavyHelpItemRewards[rng.Next(0, toolConfig.Khaos.HeavyHelpItemRewards.Length)];
				int rolls = 0;
				while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
				{
					item = toolConfig.Khaos.HeavyHelpItemRewards[rng.Next(0, toolConfig.Khaos.HeavyHelpItemRewards.Length)];
					rolls++;
				}

				relic = rng.Next(0, Constants.Khaos.ProgressionRelics.Length);

				roll = rng.Next(1, 3);
				for (int i = 0; i < 11; i++)
				{
					if (!alucardApi.HasRelic(Constants.Khaos.ProgressionRelics[relic]))
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
						alucardApi.GrantItemByName(item);
						notificationService.AddMessage($"{user} gave you a {item}");
						break;
					case 2:
						Console.WriteLine($"Heavy help rolled: {Constants.Khaos.ProgressionRelics[relic]}");
						alucardApi.GrantRelic(Constants.Khaos.ProgressionRelics[relic]);
						notificationService.AddMessage($"{user} gave you {Constants.Khaos.ProgressionRelics[relic]}");
						break;
					default:
						break;
				}
			}
		}
		public void Vampire(string user = "Khaos")
		{
			Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();
			Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			vampireTimer.Start();
			notificationService.AddMessage($"{user} used Vampire");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Vampire,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Vampire).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.Vampire);
		}
		public void BattleOrders(string user = "Khaos")
		{
			alucardApi.CurrentHp = (uint) (alucardApi.MaxtHp * Constants.Khaos.BattleOrdersHpMultiplier);
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			alucardApi.ActivatePotion(Potion.ShieldPotion);
			notificationService.AddMessage($"{user} used Battle Orders");
			Alert(KhaosActionNames.BattleOrders);
		}
		public void Magician(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;
			if (meterFull)
			{
				notificationService.KhaosMeter -= 100;
				alucardApi.GrantRelic(Relic.SoulOfBat);
				alucardApi.GrantRelic(Relic.EchoOfBat);
				alucardApi.GrantRelic(Relic.ForceOfEcho);
				alucardApi.GrantRelic(Relic.SoulOfWolf);
				alucardApi.GrantRelic(Relic.PowerOfWolf);
				alucardApi.GrantRelic(Relic.SkillOfWolf);
				alucardApi.GrantRelic(Relic.FormOfMist);
				alucardApi.GrantRelic(Relic.PowerOfMist);
				alucardApi.GrantRelic(Relic.GasCloud);
			}

			alucardApi.GrantItemByName("Wizard hat");
			alucardApi.ActivatePotion(Potion.SmartPotion);
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(99);
			manaCheat.Enable();
			manaLocked = true;
			magicianTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Magician,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} activated Shapeshifter" : $"{user} activated Magician";
			notificationService.AddMessage(message);

			Alert(KhaosActionNames.Magician);
		}
		public void ZaWarudo(string user = "Khaos")
		{
			alucardApi.ActivateStopwatch();
			zaWarudoZone = gameApi.Zone;
			zaWarudoZone2 = gameApi.Zone2;

			if (!subweaponsOnlyActive)
			{
				alucardApi.Subweapon = Subweapon.Stopwatch;
			}

			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Enable();
			zawarudoTimer.Start();
			zawarudoCheckTimer.Start();

			notificationService.AddMessage($"{user} used ZA WARUDO");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.ZaWarudo,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.ZaWarudo);
		}
		public void MeltyBlood(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;
			if (meterFull)
			{
				superMelty = true;
				SetHasteStaticSpeeds(true);
				ToggleHasteDynamicSpeeds(2);
				alucardApi.CurrentHp = alucardApi.MaxtHp;
				alucardApi.CurrentMp = alucardApi.MaxtMp;
				alucardApi.ActivatePotion(Potion.StrPotion);
				alucardApi.AttackPotionTimer = Constants.Khaos.GuiltyGearAttack;
				alucardApi.DarkMetamorphasisTimer = Constants.Khaos.GuiltyGearDarkMetamorphosis;
				alucardApi.DefencePotionTimer = Constants.Khaos.GuiltyGearDefence;
				alucardApi.InvincibilityTimer = Constants.Khaos.GuiltyGearInvincibility;
				notificationService.KhaosMeter -= 100;
			}

			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			Cheat width2 = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			Cheat height2 = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			width.Enable();
			height.Enable();
			width2.Enable();
			height2.Enable();
			alucardApi.GrantRelic(Relic.LeapStone);
			meltyTimer.Start();
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.MeltyBlood,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} activated GUILTY GEAR" : $"{user} activated Melty Blood";
			notificationService.AddMessage(message);
			if (meterFull)
			{
				Alert(KhaosActionNames.GuiltyGear);
			}
			else
			{
				Alert(KhaosActionNames.MeltyBlood);
			}
		}
		public void FourBeasts(string user = "Khaos")
		{
			Cheat invincibilityCheat = cheats.GetCheatByName("Invincibility");
			invincibilityCheat.PokeValue(1);
			invincibilityCheat.Enable();
			invincibilityLocked = true;
			Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			Cheat shineCheat = cheats.GetCheatByName("Shine");
			shineCheat.PokeValue(1);
			shineCheat.Enable();
			fourBeastsTimer.Start();

			notificationService.AddMessage($"{user} used Four Beasts");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.FourBeasts,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.FourBeasts);
		}
		public void Haste(string user = "Khaos")
		{
			bool meterFull = notificationService.KhaosMeter >= 100;

			if (meterFull)
			{
				notificationService.KhaosMeter -= 100;
				superHaste = true;
			}

			SetHasteStaticSpeeds(meterFull);
			hasteTimer.Start();
			hasteActive = true;
			speedLocked = true;
			Console.WriteLine($"{user} used {KhaosActionNames.Haste}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Haste,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault().Duration
			});
			string message = meterFull ? $"{user} activated Super Haste" : $"{user} activated Haste";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Haste);
		}
		#endregion

		public void Update()
		{
			if (gameApi.InAlucardMode() && bloodManaActive)
			{
				CheckManaUsage();
			}
			if (gameApi.InAlucardMode())
			{
				CheckDashInput();
			}
			if (subweaponsOnlyActive)
			{
				if (alucardApi.RightHand == 0)
				{
					alucardApi.RightHand = (uint) Equipment.Items.IndexOf("Orange");
					if (alucardApi.HasItemInInventory("Orange"))
					{
						alucardApi.TakeOneItemByName("Orange");
					}
				}
				if (alucardApi.LeftHand == 0)
				{
					alucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Orange");
					if (alucardApi.HasItemInInventory("Orange"))
					{
						alucardApi.TakeOneItemByName("Orange");
					}
				}
			}
		}
		public void EnqueueAction(EventAddAction eventData)
		{
			if (eventData.Command is null) throw new ArgumentNullException(nameof(eventData.Command));
			if (eventData.Command == "") throw new ArgumentException($"Parameter {nameof(eventData.Command)} is empty!");
			if (eventData.UserName is null) throw new ArgumentNullException(nameof(eventData.UserName));
			if (eventData.UserName == "") throw new ArgumentException($"Parameter {nameof(eventData.UserName)} is empty!");
			string user = eventData.UserName;
			string action = eventData.Command;

			SotnRandoTools.Configuration.Models.Action? commandAction;
			switch (action)
			{
				#region Khaotic commands
				case "kstatus":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosStatus).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaosStatus(user)));
					}
					break;
				case "kequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosEquipment).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Khaos Equipment", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosEquipment(user)) });
					}
					break;
				case "kstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosStats).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Khaos Stats", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosStats(user)) });
					}
					break;
				case "krelics":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosRelics).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Khaos Relics", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosRelics(user)) });
					}
					break;
				case "pandora":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.PandorasBox).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Pandora's Box", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => PandorasBox(user)) });
					}
					break;
				case "gamble":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Gamble).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Gamble(user)));
					}
					break;
				#endregion
				#region Debuffs
				case "bankrupt":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Bankrupt).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Bankrupt", Invoker = new MethodInvoker(() => Bankrupt(user)) });
					}
					break;
				case "weaken":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Weaken).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Weaken", Invoker = new MethodInvoker(() => Weaken(user)) });
					}
					break;
				case "respawnbosses":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RespawnBosses).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => RespawnBosses(user)));
					}
					break;
				case "subsonly":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Subweapons Only", LocksMana = true, Invoker = new MethodInvoker(() => SubweaponsOnly(user)) });
					}
					break;
				case "cripple":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Cripple", LocksSpeed = true, Invoker = new MethodInvoker(() => Cripple(user)) });
					}
					break;
				case "bloodmana":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Blood Mana", LocksMana = true, Invoker = new MethodInvoker(() => BloodMana(user)) });
					}
					break;
				case "thirst":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Thirst", Invoker = new MethodInvoker(() => Thirst(user)) });
					}
					break;
				case "horde":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Horde", Invoker = new MethodInvoker(() => Horde(user)) });
					}
					break;
				case "endurance":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Endurance).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Endurance(user)));
					}
					break;
				#endregion
				#region Buffs
				case "vampire":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Vampire).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Vampire(user)));
					}
					break;
				case "lighthelp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.LightHelp).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => LightHelp(user)));
					}
					break;
				case "mediumhelp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MediumHelp).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MediumHelp(user)));
					}
					break;
				case "heavyhelp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeavyHelp).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HeavytHelp(user)));
					}
					break;
				case "battleorders":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BattleOrders).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Battle Orders", Type = ActionType.Buff, Invoker = new MethodInvoker(() => BattleOrders(user)) });
					}
					break;
				case "magician":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Magician", Type = ActionType.Buff, LocksMana = true, Invoker = new MethodInvoker(() => Magician(user)) });
					}
					break;
				case "melty":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "MeltyBlood", Type = ActionType.Buff, Invoker = new MethodInvoker(() => MeltyBlood(user)) });
					}
					break;
				case "fourbeasts":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Four Beasts", Type = ActionType.Buff, LocksInvincibility = true, Invoker = new MethodInvoker(() => FourBeasts(user)) });
					}
					break;
				case "zawarudo":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ZaWarudo(user)));
					}
					break;
				case "haste":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Haste", Type = ActionType.Buff, LocksSpeed = true, Invoker = new MethodInvoker(() => Haste(user)) });
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
			}
		}
		private void InitializeTimers()
		{
			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 2 * (1 * 1000);

			subweaponsOnlyTimer.Elapsed += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			crippleTimer.Elapsed += CrippleOff;
			crippleTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault().Duration.TotalMilliseconds;
			bloodManaTimer.Elapsed += BloodManaOff;
			bloodManaTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault().Duration.TotalMilliseconds;
			thirstTimer.Elapsed += ThirstOff;
			thirstTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault().Duration.TotalMilliseconds;
			thirstTickTimer.Elapsed += ThirstDrain;
			thirstTickTimer.Interval = 1000;
			hordeTimer.Elapsed += HordeOff;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeSpawnTimer.Elapsed += HordeSpawn;
			hordeSpawnTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Interval.TotalMilliseconds;
			enduranceSpawnTimer.Elapsed += EnduranceSpawn;
			enduranceSpawnTimer.Interval = 2 * (1000);

			vampireTimer.Elapsed += VampireOff;
			vampireTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Vampire).FirstOrDefault().Duration.TotalMilliseconds;
			magicianTimer.Elapsed += MagicianOff;
			magicianTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault().Duration.TotalMilliseconds;
			meltyTimer.Elapsed += MeltyBloodOff;
			meltyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault().Duration.TotalMilliseconds;
			fourBeastsTimer.Elapsed += FourBeastsOff;
			fourBeastsTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault().Duration.TotalMilliseconds;
			zawarudoTimer.Elapsed += ZawarudoOff;
			zawarudoTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault().Duration.TotalMilliseconds;
			zawarudoCheckTimer.Elapsed += ZaWarudoAreaCheck;
			zawarudoCheckTimer.Interval += 2 * 1000;
			hasteTimer.Elapsed += HasteOff;
			hasteTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault().Duration.TotalMilliseconds;
			hasteOverdriveTimer.Elapsed += OverdriveOn;
			hasteOverdriveTimer.Interval = (2 * 1000);
			hasteOverdriveOffTimer.Elapsed += OverdriveOff;
			hasteOverdriveOffTimer.Interval = (2 * 1000);
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
				bool keepRichterRoom = IsInKeepRichterRoom();
				if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && !keepRichterRoom)
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
						break;
					}

					if (actionUnlocked)
					{
						queuedActions[index].Invoker();
						queuedActions.RemoveAt(index);
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
			bool keepRichterRoom = IsInKeepRichterRoom();
			bool galamothRoom = IsInGalamothRoom();
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && !keepRichterRoom && !gameApi.InTransition && !gameApi.IsLoading)
			{
				shaftHpSet = false;
				if (queuedFastActions.Count > 0)
				{
					queuedFastActions.Dequeue()();
				}
			}
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && keepRichterRoom && !shaftHpSet && !gameApi.InTransition && !gameApi.IsLoading)
			{
				SetShaftHp();
			}
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && galamothRoom && !galamothStatsSet && !gameApi.InTransition && !gameApi.IsLoading)
			{
				SetGalamothtStats();
			}
			if (!galamothRoom)
			{
				galamothStatsSet = false;
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
			alucardApi.Gold = gold;
		}
		private void RandomizeStatsActivate()
		{
			uint maxHp = alucardApi.MaxtHp;
			uint currentHp = alucardApi.CurrentHp;
			uint maxMana = alucardApi.MaxtMp;
			uint currentMana = alucardApi.CurrentMp;
			uint str = alucardApi.Str;
			uint con = alucardApi.Con;
			uint intel = alucardApi.Int;
			uint lck = alucardApi.Lck;

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

			alucardApi.Str = (6 + newStr);
			alucardApi.Con = (6 + newCon);
			alucardApi.Int = (6 + newInt);
			alucardApi.Lck = (6 + newLck);

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
				alucardApi.CurrentHp = pointsHp;
			}
			if (currentMana > pointsMp)
			{
				alucardApi.CurrentMp = pointsMp;
			}

			alucardApi.MaxtHp = pointsHp;
			alucardApi.MaxtMp = pointsMp;
		}
		private void RandomizeInventory()
		{
			bool hasHolyGlasses = alucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = alucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = alucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = alucardApi.HasItemInInventory("Silver Ring");

			alucardApi.ClearInventory();

			int itemCount = rng.Next(toolConfig.Khaos.PandoraMinItems, toolConfig.Khaos.PandoraMaxItems + 1);

			for (int i = 0; i < itemCount; i++)
			{
				int result = rng.Next(0, Equipment.Items.Count);
				alucardApi.GrantItemByName(Equipment.Items[result]);
			}

			if (hasHolyGlasses)
			{
				alucardApi.GrantItemByName("Holy glasses");
			}
			if (hasSpikeBreaker)
			{
				alucardApi.GrantItemByName("Spike Breaker");
			}
			if (hasGoldRing)
			{
				alucardApi.GrantItemByName("Gold Ring");
			}
			if (hasSilverRing)
			{
				alucardApi.GrantItemByName("Silver Ring");
			}
		}
		private void RandomizeSubweapon()
		{
			var subweapons = Enum.GetValues(typeof(Subweapon));
			alucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
		}
		private void RandomizeRelicsActivate()
		{
			bool secondCastle = gameApi.SecondCastle;
			var relics = Enum.GetValues(typeof(Relic));
			foreach (var relic in relics)
			{
				if ((int) relic < 25)
				{
					alucardApi.GrantRelic((Relic) relic);
				}
				int roll = rng.Next(0, 2);
				if (roll > 0)
				{
					if ((int) relic < 25)
					{
						alucardApi.GrantRelic((Relic) relic);
					}
				}
				else
				{
					if (!toolConfig.Khaos.KeepVladRelics || (toolConfig.Khaos.KeepVladRelics && (int) relic < 25))
					{
						alucardApi.TakeRelic((Relic) relic);
					}
				}
			}

			if (secondCastle)
			{
				int roll = rng.Next(0, Constants.Khaos.FlightRelics.Count);
				foreach (var relic in Constants.Khaos.FlightRelics[roll])
				{
					alucardApi.GrantRelic((Relic) relic);
				}
			}
		}
		private void RandomizeEquipmentSlots()
		{
			bool equippedHolyGlasses = Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (alucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";

			alucardApi.RightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			alucardApi.LeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			alucardApi.Armor = (uint) rng.Next(0, Equipment.ArmorCount + 1);
			alucardApi.Helm = Equipment.HelmStart + (uint) rng.Next(0, Equipment.HelmCount + 1);
			alucardApi.Cloak = Equipment.CloakStart + (uint) rng.Next(0, Equipment.CloakCount + 1);
			alucardApi.Accessory1 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			alucardApi.Accessory2 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);

			RandomizeSubweapon();
			if (subweaponsOnlyActive)
			{
				while (alucardApi.Subweapon == Subweapon.Empty || alucardApi.Subweapon == Subweapon.Stopwatch)
				{
					RandomizeSubweapon();
				}
			}

			if (equippedHolyGlasses)
			{
				alucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker)
			{
				alucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing)
			{
				alucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing)
			{
				alucardApi.GrantItemByName("Silver Ring");
			}
		}
		#endregion
		#region Debuff events
		private void BloodManaUpdate()
		{
			if (spentMana > 0)
			{
				uint currentHp = alucardApi.CurrentHp;
				if (currentHp > spentMana)
				{
					alucardApi.CurrentHp -= (uint) spentMana;
					alucardApi.CurrentMp += (uint) spentMana;
				}
				else
				{
					alucardApi.CurrentHp = 1;
				}
			}
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
			if (alucardApi.CurrentHp > toolConfig.Khaos.ThirstDrainPerSecond + 1 + + superDrain)
			{
				alucardApi.CurrentHp -= (toolConfig.Khaos.ThirstDrainPerSecond + superDrain);
			}
			else
			{
				alucardApi.CurrentHp = 1;
			}
		}
		private void ThirstOff(Object sender, EventArgs e)
		{
			Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.Disable();
			thirstTimer.Stop();
			thirstTickTimer.Stop();
			superThirst = false;
		}
		private void HordeOff(Object sender, EventArgs e)
		{
			superHorde = false;
			hordeEnemies.RemoveRange(0, hordeEnemies.Count);
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeTimer.Stop();
			hordeSpawnTimer.Stop();
		}
		private void HordeSpawn(Object sender, EventArgs e)
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			bool keepRichterRoom = ((mapX >= 31 && mapX <= 34) && mapY == 8);
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5 || gameApi.CanSave() || keepRichterRoom)
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

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
					hordeTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Duration.TotalMilliseconds;
					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = KhaosActionNames.KhaosHorde,
						Type = Enums.ActionType.Debuff,
						Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Duration
					});
					hordeTimer.Start();
				}
				hordeEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				actorApi.SpawnActor(hordeEnemies[enemyIndex]);
			}
		}
		private bool FindHordeEnemy()
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;

			if ((roomX == hordeTriggerRoomX && roomY == hordeTriggerRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu())
			{
				return false;
			}

			long enemy = actorApi.FindActorFrom(Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? hordeEnemy = new Actor(actorApi.GetActor(enemy));

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
			Cheat curse = cheats.GetCheatByName("CurseTimer");
			curse.Disable();
			manaLocked = false;
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			Cheat hearts = cheats.GetCheatByName("Hearts");
			hearts.Disable();
			if (gasCloudTaken)
			{
				alucardApi.GrantRelic(Relic.GasCloud);
				gasCloudTaken = false;
			}
			subweaponsOnlyTimer.Stop();
			subweaponsOnlyActive = false;
		}
		private void CrippleOff(Object sender, EventArgs e)
		{
			SetSpeed();
			Cheat underwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			underwaterPhysics.Disable();
			crippleTimer.Stop();
			speedLocked = false;
		}
		private void EnduranceSpawn(Object sender, EventArgs e)
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;
			float healthMultiplier = 1.7F;

			if ((roomX == enduranceRoomX && roomY == enduranceRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? bossCopy = null;

			long enemy = actorApi.FindActorFrom(Constants.Khaos.EnduranceBosses);
			if (enemy > 0)
			{
				LiveActor boss = actorApi.GetLiveActor(enemy);
				bossCopy = new Actor(actorApi.GetActor(enemy));
				Console.WriteLine($"Endurance boss found hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.Sprite}");

				bossCopy.Xpos = (ushort) (bossCopy.Xpos + rng.Next(-70, 70));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
				bossCopy.Hp = (ushort) (healthMultiplier * bossCopy.Hp);
				actorApi.SpawnActor(bossCopy);

				boss.Hp = (ushort) (healthMultiplier * boss.Hp);

				if (superEndurance)
				{
					superEndurance = false;

					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					actorApi.SpawnActor(bossCopy);
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
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Actors.Poison;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnCurseHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
			hitbox.DamageTypeB = (uint) Actors.Curse;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnStoneHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Actors.Stone;
			hitbox.DamageTypeB = (uint) Actors.Stone;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnSlamHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 5);
			hitbox.DamageTypeA = (uint) Actors.Slam;
			actorApi.SpawnActor(hitbox);
		}
		private void BankruptActivate()
		{
			bool hasHolyGlasses = alucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = alucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = alucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = alucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (alucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";


			alucardApi.Gold = 0;
			alucardApi.ClearInventory();
			alucardApi.RightHand = 0;
			alucardApi.LeftHand = 0;
			alucardApi.Helm = Equipment.HelmStart;
			alucardApi.Armor = 0;
			alucardApi.Cloak = Equipment.CloakStart;
			alucardApi.Accessory1 = Equipment.AccessoryStart;
			alucardApi.Accessory2 = Equipment.AccessoryStart;
			gameApi.RespawnItems();

			if (equippedHolyGlasses || hasHolyGlasses)
			{
				alucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker || hasSpikeBreaker)
			{
				alucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing || hasGoldRing)
			{
				alucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing || hasSilverRing)
			{
				alucardApi.GrantItemByName("Silver Ring");
			}
		}
		#endregion
		#region Buff events
		private void VampireOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Disable();
			Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Disable();
			vampireTimer.Stop();
		}
		private void MagicianOff(Object sender, EventArgs e)
		{
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			manaLocked = false;
			magicianTimer.Stop();
		}
		private void MeltyBloodOff(Object sender, EventArgs e)
		{
			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			Cheat width2 = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			Cheat height2 = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			width.Disable();
			height.Disable();
			width2.Disable();
			height2.Disable();

			if (superMelty)
			{
				superMelty = false;
				SetSpeed();
			}

			meltyTimer.Stop();
		}
		private void FourBeastsOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			Cheat invincibilityCheat = cheats.GetCheatByName("Invincibility");
			invincibilityCheat.Disable();
			invincibilityLocked = false;
			Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.Disable();
			Cheat shineCheat = cheats.GetCheatByName("Shine");
			shineCheat.Disable();
			fourBeastsTimer.Stop();
		}
		private void ZawarudoOff(Object sender, EventArgs e)
		{
			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Disable();
			zawarudoTimer.Stop();
			zawarudoCheckTimer.Stop();
		}
		private void ZaWarudoAreaCheck(Object sender, EventArgs e)
		{
			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (zaWarudoZone != zone || zaWarudoZone2 != zone2)
			{
				zaWarudoZone = zone;
				zaWarudoZone2 = zone2;
				alucardApi.ActivateStopwatch();
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
			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5));
			alucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2));
			alucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2));
			Console.WriteLine("Set speeds:");
			Console.WriteLine($"Wingsmash: {(uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5))}");
			Console.WriteLine($"Wolf dash right: {(sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2))}");
			Console.WriteLine($"Wolf dash left: {(sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2))}");
		}
		private void ToggleHasteDynamicSpeeds(float factor = 1)
		{
			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

			alucardApi.WalkingWholeSpeed = horizontalWhole;
			alucardApi.WalkingFractSpeed = horizontalFract;
			alucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			alucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			alucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalWhole);
			alucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalFract;
			alucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalWhole;
			alucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalFract;
			alucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			alucardApi.FallingHorizontalFractSpeed = horizontalFract;
		}
		private void OverdriveOn(object sender, System.Timers.ElapsedEventArgs e)
		{
			Cheat VisualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			VisualEffectPaletteCheat.PokeValue(33126);
			VisualEffectPaletteCheat.Enable();
			Cheat VisualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			VisualEffectTimerCheat.PokeValue(30);
			VisualEffectTimerCheat.Enable();
			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			hasteOverdriveTimer.Stop();
		}
		private void OverdriveOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			Cheat VisualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			VisualEffectPaletteCheat.Disable();
			Cheat VisualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			VisualEffectTimerCheat.Disable();
			if (hasteActive)
			{
				SetHasteStaticSpeeds(superHaste);
			}
			else
			{
				alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal);
			}
			overdriveOn = false;
			hasteOverdriveOffTimer.Stop();
		}
		#endregion

		private bool IsInGalamothRoom()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return (mapX >= Constants.Khaos.GalamothRoomMapMinX && mapX <= Constants.Khaos.GalamothRoomMapMaxX) && (mapY >= Constants.Khaos.GalamothRoomMapMinY && mapY <= Constants.Khaos.GalamothRoomMapMaxY);
		}
		private bool IsInKeepRichterRoom()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return ((mapX >= Constants.Khaos.RichterRoomMapMinX && mapX <= Constants.Khaos.RichterRoomMapMaxX) && mapY == Constants.Khaos.RichterRoomMapY);
		}
		private bool IsInSuccubusRoom()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return (mapX == Constants.Khaos.SuccubusMapX && mapY == Constants.Khaos.SuccubusMapY);
		}
		private bool IsInEntranceCutscene()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return ((mapX >= Constants.Khaos.EntranceCutsceneMapMinX && mapX <= Constants.Khaos.EntranceCutsceneMapMaxX) && mapY == Constants.Khaos.EntranceCutsceneMapY);
		}
		private void StartCheats()
		{
			Cheat faerieScroll = cheats.GetCheatByName("FaerieScroll");
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
		}
		private void SetSpeed(float factor = 1)
		{
			bool slow = factor < 1;
			bool fast = factor > 1;

			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * factor);
			alucardApi.WalkingWholeSpeed = horizontalWhole;
			alucardApi.WalkingFractSpeed = horizontalFract;
			alucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			alucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			alucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalWhole);
			alucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalFract;
			alucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalWhole;
			alucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalFract;
			alucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			alucardApi.FallingHorizontalFractSpeed = horizontalFract;
			alucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * factor);
			alucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * factor);
			alucardApi.BackdashDecel = slow == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
			Console.WriteLine($"Set all speeds with factor {factor}");
		}
		private void SetShaftHp()
		{
			long shaftAddress = actorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.ShaftActor });
			if (shaftAddress > 0)
			{
				LiveActor shaft = actorApi.GetLiveActor(shaftAddress);
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
			long galamothTorsoAddress = actorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothTorsoActor });
			if (galamothTorsoAddress > 0)
			{
				LiveActor galamothTorso = actorApi.GetLiveActor(galamothTorsoAddress);
				galamothTorso.Hp = Constants.Khaos.GalamothKhaosHp;
				galamothTorso.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
				Console.WriteLine($"gala def: {galamothTorso.Def}");
				//galamothTorso.Def = 0; Removes XP gained

				long galamothHeadAddress = actorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothHeadActor });
				LiveActor galamothHead = actorApi.GetLiveActor(galamothHeadAddress);
				galamothHead.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;

				List<long> galamothParts = actorApi.GetAllActors(new List<SearchableActor> { Constants.Khaos.GalamothPartsActors });
				foreach (var actor in galamothParts)
				{
					LiveActor galamothAnchor = actorApi.GetLiveActor(actor);
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
			uint currentMana = alucardApi.CurrentMp;
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
			uint currentExperiecne = alucardApi.Experiecne;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
		}
		private void CheckWingsmashActive()
		{
			//bool wingsmashActive = alucardApi.Action == SotnApi.Constants.Values.Alucard.States.Bat;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
		}
		private void GainKhaosMeter(short meter)
		{
			notificationService.KhaosMeter += meter;
		}
		private void Alert(string actionName)
		{
			if (!toolConfig.Khaos.Alerts)
			{
				return;
			}

			var action = toolConfig.Khaos.Actions.Where(a => a.Name == actionName).FirstOrDefault();

			if (action is not null && action.AlertPath is not null && action.AlertPath != String.Empty)
			{
				notificationService.PlayAlert(action.AlertPath);
			}
		}
		private void BotMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			JObject eventJson = JObject.Parse(Encoding.UTF8.GetString(e.Data));
			Console.WriteLine("Message from bot: \n" + eventJson.ToString());

			if (eventJson["event"] is not null && eventJson["data"] is not null && eventJson["event"].ToString() == Globals.ActionSocketEvent)
			{
				JObject actionData = JObject.Parse(eventJson["data"].ToString().Replace("/", ""));
				if (actionData["Command"] is not null && actionData["UserName"] is not null)
				{
					EnqueueAction(new EventAddAction { Command = actionData["Command"].ToString(), UserName = actionData["UserName"].ToString() });
				}
			}
			else if (eventJson["event"] is not null && eventJson["data"] is not null && eventJson["event"].ToString() == Globals.ConnectedSocketEvent)
			{
				notificationService.AddMessage($"Bot connected");
			}
		}
		private void BotDisconnected(object sender, EventArgs e)
		{
			Console.WriteLine("Bot socket disconnected");
		}
		private void BotConnected(object sender, EventArgs e)
		{
			JObject auth = JObject.FromObject(new
			{
				author = Globals.Author,
				website = Globals.Website,
				api_key = toolConfig.Khaos.BotApiKey,
				events = new string[] { Globals.ActionSocketEvent }
			});
			socketClient.SendAsync(auth.ToString(), System.Net.WebSockets.WebSocketMessageType.Text);
			Console.WriteLine("Bot socket connected, sending authentication");
		}
	}
}

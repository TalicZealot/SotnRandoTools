using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Utils;

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

		private string[] lightHelpItems =
		{
			"Leather shield",
			"Shaman shield",
			"Pot Roast",
			"Sirloin",
			"Turkey",
			"Bat Pentagram",
			"Javelin",
			"Luminus",
			"Jewel sword",
			"Icebrand",
			"Holy rod",
			"Star flail",
			"Chakram",
			"Holbein dagger",
			"Heart Refresh",
			"Antivenom",
			"Uncurse",
			"Life apple",
			"Str. potion",
			"Attack potion",
			"Shield potion",
			"Resist fire",
			"Potion",
			"Alucart shield",
			"Alucart sword",
			"Stone mask",
			"Wizard hat",
			"Platinum mail",
			"Diamond plate",
			"Healing mail",
			"Fire mail",
			"Mirror cuirass",
			"Brilliant mail",
			"Axe Lord armor",
			"Alucart mail",
			"Royal cloak",
			"Blood cloak",
			"Zircon",
			"Aquamarine",
			"Lapis lazuli",
			"Medal",
			"Talisman"
		};
		private string[] mediumHelpItems =
		{
			"Fire shield",
			"Alucard shield",
			"Cross shuriken",
			"Buffalo star",
			"Flame star",
			"Estoc",
			"Iron Fist",
			"Gram",
			"Holy sword",
			"Dark Blade",
			"Mourneblade",
			"Osafune katana",
			"Topaz circlet",
			"Beryl circlet",
			"Fury plate",
			"Jopseph's cloak",
			"Twilight cloak",
			"Moonstone",
			"Turquoise",
			"Onyx",
			"Mystic pendant",
			"Gauntlet",
			"Ring of Feanor",
			"King's stone"
		};
		private string[] heavyHelpItems =
		{
			"Shield rod",
			"Iron shield",
			"Medusa shield",
			"Alucard shield",
			"Zweihander",
			"Obsidian sword",
			"Mablung Sword",
			"Masamune",
			"Elixir",
			"Manna prism",
			"Marsil",
			"Fist of Tulkas",
			"Gurthang",
			"Alucard sword",
			"Vorpal blade",
			"Crissaegirm",
			"Yasatsuna",
			"Library card",
			"Dragon helm",
			"Holy glasses",
			"Spike Breaker",
			"Dark armor",
			"Dracula tunic",
			"God's Garb",
			"Diamond",
			"Ring of Ares",
			"Ring of Varda",
			"Duplicator",
			"Covenant stone",
			"Gold Ring",
			"Silver Ring"
		};
		private string[] progressionRelics =
		{
			"SoulOfBat",
			"SoulOfWolf",
			"FormOfMist",
			"GravityBoots",
			"LeapStone",
			"JewelOfOpen",
			"MermanStatue"
		};
		private string[] subscribers =
		{
		};

		private Queue<MethodInvoker> queuedActions = new();
		private Queue<MethodInvoker> queuedFastActions = new();
		private Timer actionTimer = new Timer();
		private Timer fastActionTimer = new Timer();

		private System.Timers.Timer honestGamerTimer = new();
		private System.Timers.Timer subweaponsOnlyTimer = new();
		private System.Timers.Timer crippleTimer = new();
		private System.Timers.Timer bloodManaTimer = new();
		private System.Timers.Timer bloodManaTickTimer = new();
		private System.Timers.Timer thirstTimer = new();
		private System.Timers.Timer thirstTickTimer = new();
		private System.Timers.Timer hordeTimer = new();
		private System.Timers.Timer hordeSpawnTimer = new();
		private System.Timers.Timer enduranceSpawnTimer = new();
		private System.Timers.Timer magicianTimer = new();
		private System.Timers.Timer meltyTimer = new();
		private System.Timers.Timer zawarudoTimer = new();
		private System.Timers.Timer hasteTimer = new();

		private uint hordeZone = 0;
		private uint hordeZone2 = 0;
		private Actor? hordeEnemy = null;
		private List<Actor> bannedEnemies = new();
		private uint storedMana = 0;
		private int spentMana = 0;
		private bool bloodManaActive = false;

		private FileSystemWatcher botFileWatcher = new FileSystemWatcher();

		public KhaosController(IToolConfig toolConfig, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, ICheatCollectionAdapter cheats, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (actorApi is null) throw new ArgumentNullException(nameof(actorApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			if (notificationService == null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.actorApi = actorApi;
			this.cheats = cheats;
			this.notificationService = notificationService;

			if (File.Exists(toolConfig.Khaos.BotActionsFilePath))
			{
				botFileWatcher.Path = Path.GetDirectoryName(toolConfig.Khaos.BotActionsFilePath);
				botFileWatcher.Filter = Path.GetFileName(toolConfig.Khaos.BotActionsFilePath);
				botFileWatcher.Changed += OnBotFileChanged;
			}
			else
			{
				Console.WriteLine("Bot action file not found!");
			}

			InitializeTimers();
		}

		public void StartKhaos()
		{
			if (File.Exists(toolConfig.Khaos.BotActionsFilePath))
			{
				botFileWatcher.EnableRaisingEvents = true;
			}
			if (File.Exists(toolConfig.Khaos.NamesFilePath))
			{
				subscribers = FileExtensions.GetText(toolConfig.Khaos.NamesFilePath).Split(',');
			}
			actionTimer.Start();
			fastActionTimer.Start();
			if (subscribers.Length > 0)
			{
				OverwriteBossNames(subscribers);
			}
			Cheat faerieScroll = cheats.GetCheatByName("FaerieScroll");
			faerieScroll.Enable();
			gameApi.OverwriteString(Strings.FleaMan, "Kappa");
			gameApi.OverwriteString(Strings.Shaft, "Talic");
			gameApi.OverwriteString(Strings.Dracula, "3snoW");
			notificationService.DisplayMessage($"Khaos started");
		}

		public void StopKhaos()
		{
			if (File.Exists(toolConfig.Khaos.BotActionsFilePath))
			{
				botFileWatcher.EnableRaisingEvents = false;
			}
			actionTimer.Stop();
			fastActionTimer.Stop();
			Cheat faerieScroll = cheats.GetCheatByName("FaerieScroll");
			faerieScroll.Disable();
			notificationService.DisplayMessage($"Khaos stopped");
		}

		public void OverwriteBossNames(string[] subscribers)
		{
			Random rnd = new Random();
			subscribers = subscribers.OrderBy(x => rnd.Next()).ToArray();
			int i = 0;
			foreach (var boss in Strings.BossNameAddresses)
			{
				if (i == subscribers.Length)
				{
					break;
				}
				gameApi.OverwriteString(boss.Value, subscribers[i]);
				i++;
			}
		}

		#region Khaotic Effects
		public void KhaosStatus(string user = "Khaos")
		{
			Random rnd = new Random();
			int result = rnd.Next(1, 4);
			switch (result)
			{
				case 1:
					SpawnPoisonHitbox();
					notificationService.DisplayMessage($"{user} poisoned you");
					break;
				case 2:
					SpawnCurseHitbox();
					notificationService.DisplayMessage($"{user} cursed you");
					break;
				case 3:
					SpawnStoneHitbox();
					notificationService.DisplayMessage($"{user} petrified you");
					break;
				default:
					break;
			}

			Alert("Khaos Status");
		}


		public void KhaosEquipment(string user = "Khaos")
		{
			RandomizeEquipmentSlots();
			notificationService.DisplayMessage($"{user} used Khaos Equipment");
			notificationService.DequeueAction();
			Alert("Khaos Equipment");
		}
		public void KhaosStats(string user = "Khaos")
		{
			RandomizeStatsActivate();
			notificationService.DisplayMessage($"{user} used Khaos Stats");
			notificationService.DequeueAction();
			Alert("Khaos Stats");
		}
		public void KhaosRelics(string user = "Khaos")
		{
			RandomizeRelicsActivate();
			notificationService.DisplayMessage($"{user} used Khaos Relics");
			notificationService.DequeueAction();
			Alert("Khaos Relics");
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
			notificationService.DisplayMessage($"{user} opened Pandora's Box");
			notificationService.DequeueAction();
			Alert("Pandora's Box");
		}
		public void Gamble(string user = "Khaos")
		{
			Random rnd = new Random();
			double goldPercent = rnd.NextDouble();
			uint newGold = (uint) ((double) alucardApi.Gold * goldPercent);
			uint goldSpent = alucardApi.Gold - newGold;
			alucardApi.Gold = newGold;
			string item = Equipment.Items[rnd.Next(1, Equipment.Items.Count)];
			while (item.Contains("empty hand") || item.Contains("-"))
			{
				item = Equipment.Items[rnd.Next(1, Equipment.Items.Count)];
			}
			alucardApi.GrantItemByName(item);


			notificationService.DisplayMessage($"{user} gambled {goldSpent} gold for {item}");
			notificationService.DequeueAction();
			Alert("Gamble");
		}
		#endregion
		#region Debuffs
		public void Thirst(string user = "Khaos")
		{
			alucardApi.DarkMetamorphasisTimer = 40;
			thirstTimer.Start();
			thirstTickTimer.Start();

			notificationService.DisplayMessage($"{user} used Thirst");
			notificationService.DequeueAction();
			Alert("Thirst");
		}
		public void Weaken(string user = "Khaos")
		{
			alucardApi.CurrentHp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor);
			alucardApi.CurrentMp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor);
			alucardApi.CurrentHearts = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor);
			alucardApi.MaxtHp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor);
			alucardApi.MaxtMp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor);
			alucardApi.MaxtHearts = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor);
			alucardApi.Str = (uint) (alucardApi.Str * toolConfig.Khaos.WeakenFactor);
			alucardApi.Con = (uint) (alucardApi.Con * toolConfig.Khaos.WeakenFactor);
			alucardApi.Int = (uint) (alucardApi.Int * toolConfig.Khaos.WeakenFactor);
			alucardApi.Lck = (uint) (alucardApi.Lck * toolConfig.Khaos.WeakenFactor);
			uint newLevel = (uint) (alucardApi.Level * toolConfig.Khaos.WeakenFactor);
			alucardApi.Level = newLevel;
			uint newExperience;
			if (newLevel <= StatsValues.ExperienceValues.Length)
			{
				newExperience = (uint)StatsValues.ExperienceValues[(int)newLevel - 1];
			}
			else
			{
				newExperience = (uint) StatsValues.ExperienceValues[StatsValues.ExperienceValues.Length - 1];
			}
			alucardApi.Experiecne = newExperience;

			notificationService.DisplayMessage($"{user} used Weaken");
			notificationService.DequeueAction();
			Alert("Weaken");
		}
		public void Cripple(string user = "Khaos")
		{
			SetSpeed(toolConfig.Khaos.CrippleFactor);
			crippleTimer.Start();
			notificationService.DisplayMessage($"{user} used Cripple");
			notificationService.DequeueAction();
			Alert("Cripple");
		}
		public void BloodMana(string user = "Khaos")
		{
			storedMana = alucardApi.CurrentMp;
			bloodManaActive = true;
			bloodManaTimer.Start();
			bloodManaTickTimer.Start();
			notificationService.DisplayMessage($"{user} used Blood Mana");
			notificationService.DequeueAction();
			Alert("Blood Mana");
		}
		public void HonestGamer(string user = "Khaos")
		{
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(5);
			manaCheat.Enable();
			honestGamerTimer.Start();
			notificationService.DisplayMessage($"{user} used Honest Gamer");
			notificationService.DequeueAction();
			Alert("Honest Gamer");

		}
		public void SubweaponsOnly(string user = "Khaos")
		{
			Random rnd = new Random();
			int roll = rnd.Next(1, 10);
			while (roll == 6)
			{
				roll = rnd.Next(1, 10);
			}
			alucardApi.Subweapon = (Subweapon) roll;
			alucardApi.CurrentHearts = 200;
			alucardApi.ActivatePotion(Potion.SmartPotion);
			alucardApi.GrantRelic(Relic.CubeOfZoe);
			HonestGamer();
			Cheat curse = cheats.GetCheatByName("CurseTimer");
			curse.Enable();
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(5);
			manaCheat.Enable();
			subweaponsOnlyTimer.Start();
			notificationService.DisplayMessage($"{user} used Subweapons Only");
			notificationService.DequeueAction();
			Alert("Subweapons Only");
		}
		public void Bankrupt(string user = "Khaos")
		{
			BankruptActivate();
			notificationService.DisplayMessage($"{user} used Bankrupt");
			notificationService.DequeueAction();
			Alert("Bankrupt");
		}
		public void RespawnBosses(string user = "Khaos")
		{
			gameApi.RespawnBosses();
			notificationService.DisplayMessage($"{user} used Respawn Bosses");
			Alert("Respawn Bosses");
		}
		public void Horde(string user = "Khaos")
		{
			hordeTimer.Start();
			hordeSpawnTimer.Start();
			notificationService.DisplayMessage($"{user} summoned the Horde");
			notificationService.DequeueAction();
			Alert("Khaos Horde");
		}
		public void Endurance(string user = "Khaos")
		{
			enduranceSpawnTimer.Start();
			notificationService.DisplayMessage($"{user} used Endurance");
			Alert("Endurance");
		}
		#endregion
		#region Buffs
		public void LightHelp(string user = "Khaos")
		{
			Random rnd = new Random();
			string item = lightHelpItems[rnd.Next(0, lightHelpItems.Length)];

			int roll = rnd.Next(1, 4);
			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					notificationService.DisplayMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Potion);
					notificationService.DisplayMessage($"{user} healed you");
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.DisplayMessage($"{user} used a Shield Potion");
					break;
				default:
					break;
			}
			Alert("Light Help");
		}
		public void MediumHelp(string user = "Khaos")
		{
			Random rnd = new Random();
			string item = mediumHelpItems[rnd.Next(0, mediumHelpItems.Length)];

			int roll = rnd.Next(1, 4);
			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					notificationService.DisplayMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Elexir);
					notificationService.DisplayMessage($"{user} healed you");
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.ManaPrism);
					notificationService.DisplayMessage($"{user} used a Mana Prism");
					break;
				default:
					break;
			}
			Alert("Medium Help");
		}
		public void HeavytHelp(string user = "Khaos")
		{
			Random rnd = new Random();
			string item = heavyHelpItems[rnd.Next(0, heavyHelpItems.Length)];
			int relic = rnd.Next(0, progressionRelics.Length);

			int roll = rnd.Next(1, 3);
			for (int i = 0; i < 11; i++)
			{
				if (!alucardApi.HasRelic((Relic) Enum.Parse(typeof(Relic), progressionRelics[relic])))
				{
					break;
				}
				if (i == 10)
				{
					roll = 1;
					break;
				}
				relic = rnd.Next(0, progressionRelics.Length);
			}
			switch (roll)
			{
				case 1:
					Console.WriteLine(item);
					alucardApi.GrantItemByName(item);
					notificationService.DisplayMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.GrantRelic((Relic) Enum.Parse(typeof(Relic), progressionRelics[relic]));
					notificationService.DisplayMessage($"{user} gave you {(Relic) Enum.Parse(typeof(Relic), progressionRelics[relic])}");
					break;
				default:
					break;
			}
			Alert("Heavy Help");
		}
		public void Vampire(string user = "Khaos")
		{
			alucardApi.DarkMetamorphasisTimer = 0xD;
			alucardApi.ActivatePotion(Potion.AttackPotion);
			notificationService.DisplayMessage($"{user} used Vampire");
			Alert("Vampire");
		}
		public void BattleOrders(string user = "Khaos")
		{
			alucardApi.CurrentHp = alucardApi.MaxtHp * 2;
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			alucardApi.ActivatePotion(Potion.ShieldPotion);
			notificationService.DisplayMessage($"{user} used Battle Orders");
			notificationService.DequeueAction();
			Alert("Battle Orders");
		}
		public void Magician(string user = "Khaos")
		{
			alucardApi.ActivatePotion(Potion.SmartPotion);
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(99);
			manaCheat.Enable();
			magicianTimer.Start();
			notificationService.DisplayMessage($"{user} activated Magician mode");
			notificationService.DequeueAction();
			Alert("Magician");
		}
		public void ZaWarudo(string user = "Khaos")
		{
			alucardApi.ActivateStopwatch();
			alucardApi.Subweapon = Subweapon.Stopwatch;

			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Enable();
			zawarudoTimer.Start();

			notificationService.DisplayMessage($"{user} used ZA WARUDO");
			notificationService.DequeueAction();
			Alert("ZA WARUDO");
		}
		public void MeltyBlood(string user = "Khaos")
		{
			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			width.Enable();
			height.Enable();
			meltyTimer.Start();
			notificationService.DisplayMessage($"{user} activated Melty Blood");
			notificationService.DequeueAction();
			Alert("Melty Blood");
		}
		public void FourBeasts(string user = "Khaos")
		{
			alucardApi.InvincibilityTimer = 0xD;
			alucardApi.AttackPotionTimer = 0xD;
			alucardApi.ShineTimer = 0xD;
			notificationService.DisplayMessage($"{user} used Four Beasts");
			notificationService.DequeueAction();
			Alert("Four Beasts");
		}
		public void Haste(string user = "Khaos")
		{
			SetSpeed(toolConfig.Khaos.HasteFactor);
			hasteTimer.Start();
			notificationService.DisplayMessage($"{user} used Haste");
			notificationService.DequeueAction();
			Alert("Haste");
		}
		#endregion

		public void Update()
		{
			if (bloodManaActive)
			{
				CheckManaUsage();
			}
		}
		public void EnqueueAction(string command)
		{
			if (command is null) throw new ArgumentNullException(nameof(command));
			if (command == "") throw new ArgumentException($"Parameter {nameof(command)} is empty!");

			string user = String.Empty;
			string[] commandArgs = command.Split(' ');
			string action = commandArgs[0];
			if (commandArgs.Length > 1)
			{
				user = commandArgs[1];
			}
			if (user == String.Empty)
			{
				user = "Khaos";
			}

			SotnRandoTools.Configuration.Models.Action commandAction;
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
						queuedActions.Enqueue(new MethodInvoker(() => KhaosEquipment(user)));
						notificationService.AddAction(Enums.ActionType.Khaotic);
					}
					break;
				case "kstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosStats).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => KhaosStats(user)));
						notificationService.AddAction(Enums.ActionType.Khaotic);
					}
					break;
				case "krelics":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosRelics).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => KhaosRelics(user)));
						notificationService.AddAction(Enums.ActionType.Khaotic);
					}
					break;
				case "pandora":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.PandorasBox).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => PandorasBox(user)));
						notificationService.AddAction(Enums.ActionType.Khaotic);
					}
					break;
				case "gamble":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Gamble).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Gamble(user)));
						notificationService.AddAction(Enums.ActionType.Khaotic);
					}
					break;
				#endregion
				#region Debuffs
				case "bankrupt":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Bankrupt).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Bankrupt(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "weaken":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Weaken).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Weaken(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "respawnbosses":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RespawnBosses).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => RespawnBosses(user)));
					}
					break;
				case "honest":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HonestGamer).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => HonestGamer(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "subsonly":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => SubweaponsOnly(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "cripple":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Cripple(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "bloodmana":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => BloodMana(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "thirst":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Thirst(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "horde":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Horde(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
					}
					break;
				case "endurance":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Endurance).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Endurance(user)));
						notificationService.AddAction(Enums.ActionType.Debuff);
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
						queuedActions.Enqueue(new MethodInvoker(() => BattleOrders(user)));
						notificationService.AddAction(Enums.ActionType.Buff);
					}
					break;
				case "magician":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Magician(user)));
						notificationService.AddAction(Enums.ActionType.Buff);
					}
					break;
				case "melty":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => MeltyBlood(user)));
						notificationService.AddAction(Enums.ActionType.Buff);
					}
					break;
				case "fourbeasts":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => FourBeasts(user)));
						notificationService.AddAction(Enums.ActionType.Buff);
					}
					break;
				case "zawarudo":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => ZaWarudo(user)));
						notificationService.AddAction(Enums.ActionType.Buff);
					}
					break;
				case "haste":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Enqueue(new MethodInvoker(() => Haste(user)));
						notificationService.AddAction(Enums.ActionType.Buff);
					}
					break;
				default:
					break;
					#endregion
			}
		}
		private void InitializeTimers()
		{
			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 2 * (1 * 1000);

			honestGamerTimer.Elapsed += HonestGamerOff;
			honestGamerTimer.Interval = 1 * (60 * 1000);
			subweaponsOnlyTimer.Elapsed += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			crippleTimer.Elapsed += CrippleOff;
			crippleTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault().Duration.TotalMilliseconds;
			bloodManaTimer.Elapsed += BloodManaOff;
			bloodManaTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault().Duration.TotalMilliseconds;
			bloodManaTickTimer.Elapsed += BloodManaUpdate;
			bloodManaTickTimer.Interval = 100;
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

			magicianTimer.Elapsed += MagicianOff;
			magicianTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault().Duration.TotalMilliseconds;
			meltyTimer.Elapsed += MeltyBloodOff;
			meltyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault().Duration.TotalMilliseconds;
			zawarudoTimer.Elapsed += ZawarudoOff;
			zawarudoTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault().Duration.TotalMilliseconds;
			hasteTimer.Elapsed += HasteOff;
			hasteTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault().Duration.TotalMilliseconds;
		}
		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 5)
			{
				if (queuedActions.Count > 0)
				{
					queuedActions.Dequeue()();
					if (actionTimer.Interval < toolConfig.Khaos.QueueInterval.TotalMilliseconds)
					{
						actionTimer.Interval = (int)toolConfig.Khaos.QueueInterval.TotalMilliseconds;
					}
				}
				else
				{
					actionTimer.Interval = 2 * (1 * 1000);
				}
			}
		}
		private void ExecuteFastAction(Object sender, EventArgs e)
		{
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 5)
			{
				if (queuedFastActions.Count > 0)
				{
					queuedFastActions.Dequeue()();
				}
			}
		}

		#region Khaotic events
		private void RandomizeGold()
		{
			Random rnd = new Random();
			uint gold = (uint) rnd.Next(0, 5000);
			uint roll = (uint) rnd.Next(0, 21);
			if (roll > 16 && roll < 20)
			{
				gold = gold * (uint) rnd.Next(1, 11);
			}
			else if (roll > 19)
			{
				gold = gold * (uint) rnd.Next(10, 81);
			}
			alucardApi.Gold = gold;
		}
		private void RandomizeStatsActivate()
		{
			Random rnd = new Random();
			uint maxHp = alucardApi.MaxtHp;
			uint currentHp = alucardApi.CurrentHp;
			uint maxMana = alucardApi.MaxtMp;
			uint currentMana = alucardApi.CurrentMp;
			uint str = alucardApi.Str;
			uint con = alucardApi.Con;
			uint intel = alucardApi.Int;
			uint lck = alucardApi.Lck;
			uint statPool = str + con + intel + lck > 28 ? str + con + intel + lck - 28 : str + con + intel + lck;
			uint offset = (uint) (rnd.NextDouble() * statPool);

			int statPoolRoll = rnd.Next(1, 3);
			if (statPoolRoll > 1)
			{
				statPool = statPool + offset;
			}
			else
			{
				statPool = ((int) statPool - (int) offset) < 0 ? 0 : statPool - offset;
			}

			double a = rnd.NextDouble();
			double b = rnd.NextDouble();
			double c = rnd.NextDouble();
			double d = rnd.NextDouble();
			double sum = a + b + c + d;
			double percentageStr = (a / sum);
			double percentageCon = (b / sum);
			double percentageInt = (c / sum);
			double percentageLck = (d / sum);

			alucardApi.Str = (uint) (6 + (statPool * percentageStr));
			alucardApi.Con = (uint) (6 + (statPool * percentageCon));
			alucardApi.Int = (uint) (6 + (statPool * percentageInt));
			alucardApi.Lck = (uint) (6 + (statPool * percentageLck));

			uint pointsPool = maxHp + maxMana > 110 ? maxHp + maxMana - 110 : maxHp + maxMana;
			if (maxHp + maxMana < 110)
			{
				pointsPool = 110;
			}
			offset = (uint) (rnd.NextDouble() * pointsPool);

			int pointsRoll = rnd.Next(1, 3);
			if (pointsRoll > 1)
			{
				pointsPool = pointsPool + offset;
			}
			else
			{
				pointsPool = ((int) pointsPool - (int) offset) < 0 ? 0 : pointsPool - offset;
			}

			double hpPercent = rnd.NextDouble();
			uint pointsHp = 80 + (uint) (hpPercent * pointsPool);
			uint pointsMp = 30 + (uint) (pointsPool - (hpPercent * pointsPool));

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
			Random rnd = new Random();

			int itemCount = rnd.Next(toolConfig.Khaos.PandoraMinItems, toolConfig.Khaos.PandoraMaxItems + 1);

			for (int i = 0; i < itemCount; i++)
			{
				int result = rnd.Next(0, Equipment.Items.Count);
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
			Random rnd = new Random();
			var subweapons = Enum.GetValues(typeof(Subweapon));
			alucardApi.Subweapon = (Subweapon) subweapons.GetValue(rnd.Next(subweapons.Length));
		}
		private void RandomizeRelicsActivate()
		{
			Random rnd = new Random();
			var relics = Enum.GetValues(typeof(Relic));
			foreach (var relic in relics)
			{
				alucardApi.TakeRelic((Relic) relic);
			}
			foreach (var relic in relics)
			{
				if (rnd.Next(0, 2) > 0)
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

			Random rnd = new Random();
			alucardApi.RightHand = (uint) rnd.Next(0, Equipment.HandCount + 1);
			alucardApi.LeftHand = (uint) rnd.Next(0, Equipment.HandCount + 1);
			alucardApi.Armor = (uint) rnd.Next(0, Equipment.ArmorCount + 1);
			alucardApi.Helm = Equipment.HelmStart + (uint) rnd.Next(0, Equipment.HelmCount + 1);
			alucardApi.Cloak = Equipment.CloakStart + (uint) rnd.Next(0, Equipment.CloakCount + 1);
			alucardApi.Accessory1 = Equipment.AccessoryStart + (uint) rnd.Next(0, Equipment.AccessoryCount + 1);
			alucardApi.Accessory2 = Equipment.AccessoryStart + (uint) rnd.Next(0, Equipment.AccessoryCount + 1);
			RandomizeSubweapon();

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
		private void BloodManaUpdate(Object sender, EventArgs e)
		{
			if (spentMana > 1)
			{
				alucardApi.CurrentMp += (uint) spentMana;

				uint currentHp = alucardApi.CurrentHp;
				if (currentHp > spentMana)
				{
					alucardApi.CurrentHp -= (uint) spentMana;
				}
				else
				{
					alucardApi.CurrentHp = 1;
				}
			}
		}
		private void BloodManaOff(Object sender, EventArgs e)
		{
			bloodManaActive = false;
			bloodManaTimer.Stop();
			bloodManaTickTimer.Stop();
		}
		private void ThirstDrain(Object sender, EventArgs e)
		{
			if (alucardApi.CurrentHp > toolConfig.Khaos.ThirstDrainPerSecond + 1)
			{
				alucardApi.CurrentHp -= toolConfig.Khaos.ThirstDrainPerSecond;
			}
			else
			{
				alucardApi.CurrentHp = 1;
			}
		}
		private void ThirstOff(Object sender, EventArgs e)
		{
			thirstTimer.Stop();
			thirstTickTimer.Stop();
		}
		private void HordeOff(Object sender, EventArgs e)
		{
			hordeEnemy = null;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeTimer.Stop();
			hordeSpawnTimer.Stop();
		}
		private void HordeSpawn(Object sender, EventArgs e)
		{
			Random rnd = new Random();
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (hordeZone != zone || hordeZone2 != zone2 || hordeEnemy == null)
			{
				long bannedEnemy = actorApi.FindEnemy(gameApi.SecondCastle ? 101 : 31, 10000);
				if (bannedEnemy > 0)
				{
					bannedEnemies.Add(new Actor(actorApi.GetActor(bannedEnemy)));
				}
				long enemy = actorApi.FindEnemy(1, gameApi.SecondCastle ? 100 : 30, new int[] {26, 16, 18, 22});
				if (enemy > 0)
				{
					hordeEnemy = new Actor(actorApi.GetActor(enemy));
					hordeEnemy.Palette = (ushort)(hordeEnemy.Palette + rnd.Next(1, 10));
					foreach (Actor bannedEnemyActor in bannedEnemies)
					{
						if (hordeEnemy.Damage == bannedEnemyActor.Damage && hordeEnemy.HitboxHeight == bannedEnemyActor.HitboxHeight)
						{
							hordeEnemy = null;
						}
					}
				}
				else
				{
					hordeEnemy = null;
				}
				hordeZone = zone;
				hordeZone2 = zone2;
			}
			else if (hordeEnemy is not null)
			{
				if (hordeTimer.Interval == 5 * (60 * 1000))
				{
					hordeTimer.Stop();
					hordeTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Duration.TotalMilliseconds;
					hordeTimer.Start();
				}
				hordeEnemy.Xpos = (ushort) rnd.Next(10, 245);
				hordeEnemy.Ypos = (ushort) rnd.Next(10, 245);
				actorApi.SpawnActor(hordeEnemy);
			}
		}
		private void HonestGamerOff(Object sender, EventArgs e)
		{
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			honestGamerTimer.Stop();
		}
		private void SubweaponsOnlyOff(object sender, EventArgs e)
		{
			Cheat curse = cheats.GetCheatByName("CurseTimer");
			curse.Disable();
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			subweaponsOnlyTimer.Stop();
		}
		private void CrippleOff(Object sender, EventArgs e)
		{
			SetSpeed();
			crippleTimer.Stop();
		}
		private void EnduranceSpawn(Object sender, EventArgs e)
		{
			Random rnd = new Random();
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? boss = null;

			long enemy = actorApi.FindEnemy(gameApi.SecondCastle ? 886 : 199, 2000);
			if (enemy > 0)
			{
				boss = new Actor(actorApi.GetActor(enemy));
				boss.Xpos = (ushort) rnd.Next(70, 170);
				boss.Palette = (ushort) (boss.Palette + rnd.Next(1, 10));
				actorApi.SpawnActor(boss);
				enduranceSpawnTimer.Stop();
			}
			else
			{
				return;
			}
		}
		private void SpawnPoisonHitbox()
		{
			Actor poison = new Actor();
			poison.HitboxHeight = 255;
			poison.HitboxWidth = 255;
			poison.AutoToggle = 1;
			poison.Damage = 10;
			poison.DamageTypeA = (uint) Actors.Poison;
			actorApi.SpawnActor(poison);
		}
		private void SpawnCurseHitbox()
		{
			Actor poison = new Actor();
			poison.HitboxHeight = 255;
			poison.HitboxWidth = 255;
			poison.AutoToggle = 1;
			poison.Damage = 10;
			poison.DamageTypeB = (uint) Actors.Curse;
			actorApi.SpawnActor(poison);
		}
		private void SpawnStoneHitbox()
		{
			Actor poison = new Actor();
			poison.HitboxHeight = 255;
			poison.HitboxWidth = 255;
			poison.AutoToggle = 1;
			poison.Damage = 10;
			poison.DamageTypeA = (uint) Actors.Stone;
			poison.DamageTypeB = (uint) Actors.Stone;
			actorApi.SpawnActor(poison);
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
		private void MagicianOff(Object sender, EventArgs e)
		{
			//int potion
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			magicianTimer.Stop();
		}
		private void MeltyBloodOff(Object sender, EventArgs e)
		{
			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			width.Disable();
			height.Disable();
			meltyTimer.Stop();
		}
		private void ZawarudoOff(Object sender, EventArgs e)
		{
			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Disable();
			zawarudoTimer.Stop();
		}
		private void HasteOff(Object sender, EventArgs e)
		{
			SetSpeed();
			hasteTimer.Stop();
		}
		#endregion

		private void SetSpeed(float factor = 1)
		{
			bool slow = factor < 1;
			bool fast = factor > 1;
			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * factor);
			alucardApi.WalkingWholeSpeed = fast == true ? (uint) (DefaultSpeeds.WalkingWhole * (factor * 2)) : (uint) (DefaultSpeeds.WalkingWhole * factor);
			alucardApi.WalkingFractSpeed = (uint) (DefaultSpeeds.WalkingFract * factor);
			alucardApi.JumpingHorizontalWholeSpeed = fast == true ? (uint) (DefaultSpeeds.WalkingWhole * (factor * 2)) : (uint) (DefaultSpeeds.WalkingWhole * factor);
			alucardApi.JumpingHorizontalFractSpeed = (uint) (DefaultSpeeds.WalkingFract * factor);
			alucardApi.FallingHorizontalWholeSpeed = fast == true ? (uint) (DefaultSpeeds.WalkingWhole * (factor * 2)) : (uint) (DefaultSpeeds.WalkingWhole * factor);
			alucardApi.FallingHorizontalFractSpeed = (uint) (DefaultSpeeds.WalkingFract * factor);
			alucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * factor);
			alucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling(DefaultSpeeds.WolfDashTopLeft * factor);
			alucardApi.BackdashDecel = slow == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
		}
		private void CheckManaUsage()
		{
			uint currentMana = alucardApi.CurrentMp;
			spentMana = (int) storedMana - (int) currentMana;
			storedMana = currentMana;
		}
		private void CheckExperience()
		{
			uint currentExperiecne = alucardApi.Experiecne;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
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
		private void OnBotFileChanged(object sender, FileSystemEventArgs e)
		{
			string lastLine = FileExtensions.GetLastLine(toolConfig.Khaos.BotActionsFilePath);
			EnqueueAction(lastLine);
		}
	}
}

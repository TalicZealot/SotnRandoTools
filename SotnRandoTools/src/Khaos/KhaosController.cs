using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using BizHawk.Client.Common;
using SotnApi.Constants.Addresses;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Utils;

namespace SotnRandoTools.Khaos
{
	public class KhaosController
	{
		private const float WeakenFactor = 0.5F;
		private const float CrippleFactor = 0.8F;
		private const int DrainPerSecond = 1;
		private const int PandoraMinItems = 3;
		private const int PandoraMaxItems = 32;

		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IActorApi actorApi;
		private readonly ICheatCollectionAdapter cheats;
		//replace with WMPLib.WindowsMediaPlayer
		private MediaPlayer audioPlayer = new MediaPlayer();


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
			"Fist of Tuklas",
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
			"JewelOfOpen"
		};
		private string[] subscribers =
		{
		};

		private Queue<MethodInvoker> queuedActions = new Queue<MethodInvoker>();
		private Queue<MethodInvoker> queuedFastActions = new Queue<MethodInvoker>();
		private Timer actionTimer = new Timer();
		private Timer fastActionTimer = new Timer();

		private Timer zawarudoTimer = new Timer();
		private Timer honestGamerTimer = new Timer();
		private Timer subweaponsOnlyTimer = new Timer();
		private Timer magicianTimer = new Timer();
		private Timer meltyTimer = new Timer();
		private Timer crippleTimer = new Timer();
		private Timer bloodManaTimer = new Timer();
		private Timer bloodManaTickTimer = new Timer();
		private Timer thirstTimer = new Timer();
		private Timer thirstTickTimer = new Timer();
		private Timer hordeTimer = new Timer();
		private Timer hordeSpawnTimer = new Timer();

		private uint hordeZone = 0;
		private uint hordeZone2 = 0;
		private Actor? hordeEnemy = null;
		private uint storedMana = 0;
		private int spentMana = 0;
		private bool bloodManaActive = false;

		private FileSystemWatcher botFileWatcher = new FileSystemWatcher();

		public KhaosController(IToolConfig toolConfig, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, ICheatCollectionAdapter cheats)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (actorApi is null) throw new ArgumentNullException(nameof(actorApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			this.toolConfig = toolConfig;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.actorApi = actorApi;
			this.cheats = cheats;

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

			audioPlayer.Volume = (float) toolConfig.Khaos.Volume / 10F;

			InitializeTimers();
		}

		public void StartKhaos()
		{
			if (File.Exists(toolConfig.Khaos.BotActionsFilePath))
			{
				botFileWatcher.EnableRaisingEvents = true;
			}
			actionTimer.Start();
			fastActionTimer.Start();
			//OverwriteBossNames(subscribers);
			Cheat faerieScroll = cheats.GetCheatByName("FaerieScroll");
			faerieScroll.Enable();
			gameApi.OverwriteString(Strings.FleaMan, "Kappa");
			gameApi.OverwriteString(Strings.Shaft, "Talic");
			gameApi.OverwriteString(Strings.Dracula, "3snoW");
		}

		public void OverwriteBossNames(string[] subscribers)
		{
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

		public void RandomizeRelics()
		{
			RandomizeRelicsActivate();
			audioPlayer.Open(new Uri(Paths.AlertAlucardWhat, UriKind.Relative));
			audioPlayer.Play();
		}

		public void RandomizeEquipment()
		{
			RandomizeEquipmentSlots();

			audioPlayer.Open(new Uri(Paths.AlertAlucardWhat, UriKind.Relative));
			audioPlayer.Play();
		}

		public void RandomizeStats()
		{
			RandomizeStatsActivate();
			audioPlayer.Open(new Uri(Paths.AlertAlucardWhat, UriKind.Relative));
			audioPlayer.Play();
		}

		public void RandomLightHelp()
		{
			Random rnd = new Random();
			string item = lightHelpItems[rnd.Next(0, lightHelpItems.Length)];

			int roll = rnd.Next(1, 4);
			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Potion);
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.ShieldPotion);
					break;
				default:
					break;
			}
			audioPlayer.Open(new Uri(Paths.AlertFairyPotion, UriKind.Relative));
			audioPlayer.Play();
		}

		public void RandomMediumHelp()
		{
			Random rnd = new Random();
			string item = mediumHelpItems[rnd.Next(0, mediumHelpItems.Length)];

			int roll = rnd.Next(1, 4);
			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Elexir);
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.ManaPrism);
					break;
				default:
					break;
			}
			audioPlayer.Open(new Uri(Paths.AlertFairyPotion, UriKind.Relative));
			audioPlayer.Play();
		}

		public void RandomHeavytHelp()
		{
			Random rnd = new Random();
			string item = heavyHelpItems[rnd.Next(0, heavyHelpItems.Length)];
			int relic = rnd.Next(0, progressionRelics.Length);

			int roll = rnd.Next(1, 3);
			for (int i = 0; i < 11; i++)
			{
				if (!alucardApi.HasRelic((Relic) relic))
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
					alucardApi.GrantItemByName(item);
					break;
				case 2:
					alucardApi.GrantRelic((Relic) relic);
					break;
				default:
					break;
			}
			audioPlayer.Open(new Uri(Paths.AlertFairyPotion, UriKind.Relative));
			audioPlayer.Play();
		}

		public void PandorasBox()
		{
			RandomizeGold();
			RandomizeStatsActivate();
			RandomizeEquipmentSlots();
			RandomizeRelicsActivate();
			RandomizeInventory();
			RandomizeSubweapon();
			gameApi.RespawnBosses();
			audioPlayer.Open(new Uri(Paths.AlertAlucardWhat, UriKind.Relative));
			audioPlayer.Play();
		}

		public void InflictRandomStatus()
		{
			Random rnd = new Random();
			int result = rnd.Next(1, 4);
			switch (result)
			{
				case 1:
					SpawnPoisonHitbox();
					break;
				case 2:
					SpawnCurseHitbox();
					break;
				case 3:
					SpawnStoneHitbox();
					break;
				default:
					break;
			}
			audioPlayer.Open(new Uri(Paths.AlertDeathLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void Vampire()
		{
			alucardApi.DarkMetamorphasisTimer = 0xD;
		}

		public void Weaken()
		{
			alucardApi.CurrentHp = (uint) (alucardApi.CurrentHp * WeakenFactor);
			alucardApi.CurrentMp = (uint) (alucardApi.CurrentHp * WeakenFactor);
			alucardApi.CurrentHearts = (uint) (alucardApi.CurrentHp * WeakenFactor);
			alucardApi.MaxtHp = (uint) (alucardApi.MaxtHp * WeakenFactor);
			alucardApi.MaxtMp = (uint) (alucardApi.MaxtHp * WeakenFactor);
			alucardApi.MaxtHearts = (uint) (alucardApi.MaxtHp * WeakenFactor);
			alucardApi.Str = (uint) (alucardApi.Str * WeakenFactor);
			alucardApi.Con = (uint) (alucardApi.Con * WeakenFactor);
			alucardApi.Int = (uint) (alucardApi.Int * WeakenFactor);
			alucardApi.Lck = (uint) (alucardApi.Lck * WeakenFactor);

			audioPlayer.Open(new Uri(Paths.AlertRichterLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void Cripple()
		{
			CrippleToggle(true);
			crippleTimer.Start();
			audioPlayer.Open(new Uri(Paths.AlertAlucardWhat, UriKind.Relative));
			audioPlayer.Play();
		}

		public void BloodMana()
		{
			storedMana = alucardApi.CurrentMp;
			bloodManaActive = true;
			bloodManaTimer.Start();
			bloodManaTickTimer.Start();
			audioPlayer.Open(new Uri(Paths.AlertRichterLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void SubweaponsOnly()
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
			audioPlayer.Open(new Uri(Paths.AlertRichterLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void Bankrupt()
		{
			BankruptActivate();

			audioPlayer.Open(new Uri(Paths.AlertDeathLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void RespawnBosses()
		{
			gameApi.RespawnBosses();
			audioPlayer.Open(new Uri(Paths.AlertDeathLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void Gamble()
		{
			Random rnd = new Random();
			double goldPercent = rnd.NextDouble();
			alucardApi.Gold = (uint) ((double) alucardApi.Gold * goldPercent);
			string item = Equipment.Items[rnd.Next(1, Equipment.Items.Count)];
			while (item.Contains("empty hand") || item.Contains("-"))
			{
				item = Equipment.Items[rnd.Next(1, Equipment.Items.Count)];
			}
			alucardApi.GrantItemByName(item);

			audioPlayer.Open(new Uri(Paths.AlertLibrarianThankYou, UriKind.Relative));
			audioPlayer.Play();
		}

		public void BattleOrders()
		{
			alucardApi.CurrentHp = alucardApi.MaxtHp * 2;
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			alucardApi.ActivatePotion(Potion.ShieldPotion);
		}

		public void HonestGamer()
		{
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(5);
			manaCheat.Enable();
			honestGamerTimer.Start();
			audioPlayer.Open(new Uri(Paths.AlertRichterLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void Magician()
		{
			alucardApi.ActivatePotion(Potion.SmartPotion);
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(99);
			manaCheat.Enable();
			magicianTimer.Start();
		}

		public void ZaWarudo()
		{
			alucardApi.ActivateStopwatch();
			alucardApi.Subweapon = Subweapon.Stopwatch;

			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Enable();
			zawarudoTimer.Start();

			audioPlayer.Open(new Uri(Paths.AlertZaWarudo, UriKind.Relative));
			audioPlayer.Play();
		}

		public void MeltyBlood()
		{
			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			width.Enable();
			height.Enable();
			meltyTimer.Start();
			audioPlayer.Open(new Uri(Paths.AlertMelty, UriKind.Relative));
			audioPlayer.Play();
		}

		public void FourBeasts()
		{
			alucardApi.InvincibilityTimer = 0xD;
			alucardApi.AttackPotionTimer = 0xD;
			alucardApi.ShineTimer = 0xD;
		}

		public void Thirst()
		{
			alucardApi.DarkMetamorphasisTimer = 40;
			thirstTimer.Start();
			thirstTickTimer.Start();
			audioPlayer.Open(new Uri(Paths.AlertDeathLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

		public void Horde()
		{
			hordeTimer.Start();
			hordeSpawnTimer.Start();
			audioPlayer.Open(new Uri(Paths.AlertRichterLaugh, UriKind.Relative));
			audioPlayer.Play();
		}

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
			string user = "";

			string[] commandArgs = command.Split(' ');
			Console.WriteLine(String.Join(" + ", commandArgs));
			string action = commandArgs[0];
			if (commandArgs.Length > 1)
			{
				user = commandArgs[1];
			}

			switch (action)
			{
				case "kstatus":
					queuedFastActions.Enqueue(InflictRandomStatus);
					break;
				case "kequipment":
					queuedFastActions.Enqueue(RandomizeEquipment);
					break;
				case "kstats":
					queuedFastActions.Enqueue(RandomizeStats);
					break;
				case "krelics":
					queuedFastActions.Enqueue(RandomizeRelics);
					break;
				case "pandora":
					queuedActions.Enqueue(PandorasBox);
					break;
				case "gamble":
					queuedFastActions.Enqueue(Gamble);
					break;
				case "bankrupt":
					queuedFastActions.Enqueue(Bankrupt);
					break;
				case "weaken":
					queuedActions.Enqueue(Weaken);
					break;
				case "respawnbosses":
					queuedActions.Enqueue(RespawnBosses);
					break;
				case "honest":
					queuedFastActions.Enqueue(HonestGamer);
					break;
				case "subsonly":
					queuedActions.Enqueue(SubweaponsOnly);
					break;
				case "cripple":
					queuedFastActions.Enqueue(Cripple);
					break;
				case "bloodmana":
					queuedFastActions.Enqueue(BloodMana);
					break;
				case "thirst":
					queuedFastActions.Enqueue(Thirst);
					break;
				case "horde":
					queuedFastActions.Enqueue(Horde);
					break;
				case "vampire":
					queuedFastActions.Enqueue(Vampire);
					break;
				case "lighthelp":
					queuedFastActions.Enqueue(RandomLightHelp);
					break;
				case "mediumhelp":
					queuedFastActions.Enqueue(RandomMediumHelp);
					break;
				case "heavyhelp":
					queuedFastActions.Enqueue(RandomHeavytHelp);
					break;
				case "battleorders":
					queuedFastActions.Enqueue(BattleOrders);
					break;
				case "magician":
					queuedFastActions.Enqueue(Magician);
					break;
				case "melty":
					queuedFastActions.Enqueue(MeltyBlood);
					break;
				case "fourbeasts":
					queuedFastActions.Enqueue(FourBeasts);
					break;
				case "zawarudo":
					queuedFastActions.Enqueue(ZaWarudo);
					break;
				default:
					break;
			}
		}

		private void InitializeTimers()
		{
			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 1 * (60 * 1000);
			honestGamerTimer.Tick += HonestGamerOff;
			honestGamerTimer.Interval = 1 * (60 * 1000);
			subweaponsOnlyTimer.Tick += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = 1 * (60 * 1000);
			magicianTimer.Tick += MagicianOff;
			magicianTimer.Interval = 1 * (60 * 1000);
			meltyTimer.Tick += MeltyBloodOff;
			meltyTimer.Interval = 1 * (60 * 1000);
			crippleTimer.Tick += CrippleOff;
			crippleTimer.Interval = 1 * (60 * 1000);
			bloodManaTimer.Tick += BloodManaOff;
			bloodManaTimer.Interval = 1 * (60 * 1000);
			bloodManaTickTimer.Tick += BloodManaUpdate;
			bloodManaTickTimer.Interval = 100;
			thirstTimer.Tick += ThirstOff;
			thirstTimer.Interval = 2 * (60 * 1000);
			thirstTickTimer.Tick += new EventHandler(ThirstDrain);
			thirstTickTimer.Interval = 1000;
			hordeTimer.Tick += new EventHandler(HordeOff);
			hordeTimer.Interval = 1 * (60 * 1000);
			hordeSpawnTimer.Tick += new EventHandler(HordeSpawn);
			hordeSpawnTimer.Interval = 1 * (1000);
			zawarudoTimer.Tick += new EventHandler(ZawarudoOff);
			zawarudoTimer.Interval = 1 * (40 * 1000);
		}

		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 5)
			{
				if (queuedActions.Count > 0)
				{
					queuedActions.Dequeue()();
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

		private void CrippleOff(Object sender, EventArgs e)
		{
			CrippleToggle(false);
			crippleTimer.Stop();
		}

		private void BloodManaUpdate(Object sender, EventArgs e)
		{
			if (spentMana > 1)
			{
				alucardApi.CurrentMp += (uint) spentMana;
				alucardApi.CurrentHp -= (uint) spentMana;
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
			if (alucardApi.CurrentHp > 1)
			{
				alucardApi.CurrentHp -= DrainPerSecond;
			}
		}

		private void ThirstOff(Object sender, EventArgs e)
		{
			thirstTimer.Stop();
			thirstTickTimer.Stop();
		}

		private void HordeOff(Object sender, EventArgs e)
		{
			hordeTimer.Stop();
			hordeSpawnTimer.Stop();
		}

		private void ZawarudoOff(Object sender, EventArgs e)
		{
			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Disable();
			zawarudoTimer.Stop();
		}

		private void HordeSpawn(Object sender, EventArgs e)
		{
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (hordeZone != zone || hordeZone2 != zone2 || hordeEnemy == null)
			{
				long enemy = actorApi.FindEnemy(0, gameApi.SecondCastle ? 100 : 30);
				if (enemy > 0)
				{
					hordeEnemy = new Actor(actorApi.GetActor(enemy));
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
				Random rnd = new Random();
				hordeEnemy.Xpos = (ushort) rnd.Next(10, 245);
				hordeEnemy.Ypos = (ushort) rnd.Next(10, 245);
				actorApi.SpawnActor(hordeEnemy);
			}
		}

		private void EnduranceSpawn(Object sender, EventArgs e)
		{
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? boss = null;

			long enemy = actorApi.FindEnemy(gameApi.SecondCastle ? 886 : 199, 2000);
			if (enemy > 0)
			{
				boss = new Actor(actorApi.GetActor(enemy));
				Random rnd = new Random();
				boss.Xpos = (ushort) rnd.Next(70, 170);
				actorApi.SpawnActor(boss);
				//stop timer
			}
			else
			{
				return;
			}
		}

		private void OnBotFileChanged(object sender, FileSystemEventArgs e)
		{
			string lastLine = FileExtensions.GetLastLine(toolConfig.Khaos.BotActionsFilePath);
			EnqueueAction(lastLine);
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

		private void CrippleToggle(bool on)
		{
			float factor = on == true ? CrippleFactor : 1;

			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * factor);
			alucardApi.WalkingWholeSpeed = (uint) (DefaultSpeeds.WalkingWhole * factor);
			alucardApi.WalkingFractSpeed = (uint) (DefaultSpeeds.WalkingFract * factor);
			alucardApi.JumpingHorizontalWholeSpeed = (uint) (DefaultSpeeds.WalkingWhole * factor);
			alucardApi.JumpingHorizontalFractSpeed = (uint) (DefaultSpeeds.WalkingFract * factor);
			alucardApi.FallingHorizontalWholeSpeed = (uint) (DefaultSpeeds.WalkingWhole * factor);
			alucardApi.FallingHorizontalFractSpeed = (uint) (DefaultSpeeds.WalkingFract * factor);
			alucardApi.WolfDashTopRightSpeed = (sbyte) ((sbyte) DefaultSpeeds.WolfDashTopRight - 1);
			alucardApi.WolfDashTopLeftSpeed = (sbyte) ((sbyte) DefaultSpeeds.WolfDashTopLeft - 1);
			uint adjustedBackdashDecel = on == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
			alucardApi.BackdashDecel = adjustedBackdashDecel;
		}

		private void BankruptActivate()
		{
			alucardApi.Gold = 0;
			alucardApi.ClearInventory();
			alucardApi.RightHand = 0;
			alucardApi.LeftHand = 0;
			alucardApi.Helm = Equipment.HelmStart;
			alucardApi.Armor = 0;
			alucardApi.Cloak = Equipment.CloakStart;
			alucardApi.Accessory1 = Equipment.AccessoryStart;
			alucardApi.Accessory2 = Equipment.AccessoryStart;
		}

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

			int itemCount = rnd.Next(PandoraMinItems, PandoraMaxItems + 1);

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
			bool equippedHolyGlasses = Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (alucardApi.Armor + Equipment.HandCount)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount)] == "Gold Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount)] == "Silver Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount)] == "Silver Ring";

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

		private void CheckManaUsage()
		{
			uint currentMana = alucardApi.CurrentMp;
			spentMana = (int) storedMana - (int) currentMana;
			storedMana = currentMana;
		}
	}
}

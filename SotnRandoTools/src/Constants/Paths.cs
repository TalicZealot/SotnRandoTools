using System.Collections.Generic;

namespace SotnRandoTools.Constants
{
	public static class Paths
	{
		public const string ItemPickupSound = "./ExternalTools/SotnRandoTools/Sounds/Item.mp3";
		public const string AlucardWhatSound = "./ExternalTools/SotnRandoTools/Sounds/AlucardWhat.mp3";
		public const string LibrarianThankYouSound = "./ExternalTools/SotnRandoTools/Sounds/LibrarianThankYou.mp3";
		public const string DeathLaughSound = "./ExternalTools/SotnRandoTools/Sounds/DeathLaugh.mp3";
		public const string RichterLaughSound = "./ExternalTools/SotnRandoTools/Sounds/RichterLaugh.mp3";
		public const string FairyPotionSound = "./ExternalTools/SotnRandoTools/Sounds/FairyPotion.mp3";
		public const string MeltySound = "./ExternalTools/SotnRandoTools/Sounds/Melty.mp3";
		public const string DragonInstallSound = "./ExternalTools/SotnRandoTools/Sounds/DragonInstall.mp3";
		public const string ZaWarudoSound = "./ExternalTools/SotnRandoTools/Sounds/ZaWarudo.mp3";
		public const string DeathLaughAlternateSound = "./ExternalTools/SotnRandoTools/Sounds/DeathLaughAlternate.mp3";
		public const string DieSound = "./ExternalTools/SotnRandoTools/Sounds/Die.mp3";
		public const string DracLaughSound = "./ExternalTools/SotnRandoTools/Sounds/DracLaugh.mp3";
		public const string HohoSound = "./ExternalTools/SotnRandoTools/Sounds/Hoho.mp3";
		public const string SlowWhatSound = "./ExternalTools/SotnRandoTools/Sounds/SlowWhat.mp3";
		public const string SwordBroSound = "./ExternalTools/SotnRandoTools/Sounds/SwordBro.mp3";
		public const string AlreadyDeadSound = "./ExternalTools/SotnRandoTools/Sounds/AlreadyDead.mp3";
		public const string BattleOrdersSound = "./ExternalTools/SotnRandoTools/Sounds/BattleOrders.mp3";
		public const string QuadSound = "./ExternalTools/SotnRandoTools/Sounds/Quad.mp3";
		public const string ExcellentSound = "./ExternalTools/SotnRandoTools/Sounds/Excellent.mp3";

		public const string NamesFilePath = "./ExternalTools/SotnRandoTools/Khaos/names.txt";

		public const string SourceLink = "https://github.com/TalicZealot/SotnRandoTools/";
		public const string ReadmeLink = "https://github.com/TalicZealot/SotnRandoTools/blob/main/README.md";
		public const string ApiLink = "https://github.com/TalicZealot/SotnApi";
		public const string UpdaterLink = "https://github.com/TalicZealot/SimpleLatestReleaseUpdater";
		public const string RandoSourceLink = "https://github.com/3snowp7im/SotN-Randomizer";
		public const string DonateLink = "https://www.paypal.com/donate?hosted_button_id=5F8565K23F2F8";

		public const string LatestReleaseApi = "https://api.github.com/repos/taliczealot/sotnrandotools/releases";
		public const string LatestReleaseUrl = "https://github.com/TalicZealot/SotnRandoTools/releases/latest";
		public const string UpdaterPath = @"\ExternalTools\SotnRandoTools\Updater\SimpleLatestReleaseUpdater.exe";
		public const string UpdaterFolderPath = @"\ExternalTools\SotnRandoTools\Updater\";

		public const string RelicWatchesPath = "./ExternalTools/SotnRandoTools/Watches/Relics.wch";
		public const string SafeLocationWatchesPath = "./ExternalTools/SotnRandoTools/Watches/SafeLocations.wch";
		public const string EquipmentLocationWatchesPath = "./ExternalTools/SotnRandoTools/Watches/EquipmentLocations.wch";
		public const string ProgressionItemWatchesPath = "./ExternalTools/SotnRandoTools/Watches/ProgressionItems.wch";
		public const string ThrustSwordWatchesPath = "./ExternalTools/SotnRandoTools/Watches/ThrustSwords.wch";
		public const string WarpsAndShortcutsWatchPath = "./ExternalTools/SotnRandoTools/Watches/WarpsAndShortcuts.wch";

		public const string ImagesPath = "./ExternalTools/SotnRandoTools/Images/";
		public const string TextboxImage = "./ExternalTools/SotnRandoTools/Images/SotnTextBox.png";
		public const string IconVermillionBird = "./ExternalTools/SotnRandoTools/Images/VermillionBird.png";
		public const string IconWhiteTiger = "./ExternalTools/SotnRandoTools/Images/WhiteTiger.png";
		public const string IconAzureDragon = "./ExternalTools/SotnRandoTools/Images/AzureDragon.png";
		public const string IconBlackTortoise = "./ExternalTools/SotnRandoTools/Images/BlackTortoise.png";
		public static readonly Dictionary<string, string> RelicImages = new Dictionary<string, string>
		{
			{"SoulOfBat", "./ExternalTools/SotnRandoTools/Images/SoulOfBat.png"},
			{"FireOfBat", "./ExternalTools/SotnRandoTools/Images/FireOfBat.png"},
			{"EchoOfBat", "./ExternalTools/SotnRandoTools/Images/EchoOfBat.png"},
			{"ForceOfEcho", "./ExternalTools/SotnRandoTools/Images/ForceOfEcho.png"},
			{"SoulOfWolf", "./ExternalTools/SotnRandoTools/Images/SoulOfWolf.png"},
			{"PowerOfWolf", "./ExternalTools/SotnRandoTools/Images/PowerOfWolf.png"},
			{"SkillOfWolf", "./ExternalTools/SotnRandoTools/Images/SkillOfWolf.png"},
			{"FormOfMist", "./ExternalTools/SotnRandoTools/Images/FormOfMist.png"},
			{"PowerOfMist", "./ExternalTools/SotnRandoTools/Images/PowerOfMist.png"},
			{"GasCloud", "./ExternalTools/SotnRandoTools/Images/GasCloud.png"},
			{"CubeOfZoe", "./ExternalTools/SotnRandoTools/Images/CubeOfZoe.png"},
			{"SpiritOrb", "./ExternalTools/SotnRandoTools/Images/SpiritOrb.png"},
			{"GravityBoots", "./ExternalTools/SotnRandoTools/Images/GravityBoots.png"},
			{"LeapStone", "./ExternalTools/SotnRandoTools/Images/LeapStone.png"},
			{"HolySymbol", "./ExternalTools/SotnRandoTools/Images/HolySymbol.png"},
			{"FaerieScroll", "./ExternalTools/SotnRandoTools/Images/FaerieScroll.png"},
			{"JewelOfOpen", "./ExternalTools/SotnRandoTools/Images/JewelOfOpen.png"},
			{"MermanStatue", "./ExternalTools/SotnRandoTools/Images/MermanStatue.png"},
			{"BatCard", "./ExternalTools/SotnRandoTools/Images/BatCard.png"},
			{"GhostCard", "./ExternalTools/SotnRandoTools/Images/GhostCard.png"},
			{"FaerieCard", "./ExternalTools/SotnRandoTools/Images/FaerieCard.png"},
			{"DemonCard", "./ExternalTools/SotnRandoTools/Images/DemonCard.png"},
			{"SwordCard", "./ExternalTools/SotnRandoTools/Images/SwordCard.png"},
			{"SpriteCard" , "./ExternalTools/SotnRandoTools/Images/SpriteCard.png"},
			{"NoseDevilCard", "./ExternalTools/SotnRandoTools/Images/NoseDevilCard.png"},
			{"HeartOfVlad", "./ExternalTools/SotnRandoTools/Images/HeartOfVlad.png"},
			{"ToothOfVlad", "./ExternalTools/SotnRandoTools/Images/ToothOfVlad.png"},
			{"RibOfVlad", "./ExternalTools/SotnRandoTools/Images/RibOfVlad.png"},
			{"RingOfVlad", "./ExternalTools/SotnRandoTools/Images/RingOfVlad.png"},
			{"EyeOfVlad", "./ExternalTools/SotnRandoTools/Images/EyeOfVlad.png"}
		};

		public const string LogsPath = "./ExternalTools/SotnRandoTools/Logs/";
		public const string ReplaysPath = "./ExternalTools/SotnRandoTools/Replays/";
		public const string ChangeLogPath = @"\ExternalTools\SotnRandoTools\ChangeLog.txt";

		public const string CasualPresetPath = "./ExternalTools/SotnRandoTools/Presets/casual.json";
		public const string SafePresetPath = "./ExternalTools/SotnRandoTools/Presets/safe.json";
		public const string SpeedrunPresetPath = "./ExternalTools/SotnRandoTools/Presets/speedrun.json";
		public const string BatMasterPresetPath = "./ExternalTools/SotnRandoTools/Presets/bat-master.json";

		public const string ConfigPath = "./ExternalTools/SotnRandoTools/ToolConfig.ini";
		public const string SeedInfoPath = "./ExternalTools/SotnRandoTools/TrackerOverlay/SeedInfo.txt";
		public const string CheatsPath = "./ExternalTools/SotnRandoTools/Cheats/Cheats.cht";
		public const string CheatsBackupPath = "./ExternalTools/SotnRandoTools/Cheats/Cheats.cht.bkp";

		public const string KhaosDatabase = "./ExternalTools/SotnRandoTools/Khaos/Khaos.db";
		public const string TwitchRedirectUri = "http://localhost:8080/redirect/";
	}
}

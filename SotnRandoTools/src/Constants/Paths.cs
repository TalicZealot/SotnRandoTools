using System.Collections.Generic;

namespace SotnRandoTools.Constants
{
	public static class Paths
	{
		public const string ItemPickupSound = "./ExternalTools/SotnRandoTools/Sounds/Item.mp3";

		public const string SourceLink = "https://github.com/TalicZealot/SotnRandoTools/";
		public const string ReadmeLink = "https://github.com/TalicZealot/SotnRandoTools/blob/main/README.md";
		public const string ApiLink = "https://github.com/TalicZealot/SotnApi";
		public const string UpdaterLink = "https://github.com/TalicZealot/SimpleLatestReleaseUpdater";
		public const string RandoSourceLink = "https://github.com/3snowp7im/SotN-Randomizer";

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
		public const string SpeedrunPresetPath = "./ExternalTools/SotnRandoTools/Presets/speedrun.json";
		public const string BatMasterPresetPath = "./ExternalTools/SotnRandoTools/Presets/bat-master.json";
		public const string OpenCasualPresetPath = "./ExternalTools/SotnRandoTools/Presets/open-casual.json";
		public const string OpenSpeedrunPresetPath = "./ExternalTools/SotnRandoTools/Presets/open-speedrun.json";

		public const string ConfigPath = "./ExternalTools/SotnRandoTools/ToolConfig.ini";
		public const string SeedInfoPath = "./ExternalTools/SotnRandoTools/TrackerOverlay/SeedInfo.txt";
	}
}

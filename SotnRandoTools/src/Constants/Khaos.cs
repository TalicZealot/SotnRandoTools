using System.Collections.Generic;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Models;
using MapLocation = SotnRandoTools.RandoTracker.Models.MapLocation;

namespace SotnRandoTools.Constants
{
	public static class Khaos
	{
		public static Relic[] ProgressionRelics =
		{
			Relic.SoulOfBat,
			Relic.SoulOfWolf,
			Relic.FormOfMist,
			Relic.GravityBoots,
			Relic.LeapStone,
			Relic.JewelOfOpen,
			Relic.MermanStatue
		};
		public static List<Relic[]> FlightRelics = new List<Relic[]>{
			new Relic[] {Relic.SoulOfBat},
			new Relic[] {Relic.LeapStone, Relic.GravityBoots},
			new Relic[] {Relic.FormOfMist, Relic.PowerOfMist},
			new Relic[] {Relic.SoulOfWolf, Relic.GravityBoots},
		};

		public static List<MapLocation> LoadingRooms = new List<MapLocation>
		{
			new MapLocation{X =17,Y = 36, SecondCastle = 0},
			new MapLocation{X =21,Y = 26, SecondCastle = 0},
			new MapLocation{X =20,Y = 36, SecondCastle = 0},
			new MapLocation{X =30,Y = 25, SecondCastle = 0},
			new MapLocation{X =26,Y = 22, SecondCastle = 0},
			new MapLocation{X =13,Y = 22, SecondCastle = 0},
			new MapLocation{X = 4,Y = 28, SecondCastle = 0},
			new MapLocation{X =36,Y = 21, SecondCastle = 0},
			new MapLocation{X =17,Y = 19, SecondCastle = 0},
			new MapLocation{X =29,Y = 12, SecondCastle = 0},
			new MapLocation{X =39,Y = 12, SecondCastle = 0},
			new MapLocation{X =39,Y = 10, SecondCastle = 0},
			new MapLocation{X =60,Y = 14, SecondCastle = 0},
			new MapLocation{X =60,Y = 17, SecondCastle = 0},
			new MapLocation{X =59,Y = 21, SecondCastle = 0},
			new MapLocation{X =60,Y = 25, SecondCastle = 0},
			new MapLocation{X =40,Y = 26, SecondCastle = 0},
			new MapLocation{X =15,Y = 41, SecondCastle = 0},
			new MapLocation{X =28,Y = 38, SecondCastle = 0},
			new MapLocation{X =34,Y = 44, SecondCastle = 0},
			new MapLocation{X =32,Y = 49, SecondCastle = 0},
			new MapLocation{X =16,Y = 38, SecondCastle = 0},

			new MapLocation{X =24,Y = 51, SecondCastle = 1},
			new MapLocation{X =24,Y = 53, SecondCastle = 1},
			new MapLocation{X = 3,Y = 49, SecondCastle = 1},
			new MapLocation{X = 3,Y = 46, SecondCastle = 1},
			new MapLocation{X = 4,Y = 42, SecondCastle = 1},
			new MapLocation{X = 3,Y = 38, SecondCastle = 1},
			new MapLocation{X =23,Y = 37, SecondCastle = 1},
			new MapLocation{X =35,Y = 25, SecondCastle = 1},
			new MapLocation{X =29,Y = 19, SecondCastle = 1},
			new MapLocation{X =31,Y = 14, SecondCastle = 1},
			new MapLocation{X =48,Y = 22, SecondCastle = 1},
			new MapLocation{X =47,Y = 25, SecondCastle = 1},
			new MapLocation{X =43,Y = 27, SecondCastle = 1},
			new MapLocation{X =46,Y = 27, SecondCastle = 1},
			new MapLocation{X =59,Y = 35, SecondCastle = 1},
			new MapLocation{X =42,Y = 37, SecondCastle = 1},
			new MapLocation{X =33,Y = 38, SecondCastle = 1},
			new MapLocation{X =37,Y = 41, SecondCastle = 1},
			new MapLocation{X =50,Y = 41, SecondCastle = 1},
			new MapLocation{X =46,Y = 44, SecondCastle = 1},
			new MapLocation{X =27,Y = 42, SecondCastle = 1},
			new MapLocation{X =34,Y = 51, SecondCastle = 1},
		};
		public static List<MapLocation> SuccubusRoom = new List<MapLocation>
		{
			new MapLocation{X = 0, Y = 0, SecondCastle = 0}
		};
		public static List<MapLocation> ShopRoom = new List<MapLocation>
		{
			new MapLocation{X = 49, Y = 20, SecondCastle = 0}
		};
		public static List<MapLocation> RichterRooms = new List<MapLocation>
		{
			new MapLocation{X = 31, Y = 8, SecondCastle = 0},
			new MapLocation{X = 32, Y = 8, SecondCastle = 0},
			new MapLocation{X = 33, Y = 8, SecondCastle = 0},
			new MapLocation{X = 34, Y = 8, SecondCastle = 0},
		};
		public static List<MapLocation> EntranceCutsceneRooms = new List<MapLocation>
		{
			new MapLocation{X = 0, Y = 44, SecondCastle = 0},
			new MapLocation{X = 1, Y = 44, SecondCastle = 0},
			new MapLocation{X = 2, Y = 44, SecondCastle = 0},
			new MapLocation{X = 3, Y = 44, SecondCastle = 0},
			new MapLocation{X = 4, Y = 44, SecondCastle = 0},
			new MapLocation{X = 5, Y = 44, SecondCastle = 0},
			new MapLocation{X = 6, Y = 44, SecondCastle = 0},
			new MapLocation{X = 7, Y = 44, SecondCastle = 0},
			new MapLocation{X = 8, Y = 44, SecondCastle = 0},
			new MapLocation{X = 9, Y = 44, SecondCastle = 0},
			new MapLocation{X = 10, Y = 44, SecondCastle = 0},
			new MapLocation{X = 11, Y = 44, SecondCastle = 0},
			new MapLocation{X = 12, Y = 44, SecondCastle = 0},
			new MapLocation{X = 13, Y = 44, SecondCastle = 0},
			new MapLocation{X = 14, Y = 44, SecondCastle = 0},
			new MapLocation{X = 15, Y = 44, SecondCastle = 0},
			new MapLocation{X = 16, Y = 44, SecondCastle = 0},
			new MapLocation{X = 17, Y = 44, SecondCastle = 0},
			new MapLocation{X = 18, Y = 44, SecondCastle = 0},
		};
		public static List<MapLocation> SwitchRoom = new List<MapLocation>
		{
			new MapLocation{X = 46, Y = 24, SecondCastle = 0}
		};
		public static List<MapLocation> GalamothRooms = new List<MapLocation>
		{
			new MapLocation{X = 44, Y = 12, SecondCastle = 1},
			new MapLocation{X = 45, Y = 12, SecondCastle = 1},
			new MapLocation{X = 44, Y = 13, SecondCastle = 1},
			new MapLocation{X = 45, Y = 13, SecondCastle = 1},
		};
		public static List<MapLocation> LesserDemonZone = new List<MapLocation>
		{
			new MapLocation{X = 45, Y = 20, SecondCastle = 0},
			new MapLocation{X = 46, Y = 20, SecondCastle = 0},
			new MapLocation{X = 47, Y = 20, SecondCastle = 0},
			new MapLocation{X = 48, Y = 20, SecondCastle = 0},
			new MapLocation{X = 48, Y = 19, SecondCastle = 0},
			new MapLocation{X = 47, Y = 19, SecondCastle = 0}
		};

		public static List<SearchableActor> AcceptedHordeEnemies = new List<SearchableActor>
		{
			new SearchableActor { Name="Zombie", Hp = 1, Damage = 14, Sprite = 25776 },
			new SearchableActor { Name="Bat", Hp = 1, Damage = 16, Sprite = 25188 },
			new SearchableActor { Name="Bone Scimitar", Hp = 18 , Damage = 5, Sprite = 23212  },
			new SearchableActor { Name="Bloody Zombie", Hp = 24 , Damage = 10, Sprite = 42580  },
			new SearchableActor { Hp = 48, Damage = 13, Sprite = 4612 },
			new SearchableActor { Name="Bone Scimitar Alchemy Lab", Hp = 18, Damage = 5, Sprite = 14308  },
			new SearchableActor { Name="Blood Skeleton", Hp = 9, Damage = 8, Sprite = 31064  },
			new SearchableActor { Name="Skeleton", Hp = 9, Damage = 2, Sprite = 24516 },
			new SearchableActor { Name="Spittle Bone", Hp = 18, Damage = 7, Sprite = 26412 },
			new SearchableActor { Name="Axe Knight", Hp = 32, Damage = 6, Sprite = 17852  },
			new SearchableActor { Name="Axe Knight Marble Gallery", Hp = 32, Damage = 6, Sprite = 46300  },
			new SearchableActor { Name="Ouija Table", Hp = 20, Damage = 4, Sprite = 48588 },
			new SearchableActor { Name="Slinger", Hp = 12, Damage = 5, Sprite = 30320  },
			new SearchableActor { Name="Marionette", Hp = 20, Damage = 8, Sprite = 26360 },
			new SearchableActor { Hp = 5, Damage = 4, Sprite = 48588  },
			new SearchableActor { Name="Flea Man", Hp = 11, Damage = 9, Sprite = 51080  },
			new SearchableActor { Name="Skeleton Marble Gallery", Hp = 9, Damage = 2, Sprite = 52040  },
			new SearchableActor { Name="Skeleton Outer Wall", Hp = 9, Damage = 2, Sprite = 54896 },
			new SearchableActor { Name="Spear Guard Outer Wall", Hp = 20, Damage = 12, Sprite = 14964  },
			new SearchableActor { Name="Bone Musket Outer Wall", Hp = 24, Damage = 12, Sprite = 60200  },
			new SearchableActor { Name="Blue Medusa Head Outer Wall", Hp = 12, Damage = 12, Sprite = 22572 },
			new SearchableActor { Name="Gold Medusa Head Outer Wall", Hp = 12, Damage = 7, Sprite = 22536 },
			new SearchableActor { Name="Dhuron", Hp = 32, Damage = 7, Sprite = 49236 },
			new SearchableActor { Name="Flea Man Library", Hp = 11, Damage = 9, Sprite = 772  },
			new SearchableActor { Name="Thronweed Library", Hp = 12, Damage = 10, Sprite = 56172 },
			new SearchableActor { Name="Flea Armor", Hp = 18, Damage = 17, Sprite = 64000 },
			//new SearchableActor { Name="Skeleton Ape Outer Wall", Hp = 10, Damage = 10, Sprite = 18916 },
			new SearchableActor { Name="Skeleton Archer Outer Wall", Hp = 10, Damage = 12, Sprite = 1432  },
			new SearchableActor { Name="Phantom Skull", Hp = 15, Damage = 14, Sprite = 59616 },
			new SearchableActor { Name="Gold Medusa Head Clock Tower", Hp = 12, Damage = 7, Sprite = 916  },
			new SearchableActor { Name="Blue Medusa Head Clock Tower", Hp = 12, Damage = 12, Sprite = 952  },
			new SearchableActor { Name="Harpy", Hp = 26, Damage = 18, Sprite = 43308 },
			new SearchableActor { Name="Karasuman Crow", Hp = 20, Damage = 32, Sprite = 49128 },
			new SearchableActor {Hp = 20, Damage = 32, Sprite = 50472 },
			new SearchableActor { Name="Flea Rider", Hp = 17, Damage = 18, Sprite = 34488 },
			new SearchableActor { Name="Blue Axe Knight Keep", Hp = 42, Damage = 10, Sprite = 38568 },
			new SearchableActor { Name="Black Crow", Hp = 15, Damage = 10, Sprite = 16344  },
			new SearchableActor { Name="Winged Guard", Hp = 15, Damage = 12, Sprite = 14276 },
			new SearchableActor { Name="Bone Halberd", Hp = 30, Damage = 12, Sprite = 12196  },
			new SearchableActor { Name="Bat Chapel", Hp = 1, Damage = 16, Sprite = 15756 },
			new SearchableActor { Name="Baby Hippogryph", Hp = 20, Damage = 10, Sprite = 16340 },
			new SearchableActor { Name="Skelerang Chapel", Hp = 18, Damage = 12, Sprite = 18060 },
			new SearchableActor { Name="Bloody Zombie Alchemy Lab", Hp = 24, Damage = 10, Sprite = 21864 },
			new SearchableActor { Name="Skelerang Olrox's Quarters", Hp = 18, Damage = 12, Sprite = 11068 },
			new SearchableActor { Name="Blade Soldier", Hp = 16, Damage = 15, Sprite = 18404 },
			new SearchableActor {Hp = 24, Damage = 12, Sprite = 20436 },
			new SearchableActor { Name="Bloody Zombie Olrox's Quarters", Hp = 24, Damage = 10, Sprite = 15440 },
			new SearchableActor { Name="Olrox Bat", Hp = 13, Damage = 21, Sprite = 64244 },
			new SearchableActor { Name="Olrox Skull", Hp = 15, Damage = 23, Sprite = 62980 },
			new SearchableActor { Name="Spear Guard Caverns", Hp = 20, Damage = 12, Sprite = 49068 },
			new SearchableActor { Name="Bat Caverns", Hp = 1, Damage = 16, Sprite = 36428 },
			new SearchableActor { Name="Toad", Hp = 10, Damage = 14, Sprite = 31116 },
			//new SearchableActor { Name="Trapped Spear Guard Caverns", Hp = 20, Damage = 12, Sprite = 33464 },
			new SearchableActor { Name="Frog", Hp = 2, Damage = 13, Sprite = 33204 },
			new SearchableActor { Name="Gremlin Mines", Hp = 100, Damage = 20, Sprite = 38856 },
			new SearchableActor { Name="Gremlin Caverns", Hp = 100, Damage = 20, Sprite = 8932 },
			new SearchableActor { Name="Lossoth", Hp = 99, Damage = 18, Sprite = 64232 },
			new SearchableActor { Name="Thornweed Caverns", Hp = 12, Damage = 10, Sprite = 22344 },
			new SearchableActor { Name="Granfaloon Zombie", Hp = 10, Damage = 20, Sprite = 17872 },
			new SearchableActor {Hp = 22, Damage = 28, Sprite = 10100 },
			new SearchableActor { Name="Bomb Knight", Hp = 46, Damage = 37, Sprite = 48728 },
			new SearchableActor { Name="Gold Medusa Head Inverted Clock Tower", Hp = 12, Damage = 7, Sprite = 45404 },
			new SearchableActor { Name="Tombstone", Hp = 5, Damage = 40, Sprite = 54652 },
			new SearchableActor { Name="Balloon Pod", Hp = 3, Damage = 55, Sprite = 18024 },
			new SearchableActor { Name="Black Panther", Hp = 35, Damage = 45, Sprite = 24640 },
			new SearchableActor { Name="Imp", Hp = 43, Damage = 10, Sprite = 14584 },
			new SearchableActor { Name="Blue Medusa Head Death Wing's Lair", Hp = 12, Damage = 12, Sprite = 45836 },
			new SearchableActor { Name="Gold Medusa Head Death Wing's Lair", Hp = 12, Damage = 7, Sprite = 45800 },
			new SearchableActor { Name="Ghost Dancer", Hp = 30, Damage = 56, Sprite = 43916 },
			new SearchableActor { Name="Werewolf Inverted Colosseum", Hp = 280, Damage = 40, Sprite = 29328 },
			new SearchableActor { Name="Zombie Trevor", Hp = 180, Damage = 40, Sprite = 37884 },
			new SearchableActor {Hp = 10, Damage = 66, Sprite = 14076 },
			new SearchableActor {Hp = 43, Damage = 10, Sprite = 2536 },
			new SearchableActor {Hp = 12, Damage = 7, Sprite = 9876 },
			new SearchableActor { Name="Corpseweed", Hp = 12, Damage = 10, Sprite = 27132 },
			new SearchableActor { Name="Schmoo", Hp = 50, Damage = 40, Sprite = 64648 },
			new SearchableActor { Name="Blue Venus Weed Rose", Hp = 1, Damage = 70, Sprite = 33952 },
			new SearchableActor {Hp = 46, Damage = 37, Sprite = 39840 },
			new SearchableActor {Hp = 10, Damage = 100, Sprite = 38772 },
			new SearchableActor { Name="Gaibon Inverted Mine", Hp = 200, Damage = 7, Sprite = 27600 },
			new SearchableActor { Name="Slogra Inverted Mine", Hp = 200, Damage = 6, Sprite = 23676 },
			new SearchableActor { Name="Death Sickle", Hp = 0, Damage = 55, Sprite = 50968 },
			new SearchableActor { Name="Bat Inverted Mine", Hp = 1, Damage = 16, Sprite = 43228 },
			new SearchableActor { Name="Thornweed Inverted Mine", Hp = 12, Damage = 10, Sprite = 33052 },
			new SearchableActor { Name="Bat Floating Catacombs", Hp = 1, Damage = 16, Sprite = 15412 },
			new SearchableActor { Name="Blood Skeleton Floating Catacombs", Hp = 9, Damage = 8, Sprite = 7536  },
			new SearchableActor { Name="Skeleton Floating Catacombs", Hp = 9, Damage = 2, Sprite = 8684  }
		};
		public static List<SearchableActor> AcceptedRomhackHordeEnemies = new List<SearchableActor>
		{
			new SearchableActor {Sprite = 25776 },
			new SearchableActor {Sprite = 25188 },
			new SearchableActor {Sprite = 23212  },
			new SearchableActor {Sprite = 42580  },
			new SearchableActor {Sprite = 4612 },
			new SearchableActor {Sprite = 14308  },
			new SearchableActor {Sprite = 31064  },
			new SearchableActor {Sprite = 24516 },
			new SearchableActor {Sprite = 26412 },
			new SearchableActor {Sprite = 17852  },
			new SearchableActor {Sprite = 46300  },
			new SearchableActor {Sprite = 48588 },
			new SearchableActor {Sprite = 30320  },
			new SearchableActor {Sprite = 26360 },
			new SearchableActor {Sprite = 48588  },
			new SearchableActor {Sprite = 51080  },
			new SearchableActor {Sprite = 52040  },
			new SearchableActor {Sprite = 54896 },
			new SearchableActor {Sprite = 14964  },
			new SearchableActor {Sprite = 60200  },
			new SearchableActor {Sprite = 22572 },
			new SearchableActor {Sprite = 49236 },
			new SearchableActor {Sprite = 772  },
			new SearchableActor {Sprite = 56172 },
			new SearchableActor {Sprite = 64000 },
			new SearchableActor {Sprite = 18916 },
			new SearchableActor {Sprite = 1432  },
			new SearchableActor {Sprite = 59616 },
			new SearchableActor {Sprite = 916  },
			new SearchableActor {Sprite = 43308 },
			new SearchableActor {Sprite = 50472 },
			new SearchableActor {Sprite = 34488 },
			new SearchableActor {Sprite = 38568 },
			new SearchableActor {Sprite = 16344  },
			new SearchableActor {Sprite = 14276 },
			new SearchableActor {Sprite = 12196  },
			new SearchableActor {Sprite = 15756 },
			new SearchableActor {Sprite = 18060 },
			new SearchableActor {Sprite = 21864 },
			new SearchableActor {Sprite = 11068 },
			new SearchableActor {Sprite = 18404 },
			new SearchableActor {Sprite = 20436 },
			new SearchableActor {Sprite = 15440 },
			new SearchableActor {Sprite = 49068 },
			new SearchableActor {Sprite = 36428 },
			new SearchableActor {Sprite = 31116 },
			new SearchableActor {Sprite = 33464 },
			new SearchableActor {Sprite = 33204 },
			new SearchableActor {Sprite = 38856 },
			new SearchableActor {Sprite = 8932 },
			new SearchableActor {Sprite = 64232 },
			new SearchableActor {Sprite = 22344 },
			new SearchableActor {Sprite = 17300 },
			new SearchableActor {Sprite = 10100 },
			new SearchableActor {Sprite = 48728 },
			new SearchableActor {Sprite = 45404 },
			new SearchableActor {Sprite = 54652 },
			new SearchableActor {Sprite = 18024 },
			new SearchableActor {Sprite = 24640 },
			new SearchableActor {Sprite = 14584 },
			new SearchableActor {Sprite = 45800 },
			new SearchableActor {Sprite = 43916 },
			new SearchableActor {Sprite = 29328 },
			new SearchableActor {Sprite = 14076 },
			new SearchableActor {Sprite = 2536 },
			new SearchableActor {Sprite = 9876 },
			new SearchableActor {Sprite = 27132 },
			new SearchableActor {Sprite = 64648 },
			new SearchableActor {Sprite = 33952 },
			new SearchableActor {Sprite = 39840 },
			new SearchableActor {Sprite = 38772 },
			new SearchableActor {Sprite = 27600 },
			new SearchableActor {Sprite = 23676 },
			new SearchableActor {Sprite = 43228 },
			new SearchableActor {Sprite = 33052 },
			new SearchableActor {Sprite = 15412 },
			new SearchableActor {Sprite = 7536  },
			new SearchableActor {Sprite = 8684  }
		};
		public static List<SearchableActor> EnduranceBosses = new List<SearchableActor>
		{
			//new SearchableActor {Name = "Slogra", Hp = 200, Damage = 6, Sprite = 18296},  //It always detects Slogra, experimenting with Gaibon clone instead
			new SearchableActor {Name = "Gaibon", Hp = 200, Damage = 7, Sprite = 22392},
			new SearchableActor {Name = "Doppleganger 10", Hp = 120, Damage = 7, Sprite = 14260},
			new SearchableActor {Name = "Minotaur", Hp = 300, Damage = 20, Sprite = 9884},
			new SearchableActor {Name = "Werewolf", Hp = 260, Damage = 20, Sprite = 14428},
			new SearchableActor {Name = "Lesser Demon", Hp = 400, Damage = 20, Sprite = 56036},
			new SearchableActor {Name = "Karasuman", Hp = 500, Damage = 20, Sprite = 43920},
			//new SearchableActor {Name = "Hippogryph", Hp = 800, Damage = 18, Sprite = 7188},  //Can trigger the door closing and locking the player on the wrong side.
			new SearchableActor {Name = "Olrox", Hp = 666, Damage = 20, Sprite = 54072},
			new SearchableActor {Name = "Succubus", Hp = 666, Damage = 25, Sprite = 8452},
			new SearchableActor {Name = "Cerberus", Hp = 800, Damage = 20, Sprite = 19772},
			//new SearchableActor {Name = "Granfaloon", Hp = 400, Damage = 30, Sprite = 6264},  //Only spawns core, no tentacles or shell
			new SearchableActor {Name = "Richter", Hp = 400, Damage = 25, Sprite = 27332},
			new SearchableActor {Name = "Darkwing Bat", Hp = 600, Damage = 35, Sprite = 40376},
			//new SearchableActor {Name = "Creature", Hp = 1100, Damage = 30, Sprite = 31032},//Hammer doesn't have hitbox and body only does 1 damage
			new SearchableActor {Name = "Doppleganger 40", Hp = 777, Damage = 35, Sprite = 11664},
			//new SearchableActor {Name = "Death", Hp = 888, Damage = 35, Sprite = 46380},
			new SearchableActor {Name = "Medusa", Hp = 1100, Damage = 35, Sprite = 6044},
			new SearchableActor {Name = "Akmodan", Hp = 1200, Damage = 40, Sprite = 16564},
			new SearchableActor {Name = "Sypha", Hp = 1000, Damage = 9, Sprite = 30724},
			new SearchableActor {Name = "Shaft", Hp = 1300, Damage = 40, Sprite = 43772}
		};
		public static List<SearchableActor> EnduranceAlternateBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Hippogryph", Hp = 800, Damage = 18, Sprite = 7188},
			new SearchableActor {Name = "Scylla", Hp = 200, Damage = 16, Sprite = 10988},
			new SearchableActor {Name = "Granfaloon", Hp = 400, Damage = 30, Sprite = 6264},
			new SearchableActor {Name = "Creature", Hp = 1100, Damage = 30, Sprite = 31032},
			new SearchableActor {Name = "Death", Hp = 888, Damage = 35, Sprite = 46380},
			new SearchableActor {Name = "Beelzebub", Hp = 2000, Damage = 60, Sprite = 11356},
			new SearchableActor {Name = "Dracula", Hp = 10000, Damage = 39, Sprite = 56220},
		};
		public static List<SearchableActor> EnduranceRomhackBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Gaibon", Sprite = 22392},
			new SearchableActor {Name = "Doppleganger 10", Sprite = 14260},
			new SearchableActor {Name = "Minotaur", Sprite = 9884},
			new SearchableActor {Name = "Werewolf", Sprite = 14428},
			new SearchableActor {Name = "Lesser Demon", Sprite = 56036},
			new SearchableActor {Name = "Karasuman", Sprite = 43920},
			new SearchableActor {Name = "Olrox", Sprite = 54072},
			new SearchableActor {Name = "Succubus", Sprite = 8452},
			new SearchableActor {Name = "Cerberus", Sprite = 19772},
			new SearchableActor {Name = "Richter", Sprite = 27332},
			new SearchableActor {Name = "Darkwing Bat", Sprite = 40376},
			new SearchableActor {Name = "Creature", Sprite = 31032},
			new SearchableActor {Name = "Doppleganger 40", Sprite = 11664},
			new SearchableActor {Name = "Medusa", Sprite = 6044},
			new SearchableActor {Name = "Akmodan", Sprite = 16564},
			new SearchableActor {Name = "Sypha", Sprite = 30724},
			new SearchableActor {Name = "Shaft", Sprite = 43772}
		};
		public static List<SearchableActor> EnduranceAlternateRomhackBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Hippogryph", Sprite = 7188},
			new SearchableActor {Name = "Scylla", Sprite = 10988},
			new SearchableActor {Name = "Granfaloon", Sprite = 6264},
			new SearchableActor {Name = "Creature", Sprite = 31032},
			new SearchableActor {Name = "Death", Sprite = 46380},
			new SearchableActor {Name = "Beelzebub", Sprite = 11356},
			new SearchableActor {Name = "Dracula", Sprite = 56220},
		};
		public static SearchableActor DraculaActor = new SearchableActor { Hp = 10000, Damage = 39, Sprite = 56220 };
		public static SearchableActor GalamothTorsoActor = new SearchableActor { Hp = 12000, Damage = 50, Sprite = 23936 };
		public static SearchableActor GalamothHeadActor = new SearchableActor { Hp = 32767, Damage = 50, Sprite = 31516 };
		public static SearchableActor GalamothPartsActors = new SearchableActor { Hp = 12000, Damage = 50, Sprite = 31516 };
		public static SearchableActor ShaftOrbActor = new SearchableActor { Hp = 10, Damage = 0, Sprite = 0, };

		public static string[] RandomNames =
	   {
			"Josh",
			"Jimmy",
			"Steve",
			"John",
			"Jack",
			"Nick",
			"Tony",
			"Tom",
			"James",
			"Rob",
			"Mike",
			"Will",
			"Charlie",
			"Chris",
			"Matt",
			"Mark",
			"Paul",
			"Kenny",
			"Kevin",
			"Karen",
			"Mary",
			"Mimi",
			"Linda",
			"Sarah",
			"Lisa",
			"Emily",
		};

		public static SearchableActor SpiritActor = new SearchableActor { Hp = 0, Sprite = 39012 };
		public static uint SpiritPalette = 0x5B;
		public static uint SpiritLockOn = 2;
		public static string SpiritLockOnName = "SpiritLockOn";
		public static uint WhiteTigerBallSpeedLeft = 0xFFFF;
		public static uint WhiteTigerBallSpeedRight = 0x0000;
		public static string WhiteTigerBallSpeedName = "WhiteTigerBallSpeed";

		public static List<byte> FireballActorBytes = new List<byte> { 0x00, 0xC0, 0xB2, 0x00, 0x00, 0x00, 0xB3, 0x00, 0x00, 0x80, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x96, 0x00, 0x1A, 0x00, 0xDC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x08, 0x00, 0x00, 0x03, 0x00, 0x02, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x80, 0x00, 0x00, 0x04, 0x04, 0x00, 0x14, 0x00, 0x00, 0x98, 0x07, 0x0B, 0x80, 0x04, 0x00, 0x01, 0x00, 0x09, 0x00, 0x05, 0x00, 0x08 };
		public static List<byte> DarkFireballActorBytes = new List<byte> { 0xE0, 0xBE, 0x7D, 0x00, 0x00, 0x00, 0xAF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x96, 0x00, 0x1B, 0x00, 0x40, 0x78, 0x12, 0x80, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x08, 0x00, 0x00, 0x03, 0x00, 0x02, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x80, 0x00, 0x00, 0x08, 0x08, 0x00, 0x04, 0x00, 0x00, 0xC8, 0x07, 0x0B, 0x80, 0x17, 0x00, 0x01, 0x00, 0x09, 0x00, 0x09, 0x00, 0x08 };

		public static string[] AcceptedMusicTrackTitles =
	   {
			"lost painting",
			"cursed sanctuary",
			"requiem for the gods",
			"rainbow cemetary",
			"wood carving partita",
			"crystal teardrops",
			"marble galery",
			"dracula castle",
			"the tragic prince",
			"tower of evil mist",
			"doorway of spirits",
			"dance of pearls",
			"abandoned pit",
			"heavenly doorway",
			"festival of servants",
			"dance of illusions",
			"prologue",
			"wandering ghosts",
			"doorway to the abyss",
			"metamorphosis",
			"metamorphosis 2",
			"dance of gold",
			"enchanted banquet",
			"prayer",
			"death's ballad",
			"blood relations",
			"finale toccata",
			"black banquet",
			"silence",
			"nocturne",
			"moonlight nocturne"
		};
		public static Dictionary<string, string> AlternateTrackTitles = new Dictionary<string, string>
		{
			{ "deaths ballad", "death's ballad" },
			{ "death ballad", "death's ballad" },
			{ "poetic death", "death's ballad" },
			{ "illusionary dance", "dance of illusions" },
			{ "dracula", "dance of illusions" },
			{ "nocturne in the moonlight", "moonlight nocturne" },
			{ "dracula's castle", "dracula castle" },
			{ "draculas castle", "dracula castle" },
			{ "castle entrance", "dracula castle" },
			{ "entrance", "dracula castle" },
			{ "tower of mist", "tower of evil mist" },
			{ "outer wall", "tower of evil mist" },
			{ "library", "wood carving partita" },
			{ "alchemy lab", "dance of gold" },
			{ "alchemy laboratory", "dance of gold" },
			{ "chapel", "requiem for the gods" },
			{ "royal chapel", "requiem for the gods" },
			{ "crystal teardrop", "crystal teardrops" },
			{ "caverns", "crystal teardrops" },
			{ "underground caverns", "crystal teardrops" },
			{ "departer way", "abandoned pit" },
			{ "pit", "abandoned pit" },
			{ "mines", "abandoned pit" },
			{ "mine", "abandoned pit" },
			{ "catacombs", "rainbow cemetary" },
			{ "lost paintings", "lost painting" },
			{ "antichapel", "lost painting" },
			{ "reverse caverns", "lost painting" },
			{ "forbidden library", "lost painting" },
			{ "waltz of pearls", "dance of pearls" },
			{ "olrox's quarters", "dance of pearls" },
			{ "olroxs quarters", "dance of pearls" },
			{ "olrox quarters", "dance of pearls" },
			{ "cursed zone", "cursed sanctuary" },
			{ "floating catacombs", "cursed sanctuary" },
			{ "reverse catacombs", "cursed sanctuary" },
			{ "demonic banquet", "enchanted banquet" },
			{ "medusa", "enchanted banquet" },
			{ "succubus", "enchanted banquet" },
			{ "colosseum", "wandering ghosts" },
			{ "wandering ghost", "wandering ghosts" },
			{ "pitiful scion", "the tragic prince" },
			{ "clock tower", "the tragic prince" },
			{ "tragic prince", "the tragic prince" },
			{ "alucard", "the tragic prince" },
			{ "door to the abyss", "doorway to the abyss" },
			{ "doorway to heaven", "heavenly doorway" },
			{ "keep", "heavenly doorway" },
			{ "castle keep", "heavenly doorway" },
			{ "divine bloodlines", "blood relations" },
			{ "strange bloodlines", "blood relations" },
			{ "richter belmont", "blood relations" },
			{ "richter", "blood relations" },
		};

		public static float SuperWeakenFactor = 0.5F;
		public static float SuperCrippleFactor = 0.5F;
		public static int SlowQueueIntervalEnd = 3;
		public static int FastQueueIntervalStart = 8;
		public static uint SuperThirstExtraDrain = 2u;
		public static int HelpItemRetryCount = 15;
		public static float BattleOrdersHpMultiplier = 2F;
		public static uint GuiltyGearInvincibility = 3;
		public static uint GuiltyGearAttack = 50;
		public static uint GuiltyGearDefence = 50;
		public static uint GuiltyGearDarkMetamorphosis = 50;
		public static uint ShaftKhaosHp = 25;
		public static uint GalamothKhaosHp = 2000;
		public static uint GalamothKhaosPositionOffset = 100;
		public static float HasteDashFactor = 1.8F;
		public static int SaveIcosahedronFirstCastle = 0xBCAA;
		public static int SaveIcosahedronSecondCastle = 0x1150;
		public static int KhaosActionsCount = 30;

		public static int AutoKhaosDifficultyEasy = 70;
		public static int AutoKhaosDifficultyNormal = 50;
		public static int AutoKhaosDifficultyHard = 20;
	}
}

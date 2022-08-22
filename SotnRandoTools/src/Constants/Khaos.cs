using System.Collections.Generic;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Models;
using MapLocation = SotnRandoTools.RandoTracker.Models.MapLocation;

namespace SotnRandoTools.Constants
{
	public static class Khaos
	{
		public static readonly Relic[] ProgressionRelics =
		{
			Relic.SoulOfBat,
			Relic.SoulOfWolf,
			Relic.FormOfMist,
			Relic.GravityBoots,
			Relic.LeapStone,
			Relic.JewelOfOpen,
			Relic.MermanStatue
		};
		public static readonly List<Relic[]> FlightRelics = new()
		{
			new Relic[] { Relic.SoulOfBat },
			new Relic[] { Relic.LeapStone, Relic.GravityBoots },
			new Relic[] { Relic.FormOfMist, Relic.PowerOfMist },
			new Relic[] { Relic.SoulOfWolf, Relic.GravityBoots },
		};

		public static readonly List<MapLocation> LoadingRooms = new List<MapLocation>
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
		public static readonly List<MapLocation> SuccubusRoom = new List<MapLocation>
		{
			new MapLocation{X = 0, Y = 0, SecondCastle = 0}
		};
		public static readonly List<MapLocation> ShopRoom = new List<MapLocation>
		{
			new MapLocation{X = 49, Y = 20, SecondCastle = 0}
		};
		public static readonly List<MapLocation> RichterRooms = new List<MapLocation>
		{
			new MapLocation{X = 31, Y = 8, SecondCastle = 0},
			new MapLocation{X = 32, Y = 8, SecondCastle = 0},
			new MapLocation{X = 33, Y = 8, SecondCastle = 0},
			new MapLocation{X = 34, Y = 8, SecondCastle = 0},
		};
		public static readonly List<MapLocation> EntranceCutsceneRooms = new List<MapLocation>
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
		public static readonly List<MapLocation> SwitchRoom = new List<MapLocation>
		{
			new MapLocation{X = 46, Y = 24, SecondCastle = 0}
		};
		public static readonly List<MapLocation> GalamothRooms = new List<MapLocation>
		{
			new MapLocation{X = 44, Y = 12, SecondCastle = 1},
			new MapLocation{X = 45, Y = 12, SecondCastle = 1},
			new MapLocation{X = 44, Y = 13, SecondCastle = 1},
			new MapLocation{X = 45, Y = 13, SecondCastle = 1},
		};
		public static readonly List<MapLocation> LesserDemonZone = new List<MapLocation>
		{
			new MapLocation{X = 45, Y = 20, SecondCastle = 0},
			new MapLocation{X = 46, Y = 20, SecondCastle = 0},
			new MapLocation{X = 47, Y = 20, SecondCastle = 0},
			new MapLocation{X = 48, Y = 20, SecondCastle = 0},
			new MapLocation{X = 48, Y = 19, SecondCastle = 0},
			new MapLocation{X = 47, Y = 19, SecondCastle = 0}
		};
		public static readonly List<MapLocation> LibraryRoom = new List<MapLocation>
		{
			new MapLocation{X = 49, Y = 20, SecondCastle = 0}
		};
		public static readonly List<MapLocation> DraculaRoom = new List<MapLocation>
		{
			new MapLocation{X = 31, Y = 30, SecondCastle = 1}
		};
		public static readonly List<MapLocation> ShaftRooms = new List<MapLocation>
		{
			new MapLocation{X = 30, Y = 31, SecondCastle = 1},
			new MapLocation{X = 31, Y = 31, SecondCastle = 1},
			new MapLocation{X = 32, Y = 31, SecondCastle = 1},
			new MapLocation{X = 30, Y = 32, SecondCastle = 1},
			new MapLocation{X = 31, Y = 32, SecondCastle = 1},
			new MapLocation{X = 32, Y = 32, SecondCastle = 1},
			new MapLocation{X = 30, Y = 33, SecondCastle = 1},
			new MapLocation{X = 31, Y = 33, SecondCastle = 1},
			new MapLocation{X = 32, Y = 33, SecondCastle = 1}
		};

		public static readonly List<SearchableActor> AcceptedHordeEnemies = new List<SearchableActor>
		{
			new SearchableActor { Name="Zombie", Hp = 1, Damage = 14, AiId = 25776 },
			new SearchableActor { Name="Bat", Hp = 1, Damage = 16, AiId = 25188 },
			new SearchableActor { Name="Bone Scimitar", Hp = 18 , Damage = 5, AiId = 23212  },
			new SearchableActor { Name="Bloody Zombie", Hp = 24 , Damage = 10, AiId = 42580  },
			new SearchableActor { Hp = 48, Damage = 13, AiId = 4612 },
			new SearchableActor { Name="Bone Scimitar Alchemy Lab", Hp = 18, Damage = 5, AiId = 14308  },
			new SearchableActor { Name="Blood Skeleton", Hp = 9, Damage = 8, AiId = 31064  },
			new SearchableActor { Name="Skeleton", Hp = 9, Damage = 2, AiId = 24516 },
			new SearchableActor { Name="Spittle Bone", Hp = 18, Damage = 7, AiId = 26412 },
			new SearchableActor { Name="Axe Knight", Hp = 32, Damage = 6, AiId = 17852  },
			new SearchableActor { Name="Axe Knight Marble Gallery", Hp = 32, Damage = 6, AiId = 46300  },
			//new SearchableActor { Name="Ouija Table", Hp = 20, Damage = 4, AiId = 48588 },
			new SearchableActor { Name="Slinger", Hp = 12, Damage = 5, AiId = 30320  },
			new SearchableActor { Name="Marionette", Hp = 20, Damage = 8, AiId = 26360 },
			new SearchableActor { Hp = 5, Damage = 4, AiId = 48588  },
			new SearchableActor { Name="Flea Man", Hp = 11, Damage = 9, AiId = 51080  },
			new SearchableActor { Name="Skeleton Marble Gallery", Hp = 9, Damage = 2, AiId = 52040  },
			new SearchableActor { Name="Skeleton Outer Wall", Hp = 9, Damage = 2, AiId = 54896 },
			new SearchableActor { Name="Spear Guard Outer Wall", Hp = 20, Damage = 12, AiId = 14964  },
			//new SearchableActor { Name="Bone Musket Outer Wall", Hp = 24, Damage = 12, AiId = 60200  },
			new SearchableActor { Name="Blue Medusa Head Outer Wall", Hp = 12, Damage = 12, AiId = 22572 },
			new SearchableActor { Name="Gold Medusa Head Outer Wall", Hp = 12, Damage = 7, AiId = 22536 },
			new SearchableActor { Name="Dhuron", Hp = 32, Damage = 7, AiId = 49236 },
			new SearchableActor { Name="Flea Man Library", Hp = 11, Damage = 9, AiId = 772  },
			new SearchableActor { Name="Thronweed Library", Hp = 12, Damage = 10, AiId = 56172 },
			new SearchableActor { Name="Flea Armor", Hp = 18, Damage = 17, AiId = 64000 },
			//new SearchableActor { Name="Skeleton Ape Outer Wall", Hp = 10, Damage = 10, AiId = 18916 },
			new SearchableActor { Name="Skeleton Archer Outer Wall", Hp = 10, Damage = 12, AiId = 1432  },
			new SearchableActor { Name="Phantom Skull", Hp = 15, Damage = 14, AiId = 59616 },
			new SearchableActor { Name="Gold Medusa Head Clock Tower", Hp = 12, Damage = 7, AiId = 916  },
			new SearchableActor { Name="Blue Medusa Head Clock Tower", Hp = 12, Damage = 12, AiId = 952  },
			new SearchableActor { Name="Harpy", Hp = 26, Damage = 18, AiId = 43308 },
			new SearchableActor { Name="Karasuman Crow", Hp = 20, Damage = 32, AiId = 49128 },
			new SearchableActor {Hp = 20, Damage = 32, AiId = 50472 },
			new SearchableActor { Name="Flea Rider", Hp = 17, Damage = 18, AiId = 34488 },
			new SearchableActor { Name="Blue Axe Knight Keep", Hp = 42, Damage = 10, AiId = 38568 },
			new SearchableActor { Name="Black Crow", Hp = 15, Damage = 10, AiId = 16344  },
			new SearchableActor { Name="Winged Guard", Hp = 15, Damage = 12, AiId = 14276 },
			new SearchableActor { Name="Bone Halberd", Hp = 30, Damage = 12, AiId = 12196  },
			new SearchableActor { Name="Bat Chapel", Hp = 1, Damage = 16, AiId = 15756 },
			new SearchableActor { Name="Baby Hippogryph", Hp = 20, Damage = 10, AiId = 16340 },
			new SearchableActor { Name="Skelerang Chapel", Hp = 18, Damage = 12, AiId = 18060 },
			new SearchableActor { Name="Bloody Zombie Alchemy Lab", Hp = 24, Damage = 10, AiId = 21864 },
			new SearchableActor { Name="Skelerang Olrox's Quarters", Hp = 18, Damage = 12, AiId = 11068 },
			new SearchableActor { Name="Blade Soldier", Hp = 16, Damage = 15, AiId = 18404 },
			new SearchableActor {Hp = 24, Damage = 12, AiId = 20436 },
			new SearchableActor { Name="Bloody Zombie Olrox's Quarters", Hp = 24, Damage = 10, AiId = 15440 },
			//new SearchableActor { Name="Olrox Bat", Hp = 13, Damage = 21, AiId = 64244 },
			new SearchableActor { Name="Olrox Skull", Hp = 15, Damage = 23, AiId = 62980 },
			new SearchableActor { Name="Spear Guard Caverns", Hp = 20, Damage = 12, AiId = 49068 },
			new SearchableActor { Name="Bat Caverns", Hp = 1, Damage = 16, AiId = 36428 },
			new SearchableActor { Name="Toad", Hp = 10, Damage = 14, AiId = 31116 },
			//new SearchableActor { Name="Trapped Spear Guard Caverns", Hp = 20, Damage = 12, AiId = 33464 },
			new SearchableActor { Name="Frog", Hp = 2, Damage = 13, AiId = 33204 },
			new SearchableActor { Name="Gremlin Mines", Hp = 100, Damage = 20, AiId = 38856 },
			new SearchableActor { Name="Gremlin Caverns", Hp = 100, Damage = 20, AiId = 8932 },
			new SearchableActor { Name="Lossoth", Hp = 99, Damage = 18, AiId = 64232 },
			new SearchableActor { Name="Thornweed Caverns", Hp = 12, Damage = 10, AiId = 22344 },
			new SearchableActor { Name="Granfaloon Zombie", Hp = 10, Damage = 20, AiId = 17872 },
			new SearchableActor {Hp = 22, Damage = 28, AiId = 10100 },
			new SearchableActor { Name="Bomb Knight", Hp = 46, Damage = 37, AiId = 48728 },
			new SearchableActor { Name="Gold Medusa Head Inverted Clock Tower", Hp = 12, Damage = 7, AiId = 45404 },
			new SearchableActor { Name="Tombstone", Hp = 5, Damage = 40, AiId = 54652 },
			new SearchableActor { Name="Balloon Pod", Hp = 3, Damage = 55, AiId = 18024 },
			new SearchableActor { Name="Black Panther", Hp = 35, Damage = 45, AiId = 24640 },
			new SearchableActor { Name="Imp", Hp = 43, Damage = 10, AiId = 14584 },
			new SearchableActor { Name="Blue Medusa Head Death Wing's Lair", Hp = 12, Damage = 12, AiId = 45836 },
			new SearchableActor { Name="Gold Medusa Head Death Wing's Lair", Hp = 12, Damage = 7, AiId = 45800 },
			new SearchableActor { Name="Ghost Dancer", Hp = 30, Damage = 56, AiId = 43916 },
			new SearchableActor { Name="Werewolf Inverted Colosseum", Hp = 280, Damage = 40, AiId = 29328 },
			//new SearchableActor { Name="Zombie Trevor", Hp = 180, Damage = 40, AiId = 37884 },
			new SearchableActor {Hp = 10, Damage = 66, AiId = 14076 },
			new SearchableActor {Hp = 43, Damage = 10, AiId = 2536 },
			new SearchableActor {Hp = 12, Damage = 7, AiId = 9876 },
			new SearchableActor { Name="Corpseweed", Hp = 12, Damage = 10, AiId = 27132 },
			new SearchableActor { Name="Schmoo", Hp = 50, Damage = 40, AiId = 64648 },
			new SearchableActor { Name="Blue Venus Weed Rose", Hp = 1, Damage = 70, AiId = 33952 },
			new SearchableActor {Hp = 46, Damage = 37, AiId = 39840 },
			new SearchableActor {Hp = 10, Damage = 100, AiId = 38772 },
			new SearchableActor { Name="Gaibon Inverted Mine", Hp = 200, Damage = 7, AiId = 27600 },
			new SearchableActor { Name="Slogra Inverted Mine", Hp = 200, Damage = 6, AiId = 23676 },
			new SearchableActor { Name="Death Sickle", Hp = 0, Damage = 55, AiId = 50968 },
			new SearchableActor { Name="Bat Inverted Mine", Hp = 1, Damage = 16, AiId = 43228 },
			new SearchableActor { Name="Thornweed Inverted Mine", Hp = 12, Damage = 10, AiId = 33052 },
			new SearchableActor { Name="Bat Floating Catacombs", Hp = 1, Damage = 16, AiId = 15412 },
			new SearchableActor { Name="Blood Skeleton Floating Catacombs", Hp = 9, Damage = 8, AiId = 7536  },
			new SearchableActor { Name="Skeleton Floating Catacombs", Hp = 9, Damage = 2, AiId = 8684  }
		};
		public static readonly List<SearchableActor> AcceptedRomhackHordeEnemies = new List<SearchableActor>
		{
			new SearchableActor {AiId = 25776 },
			new SearchableActor {AiId = 25188 },
			new SearchableActor {AiId = 23212  },
			new SearchableActor {AiId = 42580  },
			new SearchableActor {AiId = 4612 },
			new SearchableActor {AiId = 14308  },
			new SearchableActor {AiId = 31064  },
			new SearchableActor {AiId = 24516 },
			new SearchableActor {AiId = 26412 },
			new SearchableActor {AiId = 17852  },
			new SearchableActor {AiId = 46300  },
			new SearchableActor {AiId = 48588 },
			new SearchableActor {AiId = 30320  },
			new SearchableActor {AiId = 26360 },
			new SearchableActor {AiId = 48588  },
			new SearchableActor {AiId = 51080  },
			new SearchableActor {AiId = 52040  },
			new SearchableActor {AiId = 54896 },
			new SearchableActor {AiId = 14964  },
			new SearchableActor {AiId = 60200  },
			new SearchableActor {AiId = 22572 },
			new SearchableActor {AiId = 49236 },
			new SearchableActor {AiId = 772  },
			new SearchableActor {AiId = 56172 },
			new SearchableActor {AiId = 64000 },
			new SearchableActor {AiId = 18916 },
			new SearchableActor {AiId = 1432  },
			new SearchableActor {AiId = 59616 },
			new SearchableActor {AiId = 916  },
			new SearchableActor {AiId = 43308 },
			new SearchableActor {AiId = 50472 },
			new SearchableActor {AiId = 34488 },
			new SearchableActor {AiId = 38568 },
			new SearchableActor {AiId = 16344  },
			new SearchableActor {AiId = 14276 },
			new SearchableActor {AiId = 12196  },
			new SearchableActor {AiId = 15756 },
			new SearchableActor {AiId = 18060 },
			new SearchableActor {AiId = 21864 },
			new SearchableActor {AiId = 11068 },
			new SearchableActor {AiId = 18404 },
			new SearchableActor {AiId = 20436 },
			new SearchableActor {AiId = 15440 },
			new SearchableActor {AiId = 49068 },
			new SearchableActor {AiId = 36428 },
			new SearchableActor {AiId = 31116 },
			new SearchableActor {AiId = 33464 },
			new SearchableActor {AiId = 33204 },
			new SearchableActor {AiId = 38856 },
			new SearchableActor {AiId = 8932 },
			new SearchableActor {AiId = 64232 },
			new SearchableActor {AiId = 22344 },
			new SearchableActor {AiId = 17300 },
			new SearchableActor {AiId = 10100 },
			new SearchableActor {AiId = 48728 },
			new SearchableActor {AiId = 45404 },
			new SearchableActor {AiId = 54652 },
			new SearchableActor {AiId = 18024 },
			new SearchableActor {AiId = 24640 },
			new SearchableActor {AiId = 14584 },
			new SearchableActor {AiId = 45800 },
			new SearchableActor {AiId = 43916 },
			new SearchableActor {AiId = 29328 },
			new SearchableActor {AiId = 14076 },
			new SearchableActor {AiId = 2536 },
			new SearchableActor {AiId = 9876 },
			new SearchableActor {AiId = 27132 },
			new SearchableActor {AiId = 64648 },
			new SearchableActor {AiId = 33952 },
			new SearchableActor {AiId = 39840 },
			new SearchableActor {AiId = 38772 },
			new SearchableActor {AiId = 27600 },
			new SearchableActor {AiId = 23676 },
			new SearchableActor {AiId = 43228 },
			new SearchableActor {AiId = 33052 },
			new SearchableActor {AiId = 15412 },
			new SearchableActor {AiId = 7536  },
			new SearchableActor {AiId = 8684  }
		};
		public static readonly List<SearchableActor> EnduranceBosses = new List<SearchableActor>
		{
			//new SearchableActor {Name = "Slogra", Hp = 200, Damage = 6, AiId = 18296},  //It always detects Slogra, experimenting with Gaibon clone instead
			new SearchableActor {Name = "Gaibon", Hp = 200, Damage = 7, AiId = 22392},
			new SearchableActor {Name = "Doppleganger 10", Hp = 120, Damage = 7, AiId = 14260},
			//new SearchableActor {Name = "Minotaur", Hp = 300, Damage = 20, AiId = 9884},
			//new SearchableActor {Name = "Werewolf", Hp = 260, Damage = 20, AiId = 14428},
			new SearchableActor {Name = "Lesser Demon", Hp = 400, Damage = 20, AiId = 56036},
			new SearchableActor {Name = "Karasuman", Hp = 500, Damage = 20, AiId = 43920},
			//new SearchableActor {Name = "Hippogryph", Hp = 800, Damage = 18, AiId = 7188},  //Can trigger the door closing and locking the player on the wrong side.
			new SearchableActor {Name = "Olrox", Hp = 666, Damage = 20, AiId = 54072},
			new SearchableActor {Name = "Succubus", Hp = 666, Damage = 25, AiId = 8452},
			new SearchableActor {Name = "Cerberus", Hp = 800, Damage = 20, AiId = 19772},
			//new SearchableActor {Name = "Granfaloon", Hp = 400, Damage = 30, AiId = 6264},  //Only spawns core, no tentacles or shell
			new SearchableActor {Name = "Richter", Hp = 400, Damage = 25, AiId = 27332},
			new SearchableActor {Name = "Darkwing Bat", Hp = 600, Damage = 35, AiId = 40376},
			//new SearchableActor {Name = "Creature", Hp = 1100, Damage = 30, AiId = 31032},//Hammer doesn't have hitbox and body only does 1 damage
			new SearchableActor {Name = "Doppleganger 40", Hp = 777, Damage = 35, AiId = 11664},
			//new SearchableActor {Name = "Death", Hp = 888, Damage = 35, AiId = 46380},
			//new SearchableActor {Name = "Medusa", Hp = 1100, Damage = 35, AiId = 6044},
			new SearchableActor {Name = "Akmodan", Hp = 1200, Damage = 40, AiId = 16564},
			new SearchableActor {Name = "Sypha", Hp = 1000, Damage = 9, AiId = 30724},
			new SearchableActor {Name = "Shaft", Hp = 1300, Damage = 40, AiId = 43772}
		};
		public static readonly List<SearchableActor> EnduranceAlternateBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Werewolf", Hp = 260, Damage = 20, AiId = 14428},
			new SearchableActor {Name = "Hippogryph", Hp = 800, Damage = 18, AiId = 7188},
			new SearchableActor {Name = "Scylla", Hp = 200, Damage = 16, AiId = 10988},
			new SearchableActor {Name = "Granfaloon", Hp = 400, Damage = 30, AiId = 6264},
			new SearchableActor {Name = "Medusa", Hp = 1100, Damage = 35, AiId = 6044},
			new SearchableActor {Name = "Creature", Hp = 1100, Damage = 30, AiId = 31032},
			new SearchableActor {Name = "Death", Hp = 888, Damage = 35, AiId = 46380},
			new SearchableActor {Name = "Beelzebub", Hp = 2000, Damage = 60, AiId = 11356},
			new SearchableActor {Name = "Dracula", Hp = 10000, Damage = 39, AiId = 56220},
		};
		public static readonly List<SearchableActor> EnduranceRomhackBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Gaibon", AiId = 22392},
			new SearchableActor {Name = "Doppleganger 10", AiId = 14260},
			new SearchableActor {Name = "Minotaur", AiId = 9884},
			new SearchableActor {Name = "Werewolf", AiId = 14428},
			new SearchableActor {Name = "Lesser Demon", AiId = 56036},
			new SearchableActor {Name = "Karasuman", AiId = 43920},
			new SearchableActor {Name = "Olrox", AiId = 54072},
			new SearchableActor {Name = "Succubus", AiId = 8452},
			new SearchableActor {Name = "Cerberus", AiId = 19772},
			new SearchableActor {Name = "Richter", AiId = 27332},
			new SearchableActor {Name = "Darkwing Bat", AiId = 40376},
			new SearchableActor {Name = "Creature", AiId = 31032},
			new SearchableActor {Name = "Doppleganger 40", AiId = 11664},
			new SearchableActor {Name = "Medusa", AiId = 6044},
			new SearchableActor {Name = "Akmodan", AiId = 16564},
			new SearchableActor {Name = "Sypha", AiId = 30724},
			new SearchableActor {Name = "Shaft", AiId = 43772}
		};
		public static readonly List<SearchableActor> EnduranceAlternateRomhackBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Hippogryph", AiId = 7188},
			new SearchableActor {Name = "Scylla", AiId = 10988},
			new SearchableActor {Name = "Granfaloon", AiId = 6264},
			new SearchableActor {Name = "Creature", AiId = 31032},
			new SearchableActor {Name = "Death", AiId = 46380},
			new SearchableActor {Name = "Beelzebub", AiId = 11356},
			new SearchableActor {Name = "Dracula", AiId = 56220},
		};
		public static readonly SearchableActor DraculaActor = new SearchableActor { Hp = 10000, Damage = 39, AiId = 56220 };
		public static readonly SearchableActor GalamothTorsoActor = new SearchableActor { Hp = 12000, Damage = 50, AiId = 23936 };
		public static readonly SearchableActor GalamothHeadActor = new SearchableActor { Hp = 32767, Damage = 50, AiId = 31516 };
		public static readonly SearchableActor GalamothPartsActors = new SearchableActor { Hp = 12000, Damage = 50, AiId = 31516 };
		public static readonly SearchableActor ShaftOrbActor = new SearchableActor { Hp = 10, Damage = 0, AiId = 0, };

		public static readonly string[] RandomNames =
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

		public static readonly SearchableActor SpiritActor = new SearchableActor { Hp = 0, AiId = 39012 };
		public const uint SpiritPalette = 0x5B;
		public const uint SpiritLockOn = 2;
		public const string SpiritLockOnName = "SpiritLockOn";
		public const uint WhiteTigerBallSpeedLeft = 0xFFFF;
		public const uint WhiteTigerBallSpeedRight = 0x0000;
		public const string WhiteTigerBallSpeedName = "WhiteTigerBallSpeed";
		public const int BloodthirstColorPalette = 33126;
		public const int OverdriveColorPalette = 33127;

		public static readonly List<byte> FireballEntityBytes = new List<byte> { 0x00, 0xC0, 0xB2, 0x00, 0x00, 0x00, 0xB3, 0x00, 0x00, 0x80, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x96, 0x00, 0x1A, 0x00, 0xDC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x08, 0x00, 0x00, 0x03, 0x00, 0x02, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x80, 0x00, 0x00, 0x04, 0x04, 0x00, 0x14, 0x00, 0x00, 0x98, 0x07, 0x0B, 0x80, 0x04, 0x00, 0x01, 0x00, 0x09, 0x00, 0x05, 0x00, 0x08 };
		public static readonly List<byte> DarkFireballEntityBytes = new List<byte> { 0xE0, 0xBE, 0x7D, 0x00, 0x00, 0x00, 0xAF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x96, 0x00, 0x1B, 0x00, 0x40, 0x78, 0x12, 0x80, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x08, 0x00, 0x00, 0x03, 0x00, 0x02, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x80, 0x00, 0x00, 0x08, 0x08, 0x00, 0x04, 0x00, 0x00, 0xC8, 0x07, 0x0B, 0x80, 0x17, 0x00, 0x01, 0x00, 0x09, 0x00, 0x09, 0x00, 0x08 };

		public static readonly string[] AcceptedMusicTrackTitles =
	   {
			"lost painting",
			"cursed sanctuary",
			"requiem for the gods",
			"rainbow cemetary",
			"wood carving partita",
			"crystal teardrops",
			"marble gallery",
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
		public static readonly Dictionary<string, string> AlternateTrackTitles = new Dictionary<string, string>
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
			{ "rainbow cemetery", "rainbow cemetary" },
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

		public const string KhaosName = "Khaos";

		public const float SuperWeakenFactor = 0.7F;
		public const float SuperCrippleFactor = 0.5F;
		public const int SlowQueueIntervalEnd = 3;
		public const int FastQueueIntervalStart = 8;
		public const uint SuperThirstExtraDrain = 2u;
		public const int HelpItemRetryCount = 15;
		public const float BattleOrdersHpMultiplier = 2F;
		public const uint GuiltyGearInvincibility = 3;
		public const uint GuiltyGearAttack = 50;
		public const uint GuiltyGearDefence = 50;
		public const uint GuiltyGearDarkMetamorphosis = 50;
		public const uint ShaftKhaosHp = 25;
		public const uint GalamothKhaosHp = 2000;
		public const uint GalamothKhaosPositionOffset = 100;
		public const float HasteDashFactor = 1.8F;
		public const int SaveIcosahedronFirstCastle = 0xBC9E;
		public const int SaveIcosahedronSecondCastle = 0x1144;
		public const int KhaosActionsCount = 30;

		public const int AutoKhaosDifficultyEasy = 70;
		public const int AutoKhaosDifficultyNormal = 50;
		public const int AutoKhaosDifficultyHard = 20;

		public const uint MinimumHp = 70;
		public const uint MinimumMp = 30;
		public const uint MinimumHearts = 60;
		public const uint MinimumStat = 6;
	}
}

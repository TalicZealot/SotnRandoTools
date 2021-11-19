using System.Collections.Generic;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Models;

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
		public static List<SearchableActor> AcceptedHordeEnemies = new List<SearchableActor>
		{
			new SearchableActor {Hp = 1, Damage = 14, Sprite = 25776 },
			new SearchableActor {Hp = 1, Damage = 16, Sprite = 25188 },
			new SearchableActor {Hp = 18 , Damage = 5, Sprite = 23212  },
			new SearchableActor {Hp = 24 , Damage = 10, Sprite = 42580  },
			new SearchableActor {Hp = 48, Damage = 13, Sprite = 4612 },
			new SearchableActor {Hp = 18, Damage = 5, Sprite = 14308  },
			new SearchableActor {Hp = 9, Damage = 8, Sprite = 31064  },
			new SearchableActor {Hp = 9, Damage = 2, Sprite = 24516 },
			new SearchableActor {Hp = 18, Damage = 7, Sprite = 26412 },
			new SearchableActor {Hp = 32, Damage = 6, Sprite = 17852  },
			new SearchableActor {Hp = 32, Damage = 6, Sprite = 46300  },
			new SearchableActor {Hp = 20, Damage = 4, Sprite = 48588 },
			new SearchableActor {Hp = 12, Damage = 5, Sprite = 30320  },
			new SearchableActor {Hp = 20, Damage = 8, Sprite = 26360 },
			new SearchableActor {Hp = 5, Damage = 4, Sprite = 48588  },
			new SearchableActor {Hp = 11, Damage = 9, Sprite = 51080  },
			new SearchableActor {Hp = 9, Damage = 2, Sprite = 52040  },
			new SearchableActor {Hp = 9, Damage = 2, Sprite = 54896 },
			new SearchableActor {Hp = 20, Damage = 12, Sprite = 14964  },
			new SearchableActor {Hp = 24, Damage = 12, Sprite = 60200  },
			new SearchableActor {Hp = 12, Damage = 12, Sprite = 22572 },
			new SearchableActor {Hp = 32, Damage = 7, Sprite = 49236 },
			new SearchableActor {Hp = 11, Damage = 9, Sprite = 772  },
			new SearchableActor {Hp = 12, Damage = 10, Sprite = 56172 },
			new SearchableActor {Hp = 18, Damage = 17, Sprite = 64000 },
			new SearchableActor {Hp = 10, Damage = 10, Sprite = 18916 },
			new SearchableActor {Hp = 10, Damage = 12, Sprite = 1432  },
			new SearchableActor {Hp = 15, Damage = 14, Sprite = 59616 },
			new SearchableActor {Hp = 12, Damage = 7, Sprite = 916  },
			new SearchableActor {Hp = 26, Damage = 18, Sprite = 43308 },
			new SearchableActor {Hp = 20, Damage = 32, Sprite = 50472 },
			new SearchableActor {Hp = 17, Damage = 18, Sprite = 34488 },
			new SearchableActor {Hp = 42, Damage = 10, Sprite = 38568 },
			new SearchableActor {Hp = 15, Damage = 10, Sprite = 16344  },
			new SearchableActor {Hp = 15, Damage = 12, Sprite = 14276 },
			new SearchableActor {Hp = 30, Damage = 12, Sprite = 12196  },
			new SearchableActor {Hp = 1, Damage = 16, Sprite = 15756 },
			new SearchableActor {Hp = 18, Damage = 12, Sprite = 18060 },
			new SearchableActor {Hp = 24, Damage = 10, Sprite = 21864 },
			new SearchableActor {Hp = 18, Damage = 12, Sprite = 11068 },
			new SearchableActor {Hp = 16, Damage = 15, Sprite = 18404 },
			new SearchableActor {Hp = 24, Damage = 12, Sprite = 20436 },
			new SearchableActor {Hp = 24, Damage = 10, Sprite = 15440 },
			new SearchableActor {Hp = 20, Damage = 12, Sprite = 49068 },
			new SearchableActor {Hp = 1, Damage = 16, Sprite = 36428 },
			new SearchableActor {Hp = 10, Damage = 14, Sprite = 31116 },
			new SearchableActor {Hp = 20, Damage = 12, Sprite = 33464 },
			new SearchableActor {Hp = 2, Damage = 13, Sprite = 33204 },
			new SearchableActor {Hp = 100, Damage = 20, Sprite = 38856 },
			new SearchableActor {Hp = 100, Damage = 20, Sprite = 8932 },
			new SearchableActor {Hp = 99, Damage = 18, Sprite = 64232 },
			new SearchableActor {Hp = 12, Damage = 10, Sprite = 22344 },
			new SearchableActor {Hp = 10, Damage = 20, Sprite = 17300 },
			new SearchableActor {Hp = 22, Damage = 28, Sprite = 10100 },
			new SearchableActor {Hp = 46, Damage = 37, Sprite = 48728 },
			new SearchableActor {Hp = 12, Damage = 7, Sprite = 45404 },
			new SearchableActor {Hp = 5, Damage = 40, Sprite = 54652 },
			new SearchableActor {Hp = 3, Damage = 55, Sprite = 18024 },
			new SearchableActor {Hp = 35, Damage = 45, Sprite = 24640 },
			new SearchableActor {Hp = 43, Damage = 10, Sprite = 14584 },
			new SearchableActor {Hp = 12, Damage = 7, Sprite = 45800 },
			new SearchableActor {Hp = 30, Damage = 56, Sprite = 43916 },
			new SearchableActor {Hp = 280, Damage = 40, Sprite = 29328 },
			new SearchableActor {Hp = 10, Damage = 66, Sprite = 14076 },
			new SearchableActor {Hp = 43, Damage = 10, Sprite = 2536 },
			new SearchableActor {Hp = 12, Damage = 7, Sprite = 9876 },
			new SearchableActor {Hp = 12, Damage = 10, Sprite = 27132 },
			new SearchableActor {Hp = 50, Damage = 40, Sprite = 64648 },
			new SearchableActor {Hp = 1, Damage = 70, Sprite = 33952 },
			new SearchableActor {Hp = 46, Damage = 37, Sprite = 39840 },
			new SearchableActor {Hp = 10, Damage = 100, Sprite = 38772 },
			new SearchableActor {Hp = 200, Damage = 7, Sprite = 27600 },
			new SearchableActor {Hp = 200, Damage = 6, Sprite = 23676 },
			new SearchableActor {Hp = 1, Damage = 16, Sprite = 43228 },
			new SearchableActor {Hp = 12, Damage = 10, Sprite = 33052 },
			new SearchableActor {Hp = 1, Damage = 16, Sprite = 15412 },
			new SearchableActor {Hp = 9, Damage = 8, Sprite = 7536  },
			new SearchableActor {Hp = 9, Damage = 2, Sprite = 8684  }
		};
		public static List<SearchableActor> EnduranceBosses = new List<SearchableActor>
		{
			new SearchableActor {Hp = 200, Damage = 6, Sprite = 18296},    // Slogra
			new SearchableActor {Hp = 200, Damage = 7, Sprite = 22392},    // Gaibon
			new SearchableActor {Hp = 120, Damage = 7, Sprite = 14260},    // Doppleganger 10
			new SearchableActor {Hp = 300, Damage = 20, Sprite = 9884},    // Minotaur
			new SearchableActor {Hp = 260, Damage = 20, Sprite = 14428},   // Werewolf
			new SearchableActor {Hp = 400, Damage = 20, Sprite = 56036},   // Lesser Demon
			new SearchableActor {Hp = 500, Damage = 20, Sprite = 43920},   // Karasuman
			//new SearchableActor {Hp = 800, Damage = 18, Sprite = 7188},  // Hippogryph - Can trigger the door closing and locking the player on the wrong side.
			new SearchableActor {Hp = 666, Damage = 20, Sprite = 54072},   // Olrox
			new SearchableActor {Hp = 666, Damage = 25, Sprite = 8452},    // Succubus
			new SearchableActor {Hp = 800, Damage = 20, Sprite = 19772},   // Cerberus
			new SearchableActor {Hp = 400, Damage = 30, Sprite = 6264},    // Granfaloon
			new SearchableActor {Hp = 400, Damage = 25, Sprite = 27332},   // Richter
			new SearchableActor {Hp = 600, Damage = 35, Sprite = 40376},   // Darkwing Bat
			new SearchableActor {Hp = 1100, Damage = 30, Sprite = 31032},  // Creature
			new SearchableActor {Hp = 777, Damage = 35, Sprite = 11664},   // Doppleganger 40
			new SearchableActor {Hp = 888, Damage = 35, Sprite = 46380},   // Death
			new SearchableActor {Hp = 1100, Damage = 35, Sprite = 6044},   // Medusa
			new SearchableActor {Hp = 1200, Damage = 40, Sprite = 16564},  // Akmodan
			new SearchableActor {Hp = 1000, Damage = 9, Sprite = 30724},   // Sypha
			new SearchableActor {Hp = 1300, Damage = 40, Sprite = 43772}   // Shaft
		};
		public static SearchableActor GalamothTorsoActor = new SearchableActor { Hp = 12000, Damage = 50, Sprite = 23936 };
		public static SearchableActor GalamothHeadActor = new SearchableActor { Hp = 32767, Damage = 50, Sprite = 31516 };
		public static SearchableActor GalamothPartsActors = new SearchableActor { Hp = 12000, Damage = 50, Sprite = 31516 };
		public static SearchableActor ShaftActor = new SearchableActor { Hp = 10, Damage = 0, Sprite = 0 };
		public static int RichterRoomMapMinX = 31;
		public static int RichterRoomMapMaxX = 34;
		public static int RichterRoomMapY = 8;
		public static int EntranceCutsceneMapMinX = 0;
		public static int EntranceCutsceneMapMaxX = 18;
		public static int EntranceCutsceneMapY = 44;
		public static int GalamothRoomMapMinX = 44;
		public static int GalamothRoomMapMaxX = 45;
		public static int GalamothRoomMapMinY = 12;
		public static int GalamothRoomMapMaxY = 13;
		public static int SuccubusMapX = 0;
		public static int SuccubusMapY = 0;
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
	}
}

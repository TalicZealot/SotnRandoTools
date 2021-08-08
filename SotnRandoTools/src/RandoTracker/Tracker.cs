using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BizHawk.Client.Common;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools.RandoTracker
{
	public class Tracker : ITracker
	{
		const string DefaultSeedInfo = "seed(preset)";

		private readonly IGraphics? formGraphics;
		private readonly IToolConfig toolConfig;
		private readonly TrackerGraphicsEngine trackerGraphicsEngine;
		private readonly IWatchlistService watchlistService;
		private readonly IRenderingApi renderingApi;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;

		private List<Relic> relics = new List<Relic>
		{
				new Relic { Name = "SoulOfBat", Progression = true},
				new Relic { Name = "FireOfBat", Progression = false},
				new Relic { Name = "EchoOfBat", Progression = true},
				new Relic { Name = "ForceOfEcho", Progression = false},
				new Relic { Name = "SoulOfWolf", Progression = true},
				new Relic { Name = "PowerOfWolf", Progression = true},
				new Relic { Name = "SkillOfWolf", Progression = true},
				new Relic { Name = "FormOfMist", Progression = true},
				new Relic { Name = "PowerOfMist", Progression = true},
				new Relic { Name = "GasCloud", Progression = false},
				new Relic { Name = "CubeOfZoe", Progression = false},
				new Relic { Name = "SpiritOrb", Progression = false},
				new Relic { Name = "GravityBoots", Progression = true},
				new Relic { Name = "LeapStone", Progression = true},
				new Relic { Name = "HolySymbol", Progression = false},
				new Relic { Name = "FaerieScroll", Progression = false},
				new Relic { Name = "JewelOfOpen", Progression = true},
				new Relic { Name = "MermanStatue", Progression = true},
				new Relic { Name = "BatCard", Progression = false},
				new Relic { Name = "GhostCard", Progression = false},
				new Relic { Name = "FaerieCard", Progression = false},
				new Relic { Name = "DemonCard", Progression = false},
				new Relic { Name = "SwordCard", Progression = false},
				new Relic { Name = "SpriteCard" , Progression = false},
				new Relic { Name = "NoseDevilCard", Progression = false},
				new Relic { Name = "HeartOfVlad", Progression = true},
				new Relic { Name = "ToothOfVlad", Progression = true},
				new Relic { Name = "RibOfVlad", Progression = true},
				new Relic { Name = "RingOfVlad", Progression = true},
				new Relic { Name = "EyeOfVlad", Progression = true}
		};
		private List<Location> locations = new List<Location>
		{
			new Location { Name = "SoulOfBat", MapRow = 132, MapCol = 386, Rooms = new List<Room>{ new Room { Name = "SoulOfBat", Values = new int[] { 0x01 }}}},
			new Location { Name = "FireOfBat", MapRow = 52, MapCol = 474,  Rooms = new List<Room>{ new Room { Name = "FireOfBat", Values = new int[] { 0x01 } }}},
			new Location { Name = "EchoOfBat",  MapRow = 92, MapCol = 130,  Rooms = new List<Room>{ new Room { Name = "EchoOfBat", Values = new int[] { 0x04 } }}},
			new Location { Name = "SoulOfWolf",  MapRow = 108, MapCol = 490,  Rooms = new List<Room>
			{ new Room { Name = "SoulOfWolf1", Values = new int[] { 0x10 } }, new Room { Name = "SoulOfWolf2", Values = new int[] { 0x10 } }}},
			new Location { Name = "PowerOfWolf",  MapRow = 268, MapCol = 18,  Rooms = new List<Room>{ new Room { Name = "PowerOfWolf", Values = new int[] { 0x01, 0x04 } }}},
			new Location { Name = "SkillOfWolf",  MapRow = 228, MapCol = 122,  Rooms = new List<Room>{ new Room { Name = "SkillOfWolf", Values = new int[] { 0x01 } }}},
			new Location { Name = "FormOfMist",  MapRow = 140, MapCol = 170,  Rooms = new List<Room>{ new Room { Name = "FormOfMist", Values = new int[] { 0x04, 0x10 } }}},
			new Location { Name = "PowerOfMist",  MapRow = 36, MapCol = 250,  Rooms = new List<Room>
			{ new Room { Name = "PowerOfMist1", Values = new int[] { 0x01 } }, new Room { Name = "PowerOfMist2", Values = new int[] { 0x01 } }, new Room { Name = "PowerOfMist3", Values = new int[] { 0x40 } }}},
			new Location { Name = "CubeOfZoe",  MapRow = 252, MapCol = 154,  Rooms = new List<Room>{ new Room { Name = "CubeOfZoe", Values = new int[] { 0x01, 0x04 } }}},
			new Location { Name = "SpiritOrb",  MapRow = 214, MapCol = 202,  Rooms = new List<Room>
			{ new Room { Name = "SpiritOrb1", Values = new int[] { 0x10 } }, new Room { Name = "SpiritOrb2", Values = new int[] { 0x04 } }}},
			new Location { Name = "GravityBoots",  MapRow = 148, MapCol = 274,  Rooms = new List<Room>{ new Room { Name = "GravityBoots", Values = new int[] { 0x04 } }}},
			new Location { Name = "LeapStone",  MapRow = 52, MapCol = 250,  Rooms = new List<Room>
			{ new Room { Name = "LeapStone1", Values = new int[] { 0x01 } }, new Room { Name = "LeapStone2", Values = new int[] { 0x01 } }}},
			new Location { Name = "HolySymbol",  MapRow = 292, MapCol = 442,  Rooms = new List<Room>{ new Room { Name = "HolySymbol", Values = new int[] { 0x01 } }}},
			new Location { Name = "FaerieScroll",  MapRow = 108, MapCol = 474,  Rooms = new List<Room>{ new Room { Name = "FaerieScroll", Values = new int[] { 0x01 } }}},
			new Location { Name = "JewelOfOpen",  MapRow = 124, MapCol = 394,  Rooms = new List<Room>{ new Room { Name = "JewelOfOpen", Values = new int[] { 0x10 } }}},
			new Location { Name = "MermanStatue",  MapRow = 300, MapCol = 66,  Rooms = new List<Room>{ new Room { Name = "MermanStatue", Values = new int[] { 0x40 } }}},
			new Location { Name = "BatCard",  MapRow = 180, MapCol = 106,  Rooms = new List<Room>{ new Room { Name = "BatCard", Values = new int[] { 0x10 } }}},
			new Location { Name = "GhostCard",  MapRow = 20, MapCol = 314,  Rooms = new List<Room>{ new Room { Name = "GhostCard", Values = new int[] { 0x01, 0x04 } }}},
			new Location { Name = "FaerieCard",  MapRow = 108, MapCol = 418,  Rooms = new List<Room>{ new Room { Name = "FaerieCard", Values = new int[] { 0x40 } }}},
			new Location { Name = "DemonCard",  MapRow = 316, MapCol = 234,  Rooms = new List<Room>{ new Room { Name = "DemonCard", Values = new int[] { 0x10 } }}},
			new Location { Name = "SwordCard",  MapRow = 108, MapCol = 162,  Rooms = new List<Room>{ new Room { Name = "SwordCard", Values = new int[] { 0x40 } }}},
			new Location { Name = "CrystalCloak", GuardedExtension = true,  MapRow = 268, MapCol = 322,  Rooms = new List<Room>{ new Room { Name = "CrystalCloak", Values = new int[] { 0x40 } }}},
			new Location { Name = "Mormegil", GuardedExtension = true,  MapRow = 364, MapCol = 138,  Rooms = new List<Room>{ new Room { Name = "Mormegil", Values = new int[] { 0x10 } }}},
			new Location { Name = "GoldRing",  MapRow = 228, MapCol = 362,  Rooms = new List<Room>{ new Room { Name = "GoldRing", Values = new int[] { 0x10 } }}},
			new Location { Name = "Spikebreaker",  MapRow = 372, MapCol = 330,  Rooms = new List<Room>{ new Room { Name = "Spikebreaker", Values = new int[] { 0x10 } }}},
			new Location { Name = "SilverRing",  MapRow = 84, MapCol = 68,  Rooms = new List<Room>{ new Room { Name = "SilverRing", Values = new int[] { 0x10 } }}},
			new Location { Name = "HolyGlasses",  MapRow = 212, MapCol = 258,  Rooms = new List<Room>{ new Room { Name = "HolyGlasses", Values = new int[] { 0x40 } }}},
			new Location { Name = "HeartOfVlad", SecondCastle = true,  MapRow = 330, MapCol = 320,  Rooms = new List<Room>
			{ new Room { Name = "HeartOfVlad1", Values = new int[] { 0x01 } }, new Room { Name = "HeartOfVlad2", Values = new int[] { 0x40 } }}},
			new Location { Name = "ToothOfVlad", SecondCastle = true,  MapRow = 250, MapCol = 48,  Rooms = new List<Room>{ new Room { Name = "ToothOfVlad", Values = new int[] { 0x10, 0x04 } }}},
			new Location { Name = "RibOfVlad", SecondCastle = true,  MapRow = 306, MapCol = 352,  Rooms = new List<Room>{ new Room { Name = "RibOfVlad", Values = new int[] { 0x01 } }}},
			new Location { Name = "RingOfVlad", SecondCastle = true,  MapRow = 354, MapCol = 184,  Rooms = new List<Room>{ new Room { Name = "RingOfVlad", Values = new int[] { 0x01 } }}},
			new Location { Name = "EyeOfVlad", SecondCastle = true,  MapRow = 114, MapCol = 264,  Rooms = new List<Room>{ new Room { Name = "EyeOfVlad", Values = new int[] { 0x10, 0x40 } }}},
			new Location { Name = "ForceOfEcho", SecondCastle = true,  MapRow = 106, MapCol = 64,  Rooms = new List<Room>{ new Room { Name = "ForceOfEcho", Values = new int[] { 0x40 } }}},
			new Location { Name = "GasCloud", SecondCastle = true,  MapRow = 34, MapCol = 368,  Rooms = new List<Room>{ new Room { Name = "GasCloud", Values = new int[] { 0x04 } }}},
			new Location { Name = "RingOfArcana", GuardedExtension = true, SecondCastle = true,  MapRow = 218, MapCol = 400,  Rooms = new List<Room>{ new Room { Name = "RingOfArcana", Values = new int[] { 0x04 } }}},
			new Location { Name = "DarkBlade", GuardedExtension = true, SecondCastle = true,  MapRow = 130, MapCol = 184,  Rooms = new List<Room>{ new Room { Name = "DarkBlade", Values = new int[] { 0x01 } }}},
			new Location { Name = "Trio", GuardedExtension = true, SecondCastle = true,  MapRow = 258, MapCol = 344,  Rooms = new List<Room>
			{ new Room { Name = "Trio1", Values = new int[] { 0x40 } }, new Room { Name = "Trio2", Values = new int[] { 0x01 } }}},
			new Location { Name = "Walk armor", EquipmentExtension = true, MapRow = 364, MapCol = 186, Rooms = new List<Room>{ new Room { Name = "Walk armor", Values = new int[] { 0x1 }} }},
			new Location { Name = "Icebrand", EquipmentExtension = true, MapRow = 364, MapCol = 194, Rooms = new List<Room>{ new Room { Name = "Icebrand", Values = new int[] { 0x40 }} }},
			//new Location { Name = "Balroom Mask", EquipmentExtension = true, MapRow = 364, MapCol = 210, Rooms = new List<Room>{ new Room { Name = "Balroom Mask", Values = new int[] { 0x4 }} }},
			new Location { Name = "Bloodstone", EquipmentExtension = true, MapRow = 364, MapCol = 226, Rooms = new List<Room>{ new Room { Name = "Bloodstone", Values = new int[] { 0x40 }} }},
			new Location { Name = "Combat knife", EquipmentExtension = true, MapRow = 348, MapCol = 250, Rooms = new List<Room>{ new Room { Name = "Combat knife", Values = new int[] { 0x1 }} }},
			new Location { Name = "Ring of Ares", EquipmentExtension = true, MapRow = 292, MapCol = 298, Rooms = new List<Room>{ new Room { Name = "Ring of Ares", Values = new int[] { 0x10 }} }},
			new Location { Name = "Knuckle duster", EquipmentExtension = true, MapRow = 300, MapCol = 322, Rooms = new List<Room>{ new Room { Name = "Knuckle duster", Values = new int[] { 0x40 }} }},
			new Location { Name = "Caverns Onyx", EquipmentExtension = true, MapRow = 292, MapCol = 370, Rooms = new List<Room>{ new Room { Name = "Caverns Onyx", Values = new int[] { 0x10 }} }},
			//new Location { Name = "Scimitar", EquipmentExtension = true, MapRow = 276, MapCol = 362, Rooms = new List<Room>{ new Room { Name = "Scimitar", Values = new int[] { 0x10 }} }},
			//new Location { Name = "Moonstone", EquipmentExtension = true, MapRow = 228, MapCol = 354, Rooms = new List<Room>{ new Room { Name = "Moonstone", Values = new int[] { 0x40 }} }},
			//new Location { Name = "Claymore", EquipmentExtension = true, MapRow = 196, MapCol = 346, Rooms = new List<Room>{ new Room { Name = "Claymore", Values = new int[] { 0x1 }} }},
			new Location { Name = "Bandanna", EquipmentExtension = true, MapRow = 180, MapCol = 282, Rooms = new List<Room>{ new Room { Name = "Bandanna", Values = new int[] { 0x1 }} }},
			new Location { Name = "Nunchaku", EquipmentExtension = true, MapRow = 268, MapCol = 306, Rooms = new List<Room>{ new Room { Name = "Nunchaku", Values = new int[] { 0x4, 0x10 }} }},
			new Location { Name = "Secret Boots", EquipmentExtension = true, MapRow = 276, MapCol = 194, Rooms = new List<Room>{ new Room { Name = "Secret boots1", Values = new int[] { 0x1 }}, new Room { Name = "Secret boots2", Values = new int[] { 0x1 }} }},
			//new Location { Name = "Herald Shield", EquipmentExtension = true, MapRow = 260, MapCol = 178, Rooms = new List<Room>{ new Room { Name = "Herald Shield", Values = new int[] { 0x4 }} }},
			new Location { Name = "Holy mail", EquipmentExtension = true, MapRow = 268, MapCol = 42, Rooms = new List<Room>{ new Room { Name = "Holy mail", Values = new int[] { 0x10 }} }},
			new Location { Name = "Jewel sword", EquipmentExtension = true, MapRow = 292, MapCol = 82, Rooms = new List<Room>{ new Room { Name = "Jewel sword", Values = new int[] { 0x4 }} }},
			//new Location { Name = "Hide Cuirass", EquipmentExtension = true, MapRow = 252, MapCol = 114, Rooms = new List<Room>{ new Room { Name = "Hide Cuirass", Values = new int[] { 0x4 }} }},
			//new Location { Name = "Leather Shield", EquipmentExtension = true, MapRow = 244, MapCol = 106, Rooms = new List<Room>{ new Room { Name = "Leather Shield", Values = new int[] { 0x10 }} }},
			new Location { Name = "Sunglasses", EquipmentExtension = true, MapRow = 212, MapCol = 130, Rooms = new List<Room>{ new Room { Name = "Sunglasses", Values = new int[] { 0x40 }} }},
			new Location { Name = "Basilard", EquipmentExtension = true, MapRow = 236, MapCol = 130, Rooms = new List<Room>{ new Room { Name = "Basilard", Values = new int[] { 0x40 }} }},
			new Location { Name = "Cloth cape", EquipmentExtension = true, MapRow = 196, MapCol = 82, Rooms = new List<Room>{ new Room { Name = "Cloth cape", Values = new int[] { 0x4 }} }},
			new Location { Name = "Mystic pendant", EquipmentExtension = true, MapRow = 172, MapCol = 34, Rooms = new List<Room>{ new Room { Name = "Mystic pendant1", Values = new int[] { 0x40 }}, new Room { Name = "Mystic pendant2", Values = new int[] { 0x40 }} }},
			new Location { Name = "Ankh of Life", EquipmentExtension = true, MapRow = 156, MapCol = 50, Rooms = new List<Room>{ new Room { Name = "Ankh of Life", Values = new int[] { 0x4, 0x1 }} }},
			new Location { Name = "Morningstar", EquipmentExtension = true, MapRow = 132, MapCol = 66, Rooms = new List<Room>{ new Room { Name = "Morningstar1", Values = new int[] { 0x10 }}, new Room { Name = "Morningstar2", Values = new int[] { 0x10 }} }},
			new Location { Name = "Goggles", EquipmentExtension = true, MapRow = 132, MapCol = 82, Rooms = new List<Room>{ new Room { Name = "Goggles", Values = new int[] { 0x4 }} }},
			new Location { Name = "Silver plate", EquipmentExtension = true, MapRow = 60, MapCol = 114, Rooms = new List<Room>{ new Room { Name = "Silver plate", Values = new int[] { 0x4, 0x1 }} }},
			new Location { Name = "Cutlass", EquipmentExtension = true, MapRow = 44, MapCol = 218, Rooms = new List<Room>{ new Room { Name = "Cutlass1", Values = new int[] { 0x40 }}, new Room { Name = "Cutlass2", Values = new int[] { 0x1 }} }},
			new Location { Name = "Platinum mail", EquipmentExtension = true, MapRow = 12, MapCol = 282, Rooms = new List<Room>{ new Room { Name = "Platinum mail", Values = new int[] { 0x1 }} }},
			new Location { Name = "Falchion", EquipmentExtension = true, MapRow = 28, MapCol = 314, Rooms = new List<Room>{ new Room { Name = "Falchion", Values = new int[] { 0x1 }} }},
			new Location { Name = "Gold plate", EquipmentExtension = true, MapRow = 76, MapCol = 394, Rooms = new List<Room>{ new Room { Name = "Gold plate", Values = new int[] { 0x10 }} }},
			new Location { Name = "Bekatowa", EquipmentExtension = true, MapRow = 76, MapCol = 446, Rooms = new List<Room>{ new Room { Name = "Bekatowa1", Values = new int[] { 0x1, 0x4 }}, new Room { Name = "Bekatowa2", Values = new int[] { 0x1, 0x4 }} }},
			new Location { Name = "Gladius", EquipmentExtension = true, MapRow = 148, MapCol = 474, Rooms = new List<Room>{ new Room { Name = "Gladius", Values = new int[] { 0x1 }} }},
			new Location { Name = "Jewel knuckles", EquipmentExtension = true, MapRow = 180, MapCol = 474, Rooms = new List<Room>{ new Room { Name = "Jewel knuckles", Values = new int[] { 0x1 }} }},
			new Location { Name = "Bronze cuirass", EquipmentExtension = true, MapRow = 132, MapCol = 394, Rooms = new List<Room>{ new Room { Name = "Bronze cuirass", Values = new int[] { 0x10 }} }},
			new Location { Name = "Holy rod", EquipmentExtension = true, MapRow = 108, MapCol = 402, Rooms = new List<Room>{ new Room { Name = "Holy rod", Values = new int[] { 0x4 }} }},
			new Location { Name = "Library Onyx", EquipmentExtension = true, MapRow = 132, MapCol = 378, Rooms = new List<Room>{ new Room { Name = "Library Onyx", Values = new int[] { 0x4 }} }},
			new Location { Name = "Alucart Sword", EquipmentExtension = true, MapRow = 164, MapCol = 274, Rooms = new List<Room>{ new Room { Name = "Alucart sword", Values = new int[] { 0x4 }} }},
			new Location { Name = "Broadsword", EquipmentExtension = true, MapRow = 140, MapCol = 258, Rooms = new List<Room>{ new Room { Name = "Broadsword", Values = new int[] { 0x40 }} }},
			new Location { Name = "Estoc", EquipmentExtension = true, MapRow = 84, MapCol = 242, Rooms = new List<Room>{ new Room { Name = "Estoc", Values = new int[] { 0x4 }} }},
			new Location { Name = "Olrox Garnet", EquipmentExtension = true, MapRow = 108, MapCol = 266, Rooms = new List<Room>{ new Room { Name = "Olrox garnet", Values = new int[] { 0x10 }} }},
			new Location { Name = "Holy sword", EquipmentExtension = true, MapRow = 124, MapCol = 154, Rooms = new List<Room>{ new Room { Name = "Holy sword", Values = new int[] { 0x1 }} }},
			new Location { Name = "Knight shield", EquipmentExtension = true, MapRow = 140, MapCol = 114, Rooms = new List<Room>{ new Room { Name = "Knight shield", Values = new int[] { 0x1, 0x4 }} }},
			new Location { Name = "Shield rod", EquipmentExtension = true, MapRow = 156, MapCol = 106, Rooms = new List<Room>{ new Room { Name = "Shield rod", Values = new int[] { 0x10 }} }},
			new Location { Name = "Blood cloak", EquipmentExtension = true, MapRow = 156, MapCol = 162, Rooms = new List<Room>{ new Room { Name = "Blood cloak", Values = new int[] { 0x40 }} }},
			new Location { Name = "Bastard sword", EquipmentExtension = true, SecondCastle = true, MapRow = 386, MapCol = 240, Rooms = new List<Room>{ new Room { Name = "Bastard sword", Values = new int[] { 0x4 }} }},
			new Location { Name = "Royal cloak", EquipmentExtension = true, SecondCastle = true, MapRow = 386, MapCol = 224, Rooms = new List<Room>{ new Room { Name = "Royal cloak", Values = new int[] { 0x40 }} }},
			new Location { Name = "Lightning mail", EquipmentExtension = true, SecondCastle = true, MapRow = 346, MapCol = 192, Rooms = new List<Room>{ new Room { Name = "Lightning mail", Values = new int[] { 0x40 }} }},
			new Location { Name = "Sword of Dawn", EquipmentExtension = true, SecondCastle = true, MapRow = 346, MapCol = 256, Rooms = new List<Room>{ new Room { Name = "Sword of Dawn1", Values = new int[] { 0x40 }}, new Room { Name = "Sword of Dawn2", Values = new int[] { 0x1 }} }},
			new Location { Name = "Moon rod", EquipmentExtension = true, SecondCastle = true, MapRow = 346, MapCol = 168, Rooms = new List<Room>{ new Room { Name = "Moon rod", Values = new int[] { 0x10 }} }},
			new Location { Name = "Sunstone", EquipmentExtension = true, SecondCastle = true, MapRow = 322, MapCol = 112, Rooms = new List<Room>{ new Room { Name = "Sunstone", Values = new int[] { 0x4 }} }},
			new Location { Name = "Luminus", EquipmentExtension = true, SecondCastle = true, MapRow = 328, MapCol = 64, Rooms = new List<Room>{ new Room { Name = "Luminus", Values = new int[] { 0x10, 0x40 }} }},
			new Location { Name = "Dragon helm", EquipmentExtension = true, SecondCastle = true, MapRow = 346, MapCol = 32, Rooms = new List<Room>{ new Room { Name = "Dragon helm", Values = new int[] { 0x40 }} }},
			new Location { Name = "Shotel", EquipmentExtension = true, SecondCastle = true, MapRow = 218, MapCol = 32, Rooms = new List<Room>{ new Room { Name = "Shotel", Values = new int[] { 0x40 }} }},
			new Location { Name = "Badelaire", EquipmentExtension = true, SecondCastle = true, MapRow = 290, MapCol = 104, Rooms = new List<Room>{ new Room { Name = "Badelaire", Values = new int[] { 0x10 }} }},
			new Location { Name = "Staurolite", EquipmentExtension = true, SecondCastle = true, MapRow = 264, MapCol = 120, Rooms = new List<Room>{ new Room { Name = "Staurolite", Values = new int[] { 0x40 }} }},
			new Location { Name = "Forbidden Library Opal", EquipmentExtension = true, SecondCastle = true, MapRow = 274, MapCol = 110, Rooms = new List<Room>{ new Room { Name = "Forbidden Library Opal", Values = new int[] { 0x04 }} }},
			new Location { Name = "Reverse Caverns Diamond", EquipmentExtension = true, SecondCastle = true, MapRow = 218, MapCol = 224, Rooms = new List<Room>{ new Room { Name = "Reverse Caverns Diamond", Values = new int[] { 0x40 }} }},
			new Location { Name = "Reverse Caverns Opal", EquipmentExtension = true, SecondCastle = true, MapRow = 178, MapCol = 208, Rooms = new List<Room>{ new Room { Name = "Reverse Caverns Opal", Values = new int[] { 0x4 }} }},
			new Location { Name = "Reverse Caverns Garnet", EquipmentExtension = true, SecondCastle = true, MapRow = 138, MapCol = 328, Rooms = new List<Room>{ new Room { Name = "Reverse Caverns Garnet", Values = new int[] { 0x10 }} }},
			new Location { Name = "Osafune katana", EquipmentExtension = true, SecondCastle = true, MapRow = 98, MapCol = 304, Rooms = new List<Room>{ new Room { Name = "Osafune katana", Values = new int[] { 0x4 }} }},
			new Location { Name = "Alucard shield", EquipmentExtension = true, SecondCastle = true, MapRow = 98, MapCol = 440, Rooms = new List<Room>{ new Room { Name = "Alucard shield", Values = new int[] { 0x1 }} }},
			new Location { Name = "Alucard sword", EquipmentExtension = true, SecondCastle = true, MapRow = 82, MapCol = 272, Rooms = new List<Room>{ new Room { Name = "Alucard sword", Values = new int[] { 0x4 }} }},
			new Location { Name = "Necklace of J", EquipmentExtension = true, SecondCastle = true, MapRow = 34, MapCol = 312, Rooms = new List<Room>{ new Room { Name = "Necklace of J", Values = new int[] { 0x1 }} }},
			new Location { Name = "Floating Catacombs Diamond", EquipmentExtension = true, SecondCastle = true, MapRow = 34, MapCol = 320, Rooms = new List<Room>{ new Room { Name = "Floating Catacombs Diamond", Values = new int[] { 0x40 }} }},
			new Location { Name = "Talwar", EquipmentExtension = true, SecondCastle = true, MapRow = 346, MapCol = 352, Rooms = new List<Room>{ new Room { Name = "Talwar1", Values = new int[] { 0x1 }}, new Room { Name = "Talwar2", Values = new int[] { 0x40 }} }},
			new Location { Name = "Twilight cloak", EquipmentExtension = true, SecondCastle = true, MapRow = 314, MapCol = 440, Rooms = new List<Room>{ new Room { Name = "Twilight cloak", Values = new int[] { 0x4 }} }},
			new Location { Name = "Alucard Mail", EquipmentExtension = true, SecondCastle = true, MapRow = 290, MapCol = 240, Rooms = new List<Room>{ new Room { Name = "Alucard mail", Values = new int[] { 0x4 }} }},
			new Location { Name = "Sword of Hador", EquipmentExtension = true, SecondCastle = true, MapRow = 258, MapCol = 248, Rooms = new List<Room>{ new Room { Name = "Sword of Hador", Values = new int[] { 0x1 }} }},
			new Location { Name = "Fury Plate", EquipmentExtension = true, SecondCastle = true, MapRow = 274, MapCol = 352, Rooms = new List<Room>{ new Room { Name = "Fury plate", Values = new int[] { 0x40 }} }},
			new Location { Name = "Gram", EquipmentExtension = true, SecondCastle = true, MapRow = 242, MapCol = 344, Rooms = new List<Room>{ new Room { Name = "Gram", Values = new int[] { 0x1 }} }},
			new Location { Name = "Goddess shield", EquipmentExtension = true, SecondCastle = true, MapRow = 186, MapCol = 376, Rooms = new List<Room>{ new Room { Name = "Goddess shield", Values = new int[] { 0x1 }} }},
			new Location { Name = "Katana", EquipmentExtension = true, SecondCastle = true, MapRow = 146, MapCol = 408, Rooms = new List<Room>{ new Room { Name = "Katana", Values = new int[] { 0x1 }} }},
			new Location { Name = "Talisman", EquipmentExtension = true, SecondCastle = true, MapRow = 138, MapCol = 344, Rooms = new List<Room>{ new Room { Name = "Talisman", Values = new int[] { 0x1 }} }},
			new Location { Name = "Beryl circlet", EquipmentExtension = true, SecondCastle = true, MapRow = 106, MapCol = 424, Rooms = new List<Room>{ new Room { Name = "Beryl circlet", Values = new int[] { 0x10 }} }},
		};
		private List<Item> progressionItems = new List<Item>
		{
			new Item { Name = "GoldRing", Value = 72 },
			new Item { Name = "SilverRing", Value = 73 },
			new Item { Name = "SpikeBreaker", Value = 14 },
			new Item { Name = "HolyGlasses", Value = 34 }
		};
		private List<Item> thrustSwords = new List<Item>
		{
			new Item { Name = "Estoc", Value = 95 },
			new Item { Name = "Claymore", Value = 98 },
			new Item { Name = "Flamberge", Value = 101 },
			new Item { Name = "Zweihander", Value = 103 },
			new Item { Name = "ObsidianSword", Value = 107 }
		};

		private string preset = "";
		private uint roomCount = 2;
		private bool guardedExtension = true;
		private bool equipmentExtension = false;
		private bool gameReset = true;
		private bool secondCastle = false;
		private bool restarted = false;
		private bool relicOrItemCollected = false;
		private List<MapLocation> replay = new();
		private int prologueTime = 0;

		public Tracker(IGraphics? formGraphics, IToolConfig toolConfig, IWatchlistService watchlistService, IRenderingApi renderingApi, IGameApi gameApi, IAlucardApi alucardApi)
		{
			if (formGraphics is null) throw new ArgumentNullException(nameof(formGraphics));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (renderingApi is null) throw new ArgumentNullException(nameof(renderingApi));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			this.formGraphics = formGraphics;
			this.toolConfig = toolConfig;
			this.watchlistService = watchlistService;
			this.renderingApi = renderingApi;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;

			if (toolConfig.Tracker.Locations)
			{
				InitializeAllLocks();
				CheckReachability();
			}
			trackerGraphicsEngine = new TrackerGraphicsEngine(formGraphics, relics, progressionItems, thrustSwords, toolConfig);
			trackerGraphicsEngine.CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			this.GraphicsEngine = trackerGraphicsEngine;
			this.SeedInfo = DefaultSeedInfo;
		}

		public string SeedInfo { get; set; }

		public TrackerGraphicsEngine GraphicsEngine { get; }

		public void DrawRelicsAndItems()
		{
			trackerGraphicsEngine.Render();
			trackerGraphicsEngine.DrawSeedInfo(SeedInfo);
		}

		public void Update()
		{
			UpdateSeedLabel();

			bool inGame = gameApi.Status == Status.InGame;
			bool updatedSecondCastle = gameApi.SecondCastle;
			relicOrItemCollected = false;

			if (gameApi.InAlucardMode())
			{
				restarted = false;

				if (updatedSecondCastle != secondCastle && toolConfig.Tracker.Locations)
				{
					secondCastle = updatedSecondCastle;
					SetMapLocations();
				}
				else if (updatedSecondCastle != secondCastle)
				{
					secondCastle = updatedSecondCastle;
				}

				UpdateRelics();
				UpdateProgressionItems();
				UpdateThrustSwords();
				SaveReplayLine();

				if (toolConfig.Tracker.Locations)
				{
					UpdateLocations();
				}
				if (gameReset && toolConfig.Tracker.Locations)
				{
					SetMapLocations();
					gameReset = false;
				}
				if (!LocationsDrawn() && toolConfig.Tracker.Locations)
				{
					SetMapLocations();
				}
			}
			else if (!inGame)
			{
				gameReset = true;
			}
			else if (gameApi.InPrologue())
			{
				prologueTime++;
				if (!restarted)
				{
					ResetToDefaults();
					DrawRelicsAndItems();
					restarted = true;
				}
			}
		}

		private void InitializeAllLocks()
		{
			LoadLocks(Paths.CasualPresetPath, false);
			LoadLocks(Paths.SafePresetPath, false);
			LoadLocks(Paths.SpeedrunPresetPath, true);
		}

		private void LoadLocks(string presetFilePath, bool outOfLogic)
		{
			var presetLocations = JObject.Parse(File.ReadAllText(presetFilePath))["lockLocation"];
			foreach (var location in presetLocations)
			{
				string name = location["location"].ToString().Replace(" ", String.Empty).ToLower();
				var trackerLocation = locations.Where(x => x.Name.Replace(" ", String.Empty).ToLower() == name).FirstOrDefault();
				if (trackerLocation != null)
				{
					foreach (var lockSet in location["locks"])
					{
						if (outOfLogic)
						{
							trackerLocation.OutOfLogicLocks.Add(lockSet.ToString().Replace(" ", String.Empty).ToLower().Split('+'));
						}
						else
						{
							trackerLocation.Locks.Add(lockSet.ToString().Replace(" ", String.Empty).ToLower().Split('+'));
						}
					}
				}
				else
				{
					Console.WriteLine($"Could not find location {name}.");
				}
			}
		}

		private void ResetToDefaults()
		{
			roomCount = 2;
			foreach (var relic in relics)
			{
				relic.Status = false;
			}
			foreach (var item in progressionItems)
			{
				item.Status = false;
			}
			foreach (var sword in thrustSwords)
			{
				sword.Status = false;
			}
			foreach (var location in locations)
			{
				location.Status = false;
				location.AvailabilityColor = MapColor.Unavailable;
			}
		}

		private void UpdateSeedLabel()
		{
			if (SeedInfo == DefaultSeedInfo && gameApi.Status == Status.MainMenu)
			{
				getSeedData();
				trackerGraphicsEngine.Render();
				trackerGraphicsEngine.DrawSeedInfo(SeedInfo);
			}
		}

		private void UpdateLocations()
		{
			uint currentRooms = alucardApi.Rooms;
			if (currentRooms > roomCount)
			{
				roomCount = currentRooms;
				watchlistService.UpdateWatchlist(watchlistService.SafeLocationWatches);
				CheckRooms(watchlistService.SafeLocationWatches);
				if (equipmentExtension)
				{
					watchlistService.UpdateWatchlist(watchlistService.EquipmentLocationWatches);
					CheckRooms(watchlistService.EquipmentLocationWatches);
				}
			}
		}

		private void UpdateRelics()
		{
			watchlistService.UpdateWatchlist(watchlistService.RelicWatches);
			for (int i = 0; i < watchlistService.RelicWatches.Count; i++)
			{
				if (watchlistService.RelicWatches[i].ChangeCount > 0)
				{
					if (watchlistService.RelicWatches[i].Value > 0)
					{
						relics[i].Status = true;
						relicOrItemCollected = true;
					}
					else
					{
						relics[i].Status = false;
					}
					DrawRelicsAndItems();
					if (toolConfig.Tracker.Locations)
					{
						CheckReachability();
					}
				}
			}
			watchlistService.RelicWatches.ClearChangeCounts();
		}

		private void UpdateProgressionItems()
		{
			watchlistService.UpdateWatchlist(watchlistService.ProgressionItemWatches);
			for (int i = 0; i < watchlistService.ProgressionItemWatches.Count; i++)
			{
				if (watchlistService.ProgressionItemWatches[i].ChangeCount > 0)
				{
					if (watchlistService.ProgressionItemWatches[i].Value > 0)
					{
						progressionItems[i].Status = true;
						relicOrItemCollected = true;
					}
					else
					{
						switch (i)
						{
							case 0:
							case 1:
								progressionItems[i].Status = (alucardApi.Accessory1 == progressionItems[i].Value) || (alucardApi.Accessory2 == progressionItems[i].Value);
								break;
							case 2:
								progressionItems[i].Status = (alucardApi.Armor == progressionItems[i].Value);
								break;
							case 3:
								progressionItems[i].Status = (alucardApi.Helm == progressionItems[i].Value);
								break;
							default:
								progressionItems[i].Status = false;
								break;
						}
					}
					DrawRelicsAndItems();
					if (toolConfig.Tracker.Locations)
					{
						CheckReachability();
					}
				}
			}
			watchlistService.ProgressionItemWatches.ClearChangeCounts();
		}

		private void UpdateThrustSwords()
		{
			watchlistService.UpdateWatchlist(watchlistService.ThrustSwordWatches);
			for (int i = 0; i < watchlistService.ThrustSwordWatches.Count; i++)
			{
				if (watchlistService.ThrustSwordWatches[i].ChangeCount > 0)
				{
					if (watchlistService.ThrustSwordWatches[i].Value > 0)
					{
						thrustSwords[i].Status = true;
						relicOrItemCollected = true;
					}
					else
					{
						thrustSwords[i].Status = (alucardApi.RightHand == thrustSwords[i].Value);
					}
					DrawRelicsAndItems();
					if (toolConfig.Tracker.Locations)
					{
						CheckReachability();
					}
				}
			}
			watchlistService.ThrustSwordWatches.ClearChangeCounts();
		}

		private void getSeedData()

		{
			string seedName = gameApi.ReadSeedName();
			preset = gameApi.ReadPresetName();
			if (preset == "tournament" || preset == "")
			{
				preset = "custom";
			}
			SeedInfo = seedName + "(" + preset + ")";
			Console.WriteLine("Randomizer seed information: " + SeedInfo);
			switch (preset)
			{
				case "adventure":
					equipmentExtension = true;
					SetEquipmentProgression();
					break;
				case "glitch":
					equipmentExtension = true;
					relics[25].Progression = false;
					SetEquipmentProgression();
					break;
				case "og":
					guardedExtension = false;
					break;
				case "custom":
					guardedExtension = toolConfig.Tracker.CustomLocationsGuarded;
					equipmentExtension = toolConfig.Tracker.CustomLocationsEquipment;
					if (equipmentExtension)
					{
						SetEquipmentProgression();
					}
					break;
				default:
					break;
			}
		}

		private void SetEquipmentProgression()
		{
			relics.Where(x => x.Name == "CubeOfZoe").FirstOrDefault().Progression = true;
			relics.Where(x => x.Name == "DemonCard").FirstOrDefault().Progression = true;
			relics.Where(x => x.Name == "NoseDevilCard").FirstOrDefault().Progression = true;
			GraphicsEngine.SetProgression();
			GraphicsEngine.CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			GraphicsEngine.Render();
		}

		private void SetMapLocations()
		{
			for (int i = 0; i < locations.Count; i++)
			{
				if (!locations[i].Status && locations[i].SecondCastle == secondCastle)
				{
					if (locations[i].EquipmentExtension && !equipmentExtension)
					{
						continue;
					}
					if (locations[i].GuardedExtension && !guardedExtension)
					{
						continue;
					}
					ColorMapRoom(i, (uint) locations[i].AvailabilityColor, locations[i].SecondCastle);
				}
			}
		}

		private void ClearMapLocation(int index)
		{
			ColorMapRoom(index, (uint) MapColor.Clear, locations[index].SecondCastle);
			foreach (var room in locations[index].Rooms)
			{
				Watch roomWatch = watchlistService.SafeLocationWatches.Where(x => x.Notes.ToLower() == room.Name.ToLower()).FirstOrDefault();
				if (roomWatch is null)
				{
					roomWatch = watchlistService.EquipmentLocationWatches.Where(x => x.Notes.ToLower() == room.Name.ToLower()).FirstOrDefault();
				}
				gameApi.SetRoomToUnvisited(roomWatch.Address);
			}
		}

		private void ColorMapRoom(int i, uint color, bool secondCastle)
		{
			uint row = (uint) locations[i].MapRow / 2;
			uint col = (uint) locations[i].MapCol / 4;
			if (secondCastle)
			{
				row = (398 / 2) - row;
				col = (504 / 4) - col;
			}
			uint borderColor = color > 0 ? (uint) MapColor.Border : 0;

			if (locations[i].EquipmentExtension)
			{
				renderingApi.ColorMapLocation(row, col, color);
			}
			else
			{
				renderingApi.ColorMapRoom(row, col, color, borderColor);
			}
		}

		private bool LocationsDrawn()
		{
			Location uncheckedLocation = locations.Where(l => (!l.Status && l.SecondCastle == secondCastle &&
			(l.EquipmentExtension == false || l.EquipmentExtension == equipmentExtension) &&
			(l.GuardedExtension == false || l.GuardedExtension == guardedExtension))).FirstOrDefault();
			if (uncheckedLocation != null)
			{
				uint row = (uint) uncheckedLocation.MapRow / 2;
				uint col = (uint) uncheckedLocation.MapCol / 4;
				if (secondCastle)
				{
					row = (398 / 2) - row;
					col = (504 / 4) - col;
				}
				return renderingApi.RoomIsRendered(row, col);
			}
			return true;
		}

		private void CheckRooms(WatchList watchlist)
		{

			foreach (var watch in watchlist)
			{
				if (watch.ChangeCount > 0)
				{
					string locationName = watch.Notes.ToLower();
					if (Char.IsDigit(watch.Notes.Last()))
					{
						locationName = locationName.Substring(0, locationName.Length - 1);
					}
					Location location = locations.Where(x => x.Name.ToLower() == locationName.ToLower()).FirstOrDefault();

					if (location != null && !location.Status && watch.Value > 0)
					{
						Room room = location.Rooms.Where(y => y.Name.ToLower() == watch.Notes.ToLower()).FirstOrDefault();
						if (room != null)
						{
							foreach (int value in room.Values)
							{
								Console.WriteLine($"Tracker: {location.Name} change, value = {value}");
								if ((watch.Value & value) == value)
								{
									location.Status = true;
									Console.WriteLine($"Tracker: {location.Name} checked.");
									Watch? coopWatch = null;
									int watchIndex = 0;
									for (int i = 0; i < watchlistService.CoopLocationWatches.Count; i++)
									{
										if (watchlistService.CoopLocationWatches[i].Notes == watch.Notes)
										{
											coopWatch = watchlistService.CoopLocationWatches[i];
											watchIndex = i;
											break;
										}
									}

									if (coopWatch is not null && watchlistService.CoopLocationValues[watchIndex] == 0)
									{
										coopWatch.Update(PreviousType.LastFrame);
										watchlistService.CoopLocationValues[watchIndex] = value;
										Console.WriteLine($"Added {coopWatch.Notes} at index {watchIndex} value {watchlistService.CoopLocationValues[watchIndex]} to coopValues.");
									}
									ClearMapLocation(locations.IndexOf(location));
								}
							}
						}
					}
				}
			}
		}

		private void CheckReachability()
		{
			int changes = 0;

			foreach (var location in locations)
			{
				if (location.Locks.Count == 0)
				{
					location.AvailabilityColor = MapColor.Available;
					continue;
				}
				foreach (var lockSet in location.Locks)
				{
					bool unlock = true;
					for (int i = 0; i < lockSet.Length; i++)
					{
						unlock = unlock && TrackedObjectStatus(lockSet[i]);
					}
					if (unlock)
					{
						changes++;
						location.AvailabilityColor = MapColor.Available;
						break;
					}
				}
				if (location.AvailabilityColor == MapColor.Available)
				{
					continue;
				}
				if (location.OutOfLogicLocks.Count == 0)
				{
					location.AvailabilityColor = MapColor.Allowed;
					continue;
				}
				foreach (var lockSet in location.OutOfLogicLocks)
				{
					bool unlock = true;
					for (int i = 0; i < lockSet.Length; i++)
					{
						unlock = unlock && TrackedObjectStatus(lockSet[i]);
					}
					if (unlock)
					{
						changes++;
						if (preset == "speedrun" || preset == "glitch")
						{
							location.AvailabilityColor = MapColor.Available;
							break;
						}
						location.AvailabilityColor = MapColor.Allowed;
						break;
					}
				}
			}

			if (changes > 0)
			{
				SetMapLocations();
			}
		}

		private bool TrackedObjectStatus(string name)
		{
			if (name is null) { throw new ArgumentNullException("name"); }

			Relic relic = relics.Where(relic => relic.Name.ToLower() == name).FirstOrDefault();
			if (relic != null)
			{
				return relic.Status;
			}
			Item progressionItem = progressionItems.Where(item => item.Name.ToLower() == name).FirstOrDefault();
			if (progressionItem != null)
			{
				return progressionItem.Status;
			}
			Item thrustSword = thrustSwords.Where(item => item.Status).FirstOrDefault();
			if (name == "thrustsword" && thrustSword != null)
			{
				return true;
			}

			return false;
		}

		private void SaveReplayLine()
		{
			int currentMapX = (int) alucardApi.MapX;
			int currentMapY = (int) alucardApi.MapY;

			if ((currentMapY == 44 && currentMapX < 19) || (currentMapX < 2 && currentMapY < 3) || currentMapX > 200 || currentMapY > 200)
			{
				return;
			}

			if (replay.Count == 0 || (replay[replay.Count - 1].X != currentMapX || replay[replay.Count - 1].Y != currentMapY))
			{
				if (replay.Count == 0)
				{
					replay.Add(new MapLocation { X = currentMapX, Y = currentMapY, Time = prologueTime, Relics = 0, ProgressionItems = 0 });
				}
				else
				{
					replay.Add(new MapLocation { X = currentMapX, Y = currentMapY, SecondCastle = secondCastle ? 1 : 0, Relics = replay[replay.Count - 1].Relics, ProgressionItems = replay[replay.Count - 1].ProgressionItems });
				}
			}

			var room = replay[replay.Count - 1];
			room.Time++;
			if (relicOrItemCollected)
			{
				room.Relics = EncodeRelics();
				room.ProgressionItems = EncodeItems();
			}
		}

		private int EncodeItems()
		{
			int itemsNumber = 0;
			for (int i = 0; i < progressionItems.Count + 1; i++)
			{
				if (i < progressionItems.Count && progressionItems[i].Status)
				{
					itemsNumber |= (int) Math.Pow(2, i);
				}
				else if (i == progressionItems.Count)
				{
					foreach (var sword in thrustSwords)
					{
						if (sword.Status)
						{
							itemsNumber |= (int) Math.Pow(2, i);
							break;
						}
					}
				}
			}

			return itemsNumber;
		}

		private int EncodeRelics()
		{
			int relicsNumber = 0;
			for (int i = 0; i < relics.Count; i++)
			{
				if (relics[i].Status)
				{
					relicsNumber |= (int) Math.Pow(2, i);
				}
			}
			return relicsNumber;
		}

		public void SaveReplay()
		{
			if (replay.Count < 10)
			{
				return;
			}

			string replayPath = Paths.ReplaysPath + SeedInfo + "-" + toolConfig.Tracker.Username + ".sotnr";
			int version = 2;
			while (File.Exists(replayPath))
			{
				replayPath = Paths.ReplaysPath + SeedInfo + "(" + version + ")" + "-" + toolConfig.Tracker.Username + ".sotnr";
				version++;
			}

			using (StreamWriter w = File.AppendText(replayPath))
			{
				foreach (var room in replay)
				{
					int time = (int) Math.Ceiling((double) (room.Time / 10));
					if (time < 1)
					{
						time = 1;
					}
					string line = $"{room.X}:{room.Y}:{time}:{room.SecondCastle}:{room.Relics}:{room.ProgressionItems}";
					w.WriteLine(line);
				}
			}
		}
	}
}

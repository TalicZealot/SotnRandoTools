using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BizHawk.Client.Common;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools.RandoTracker
{
	public class Tracker : ITracker
	{
		const string DefaultSeedInfo = "seed(preset)";
		const long DraculaActorAddress = 0x076e98;

		private readonly IGraphics? formGraphics;
		private readonly IToolConfig toolConfig;
		private readonly TrackerGraphicsEngine trackerGraphicsEngine;
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

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
			new Location { Name = "SoulOfBat", MapRow = 66, MapCol = 96, Rooms = new List<Room>{
					new Room { Name = "SoulOfBat", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "FireOfBat", MapRow = 26, MapCol = 118, Rooms = new List<Room>{
					new Room { Name = "FireOfBat", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "EchoOfBat", MapRow = 46, MapCol = 32, Rooms = new List<Room>{
					new Room { Name = "EchoOfBat", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "SoulOfWolf", MapRow = 54, MapCol = 122, Rooms = new List<Room>{
					new Room { Name = "SoulOfWolf1", Values = new int[] { 0x10 }},
					new Room { Name = "SoulOfWolf2", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "PowerOfWolf", MapRow = 134, MapCol = 4, Rooms = new List<Room>{
					new Room { Name = "PowerOfWolf", Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "SkillOfWolf", MapRow = 114, MapCol = 30, Rooms = new List<Room>{
					new Room { Name = "SkillOfWolf", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "FormOfMist", MapRow = 70, MapCol = 42, Rooms = new List<Room>{
					new Room { Name = "FormOfMist", Values = new int[] { 0x04, 0x10 }},
			}},
			new Location { Name = "PowerOfMist", MapRow = 18, MapCol = 62, Rooms = new List<Room>{
					new Room { Name = "PowerOfMist1", Values = new int[] { 0x01 }},
					new Room { Name = "PowerOfMist2", Values = new int[] { 0x01 }},
					new Room { Name = "PowerOfMist3", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "CubeOfZoe", MapRow = 126, MapCol = 38, Rooms = new List<Room>{
					new Room { Name = "CubeOfZoe", Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "SpiritOrb", MapRow = 107, MapCol = 50, Rooms = new List<Room>{
					new Room { Name = "SpiritOrb1", Values = new int[] { 0x10 }},
					new Room { Name = "SpiritOrb2", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "GravityBoots", MapRow = 74, MapCol = 68, Rooms = new List<Room>{
					new Room { Name = "GravityBoots", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "LeapStone", MapRow = 26, MapCol = 62, Rooms = new List<Room>{
					new Room { Name = "LeapStone1", Values = new int[] { 0x01 }},
					new Room { Name = "LeapStone2", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "HolySymbol", MapRow = 146, MapCol = 110, Rooms = new List<Room>{
					new Room { Name = "HolySymbol", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "FaerieScroll", MapRow = 54, MapCol = 118, Rooms = new List<Room>{
					new Room { Name = "FaerieScroll", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "JewelOfOpen", MapRow = 62, MapCol = 98, Rooms = new List<Room>{
					new Room { Name = "JewelOfOpen", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "MermanStatue", MapRow = 150, MapCol = 16, Rooms = new List<Room>{
					new Room { Name = "MermanStatue", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "BatCard", MapRow = 90, MapCol = 26, Rooms = new List<Room>{
					new Room { Name = "BatCard", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "GhostCard", MapRow = 10, MapCol = 78, Rooms = new List<Room>{
					new Room { Name = "GhostCard", Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "FaerieCard", MapRow = 54, MapCol = 104, Rooms = new List<Room>{
					new Room { Name = "FaerieCard", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "DemonCard", MapRow = 158, MapCol = 58, Rooms = new List<Room>{
					new Room { Name = "DemonCard", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "SwordCard", MapRow = 54, MapCol = 40, Rooms = new List<Room>{
					new Room { Name = "SwordCard", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "CrystalCloak",  GuardedExtension = true,  SpreadExtension = true, MapRow = 134, MapCol = 80, Rooms = new List<Room>{
					new Room { Name = "CrystalCloak", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Mormegil",  GuardedExtension = true, MapRow = 182, MapCol = 34, Rooms = new List<Room>{
					new Room { Name = "Mormegil", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "GoldRing", MapRow = 114, MapCol = 90, Rooms = new List<Room>{
					new Room { Name = "GoldRing", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Spikebreaker", MapRow = 186, MapCol = 82, Rooms = new List<Room>{
					new Room { Name = "Spikebreaker", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "SilverRing", MapRow = 42, MapCol = 17, Rooms = new List<Room>{
					new Room { Name = "SilverRing", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "HolyGlasses", MapRow = 106, MapCol = 64, Rooms = new List<Room>{
					new Room { Name = "HolyGlasses", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "HeartOfVlad",  SecondCastle = true, MapRow = 165, MapCol = 80, Rooms = new List<Room>{
					new Room { Name = "HeartOfVlad1", Values = new int[] { 0x01 }},
					new Room { Name = "HeartOfVlad2", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "ToothOfVlad",  SecondCastle = true, MapRow = 125, MapCol = 12, Rooms = new List<Room>{
					new Room { Name = "ToothOfVlad", Values = new int[] { 0x10, 0x04 }},
			}},
			new Location { Name = "RibOfVlad",  SecondCastle = true, MapRow = 153, MapCol = 88, Rooms = new List<Room>{
					new Room { Name = "RibOfVlad", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "RingOfVlad",  SecondCastle = true, MapRow = 177, MapCol = 46, Rooms = new List<Room>{
					new Room { Name = "RingOfVlad", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "EyeOfVlad",  SecondCastle = true, MapRow = 57, MapCol = 66, Rooms = new List<Room>{
					new Room { Name = "EyeOfVlad", Values = new int[] { 0x10, 0x40 }},
			}},
			new Location { Name = "ForceOfEcho",  SecondCastle = true, MapRow = 53, MapCol = 16, Rooms = new List<Room>{
					new Room { Name = "ForceOfEcho", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "GasCloud",  SecondCastle = true, MapRow = 17, MapCol = 92, Rooms = new List<Room>{
					new Room { Name = "GasCloud", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "RingOfArcana",  SecondCastle = true,  GuardedExtension = true,  SpreadExtension = true, MapRow = 109, MapCol = 100, Rooms = new List<Room>{
					new Room { Name = "RingOfArcana", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "DarkBlade",  SecondCastle = true,  GuardedExtension = true,  SpreadExtension = true, MapRow = 65, MapCol = 46, Rooms = new List<Room>{
					new Room { Name = "DarkBlade", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Trio",  SecondCastle = true,  GuardedExtension = true,  SpreadExtension = true, MapRow = 129, MapCol = 86, Rooms = new List<Room>{
					new Room { Name = "Trio1", Values = new int[] { 0x40 }},
					new Room { Name = "Trio2", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Walk armor",  EquipmentExtension = true, MapRow = 182, MapCol = 46, Rooms = new List<Room>{
					new Room { Name = "Walk armor", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Icebrand",  EquipmentExtension = true, MapRow = 182, MapCol = 48, Rooms = new List<Room>{
					new Room { Name = "Icebrand", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Bloodstone",  EquipmentExtension = true, MapRow = 182, MapCol = 56, Rooms = new List<Room>{
					new Room { Name = "Bloodstone", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Combat knife",  EquipmentExtension = true, MapRow = 174, MapCol = 62, Rooms = new List<Room>{
					new Room { Name = "Combat knife", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Ring of Ares",  EquipmentExtension = true, MapRow = 146, MapCol = 74, Rooms = new List<Room>{
					new Room { Name = "Ring of Ares", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Knuckle duster",  EquipmentExtension = true, MapRow = 150, MapCol = 80, Rooms = new List<Room>{
					new Room { Name = "Knuckle duster", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Caverns Onyx",  EquipmentExtension = true, MapRow = 146, MapCol = 92, Rooms = new List<Room>{
					new Room { Name = "Caverns Onyx", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Bandanna",  EquipmentExtension = true, MapRow = 90, MapCol = 70, Rooms = new List<Room>{
					new Room { Name = "Bandanna", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Nunchaku",  EquipmentExtension = true, MapRow = 134, MapCol = 76, Rooms = new List<Room>{
					new Room { Name = "Nunchaku", Values = new int[] { 0x04, 0x10 }},
			}},
			new Location { Name = "Secret Boots",  EquipmentExtension = true, MapRow = 138, MapCol = 48, Rooms = new List<Room>{
					new Room { Name = "Secret boots1", Values = new int[] { 0x01 }},
					new Room { Name = "Secret boots2", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Holy mail",  EquipmentExtension = true, MapRow = 134, MapCol = 10, Rooms = new List<Room>{
					new Room { Name = "Holy mail", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Jewel sword",  EquipmentExtension = true, MapRow = 146, MapCol = 20, Rooms = new List<Room>{
					new Room { Name = "Jewel sword", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Sunglasses",  EquipmentExtension = true, MapRow = 106, MapCol = 32, Rooms = new List<Room>{
					new Room { Name = "Sunglasses", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Basilard",  EquipmentExtension = true, MapRow = 118, MapCol = 32, Rooms = new List<Room>{
					new Room { Name = "Basilard", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Cloth cape",  EquipmentExtension = true, MapRow = 98, MapCol = 20, Rooms = new List<Room>{
					new Room { Name = "Cloth cape", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Mystic pendant",  EquipmentExtension = true, MapRow = 86, MapCol = 8, Rooms = new List<Room>{
					new Room { Name = "Mystic pendant1", Values = new int[] { 0x40 }},
					new Room { Name = "Mystic pendant2", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Ankh of Life",  EquipmentExtension = true, MapRow = 78, MapCol = 12, Rooms = new List<Room>{
					new Room { Name = "Ankh of Life", Values = new int[] { 0x04, 0x01 }},
			}},
			new Location { Name = "Morningstar",  EquipmentExtension = true, MapRow = 66, MapCol = 16, Rooms = new List<Room>{
					new Room { Name = "Morningstar1", Values = new int[] { 0x10 }},
					new Room { Name = "Morningstar2", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Goggles",  EquipmentExtension = true, MapRow = 66, MapCol = 20, Rooms = new List<Room>{
					new Room { Name = "Goggles", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Silver plate",  EquipmentExtension = true, MapRow = 30, MapCol = 28, Rooms = new List<Room>{
					new Room { Name = "Silver plate", Values = new int[] { 0x04, 0x01 }},
			}},
			new Location { Name = "Cutlass",  EquipmentExtension = true, MapRow = 22, MapCol = 54, Rooms = new List<Room>{
					new Room { Name = "Cutlass1", Values = new int[] { 0x40 }},
					new Room { Name = "Cutlass2", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Platinum mail",  EquipmentExtension = true, MapRow = 6, MapCol = 70, Rooms = new List<Room>{
					new Room { Name = "Platinum mail", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Falchion",  EquipmentExtension = true, MapRow = 14, MapCol = 78, Rooms = new List<Room>{
					new Room { Name = "Falchion", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Gold plate",  EquipmentExtension = true, MapRow = 38, MapCol = 98, Rooms = new List<Room>{
					new Room { Name = "Gold plate", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Bekatowa",  EquipmentExtension = true, MapRow = 38, MapCol = 111, Rooms = new List<Room>{
					new Room { Name = "Bekatowa1", Values = new int[] { 0x01, 0x04 }},
					new Room { Name = "Bekatowa2", Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "Gladius",  EquipmentExtension = true, MapRow = 74, MapCol = 118, Rooms = new List<Room>{
					new Room { Name = "Gladius", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Jewel knuckles",  EquipmentExtension = true, MapRow = 90, MapCol = 118, Rooms = new List<Room>{
					new Room { Name = "Jewel knuckles", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Bronze cuirass",  EquipmentExtension = true, MapRow = 66, MapCol = 98, Rooms = new List<Room>{
					new Room { Name = "Bronze cuirass", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Holy rod",  EquipmentExtension = true, MapRow = 54, MapCol = 100, Rooms = new List<Room>{
					new Room { Name = "Holy rod", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Library Onyx",  EquipmentExtension = true, MapRow = 66, MapCol = 94, Rooms = new List<Room>{
					new Room { Name = "Library Onyx", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Alucart Sword",  EquipmentExtension = true, MapRow = 82, MapCol = 68, Rooms = new List<Room>{
					new Room { Name = "Alucart sword", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Broadsword",  EquipmentExtension = true, MapRow = 70, MapCol = 64, Rooms = new List<Room>{
					new Room { Name = "Broadsword", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Estoc",  EquipmentExtension = true, MapRow = 42, MapCol = 60, Rooms = new List<Room>{
					new Room { Name = "Estoc", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Olrox Garnet",  EquipmentExtension = true, MapRow = 54, MapCol = 66, Rooms = new List<Room>{
					new Room { Name = "Olrox garnet", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Holy sword",  EquipmentExtension = true, MapRow = 62, MapCol = 38, Rooms = new List<Room>{
					new Room { Name = "Holy sword", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Knight shield",  EquipmentExtension = true, MapRow = 70, MapCol = 28, Rooms = new List<Room>{
					new Room { Name = "Knight shield", Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "Shield rod",  EquipmentExtension = true, MapRow = 78, MapCol = 26, Rooms = new List<Room>{
					new Room { Name = "Shield rod", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Blood cloak",  EquipmentExtension = true, MapRow = 78, MapCol = 40, Rooms = new List<Room>{
					new Room { Name = "Blood cloak", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Bastard sword",  SecondCastle = true,  EquipmentExtension = true, MapRow = 193, MapCol = 60, Rooms = new List<Room>{
					new Room { Name = "Bastard sword", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Royal cloak",  SecondCastle = true,  EquipmentExtension = true, MapRow = 193, MapCol = 56, Rooms = new List<Room>{
					new Room { Name = "Royal cloak", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Lightning mail",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 48, Rooms = new List<Room>{
					new Room { Name = "Lightning mail", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Sword of Dawn",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 64, Rooms = new List<Room>{
					new Room { Name = "Sword of Dawn1", Values = new int[] { 0x40 }},
					new Room { Name = "Sword of Dawn2", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Moon rod",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 42, Rooms = new List<Room>{
					new Room { Name = "Moon rod", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Sunstone",  SecondCastle = true,  EquipmentExtension = true, MapRow = 161, MapCol = 28, Rooms = new List<Room>{
					new Room { Name = "Sunstone", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Luminus",  SecondCastle = true,  EquipmentExtension = true, MapRow = 164, MapCol = 16, Rooms = new List<Room>{
					new Room { Name = "Luminus", Values = new int[] { 0x10, 0x40 }},
			}},
			new Location { Name = "Dragon helm",  SecondCastle = true,  EquipmentExtension = true,  SpreadExtension = true, MapRow = 173, MapCol = 8, Rooms = new List<Room>{
					new Room { Name = "Dragon helm", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Shotel",  SecondCastle = true,  EquipmentExtension = true,  SpreadExtension = true, MapRow = 109, MapCol = 8, Rooms = new List<Room>{
					new Room { Name = "Shotel", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Badelaire",  SecondCastle = true,  EquipmentExtension = true, MapRow = 145, MapCol = 26, Rooms = new List<Room>{
					new Room { Name = "Badelaire", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Staurolite",  SecondCastle = true,  EquipmentExtension = true,  SpreadExtension = true, MapRow = 132, MapCol = 30, Rooms = new List<Room>{
					new Room { Name = "Staurolite", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Forbidden Library Opal",  SecondCastle = true,  EquipmentExtension = true, MapRow = 137, MapCol = 27, Rooms = new List<Room>{
					new Room { Name = "Forbidden Library Opal", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Reverse Caverns Diamond",  SecondCastle = true,  EquipmentExtension = true, MapRow = 109, MapCol = 56, Rooms = new List<Room>{
					new Room { Name = "Reverse Caverns Diamond", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Reverse Caverns Opal",  SecondCastle = true,  EquipmentExtension = true, MapRow = 89, MapCol = 52, Rooms = new List<Room>{
					new Room { Name = "Reverse Caverns Opal", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Reverse Caverns Garnet",  SecondCastle = true,  EquipmentExtension = true, MapRow = 69, MapCol = 82, Rooms = new List<Room>{
					new Room { Name = "Reverse Caverns Garnet", Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Osafune katana",  SecondCastle = true,  EquipmentExtension = true, MapRow = 49, MapCol = 76, Rooms = new List<Room>{
					new Room { Name = "Osafune katana", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Alucard shield",  SecondCastle = true,  EquipmentExtension = true, MapRow = 49, MapCol = 110, Rooms = new List<Room>{
					new Room { Name = "Alucard shield", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Alucard sword",  SecondCastle = true,  EquipmentExtension = true, MapRow = 41, MapCol = 68, Rooms = new List<Room>{
					new Room { Name = "Alucard sword", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Necklace of J",  SecondCastle = true,  EquipmentExtension = true, MapRow = 17, MapCol = 78, Rooms = new List<Room>{
					new Room { Name = "Necklace of J", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Floating Catacombs Diamond",  SecondCastle = true,  EquipmentExtension = true, MapRow = 17, MapCol = 80, Rooms = new List<Room>{
					new Room { Name = "Floating Catacombs Diamond", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Talwar",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 88, Rooms = new List<Room>{
					new Room { Name = "Talwar1", Values = new int[] { 0x01 }},
					new Room { Name = "Talwar2", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Twilight cloak",  SecondCastle = true,  EquipmentExtension = true, MapRow = 157, MapCol = 110, Rooms = new List<Room>{
					new Room { Name = "Twilight cloak", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Alucard Mail",  SecondCastle = true,  EquipmentExtension = true, MapRow = 145, MapCol = 60, Rooms = new List<Room>{
					new Room { Name = "Alucard mail", Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Sword of Hador",  SecondCastle = true,  EquipmentExtension = true, MapRow = 129, MapCol = 62, Rooms = new List<Room>{
					new Room { Name = "Sword of Hador", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Fury Plate",  SecondCastle = true,  EquipmentExtension = true, MapRow = 137, MapCol = 88, Rooms = new List<Room>{
					new Room { Name = "Fury plate", Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Gram",  SecondCastle = true,  EquipmentExtension = true, MapRow = 121, MapCol = 86, Rooms = new List<Room>{
					new Room { Name = "Gram", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Goddess shield",  SecondCastle = true,  EquipmentExtension = true, MapRow = 93, MapCol = 94, Rooms = new List<Room>{
					new Room { Name = "Goddess shield", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Katana",  SecondCastle = true,  EquipmentExtension = true, MapRow = 73, MapCol = 102, Rooms = new List<Room>{
					new Room { Name = "Katana", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Talisman",  SecondCastle = true,  EquipmentExtension = true, MapRow = 69, MapCol = 86, Rooms = new List<Room>{
					new Room { Name = "Talisman", Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Beryl circlet",  SecondCastle = true,  EquipmentExtension = true, MapRow = 53, MapCol = 106, Rooms = new List<Room>{
					new Room { Name = "Beryl circlet", Values = new int[] { 0x10 }},
			}},
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
		private bool spreadExtension = false;
		private bool gameReset = true;
		private bool secondCastle = false;
		private bool restarted = false;
		private bool relicOrItemCollected = false;
		private List<MapLocation> replay = new();
		private int prologueTime = 0;
		private Autosplitter autosplitter;
		private bool autosplitterConnected = false;
		private int autosplitterReconnectCounter = 0;
		private bool draculaSpawned = false;

		public Tracker(IGraphics? formGraphics, IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, INotificationService notificationService)
		{
			if (formGraphics is null) throw new ArgumentNullException(nameof(formGraphics));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.formGraphics = formGraphics;
			this.toolConfig = toolConfig;
			this.watchlistService = watchlistService;
			this.sotnApi = sotnApi;
			this.notificationService = notificationService;

			if (toolConfig.Tracker.Locations)
			{
				InitializeAllLocks();
				CheckReachability();
			}
			if (toolConfig.Tracker.UseOverlay)
			{
				notificationService.StartOverlayServer();
			}
			if (toolConfig.Tracker.EnableAutosplitter)
			{
				autosplitter = new();
			}

			trackerGraphicsEngine = new TrackerGraphicsEngine(formGraphics, relics, progressionItems, thrustSwords, toolConfig);
			trackerGraphicsEngine.CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			this.GraphicsEngine = trackerGraphicsEngine;
			this.SeedInfo = DefaultSeedInfo;
		}

		public string SeedInfo { get; set; }

		public IRelicLocationDisplay RelicLocationDisplay { get; set; }

		public TrackerGraphicsEngine GraphicsEngine { get; }

		public void DrawRelicsAndItems()
		{
			trackerGraphicsEngine.Render();
			trackerGraphicsEngine.DrawSeedInfo(SeedInfo);
		}

		public void Update()
		{
			UpdateSeedLabel();

			bool inGame = sotnApi.GameApi.Status == Status.InGame;
			bool updatedSecondCastle = sotnApi.GameApi.SecondCastle;
			relicOrItemCollected = false;

			if (sotnApi.GameApi.InAlucardMode() && sotnApi.AlucardApi.HasHitbox())
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
				UpdateOverlay();

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
			else if (sotnApi.GameApi.InPrologue())
			{
				prologueTime++;
				if (!restarted)
				{
					ResetToDefaults();
					DrawRelicsAndItems();
					restarted = true;
				}
			}

			if (toolConfig.Tracker.EnableAutosplitter && !autosplitterConnected && autosplitterReconnectCounter == 120)
			{
				autosplitterConnected = autosplitter.AtemptConnect();
				autosplitterReconnectCounter = 0;
			}
			if (toolConfig.Tracker.EnableAutosplitter && !autosplitterConnected && autosplitterReconnectCounter < 120)
			{
				autosplitterReconnectCounter++;
			}
			if (toolConfig.Tracker.EnableAutosplitter && autosplitterConnected)
			{
				CheckStart();
				CheckReset();
				CheckSplit();
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
				relic.Collected = false;
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
				location.Visited = false;
				location.AvailabilityColor = MapColor.Unavailable;
			}
		}

		private void UpdateSeedLabel()
		{
			if (SeedInfo == DefaultSeedInfo && sotnApi.GameApi.Status == Status.MainMenu)
			{
				getSeedData();
				trackerGraphicsEngine.Render();
				trackerGraphicsEngine.DrawSeedInfo(SeedInfo);
			}
		}

		private void UpdateLocations()
		{
			uint currentRooms = sotnApi.AlucardApi.Rooms;
			if (currentRooms > roomCount)
			{
				roomCount = currentRooms;
				watchlistService.UpdateWatchlist(watchlistService.SafeLocationWatches);
				CheckRooms(watchlistService.SafeLocationWatches);
				if (equipmentExtension || spreadExtension)
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
						relics[i].Collected = true;
						relicOrItemCollected = true;
						Console.WriteLine($"Found relic " + relics[i].Name);

						if (RelicLocationDisplay is not null)
						{
							float adjustedX = sotnApi.AlucardApi.MapX * 2;
							float adjustedY = (secondCastle ? (sotnApi.AlucardApi.MapY - 8.75f) : (sotnApi.AlucardApi.MapY - 4.5f)) * 4;
							Console.WriteLine($"relic location: x:{adjustedX}, y:{adjustedY}");
							Location location = locations.Where(l => (l.MapRow >= adjustedY - 3 && l.MapRow <= adjustedY + 3) && (l.MapCol >= adjustedX - 3 && l.MapCol <= adjustedX + 3)).FirstOrDefault();
							SetLocationDisplay(location, relics[i].Name);
						}

					}
					else
					{
						relics[i].Collected = false;
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

		private void SetLocationDisplay(Location? location, string relicName)
		{
			string locationName = String.Empty;

			if (location is not null)
			{
				locationName = location.Name;
			}

			switch (relicName)
			{
				case "HeartOfVlad":
					if (RelicLocationDisplay.HeartOfVladLocation == String.Empty)
					{
						RelicLocationDisplay.HeartOfVladLocation = locationName;
					}
					else if (RelicLocationDisplay.HeartOfVladLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.HeartOfVladLocation = String.Empty;
					}
					break;
				case "ToothOfVlad":
					if (RelicLocationDisplay.ToothOfVladLocation == String.Empty)
					{
						RelicLocationDisplay.ToothOfVladLocation = locationName;
					}
					else if (RelicLocationDisplay.ToothOfVladLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.ToothOfVladLocation = String.Empty;
					}
					break;
				case "RibOfVlad":
					if (RelicLocationDisplay.RibOfVladLocation == String.Empty)
					{
						RelicLocationDisplay.RibOfVladLocation = locationName;
					}
					else if (RelicLocationDisplay.RibOfVladLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.RibOfVladLocation = String.Empty;
					}
					break;
				case "RingOfVlad":
					if (RelicLocationDisplay.RingOfVladLocation == String.Empty)
					{
						RelicLocationDisplay.RingOfVladLocation = locationName;
					}
					else if (RelicLocationDisplay.RingOfVladLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.RingOfVladLocation = String.Empty;
					}
					break;
				case "EyeOfVlad":
					if (RelicLocationDisplay.EyeOfVladLocation == String.Empty)
					{
						RelicLocationDisplay.EyeOfVladLocation = locationName;
					}
					else if (RelicLocationDisplay.EyeOfVladLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.EyeOfVladLocation = String.Empty;
					}
					break;
				case "SoulOfBat":
					if (RelicLocationDisplay.BatLocation == String.Empty)
					{
						RelicLocationDisplay.BatLocation = locationName;
					}
					else if (RelicLocationDisplay.BatLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.BatLocation = String.Empty;
					}
					break;
				case "SoulOfWolf":
					if (RelicLocationDisplay.WolfLocation == String.Empty)
					{
						RelicLocationDisplay.WolfLocation = locationName;
					}
					else if (RelicLocationDisplay.WolfLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.WolfLocation = String.Empty;
					}
					break;
				case "FormOfMist":
					if (RelicLocationDisplay.MistLocation == String.Empty)
					{
						RelicLocationDisplay.MistLocation = locationName;
					}
					else if (RelicLocationDisplay.MistLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.MistLocation = String.Empty;
					}
					break;
				case "PowerOfMist":
					if (RelicLocationDisplay.PowerOfMistLocation == String.Empty)
					{
						RelicLocationDisplay.PowerOfMistLocation = locationName;
					}
					else if (RelicLocationDisplay.PowerOfMistLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.PowerOfMistLocation = String.Empty;
					}
					break;
				case "JewelOfOpen":
					if (RelicLocationDisplay.JewelOfOpenLocation == String.Empty)
					{
						RelicLocationDisplay.JewelOfOpenLocation = locationName;
					}
					else if (RelicLocationDisplay.JewelOfOpenLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.JewelOfOpenLocation = String.Empty;
					}
					break;
				case "GravityBoots":
					if (RelicLocationDisplay.GravityBootsLocation == String.Empty)
					{
						RelicLocationDisplay.GravityBootsLocation = locationName;
					}
					else if (RelicLocationDisplay.GravityBootsLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.GravityBootsLocation = String.Empty;
					}
					break;
				case "LeapStone":
					if (RelicLocationDisplay.LepastoneLocation == String.Empty)
					{
						RelicLocationDisplay.LepastoneLocation = locationName;
					}
					else if (RelicLocationDisplay.LepastoneLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.LepastoneLocation = String.Empty;
					}
					break;
				case "MermanStatue":
					if (RelicLocationDisplay.MermanLocation == String.Empty)
					{
						RelicLocationDisplay.MermanLocation = locationName;
					}
					else if (RelicLocationDisplay.MermanLocation == Constants.Khaos.KhaosName)
					{
						RelicLocationDisplay.MermanLocation = String.Empty;
					}
					break;
				default:
					return;
			}

			if (location is null)
			{
				return;
			}

			if (location.SecondCastle)
			{
				notificationService.SetInvertedRelicCoordinates(relicName, location.MapCol, location.MapRow);
			}
			else
			{
				notificationService.SetRelicCoordinates(relicName, location.MapCol, location.MapRow);
			}
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
								progressionItems[i].Status = (sotnApi.AlucardApi.Accessory1 == progressionItems[i].Value) || (sotnApi.AlucardApi.Accessory2 == progressionItems[i].Value);
								break;
							case 2:
								progressionItems[i].Status = (sotnApi.AlucardApi.Armor == progressionItems[i].Value);
								break;
							case 3:
								progressionItems[i].Status = (sotnApi.AlucardApi.Helm == progressionItems[i].Value);
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
						thrustSwords[i].Status = (sotnApi.AlucardApi.RightHand == thrustSwords[i].Value);
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
			string seedName = sotnApi.GameApi.ReadSeedName();
			preset = sotnApi.GameApi.ReadPresetName();
			if (preset == "tournament" || preset == "")
			{
				preset = "custom";
			}
			SeedInfo = seedName + "(" + preset + ")";
			Console.WriteLine("Randomizer seed information: " + SeedInfo);
			SaveSeedInfo(SeedInfo);
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
				case "guarded-og":
					guardedExtension = true;
					break;
				case "bat-master":
					guardedExtension = false;
					spreadExtension = true;
					LoadLocks(Paths.BatMasterPresetPath, false);
					break;
				case "speedrun":
					LoadLocks(Paths.SpeedrunPresetPath, false);
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
				if (!locations[i].Visited && locations[i].SecondCastle == secondCastle)
				{
					if (locations[i].SpreadExtension && !locations[i].GuardedExtension && !locations[i].EquipmentExtension && !spreadExtension)
					{
						continue;
					}
					if (locations[i].EquipmentExtension && !equipmentExtension)
					{
						if (locations[i].SpreadExtension && !spreadExtension)
						{
							continue;
						}
						else if (!locations[i].SpreadExtension)
						{
							continue;
						}
					}
					if (locations[i].GuardedExtension && !guardedExtension)
					{
						if (locations[i].SpreadExtension && !spreadExtension)
						{
							continue;
						}
						else if (!locations[i].SpreadExtension)
						{
							continue;
						}
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
				sotnApi.GameApi.SetRoomToUnvisited(roomWatch.Address);
			}
		}

		private void ColorMapRoom(int i, uint color, bool secondCastle)
		{
			uint row = (uint) locations[i].MapRow;
			uint col = (uint) locations[i].MapCol;
			if (secondCastle)
			{
				row = 199 - row;
				col = 126 - col;
			}
			uint borderColor = color > 0 ? (uint) MapColor.Border : 0;

			if (locations[i].EquipmentExtension && spreadExtension == false)
			{
				sotnApi.RenderingApi.ColorMapLocation(row, col, color);
			}
			else
			{
				sotnApi.RenderingApi.ColorMapRoom(row, col, color, borderColor);
			}
		}

		private bool LocationsDrawn()
		{
			Location uncheckedLocation = locations.Where(l => (!l.Visited && l.SecondCastle == secondCastle &&
			(l.EquipmentExtension == false || l.EquipmentExtension == equipmentExtension) &&
			(l.GuardedExtension == false || l.GuardedExtension == guardedExtension))).FirstOrDefault();
			if (uncheckedLocation != null)
			{
				uint row = (uint) uncheckedLocation.MapRow;
				uint col = (uint) uncheckedLocation.MapCol;
				if (secondCastle)
				{
					row = 199 - row;
					col = 126 - col;
				}
				return sotnApi.RenderingApi.RoomIsRendered(row, col);
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

					if (location != null && !location.Visited && watch.Value > 0)
					{
						Room room = location.Rooms.Where(y => y.Name.ToLower() == watch.Notes.ToLower()).FirstOrDefault();
						if (room != null)
						{
							foreach (int value in room.Values)
							{
								if ((watch.Value & value) == value)
								{
									location.Visited = true;
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
				location.AvailabilityColor = MapColor.Unavailable;
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
				return relic.Collected;
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

		private void UpdateOverlay()
		{
			if (toolConfig.Tracker.UseOverlay)
			{
				if (relicOrItemCollected)
				{
					notificationService.UpdateTrackerOverlay(EncodeRelics(), EncodeItems());
				}
			}
		}

		private void SaveReplayLine()
		{
			int currentMapX = (int) sotnApi.AlucardApi.MapX;
			int currentMapY = (int) sotnApi.AlucardApi.MapY;

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
				if (relics[i].Collected)
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

		private void SaveSeedInfo(string info)
		{
			if (File.Exists(Paths.SeedInfoPath))
			{
				File.WriteAllText(Paths.SeedInfoPath, info);
			}
			else
			{
				using (StreamWriter sw = File.CreateText(Paths.SeedInfoPath))
				{
					sw.Write(info);
				}
			}
		}

		private void CheckStart()
		{
			if (!autosplitter.Started && sotnApi.GameApi.Hours == 0 && sotnApi.GameApi.Minutes == 0 && sotnApi.GameApi.Seconds == 3 && sotnApi.GameApi.Status == Status.InGame)
			{
				autosplitter.StartTImer();
				Console.WriteLine("StartTimer");
			}
		}

		private void CheckReset()
		{
			if (autosplitter.Started && sotnApi.GameApi.Hours == 0 && sotnApi.GameApi.Minutes == 0 && sotnApi.GameApi.Seconds == 0 && sotnApi.GameApi.Frames < 20 && sotnApi.GameApi.Status == Status.InGame)
			{
				autosplitter.Restart();
				Console.WriteLine("Restart");
			}
		}

		private void CheckSplit()
		{
			if (autosplitter.Started && sotnApi.AlucardApi.MapX == 31 && sotnApi.AlucardApi.MapY == 30 && sotnApi.GameApi.Status == Status.InGame)
			{
				LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(DraculaActorAddress);
				if (boss.Hp > 13 && boss.Hp < 10000 && boss.AiId != 0)
				{
					draculaSpawned = true;
				}
				else if (draculaSpawned && boss.Hp < 1 && boss.AiId != 0)
				{
					autosplitter.Split();
					Console.WriteLine("Split");
				}
			}
			else
			{
				draculaSpawned = false;
			}
		}

		public void CloseAutosplitter()
		{
			if (toolConfig.Tracker.EnableAutosplitter)
			{
				autosplitter.Disconnect();
			}
		}
	}
}

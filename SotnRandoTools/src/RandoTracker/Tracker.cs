using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BizHawk.Client.Common;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.RandoTracker.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools.RandoTracker
{
	internal sealed class Tracker : ITracker
	{
		const byte ReplayCooldown = 6;
		const string DefaultSeedInfo = "seed(preset)";
		const long DraculaActorAddress = 0x076e98;
		private const int AutosplitterReconnectCooldown = 120;
		private readonly ITrackerGraphicsEngine trackerGraphicsEngine;
		private readonly IToolConfig toolConfig;
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

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
					new Room { Name = "Caverns Onyx", Values = new int[] { 0x10, 0x40 }},
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
		private List<TrackerRelic> relics = new List<TrackerRelic>
		{
				new TrackerRelic { Name = "SoulOfBat", Progression = true},
				new TrackerRelic { Name = "FireOfBat", Progression = false},
				new TrackerRelic { Name = "EchoOfBat", Progression = true},
				new TrackerRelic { Name = "ForceOfEcho", Progression = false},
				new TrackerRelic { Name = "SoulOfWolf", Progression = true},
				new TrackerRelic { Name = "PowerOfWolf", Progression = true},
				new TrackerRelic { Name = "SkillOfWolf", Progression = true},
				new TrackerRelic { Name = "FormOfMist", Progression = true},
				new TrackerRelic { Name = "PowerOfMist", Progression = true},
				new TrackerRelic { Name = "GasCloud", Progression = false},
				new TrackerRelic { Name = "CubeOfZoe", Progression = false},
				new TrackerRelic { Name = "SpiritOrb", Progression = false},
				new TrackerRelic { Name = "GravityBoots", Progression = true},
				new TrackerRelic { Name = "LeapStone", Progression = true},
				new TrackerRelic { Name = "HolySymbol", Progression = false},
				new TrackerRelic { Name = "FaerieScroll", Progression = false},
				new TrackerRelic { Name = "JewelOfOpen", Progression = true},
				new TrackerRelic { Name = "MermanStatue", Progression = true},
				new TrackerRelic { Name = "BatCard", Progression = false},
				new TrackerRelic { Name = "GhostCard", Progression = false},
				new TrackerRelic { Name = "FaerieCard", Progression = false},
				new TrackerRelic { Name = "DemonCard", Progression = false},
				new TrackerRelic { Name = "SwordCard", Progression = false},
				new TrackerRelic { Name = "SpriteCard" , Progression = false},
				new TrackerRelic { Name = "NoseDevilCard", Progression = false},
				new TrackerRelic { Name = "HeartOfVlad", Progression = true},
				new TrackerRelic { Name = "ToothOfVlad", Progression = true},
				new TrackerRelic { Name = "RibOfVlad", Progression = true},
				new TrackerRelic { Name = "RingOfVlad", Progression = true},
				new TrackerRelic { Name = "EyeOfVlad", Progression = true}
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
		private string seedName = "";
		private uint roomCount = 2;
		private bool inGame = false;
		private bool guardedExtension = true;
		private bool equipmentExtension = false;
		private bool spreadExtension = false;
		private bool gameReset = true;
		private bool secondCastle = false;
		private bool restarted = false;
		private bool relicOrItemCollected = false;
		private List<ReplayState> replay = new();
		private ushort prologueTime = 0;
		private Autosplitter autosplitter;
		private bool autosplitterConnected = false;
		private ushort autosplitterReconnectCounter = 0;
		private bool draculaSpawned = false;
		private byte currentMapX = (byte) 0;
		private byte currentMapY = (byte) 0;
		private byte currentReplayCooldown = 0;
		private bool muted = false;
		private bool started = false;
		private bool finished = false;
		private TimeSpan finalTime;
		private Stopwatch stopWatch = new Stopwatch();

		public Tracker(ITrackerGraphicsEngine trackerGraphicsEngine, IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, INotificationService notificationService)
		{
			if (trackerGraphicsEngine is null) throw new ArgumentNullException(nameof(trackerGraphicsEngine));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.trackerGraphicsEngine = trackerGraphicsEngine;
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

			trackerGraphicsEngine.InitializeItems(relics, progressionItems, thrustSwords);
			trackerGraphicsEngine.CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			this.SeedInfo = DefaultSeedInfo;
			DrawRelicsAndItems();
		}

		public string SeedInfo { get; set; }

		public void DrawRelicsAndItems()
		{
			trackerGraphicsEngine.Render();
			trackerGraphicsEngine.DrawSeedInfo(SeedInfo);
		}

		public void Update()
		{
			UpdateSeedLabel();

			inGame = sotnApi.GameApi.Status == Status.InGame;
			bool updatedSecondCastle = sotnApi.GameApi.SecondCastle;
			relicOrItemCollected = false;
			currentReplayCooldown++;
			currentMapX = (byte) sotnApi.AlucardApi.MapX;
			currentMapY = (byte) sotnApi.AlucardApi.MapY;

			if (started && !finished && currentReplayCooldown > ReplayCooldown)
			{
				SaveReplayLine();
				currentReplayCooldown = 0;
			}

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

			if (toolConfig.Tracker.EnableAutosplitter && !autosplitterConnected && autosplitterReconnectCounter == AutosplitterReconnectCooldown && !started)
			{
				autosplitterConnected = autosplitter.AtemptConnect();
				autosplitterReconnectCounter = 0;
			}
			if (toolConfig.Tracker.EnableAutosplitter && !autosplitterConnected && autosplitterReconnectCounter < 120 && !sotnApi.GameApi.InAlucardMode())
			{
				autosplitterReconnectCounter++;
			}
			CheckStart();
			CheckSplit();
			MuteMusic();
		}

		private void MuteMusic()
		{
			uint currentStatus = sotnApi.GameApi.Status;
			if (!toolConfig.Tracker.MuteMusic || (currentStatus != Status.InGame && currentStatus != Status.MainMenu))
			{
				return;
			}

			if (!muted && SotnApi.Constants.Values.Game.Various.MusicTrackValues.Contains(sotnApi.GameApi.MusicTrack))
			{
				sotnApi.GameApi.MuteXA();
				muted = true;
			}
			else if (muted && !SotnApi.Constants.Values.Game.Various.MusicTrackValues.Contains(sotnApi.GameApi.MusicTrack))
			{
				sotnApi.GameApi.UnmuteXA();
				muted = false;
			}
		}

		private void InitializeAllLocks()
		{
			LoadLocks(Paths.CasualPresetPath);
			LoadLocks(Paths.SpeedrunPresetPath, true);
		}

		private void LoadLocks(string presetFilePath, bool outOfLogic = false, bool overwriteLocks = false)
		{
			var presetLocations = JObject.Parse(File.ReadAllText(presetFilePath))["lockLocation"];
			foreach (var location in presetLocations)
			{
				string name = location["location"].ToString().Replace(" ", String.Empty).ToLower();
				var trackerLocation = locations.Where(x => x.Name.Replace(" ", String.Empty).ToLower() == name).FirstOrDefault();
				if (trackerLocation != null)
				{
					if (overwriteLocks)
					{
						if (outOfLogic)
						{
							trackerLocation.OutOfLogicLocks.Clear();
						}
						else
						{
							trackerLocation.Locks.Clear();
						}
					}

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
			PrepareMapLocations();
			if (toolConfig.Tracker.Locations)
			{
				CheckReachability();
			}
		}

		private void UpdateSeedLabel()
		{
			if (SeedInfo == DefaultSeedInfo && sotnApi.GameApi.Status == Status.MainMenu)
			{
				GetSeedData();
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
						if (!relics[i].Collected)
						{
							relics[i].X = currentMapX;
							if (secondCastle)
							{
								relics[i].X += 100;
							}
							relics[i].Y = currentMapY;
							relics[i].CollectedAt = (ushort) replay.Count;
						}
						relics[i].Collected = true;
						relicOrItemCollected = true;
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

		private void UpdateProgressionItems()
		{
			watchlistService.UpdateWatchlist(watchlistService.ProgressionItemWatches);
			for (int i = 0; i < watchlistService.ProgressionItemWatches.Count; i++)
			{
				if (watchlistService.ProgressionItemWatches[i].ChangeCount > 0)
				{
					if (watchlistService.ProgressionItemWatches[i].Value > 0)
					{
						if (!progressionItems[i].Status)
						{
							progressionItems[i].X = currentMapX;
							if (secondCastle)
							{
								progressionItems[i].X += 100;
							}
							progressionItems[i].Y = currentMapY;
							progressionItems[i].CollectedAt = (ushort) replay.Count;
						}
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
						if (!thrustSwords[i].Status)
						{
							thrustSwords[i].X = currentMapX;
							if (secondCastle)
							{
								thrustSwords[i].X += 100;
							}
							thrustSwords[i].Y = currentMapY;
							thrustSwords[i].CollectedAt = (ushort) replay.Count;
						}
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

		private void GetSeedData()
		{
			seedName = sotnApi.GameApi.ReadSeedName();
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
				case "expedition":
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
					LoadLocks(Paths.BatMasterPresetPath);
					break;
				case "speedrun":
					LoadLocks(Paths.SpeedrunPresetPath);
					break;
				case "open-casual":
				case "open-safe":
					LoadLocks(Paths.OpenCasualPresetPath, false, true);
					LoadLocks(Paths.OpenSpeedrunPresetPath, true, true);
					break;
				case "open-adventure":
					LoadLocks(Paths.OpenCasualPresetPath, false, true);
					SetEquipmentProgression();
					break;
				case "custom":
					guardedExtension = toolConfig.Tracker.CustomLocationsGuarded;
					equipmentExtension = toolConfig.Tracker.CustomLocationsEquipment;
					spreadExtension = toolConfig.Tracker.CustomLocationsSpread;
					if (equipmentExtension)
					{
						guardedExtension = true;
						SetEquipmentProgression();
					}
					break;
				default:
					break;
			}
			PrepareMapLocations();
		}

		private void SetEquipmentProgression()
		{
			relics.Where(x => x.Name == "CubeOfZoe").FirstOrDefault().Progression = true;
			relics.Where(x => x.Name == "DemonCard").FirstOrDefault().Progression = true;
			relics.Where(x => x.Name == "NoseDevilCard").FirstOrDefault().Progression = true;
			trackerGraphicsEngine.SetProgression();
			trackerGraphicsEngine.CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			trackerGraphicsEngine.Render();
		}

		private void SetMapLocations()
		{
			for (int i = 0; i < locations.Count; i++)
			{
				if (!locations[i].Visited && locations[i].SecondCastle == secondCastle)
				{
					ColorMapRoom(i, (uint) locations[i].AvailabilityColor, locations[i].SecondCastle);
				}
			}
		}

		private void PrepareMapLocations()
		{
			for (int i = 0; i < locations.Count; i++)
			{
				if (locations[i].SpreadExtension && !locations[i].GuardedExtension && !locations[i].EquipmentExtension && !spreadExtension)
				{
					locations[i].Visited = true;
					continue;
				}
				if (locations[i].EquipmentExtension && !equipmentExtension)
				{
					if (locations[i].SpreadExtension && !spreadExtension)
					{
						locations[i].Visited = true;
						continue;
					}
					else if (!locations[i].SpreadExtension)
					{
						locations[i].Visited = true;
						continue;
					}
				}
				if (locations[i].GuardedExtension && !guardedExtension)
				{
					if (locations[i].SpreadExtension && !spreadExtension)
					{
						locations[i].Visited = true;
						continue;
					}
					else if (!locations[i].SpreadExtension)
					{
						locations[i].Visited = true;
						continue;
					}
				}
			}
		}

		private void ClearMapLocation(int locationIndex)
		{
			ColorMapRoom(locationIndex, (uint) MapColor.Clear, locations[locationIndex].SecondCastle);
			foreach (var room in locations[locationIndex].Rooms)
			{
				Watch roomWatch = watchlistService.SafeLocationWatches.Where(x => x.Notes.ToLower() == room.Name.ToLower()).FirstOrDefault();
				if (roomWatch is null)
				{
					roomWatch = watchlistService.EquipmentLocationWatches.Where(x => x.Notes.ToLower() == room.Name.ToLower()).FirstOrDefault();
				}
				sotnApi.GameApi.SetRoomToUnvisited(roomWatch.Address);
			}
		}

		private void ColorMapRoom(int locationIndex, uint color, bool secondCastle)
		{
			uint row = (uint) locations[locationIndex].MapRow;
			uint col = (uint) locations[locationIndex].MapCol;
			if (secondCastle)
			{
				row = 199 - row;
				col = 126 - col;
			}
			uint borderColor = color > 0 ? (uint) MapColor.Border : 0;

			if (locations[locationIndex].EquipmentExtension && spreadExtension == false)
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
			Location[] uncheckedLocation = locations.Where(l => (!l.Visited && l.SecondCastle == secondCastle)).Take(4).ToArray();

			for (int i = 0; i < uncheckedLocation.Length; i++)
			{
				uint row = (uint) uncheckedLocation[i].MapRow;
				uint col = (uint) uncheckedLocation[i].MapCol;
				if (secondCastle)
				{
					row = 199 - row;
					col = 126 - col;
				}
				if (!sotnApi.RenderingApi.RoomIsRendered(row, col))
				{
					return false;
				}
			}

			return true;
		}

		private void CheckRooms(WatchList watchlist)
		{
			for (int j = 0; j < watchlist.Count; j++)
			{
				Watch? watch = watchlist[j];
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

			for (int j = 0; j < locations.Count; j++)
			{
				Location? location = locations[j];
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

			TrackerRelic relic = relics.Where(relic => relic.Name.ToLower() == name).FirstOrDefault();
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

		private void SaveReplayLine()
		{
			byte replayX = currentMapX;
			byte replayY = currentMapY;

			if (currentMapX < 5 && currentMapY < 4)
			{
				if (sotnApi.GameApi.Zone2 == (uint) SotnApi.Constants.Values.Game.Enums.Zone.Prologue)
				{
					replayX += 2;
					replayY += 32;
				}
				else if (sotnApi.GameApi.Zone2 == (uint) SotnApi.Constants.Values.Game.Enums.Zone.Nightmare)
				{
					replayX += 46;
					replayY += 33;
				}
			}

			if ((inGame && (replayX > 1 && replayY > 0) && (replayX < 200 && replayY < 200) && !(replayY == 44 && replayX < 19 && !secondCastle)) && (replay.Count == 0 || (replay[replay.Count - 1].X != replayX || replay[replay.Count - 1].Y != replayY)))
			{
				if (replay.Count == 0)
				{
					replay.Add(new ReplayState { X = replayX, Y = replayY });
				}
				else
				{
					replay.Add(new ReplayState { X = (byte) (replayX + (secondCastle ? 100 : 0)), Y = replayY });
				}
			}

			var state = replay[replay.Count - 1];
			state.Time++;
		}

		public void SaveReplay()
		{
			if (replay.Count < 30)
			{
				return;
			}

			if (!finished)
			{
				stopWatch.Stop();
				finished = true;
				finalTime = stopWatch.Elapsed;
			}

			string username = toolConfig.Tracker.Username;

			if (username is not null && username.Length > 18)
			{
				username = username.Substring(0, 18);
			}

			byte version = 2;
			string baseReplayPath = seedName + preset.ToUpper() + "-" + username;
			string replayPath = baseReplayPath;
			while (File.Exists(Paths.ReplaysPath + replayPath + ".sotnr"))
			{
				replayPath = version + baseReplayPath;
				version++;
				if (version > 10)
				{
					return;
				}
			}
			replayPath = Paths.ReplaysPath + replayPath + ".sotnr";

			byte[] replayBytes = new byte[2 + (replay.Count * 4) + ((relics.Count + progressionItems.Count + 1) * 4)];

			int replayIndex = 0;
			byte[] finalTimeSecondsBytes = BitConverter.GetBytes((ushort) finalTime.TotalSeconds);
			replayBytes[replayIndex] = finalTimeSecondsBytes[0];
			replayIndex++;
			replayBytes[replayIndex] = finalTimeSecondsBytes[1];
			replayIndex++;

			foreach (var relic in relics)
			{
				replayBytes[replayIndex] = relic.X;
				replayIndex++;
				replayBytes[replayIndex] = relic.Y;
				replayIndex++;

				byte[] colletedBytes = BitConverter.GetBytes(relic.CollectedAt);
				replayBytes[replayIndex] = colletedBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = colletedBytes[1];
				replayIndex++;
			}

			foreach (var progressionItem in progressionItems)
			{
				replayBytes[replayIndex] = progressionItem.X;
				replayIndex++;
				replayBytes[replayIndex] = progressionItem.Y;
				replayIndex++;

				byte[] colletedBytes = BitConverter.GetBytes(progressionItem.CollectedAt);
				replayBytes[replayIndex] = colletedBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = colletedBytes[1];
				replayIndex++;
			}

			Item foundSword = thrustSwords.Where(s => s.CollectedAt > 0).FirstOrDefault();

			if (foundSword != null)
			{
				replayBytes[replayIndex] = foundSword.X;
				replayIndex++;
				replayBytes[replayIndex] = foundSword.Y;
				replayIndex++;

				byte[] colletedBytes = BitConverter.GetBytes(foundSword.CollectedAt);
				replayBytes[replayIndex] = colletedBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = colletedBytes[1];
				replayIndex++;
			}
			else
			{
				replayBytes[replayIndex] = 0;
				replayIndex++;
				replayBytes[replayIndex] = 0;
				replayIndex++;

				replayBytes[replayIndex] = 0;
				replayIndex++;
				replayBytes[replayIndex] = 0;
				replayIndex++;
			}

			foreach (var state in replay)
			{
				replayBytes[replayIndex] = state.X;
				replayIndex++;
				replayBytes[replayIndex] = state.Y;
				replayIndex++;

				byte[] timeBytes = BitConverter.GetBytes(state.Time);
				replayBytes[replayIndex] = timeBytes[0];
				replayIndex++;
				replayBytes[replayIndex] = timeBytes[1];
				replayIndex++;
			}

			File.WriteAllBytes(replayPath, replayBytes);
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
			if (sotnApi.GameApi.Hours == 0 && sotnApi.GameApi.Minutes == 0 && sotnApi.GameApi.Seconds == 3 && inGame)
			{
				if (toolConfig.Tracker.EnableAutosplitter && !autosplitter.Started)
				{
					autosplitter.StartTImer();
				}
				if (!started)
				{
					started = true;
					stopWatch.Start();
				}
			}
		}

		private void CheckSplit()
		{
			if (sotnApi.AlucardApi.MapX == 31 && sotnApi.AlucardApi.MapY == 30 && sotnApi.GameApi.Status == Status.InGame)
			{
				LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(DraculaActorAddress);
				if (boss.Hp > 13 && boss.Hp < 10000 && boss.AiId != 0)
				{
					draculaSpawned = true;
				}
				else if (draculaSpawned && boss.Hp < 1 && boss.AiId != 0)
				{
					if (toolConfig.Tracker.EnableAutosplitter)
					{
						autosplitter.Split();
					}
					stopWatch.Stop();
					finished = true;
					finalTime = stopWatch.Elapsed;
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

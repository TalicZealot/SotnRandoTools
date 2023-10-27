using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
	internal sealed class Tracker
	{
		const byte ReplayCooldown = 6;
		const string DefaultSeedInfo = "seed(preset)";
		const long DraculaEntityAddress = 0x076e98;
		private const int AutosplitterReconnectCooldown = 120;
		private readonly ITrackerGraphicsEngine trackerGraphicsEngine;
		private readonly IToolConfig toolConfig;
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

		private readonly List<Location> locations = new List<Location>
		{
			new Location { Name = "SoulOfBat", MapRow = 66, MapCol = 96, WatchIndecies = new List<int>{0}, Rooms = new List<Room>{
					new Room { WatchIndex = 0, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "FireOfBat", MapRow = 26, MapCol = 118, WatchIndecies = new List<int>{1}, Rooms = new List<Room>{
					new Room { WatchIndex = 1, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "EchoOfBat", MapRow = 46, MapCol = 32, WatchIndecies = new List<int>{2}, Rooms = new List<Room>{
					new Room { WatchIndex = 2, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "SoulOfWolf", MapRow = 54, MapCol = 122, WatchIndecies = new List<int>{3, 4}, Rooms = new List<Room>{
					new Room { WatchIndex = 3, Values = new int[] { 0x10 }},
					new Room { WatchIndex = 4, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "PowerOfWolf", MapRow = 134, MapCol = 4, WatchIndecies = new List<int>{5}, Rooms = new List<Room>{
					new Room { WatchIndex = 5, Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "SkillOfWolf", MapRow = 114, MapCol = 30, WatchIndecies = new List<int>{6}, Rooms = new List<Room>{
					new Room { WatchIndex = 6, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "FormOfMist", MapRow = 70, MapCol = 42, WatchIndecies = new List<int>{7}, Rooms = new List<Room>{
					new Room { WatchIndex = 7, Values = new int[] { 0x04, 0x10 }},
			}},
			new Location { Name = "PowerOfMist", MapRow = 18, MapCol = 62, WatchIndecies = new List<int>{8, 9, 10}, Rooms = new List<Room>{
					new Room { WatchIndex = 8, Values = new int[] { 0x01 }},
					new Room { WatchIndex = 9, Values = new int[] { 0x01 }},
					new Room { WatchIndex = 10, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "CubeOfZoe", MapRow = 126, MapCol = 38, WatchIndecies = new List<int>{11}, Rooms = new List<Room>{
					new Room { WatchIndex = 11, Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "SpiritOrb", MapRow = 107, MapCol = 50, WatchIndecies = new List<int>{12, 13}, Rooms = new List<Room>{
					new Room { WatchIndex = 12, Values = new int[] { 0x10 }},
					new Room { WatchIndex = 13, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "GravityBoots", MapRow = 74, MapCol = 68, WatchIndecies = new List<int>{14}, Rooms = new List<Room>{
					new Room { WatchIndex = 14, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "LeapStone", MapRow = 26, MapCol = 62, WatchIndecies = new List<int>{15, 16}, Rooms = new List<Room>{
					new Room { WatchIndex = 15, Values = new int[] { 0x01 }},
					new Room { WatchIndex = 16, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "HolySymbol", MapRow = 146, MapCol = 110, WatchIndecies = new List<int>{17}, Rooms = new List<Room>{
					new Room { WatchIndex = 17, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "FaerieScroll", MapRow = 54, MapCol = 118, WatchIndecies = new List<int>{18}, Rooms = new List<Room>{
					new Room { WatchIndex = 18, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "JewelOfOpen", MapRow = 62, MapCol = 98, WatchIndecies = new List<int>{19}, Rooms = new List<Room>{
					new Room { WatchIndex = 19, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "MermanStatue", MapRow = 150, MapCol = 16, WatchIndecies = new List<int>{20}, Rooms = new List<Room>{
					new Room { WatchIndex = 20, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "BatCard", MapRow = 90, MapCol = 26, WatchIndecies = new List<int>{21}, Rooms = new List<Room>{
					new Room { WatchIndex = 21, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "GhostCard", MapRow = 10, MapCol = 78, WatchIndecies = new List<int>{22}, Rooms = new List<Room>{
					new Room { WatchIndex = 22, Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "FaerieCard", MapRow = 54, MapCol = 104, WatchIndecies = new List<int>{23}, Rooms = new List<Room>{
					new Room { WatchIndex = 23, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "DemonCard", MapRow = 158, MapCol = 58, WatchIndecies = new List<int>{24}, Rooms = new List<Room>{
					new Room { WatchIndex = 24, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "SwordCard", MapRow = 54, MapCol = 40, WatchIndecies = new List<int>{25}, Rooms = new List<Room>{
					new Room { WatchIndex = 25, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "CrystalCloak",  GuardedExtension = true,  SpreadExtension = true, MapRow = 134, MapCol = 80, WatchIndecies = new List<int>{26}, Rooms = new List<Room>{
					new Room { WatchIndex = 26, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Mormegil",  GuardedExtension = true, MapRow = 182, MapCol = 34, WatchIndecies = new List<int>{27}, Rooms = new List<Room>{
					new Room { WatchIndex = 27, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "GoldRing", MapRow = 114, MapCol = 90, WatchIndecies = new List<int>{28}, Rooms = new List<Room>{
					new Room { WatchIndex = 28, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Spikebreaker", MapRow = 186, MapCol = 82, WatchIndecies = new List<int>{29}, Rooms = new List<Room>{
					new Room { WatchIndex = 29, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "SilverRing", MapRow = 42, MapCol = 17, WatchIndecies = new List<int>{30}, Rooms = new List<Room>{
					new Room { WatchIndex = 30, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "HolyGlasses", MapRow = 106, MapCol = 64, WatchIndecies = new List<int>{31}, Rooms = new List<Room>{
					new Room { WatchIndex = 31, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "HeartOfVlad",  SecondCastle = true, MapRow = 165, MapCol = 80, WatchIndecies = new List<int>{32, 33}, Rooms = new List<Room>{
					new Room { WatchIndex = 32, Values = new int[] { 0x01 }},
					new Room { WatchIndex = 33, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "ToothOfVlad",  SecondCastle = true, MapRow = 125, MapCol = 12, WatchIndecies = new List<int>{34}, Rooms = new List<Room>{
					new Room { WatchIndex = 34, Values = new int[] { 0x10, 0x04 }},
			}},
			new Location { Name = "RibOfVlad",  SecondCastle = true, MapRow = 153, MapCol = 88, WatchIndecies = new List<int>{35}, Rooms = new List<Room>{
					new Room { WatchIndex = 35, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "RingOfVlad",  SecondCastle = true, MapRow = 177, MapCol = 46, WatchIndecies = new List<int>{36}, Rooms = new List<Room>{
					new Room { WatchIndex = 36, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "EyeOfVlad",  SecondCastle = true, MapRow = 57, MapCol = 66, WatchIndecies = new List<int>{37}, Rooms = new List<Room>{
					new Room { WatchIndex = 37, Values = new int[] { 0x10, 0x40 }},
			}},
			new Location { Name = "ForceOfEcho",  SecondCastle = true, MapRow = 53, MapCol = 16, WatchIndecies = new List<int>{38}, Rooms = new List<Room>{
					new Room { WatchIndex = 38, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "GasCloud",  SecondCastle = true, MapRow = 17, MapCol = 92, WatchIndecies = new List<int>{39}, Rooms = new List<Room>{
					new Room { WatchIndex = 39, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "RingOfArcana",  SecondCastle = true,  GuardedExtension = true,  SpreadExtension = true, MapRow = 109, MapCol = 100, WatchIndecies = new List<int>{40}, Rooms = new List<Room>{
					new Room { WatchIndex = 40, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "DarkBlade",  SecondCastle = true,  GuardedExtension = true,  SpreadExtension = true, MapRow = 65, MapCol = 46, WatchIndecies = new List<int>{41}, Rooms = new List<Room>{
					new Room { WatchIndex = 41, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Trio",  SecondCastle = true,  GuardedExtension = true,  SpreadExtension = true, MapRow = 129, MapCol = 86, WatchIndecies = new List<int>{42, 43}, Rooms = new List<Room>{
					new Room { WatchIndex = 42, Values = new int[] { 0x40 }},
					new Room { WatchIndex = 43, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Walk armor",  EquipmentExtension = true, MapRow = 182, MapCol = 46, WatchIndecies = new List<int>{0}, Rooms = new List<Room>{
					new Room { WatchIndex = 0, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Icebrand",  EquipmentExtension = true, MapRow = 182, MapCol = 48, WatchIndecies = new List<int>{1}, Rooms = new List<Room>{
					new Room { WatchIndex = 1, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Bloodstone",  EquipmentExtension = true, MapRow = 182, MapCol = 56, WatchIndecies = new List<int>{3}, Rooms = new List<Room>{
					new Room { WatchIndex = 3, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Combat knife",  EquipmentExtension = true, MapRow = 174, MapCol = 62, WatchIndecies = new List<int>{4}, Rooms = new List<Room>{
					new Room { WatchIndex = 4, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Ring of Ares",  EquipmentExtension = true, MapRow = 146, MapCol = 74, WatchIndecies = new List<int>{5}, Rooms = new List<Room>{
					new Room { WatchIndex = 5, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Knuckle duster",  EquipmentExtension = true, MapRow = 150, MapCol = 80, WatchIndecies = new List<int>{6}, Rooms = new List<Room>{
					new Room { WatchIndex = 6, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Caverns Onyx",  EquipmentExtension = true, MapRow = 146, MapCol = 92, WatchIndecies = new List<int>{7}, Rooms = new List<Room>{
					new Room { WatchIndex = 7, Values = new int[] { 0x10, 0x40 }},
			}},
			new Location { Name = "Bandanna",  EquipmentExtension = true, MapRow = 90, MapCol = 70, WatchIndecies = new List<int>{11}, Rooms = new List<Room>{
					new Room { WatchIndex = 11, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Nunchaku",  EquipmentExtension = true, MapRow = 134, MapCol = 76, WatchIndecies = new List<int>{12}, Rooms = new List<Room>{
					new Room { WatchIndex = 12, Values = new int[] { 0x04, 0x10 }},
			}},
			new Location { Name = "Secret Boots",  EquipmentExtension = true, MapRow = 138, MapCol = 48, WatchIndecies = new List<int>{13, 14}, Rooms = new List<Room>{
					new Room { WatchIndex = 13, Values = new int[] { 0x01 }},
					new Room { WatchIndex = 14, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Holy mail",  EquipmentExtension = true, MapRow = 134, MapCol = 10, WatchIndecies = new List<int>{16}, Rooms = new List<Room>{
					new Room { WatchIndex = 16, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Jewel sword",  EquipmentExtension = true, MapRow = 146, MapCol = 20, WatchIndecies = new List<int>{17}, Rooms = new List<Room>{
					new Room { WatchIndex = 17, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Sunglasses",  EquipmentExtension = true, MapRow = 106, MapCol = 32, WatchIndecies = new List<int>{20}, Rooms = new List<Room>{
					new Room { WatchIndex = 20, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Basilard",  EquipmentExtension = true, MapRow = 118, MapCol = 32, WatchIndecies = new List<int>{21}, Rooms = new List<Room>{
					new Room { WatchIndex = 21, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Cloth cape",  EquipmentExtension = true, MapRow = 98, MapCol = 20, WatchIndecies = new List<int>{22}, Rooms = new List<Room>{
					new Room { WatchIndex = 22, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Mystic pendant",  EquipmentExtension = true, MapRow = 86, MapCol = 8, WatchIndecies = new List<int>{23, 24}, Rooms = new List<Room>{
					new Room { WatchIndex = 23, Values = new int[] { 0x40 }},
					new Room { WatchIndex = 24, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Ankh of Life",  EquipmentExtension = true, MapRow = 78, MapCol = 12, WatchIndecies = new List<int>{25}, Rooms = new List<Room>{
					new Room { WatchIndex = 25, Values = new int[] { 0x04, 0x01 }},
			}},
			new Location { Name = "Morningstar",  EquipmentExtension = true, MapRow = 66, MapCol = 16, WatchIndecies = new List<int>{26, 27}, Rooms = new List<Room>{
					new Room { WatchIndex = 26, Values = new int[] { 0x10 }},
					new Room { WatchIndex = 27, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Goggles",  EquipmentExtension = true, MapRow = 66, MapCol = 20, WatchIndecies = new List<int>{28}, Rooms = new List<Room>{
					new Room { WatchIndex = 28, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Silver plate",  EquipmentExtension = true, MapRow = 30, MapCol = 28, WatchIndecies = new List<int>{29}, Rooms = new List<Room>{
					new Room { WatchIndex = 29, Values = new int[] { 0x04, 0x01 }},
			}},
			new Location { Name = "Cutlass",  EquipmentExtension = true, MapRow = 22, MapCol = 54, WatchIndecies = new List<int>{30, 31}, Rooms = new List<Room>{
					new Room { WatchIndex = 30, Values = new int[] { 0x40 }},
					new Room { WatchIndex = 31, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Platinum mail",  EquipmentExtension = true, MapRow = 6, MapCol = 70, WatchIndecies = new List<int>{32}, Rooms = new List<Room>{
					new Room { WatchIndex = 32, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Falchion",  EquipmentExtension = true, MapRow = 14, MapCol = 78, WatchIndecies = new List<int>{33}, Rooms = new List<Room>{
					new Room { WatchIndex = 33, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Gold plate",  EquipmentExtension = true, MapRow = 38, MapCol = 98, WatchIndecies = new List<int>{34}, Rooms = new List<Room>{
					new Room { WatchIndex = 34, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Bekatowa",  EquipmentExtension = true, MapRow = 38, MapCol = 111, WatchIndecies = new List<int>{35, 36}, Rooms = new List<Room>{
					new Room { WatchIndex = 35, Values = new int[] { 0x01, 0x04 }},
					new Room { WatchIndex = 36, Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "Gladius",  EquipmentExtension = true, MapRow = 74, MapCol = 118, WatchIndecies = new List<int>{37}, Rooms = new List<Room>{
					new Room { WatchIndex = 37, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Jewel knuckles",  EquipmentExtension = true, MapRow = 90, MapCol = 118, WatchIndecies = new List<int>{38}, Rooms = new List<Room>{
					new Room { WatchIndex = 38, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Bronze cuirass",  EquipmentExtension = true, MapRow = 66, MapCol = 98, WatchIndecies = new List<int>{39}, Rooms = new List<Room>{
					new Room { WatchIndex = 39, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Holy rod",  EquipmentExtension = true, MapRow = 54, MapCol = 100, WatchIndecies = new List<int>{40}, Rooms = new List<Room>{
					new Room { WatchIndex = 40, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Library Onyx",  EquipmentExtension = true, MapRow = 66, MapCol = 94, WatchIndecies = new List<int>{41}, Rooms = new List<Room>{
					new Room { WatchIndex = 41, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Alucart Sword",  EquipmentExtension = true, MapRow = 82, MapCol = 68, WatchIndecies = new List<int>{42}, Rooms = new List<Room>{
					new Room { WatchIndex = 42, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Broadsword",  EquipmentExtension = true, MapRow = 70, MapCol = 64, WatchIndecies = new List<int>{43}, Rooms = new List<Room>{
					new Room { WatchIndex = 43, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Estoc",  EquipmentExtension = true, MapRow = 42, MapCol = 60, WatchIndecies = new List<int>{44}, Rooms = new List<Room>{
					new Room { WatchIndex = 44, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Olrox Garnet",  EquipmentExtension = true, MapRow = 54, MapCol = 66, WatchIndecies = new List<int>{45}, Rooms = new List<Room>{
					new Room { WatchIndex = 45, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Holy sword",  EquipmentExtension = true, MapRow = 62, MapCol = 38, WatchIndecies = new List<int>{46}, Rooms = new List<Room>{
					new Room { WatchIndex = 46, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Knight shield",  EquipmentExtension = true, MapRow = 70, MapCol = 28, WatchIndecies = new List<int>{47}, Rooms = new List<Room>{
					new Room { WatchIndex = 47, Values = new int[] { 0x01, 0x04 }},
			}},
			new Location { Name = "Shield rod",  EquipmentExtension = true, MapRow = 78, MapCol = 26, WatchIndecies = new List<int>{48}, Rooms = new List<Room>{
					new Room { WatchIndex = 48, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Blood cloak",  EquipmentExtension = true, MapRow = 78, MapCol = 40, WatchIndecies = new List<int>{49}, Rooms = new List<Room>{
					new Room { WatchIndex = 49, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Bastard sword",  SecondCastle = true,  EquipmentExtension = true, MapRow = 193, MapCol = 60, WatchIndecies = new List<int>{50}, Rooms = new List<Room>{
					new Room { WatchIndex = 50, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Royal cloak",  SecondCastle = true,  EquipmentExtension = true, MapRow = 193, MapCol = 56, WatchIndecies = new List<int>{51}, Rooms = new List<Room>{
					new Room { WatchIndex = 51, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Lightning mail",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 48, WatchIndecies = new List<int>{52}, Rooms = new List<Room>{
					new Room { WatchIndex = 52, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Sword of Dawn",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 64, WatchIndecies = new List<int>{53, 54}, Rooms = new List<Room>{
					new Room { WatchIndex = 53, Values = new int[] { 0x40 }},
					new Room { WatchIndex = 54, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Moon rod",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 42, WatchIndecies = new List<int>{55}, Rooms = new List<Room>{
					new Room { WatchIndex = 55, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Sunstone",  SecondCastle = true,  EquipmentExtension = true, MapRow = 161, MapCol = 28, WatchIndecies = new List<int>{56}, Rooms = new List<Room>{
					new Room { WatchIndex = 56, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Luminus",  SecondCastle = true,  EquipmentExtension = true, MapRow = 164, MapCol = 16, WatchIndecies = new List<int>{57}, Rooms = new List<Room>{
					new Room { WatchIndex = 57, Values = new int[] { 0x10, 0x40 }},
			}},
			new Location { Name = "Dragon helm",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 8, WatchIndecies = new List<int>{58}, Rooms = new List<Room>{
					new Room { WatchIndex = 58, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Shotel",  SecondCastle = true,  EquipmentExtension = true, MapRow = 109, MapCol = 8, WatchIndecies = new List<int>{59}, Rooms = new List<Room>{
					new Room { WatchIndex = 59, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Badelaire",  SecondCastle = true,  EquipmentExtension = true, SpreadExtension = true, MapRow = 145, MapCol = 26, WatchIndecies = new List<int>{60}, Rooms = new List<Room>{
					new Room { WatchIndex = 60, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Staurolite",  SecondCastle = true,  EquipmentExtension = true, MapRow = 132, MapCol = 30, WatchIndecies = new List<int>{62}, Rooms = new List<Room>{
					new Room { WatchIndex = 62, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Forbidden Library Opal",  SecondCastle = true,  EquipmentExtension = true, SpreadExtension = true, MapRow = 137, MapCol = 27, WatchIndecies = new List<int>{61}, Rooms = new List<Room>{
					new Room { WatchIndex = 61, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Reverse Caverns Diamond",  SecondCastle = true,  EquipmentExtension = true, MapRow = 109, MapCol = 56, WatchIndecies = new List<int>{63}, Rooms = new List<Room>{
					new Room { WatchIndex = 63, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Reverse Caverns Opal",  SecondCastle = true,  EquipmentExtension = true, MapRow = 89, MapCol = 52, WatchIndecies = new List<int>{64}, Rooms = new List<Room>{
					new Room { WatchIndex = 64, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Reverse Caverns Garnet",  SecondCastle = true,  EquipmentExtension = true, MapRow = 69, MapCol = 82, WatchIndecies = new List<int>{65}, Rooms = new List<Room>{
					new Room {  WatchIndex = 65, Values = new int[] { 0x10 }},
			}},
			new Location { Name = "Osafune katana",  SecondCastle = true,  EquipmentExtension = true, MapRow = 49, MapCol = 76, WatchIndecies = new List<int>{66}, Rooms = new List<Room>{
					new Room { WatchIndex = 66, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Alucard shield",  SecondCastle = true,  EquipmentExtension = true, MapRow = 49, MapCol = 110, WatchIndecies = new List<int>{67}, Rooms = new List<Room>{
					new Room { WatchIndex = 67, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Alucard sword",  SecondCastle = true,  EquipmentExtension = true, MapRow = 41, MapCol = 68, WatchIndecies = new List<int>{68}, Rooms = new List<Room>{
					new Room { WatchIndex = 68, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Necklace of J",  SecondCastle = true,  EquipmentExtension = true, MapRow = 17, MapCol = 78, WatchIndecies = new List<int>{69}, Rooms = new List<Room>{
					new Room { WatchIndex = 69, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Floating Catacombs Diamond",  SecondCastle = true,  EquipmentExtension = true, MapRow = 17, MapCol = 80, WatchIndecies = new List<int>{70}, Rooms = new List<Room>{
					new Room { WatchIndex = 70, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Talwar",  SecondCastle = true,  EquipmentExtension = true, MapRow = 173, MapCol = 88, WatchIndecies = new List<int>{71, 72}, Rooms = new List<Room>{
					new Room { WatchIndex = 71, Values = new int[] { 0x01 }},
					new Room { WatchIndex = 72, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Twilight cloak",  SecondCastle = true,  EquipmentExtension = true, MapRow = 157, MapCol = 110, WatchIndecies = new List<int>{73}, Rooms = new List<Room>{
					new Room { WatchIndex = 73, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Alucard Mail",  SecondCastle = true,  EquipmentExtension = true, MapRow = 145, MapCol = 60, WatchIndecies = new List<int>{74}, Rooms = new List<Room>{
					new Room { WatchIndex = 74, Values = new int[] { 0x04 }},
			}},
			new Location { Name = "Sword of Hador",  SecondCastle = true,  EquipmentExtension = true, MapRow = 129, MapCol = 62, WatchIndecies = new List<int>{75}, Rooms = new List<Room>{
					new Room { WatchIndex = 75, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Fury Plate",  SecondCastle = true,  EquipmentExtension = true, MapRow = 137, MapCol = 88, WatchIndecies = new List<int>{76}, Rooms = new List<Room>{
					new Room { WatchIndex = 76, Values = new int[] { 0x40 }},
			}},
			new Location { Name = "Gram",  SecondCastle = true,  EquipmentExtension = true, MapRow = 121, MapCol = 86, WatchIndecies = new List<int>{77}, Rooms = new List<Room>{
					new Room { WatchIndex = 77, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Goddess shield",  SecondCastle = true,  EquipmentExtension = true, MapRow = 93, MapCol = 94, WatchIndecies = new List<int>{78}, Rooms = new List<Room>{
					new Room { WatchIndex = 78, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Katana",  SecondCastle = true,  EquipmentExtension = true, MapRow = 73, MapCol = 102, WatchIndecies = new List<int>{79}, Rooms = new List<Room>{
					new Room { WatchIndex = 79, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Talisman",  SecondCastle = true,  EquipmentExtension = true, MapRow = 69, MapCol = 86, WatchIndecies = new List<int>{80}, Rooms = new List<Room>{
					new Room { WatchIndex = 80, Values = new int[] { 0x01 }},
			}},
			new Location { Name = "Beryl circlet",  SecondCastle = true,  EquipmentExtension = true, MapRow = 53, MapCol = 106, WatchIndecies = new List<int>{81}, Rooms = new List<Room>{
					new Room { WatchIndex = 81, Values = new int[] { 0x10 }},
			}},
		};
		private readonly List<TrackerRelic> relics = new List<TrackerRelic>
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
		private readonly List<Item> progressionItems = new List<Item>
		{
			new Item { Name = "GoldRing", Value = 72 },
			new Item { Name = "SilverRing", Value = 73 },
			new Item { Name = "SpikeBreaker", Value = 14 },
			new Item { Name = "HolyGlasses", Value = 34 }
		};
		private readonly List<Item> thrustSwords = new List<Item>
		{
			new Item { Name = "Estoc", Value = 95 },
			new Item { Name = "Claymore", Value = 98 },
			new Item { Name = "Flamberge", Value = 101 },
			new Item { Name = "Zweihander", Value = 103 },
			new Item { Name = "ObsidianSword", Value = 107 }
		};
		private readonly Dictionary<string, int> relicToIndex = new Dictionary<string, int>
		{
			{ "soulofbat", 0 },
			{ "fireofbat", 1 },
			{ "echoofbat", 2 },
			{ "forceofecho", 3 },
			{ "soulofwolf", 4 },
			{ "powerofwolf", 5 },
			{ "skillofwolf", 6 },
			{ "formofmist", 7 },
			{ "powerofmist", 8 },
			{ "gascloud", 9 },
			{ "cubeofzoe", 10 },
			{ "spiritorb", 11 },
			{ "gravityboots", 12 },
			{ "leapstone", 13 },
			{ "holysymbol", 14 },
			{ "faeriescroll", 15 },
			{ "jewelofopen", 16 },
			{ "mermanstatue", 17 },
			{ "batcard", 18 },
			{ "ghostcard", 19 },
			{ "faeriecard", 20 },
			{ "demoncard", 21 },
			{ "swordcard", 22 },
			{ "spritecard", 23 },
			{ "nosedevilcard", 24 },
			{ "heartofvlad", 25 },
			{ "toothofvlad", 26 },
			{ "ribofvlad", 27 },
			{ "ringofvlad", 28 },
			{ "eyeofvlad", 29 }
		};
		private readonly Dictionary<string, int> itemToIndex = new Dictionary<string, int>
		{
			{ "goldring", 0 },
			{ "silverring", 1 },
			{ "spikebreaker", 2 },
			{ "holyglasses", 3 },
		};
		private readonly Dictionary<string, int> swordToIndex = new Dictionary<string, int>
		{
			{ "estoc", 0 },
			{ "claymore", 1 },
			{ "flamberge", 2 },
			{ "zweihander", 3 },
			{ "obsidiansword", 4 },
		};
		private readonly HashSet<string> VanillaPresets = new HashSet<string>
		{
			"adventure",
			"bat-master",
			"casual",
			"empty-hand",
			"expedition",
			//"gem-farmer",
			//"glitch",
			"guarded-og",
			"lycanthrope",
			"nimble",
			"og",
			//"rat-race",
			"safe",
			"scavenger",
			"speedrun",
			"third-castle",
			"warlock"
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
		private bool replaySaved = false;
		private TimeSpan finalTime;
		private Stopwatch stopWatch = new Stopwatch();

		public Tracker(ITrackerGraphicsEngine trackerGraphicsEngine, IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, INotificationService notificationService)
		{
			this.trackerGraphicsEngine = trackerGraphicsEngine ?? throw new ArgumentNullException(nameof(trackerGraphicsEngine));
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.watchlistService = watchlistService ?? throw new ArgumentNullException(nameof(watchlistService));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));
			this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

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

			if (toolConfig.Tracker.Stereo && sotnApi.GameApi.Status == Status.MainMenu)
			{
				sotnApi.GameApi.EnableStartWithStereo();
			}

			inGame = sotnApi.GameApi.Status == Status.InGame;
			bool updatedSecondCastle = sotnApi.GameApi.SecondCastle;
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
				bool objectChanges = false;

				if (updatedSecondCastle != secondCastle && toolConfig.Tracker.Locations)
				{
					secondCastle = updatedSecondCastle;
					CheckReachability();
					SetMapLocations();
				}
				else if (updatedSecondCastle != secondCastle)
				{
					secondCastle = updatedSecondCastle;
				}

				objectChanges = objectChanges || UpdateRelics();
				objectChanges = objectChanges || UpdateProgressionItems();
				objectChanges = objectChanges || UpdateThrustSwords();
				if (objectChanges)
				{
					UpdateOverlay();
					DrawRelicsAndItems();
					if (toolConfig.Tracker.Locations)
					{
						CheckReachability();
					}
				}

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

			if (toolConfig.Tracker.EnableAutosplitter && !started && !autosplitterConnected && autosplitterReconnectCounter == AutosplitterReconnectCooldown)
			{
				autosplitterConnected = autosplitter.AtemptConnect();
				autosplitterReconnectCounter = 0;
			}
			else if (toolConfig.Tracker.EnableAutosplitter && !started && !autosplitterConnected && autosplitterReconnectCounter < 120 && !sotnApi.GameApi.InAlucardMode())
			{
				autosplitterReconnectCounter++;
			}
			else if (toolConfig.Tracker.EnableAutosplitter && !started && autosplitterConnected && !sotnApi.GameApi.InAlucardMode())
			{
				autosplitterConnected = autosplitter.IsConnected();
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

			bool currentTrackIsSong = sotnApi.GameApi.MusicTrack <= Various.MusicTrackValues.Last();

			if (!muted && currentTrackIsSong)
			{
				sotnApi.GameApi.MuteXA();
				muted = true;
			}
			else if (muted && !currentTrackIsSong)
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
			if (!File.Exists(presetFilePath))
			{
				return;
			}

			JObject preset = JObject.Parse(File.ReadAllText(presetFilePath));
			string presetId = preset.SelectToken("metadata.id").ToString();
			string relicLocationsExtension = (preset.GetValue("relicLocationsExtension") ?? "gaurded").ToString();
			JToken? presetLocations = preset["lockLocation"];
			JToken? presetLocationsAllowed = preset["lockLocationAllowed"];
			bool isVanilla = VanillaPresets.Contains(presetId);

			if (presetId == "glitch")
			{
				relicLocationsExtension = "equipment";
			}

			switch (relicLocationsExtension)
			{
				case "equipment":
					equipmentExtension = true;
					guardedExtension = true;
					SetEquipmentProgression();
					break;
				case "spread":
					spreadExtension = true;
					break;
				case "False":
					guardedExtension = false;
					break;
				default:
					guardedExtension = true;
					break;
			}

			if (presetLocations is null)
			{
				return;
			}

			foreach (JObject location in presetLocations)
			{
				string name = location["location"].ToString().Replace(" ", String.Empty).ToLower();
				var trackerLocation = locations.Where(x => x.Name.Replace(" ", String.Empty).ToLower() == name).FirstOrDefault();

				if (trackerLocation == null)
				{
					Console.WriteLine($"Could not find location {name}.");
					continue;
				}

				if (overwriteLocks)
				{
					if (outOfLogic || !isVanilla)
					{
						trackerLocation.OutOfLogicLocks.Clear();
					}
					if (!outOfLogic)
					{
						trackerLocation.Locks.Clear();
					}
				}

				foreach (JToken lockSet in location["locks"])
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

			if (presetLocationsAllowed is null)
			{
				return;
			}

			foreach (JObject location in presetLocationsAllowed)
			{
				string name = location["location"].ToString().Replace(" ", String.Empty).ToLower();
				var trackerLocation = locations.Where(x => x.Name.Replace(" ", String.Empty).ToLower() == name).FirstOrDefault();

				if (trackerLocation == null)
				{
					Console.WriteLine($"Could not find location {name}.");
					continue;
				}

				trackerLocation.OutOfLogicLocks.Clear();

				foreach (JToken lockSet in location["locks"])
				{
					trackerLocation.OutOfLogicLocks.Add(lockSet.ToString().Replace(" ", String.Empty).ToLower().Split('+'));
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
				watchlistService.UpdateWatchlist(watchlistService.SafeLocationWatches);
				CheckRooms(watchlistService.SafeLocationWatches);
				watchlistService.SafeLocationWatches.ClearChangeCounts();
				if (equipmentExtension || spreadExtension)
				{
					watchlistService.UpdateWatchlist(watchlistService.EquipmentLocationWatches);
					CheckRooms(watchlistService.EquipmentLocationWatches, true);
					watchlistService.EquipmentLocationWatches.ClearChangeCounts();
				}

			}
			roomCount = currentRooms;
		}

		private bool UpdateRelics()
		{
			int changes = 0;

			watchlistService.UpdateWatchlist(watchlistService.RelicWatches);
			for (int i = 0; i < watchlistService.RelicWatches.Count; i++)
			{
				if (watchlistService.RelicWatches[i].ChangeCount == 0)
				{
					continue;
				}

				if (watchlistService.RelicWatches[i].Value > 0)
				{
					if (relics[i].CollectedAt == 0)
					{
						relics[i].X = currentMapX;
						if (secondCastle)
						{
							relics[i].X += 100;
						}
						relics[i].Y = currentMapY;
						relics[i].CollectedAt = (ushort) replay.Count;
					}
					if (!relics[i].Collected)
					{
						changes++;
						relics[i].Collected = true;
					}
				}
				else
				{
					if (relics[i].Collected)
					{
						changes++;
						relics[i].Collected = false;
					}
				}
			}
			watchlistService.RelicWatches.ClearChangeCounts();

			return changes > 0;
		}

		private bool UpdateProgressionItems()
		{
			int changes = 0;
			watchlistService.UpdateWatchlist(watchlistService.ProgressionItemWatches);
			for (int i = 0; i < watchlistService.ProgressionItemWatches.Count; i++)
			{
				if (watchlistService.ProgressionItemWatches[i].ChangeCount == 0)
				{
					continue;
				}
				if (watchlistService.ProgressionItemWatches[i].Value > 0)
				{
					if (progressionItems[i].CollectedAt == 0)
					{
						progressionItems[i].X = currentMapX;
						if (secondCastle)
						{
							progressionItems[i].X += 100;
						}
						progressionItems[i].Y = currentMapY;
						progressionItems[i].CollectedAt = (ushort) replay.Count;
					}
					progressionItems[i].Collected = true;
				}
				else
				{
					progressionItems[i].Collected = false;
				}
			}
			watchlistService.ProgressionItemWatches.ClearChangeCounts();

			for (int i = 0; i < progressionItems.Count; i++)
			{
				switch (i)
				{
					case 0:
					case 1:
						progressionItems[i].Equipped = (sotnApi.AlucardApi.Accessory1 == progressionItems[i].Value) || (sotnApi.AlucardApi.Accessory2 == progressionItems[i].Value);
						break;
					case 2:
						progressionItems[i].Equipped = (sotnApi.AlucardApi.Armor == progressionItems[i].Value);
						break;
					case 3:
						progressionItems[i].Equipped = (sotnApi.AlucardApi.Helm == progressionItems[i].Value);
						break;
					default:
						progressionItems[i].Equipped = false;
						break;
				}

				if (!progressionItems[i].Status && (progressionItems[i].Collected || progressionItems[i].Equipped))
				{
					progressionItems[i].Status = true;
					changes++;
				}
				else if (progressionItems[i].Status && !(progressionItems[i].Collected || progressionItems[i].Equipped))
				{
					progressionItems[i].Status = false;
					changes++;
				}
			}

			return changes > 0;
		}

		private bool UpdateThrustSwords()
		{
			int changes = 0;

			watchlistService.UpdateWatchlist(watchlistService.ThrustSwordWatches);
			for (int i = 0; i < watchlistService.ThrustSwordWatches.Count; i++)
			{
				if (watchlistService.ThrustSwordWatches[i].ChangeCount == 0)
				{
					continue;
				}
				if (watchlistService.ThrustSwordWatches[i].Value > 0)
				{
					if (thrustSwords[i].CollectedAt == 0)
					{
						thrustSwords[i].X = currentMapX;
						if (secondCastle)
						{
							thrustSwords[i].X += 100;
						}
						thrustSwords[i].Y = currentMapY;
						thrustSwords[i].CollectedAt = (ushort) replay.Count;
					}
					thrustSwords[i].Collected = true;

				}
				else
				{
					thrustSwords[i].Collected = false;
				}
			}
			watchlistService.ThrustSwordWatches.ClearChangeCounts();

			for (int i = 0; i < thrustSwords.Count; i++)
			{
				thrustSwords[i].Equipped = (sotnApi.AlucardApi.RightHand == thrustSwords[i].Value);

				if (!thrustSwords[i].Status && (thrustSwords[i].Collected || thrustSwords[i].Equipped))
				{
					thrustSwords[i].Status = true;
					changes++;
				}
				else if (thrustSwords[i].Status && !(thrustSwords[i].Collected || thrustSwords[i].Equipped))
				{
					thrustSwords[i].Status = false;
					changes++;
				}
			}

			return changes > 0;
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
			if (preset == "custom")
			{
				guardedExtension = toolConfig.Tracker.CustomLocationsGuarded;
				equipmentExtension = toolConfig.Tracker.CustomLocationsEquipment;
				spreadExtension = toolConfig.Tracker.CustomLocationsSpread;
				if (equipmentExtension)
				{
					guardedExtension = true;
					SetEquipmentProgression();
				}
			}
			else
			{
				LoadLocks(Paths.PresetPath + preset + ".json", false, true);
			}
			SaveSeedInfo(SeedInfo);
			PrepareMapLocations();
		}

		private void SetEquipmentProgression()
		{
			//Set Cube of Zoe, Demon card and Nose Devil to progression
			relics[10].Progression = true;
			relics[21].Progression = true;
			relics[24].Progression = true;
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
				Watch roomWatch;
				if (locations[locationIndex].EquipmentExtension)
				{
					roomWatch = watchlistService.EquipmentLocationWatches[room.WatchIndex];
				}
				else
				{
					roomWatch = watchlistService.SafeLocationWatches[room.WatchIndex];
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

		private void CheckRooms(WatchList watchlist, bool equipment = false)
		{
			for (int j = 0; j < watchlist.Count; j++)
			{
				Watch? watch = watchlist[j];

				if (watch.ChangeCount == 0)
				{
					continue;
				}

				Location? location = locations.Where(x => x.WatchIndecies.Contains(j) && x.EquipmentExtension == equipment).FirstOrDefault();
				Room? room = location?.Rooms.Where(y => y.WatchIndex == j).FirstOrDefault();

				if (location == null || room == null || location.Visited || watch.Value == 0)
				{
					continue;
				}

				foreach (int value in room.Values)
				{
					if ((watch.Value & value) == value)
					{
						location.Visited = true;
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
			if (name is null) { throw new ArgumentNullException(nameof(name)); }

			if (relicToIndex.ContainsKey(name))
			{
				TrackerRelic relic = relics[relicToIndex[name]];
				return relic.Collected;
			}
			if (name == "holyglasses" && secondCastle)
			{
				return true;
			}
			if (itemToIndex.ContainsKey(name))
			{
				Item progressionItem = progressionItems[itemToIndex[name]];
				return progressionItem.Status;
			}
			if (name == "thrustsword" && swordToIndex.ContainsKey(name))
			{
				Item thrustSword = thrustSwords[swordToIndex[name]];
				return true;
			}

			return false;
		}

		private void UpdateOverlay()
		{
			if (toolConfig.Tracker.UseOverlay)
			{
				notificationService.UpdateTrackerOverlay(EncodeRelics(), EncodeItems());
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
				if (sotnApi.GameApi.StageId == (uint) SotnApi.Constants.Values.Game.Enums.Stage.Prologue)
				{
					replayX += 2;
					replayY += 32;
				}
				else if (sotnApi.GameApi.StageId == (uint) SotnApi.Constants.Values.Game.Enums.Stage.Nightmare)
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
			if (replaySaved || replay.Count < 30)
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
			replaySaved = true;
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
				if (toolConfig.Tracker.EnableAutosplitter)
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
				Entity boss = sotnApi.EntityApi.GetLiveEntity(DraculaEntityAddress);
				if (boss.Hp > 13 && boss.Hp < 10000 && boss.UpdateFunctionAddress != 0)
				{
					draculaSpawned = true;
				}
				else if (draculaSpawned && boss.Hp < 1 && boss.UpdateFunctionAddress != 0)
				{
					if (toolConfig.Tracker.EnableAutosplitter)
					{
						autosplitter.Split();
					}
					stopWatch.Stop();
					finished = true;
					finalTime = stopWatch.Elapsed;
					SaveReplay();
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

using System;
using BizHawk.Client.Common;
using SotnApi;
using SotnApi.Constants.Values.Game;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools.Khaos
{
	internal sealed class CheatsController : ICheatsController
	{
		private ICheatCollectionAdapter cheats;

		Cheat batCardXp;
		Cheat ghostCardXp;
		Cheat faerieCardXp;
		Cheat demonCardXp;
		Cheat swordCardXp;
		Cheat spriteCardXp;
		Cheat noseDevilCardXp;

		public CheatsController(ICheatCollectionAdapter cheats)
		{
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			this.cheats = cheats;
			GetCheats();
		}

		public ICheatCollectionAdapter Cheats
		{
			get { return cheats; }
		}

		public Cheat FaerieScroll { get; set; }
		public Cheat DarkMetamorphasis { get; set; }
		public Cheat UnderwaterPhysics { get; set; }
		public Cheat Hearts { get; set; }
		public Cheat Curse { get; set; }
		public Cheat Mana { get; set; }
		public Cheat AttackPotion { get; set; }
		public Cheat StrengthPotion { get; set; }
		public Cheat DefencePotion { get; set; }
		public Cheat SubweaponTimer { get; set; }
		public Cheat HitboxWidth { get; set; }
		public Cheat HitboxHeight { get; set; }
		public Cheat Hitbox2Width { get; set; }
		public Cheat Hitbox2Height { get; set; }
		public Cheat InvincibilityCheat { get; set; }
		public Cheat ShineCheat { get; set; }
		public Cheat VisualEffectPalette { get; set; }
		public Cheat VisualEffectTimer { get; set; }
		public Cheat SavePalette { get; set; }
		public Cheat ContactDamage { get; set; }
		public Cheat Music { get; set; }
		public Cheat Activator { get; set; }

		public void GetCheats()
		{
			FaerieScroll = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.FaerieScroll, 0x3, "FaerieScroll", WatchSize.Byte);
			DarkMetamorphasis = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.DarkMetamorphasis, 0x1, "DarkMetamorphasis", WatchSize.Byte);
			UnderwaterPhysics = Cheats.AddCheat(SotnApi.Constants.Addresses.Game.UnderwaterPhysics, SotnApi.Constants.Values.Game.Various.UnderwaterPhysicsOn, "UnderwaterPhysics", WatchSize.Word);
			Hearts = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Stats.CurrentHearts, 0x6E, "Hearts", WatchSize.Byte);
			Curse = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.Curse, 0x1, "CurseTimer", WatchSize.Byte);
			Mana = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Stats.CurrentMana, 0x0F, "Mana", WatchSize.Byte);
			AttackPotion = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.AttackPotion, 0x1, "AttackPotion", WatchSize.Byte);
			StrengthPotion = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.StrengthPotion, 0x1, "StrengthPotion", WatchSize.Byte);
			DefencePotion = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.DefencePotion, 0x1, "DefencePotion", WatchSize.Byte);
			SubweaponTimer = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.SubweaponTimer, 0x1, "SubweaponTimer", WatchSize.Byte);
			HitboxWidth = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Entity.AttackHitboxWidth_1, 0x40, "AlucardAttackHitboxWidth", WatchSize.Byte);
			HitboxHeight = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Entity.AttackHitboxHeight_1, 0x40, "AlucardAttackHitboxHeight", WatchSize.Byte);
			Hitbox2Width = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Entity.AttackHitboxWidth_2, 0x40, "AlucardAttackHitbox2Width", WatchSize.Byte);
			Hitbox2Height = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Entity.AttackHitboxHeight_2, 0x40, "AlucardAttackHitbox2Height", WatchSize.Byte);
			InvincibilityCheat = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.Invincibility, 0x0, "Invincibility", WatchSize.Word);
			ShineCheat = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.Shine, 0x01, "Shine", WatchSize.Byte);
			VisualEffectPalette = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Effects.VisualEffectPalette, Constants.Khaos.OverdriveColorPalette, "VisualEffectPalette", WatchSize.Word);
			VisualEffectTimer = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Timers.VisualEffectTimer, 0x01, "VisualEffectTimer", WatchSize.Byte);
			SavePalette = Cheats.AddCheat(SotnApi.Constants.Addresses.Game.SavePalette, Constants.Khaos.SaveIcosahedronFirstCastle, "SavePalette", WatchSize.Word);
			ContactDamage = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Stats.ContactDamage, 0x0001, "ContactDamage", WatchSize.Word);
			Music = Cheats.AddCheat(SotnApi.Constants.Addresses.Game.Music, 0x00, "Music", WatchSize.Byte);
			Activator = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Effects.Activator, 0x00, "Activator", WatchSize.Byte);

			batCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.BatCardXp, 0x00002710, "BatCardXp", WatchSize.DWord);
			ghostCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.GhostCardXp, 0x00002710, "GhostCardXp", WatchSize.DWord);
			faerieCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.FaerieCardXp, 0x00001710, "FaerieCardXp", WatchSize.DWord);
			demonCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.DemonCardXp, 0x00001710, "DemonCardXp", WatchSize.DWord);
			swordCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.SwordCardXp, 0x00002710, "SwordCardXp", WatchSize.DWord);
			spriteCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.SpriteCardXp, 0x00001710, "SpriteCardXp", WatchSize.DWord);
			noseDevilCardXp = Cheats.AddCheat(SotnApi.Constants.Addresses.Alucard.Relics.NoseDevilCardXp, 0x00001710, "NoseDevilCardXp", WatchSize.DWord);

			Cheats.DisableAll();
		}

		public void StartCheats()
		{
			FaerieScroll.Enable();
			batCardXp.PokeValue(7000);
			batCardXp.Enable();
			ghostCardXp.PokeValue(7000);
			ghostCardXp.Enable();
			faerieCardXp.Enable();
			demonCardXp.Enable();
			swordCardXp.PokeValue(7000);
			swordCardXp.Enable();
			spriteCardXp.Enable();
			noseDevilCardXp.Enable();
			SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			SavePalette.Enable();
		}
	}
}

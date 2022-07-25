using System;
using BizHawk.Client.Common;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools.Khaos
{
	public class CheatsController : ICheatsController
	{
		private ICheatCollectionAdapter cheats;

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
		public Cheat DarkMetamorphasisCheat { get; set; }
		public Cheat UnderwaterPhysics { get; set; }
		public Cheat Hearts { get; set; }
		public Cheat Curse { get; set; }
		public Cheat ManaCheat { get; set; }
		public Cheat AttackPotionCheat { get; set; }
		public Cheat DefencePotionCheat { get; set; }
		public Cheat StopwatchTimer { get; set; }
		public Cheat HitboxWidth { get; set; }
		public Cheat HitboxHeight { get; set; }
		public Cheat Hitbox2Width { get; set; }
		public Cheat Hitbox2Height { get; set; }
		public Cheat InvincibilityCheat { get; set; }
		public Cheat ShineCheat { get; set; }
		public Cheat VisualEffectPaletteCheat { get; set; }
		public Cheat VisualEffectTimerCheat { get; set; }
		public Cheat SavePalette { get; set; }
		public Cheat ContactDamage { get; set; }
		public Cheat Music { get; set; }

		public void GetCheats()
		{
			FaerieScroll = cheats.GetCheatByName("FaerieScroll");
			DarkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			UnderwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			Hearts = cheats.GetCheatByName("Hearts");
			Curse = cheats.GetCheatByName("CurseTimer");
			ManaCheat = cheats.GetCheatByName("Mana");
			AttackPotionCheat = cheats.GetCheatByName("AttackPotion");
			DefencePotionCheat = cheats.GetCheatByName("DefencePotion");
			StopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			HitboxWidth = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			HitboxHeight = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			Hitbox2Width = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			Hitbox2Height = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			InvincibilityCheat = cheats.GetCheatByName("Invincibility");
			ShineCheat = cheats.GetCheatByName("Shine");
			VisualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			VisualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			SavePalette = cheats.GetCheatByName("SavePalette");
			ContactDamage = cheats.GetCheatByName("ContactDamage");
			Music = cheats.GetCheatByName("Music");
		}

		public void StartCheats()
		{
			FaerieScroll.Enable();
			Cheat batCardXp = cheats.GetCheatByName("BatCardXp");
			batCardXp.PokeValue(7000);
			batCardXp.Enable();
			Cheat ghostCardXp = cheats.GetCheatByName("GhostCardXp");
			ghostCardXp.PokeValue(7000);
			ghostCardXp.Enable();
			Cheat faerieCardXp = cheats.GetCheatByName("FaerieCardXp");
			faerieCardXp.Enable();
			Cheat demonCardXp = cheats.GetCheatByName("DemonCardXp");
			demonCardXp.Enable();
			Cheat swordCardXp = cheats.GetCheatByName("SwordCardXp");
			swordCardXp.PokeValue(7000);
			swordCardXp.Enable();
			Cheat spriteCardXp = cheats.GetCheatByName("SpriteCardXp");
			spriteCardXp.Enable();
			Cheat noseDevilCardXp = cheats.GetCheatByName("NoseDevilCardXp");
			noseDevilCardXp.Enable();
			SavePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			SavePalette.Enable();
		}
	}
}

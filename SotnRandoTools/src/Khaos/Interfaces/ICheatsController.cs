using BizHawk.Client.Common;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface ICheatsController
	{
		ICheatCollectionAdapter Cheats { get; }
		Cheat AttackPotionCheat { get; set; }
		Cheat ContactDamage { get; set; }
		Cheat Curse { get; set; }
		Cheat DarkMetamorphasisCheat { get; set; }
		Cheat DefencePotionCheat { get; set; }
		Cheat FaerieScroll { get; set; }
		Cheat Hearts { get; set; }
		Cheat Hitbox2Height { get; set; }
		Cheat Hitbox2Width { get; set; }
		Cheat HitboxHeight { get; set; }
		Cheat HitboxWidth { get; set; }
		Cheat InvincibilityCheat { get; set; }
		Cheat ManaCheat { get; set; }
		Cheat Music { get; set; }
		Cheat SavePalette { get; set; }
		Cheat ShineCheat { get; set; }
		Cheat StopwatchTimer { get; set; }
		Cheat UnderwaterPhysics { get; set; }
		Cheat VisualEffectPaletteCheat { get; set; }
		Cheat VisualEffectTimerCheat { get; set; }

		void GetCheats();
		void StartCheats();
	}
}
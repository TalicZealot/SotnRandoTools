using BizHawk.Client.Common;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface ICheatsController
	{
		ICheatCollectionAdapter Cheats { get; }
		Cheat AttackPotion { get; set; }
		Cheat ContactDamage { get; set; }
		Cheat Curse { get; set; }
		Cheat DarkMetamorphasis { get; set; }
		Cheat DefencePotion { get; set; }
		Cheat FaerieScroll { get; set; }
		Cheat Hearts { get; set; }
		Cheat Hitbox2Height { get; set; }
		Cheat Hitbox2Width { get; set; }
		Cheat HitboxHeight { get; set; }
		Cheat HitboxWidth { get; set; }
		Cheat InvincibilityCheat { get; set; }
		Cheat Mana { get; set; }
		Cheat Music { get; set; }
		Cheat SavePalette { get; set; }
		Cheat ShineCheat { get; set; }
		Cheat SubweaponTimer { get; set; }
		Cheat UnderwaterPhysics { get; set; }
		Cheat VisualEffectPalette { get; set; }
		Cheat VisualEffectTimer { get; set; }
		Cheat Activator { get; set; }
		void GetCheats();
		void StartCheats();
	}
}
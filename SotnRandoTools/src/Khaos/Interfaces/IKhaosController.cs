using System.Collections.Generic;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IKhaosController
	{
		//public void EnqueueAction(EventAddAction eventData);
		bool AutoKhaosOn { get; set; }
		bool SpeedLocked { get; set; }
		bool ManaLocked { get; set; }
		bool InvincibilityLocked { get; set; }
		bool SpawnActive { get; set; }
		bool ShaftHpSet { get; set; }
		bool GalamothStatsSet { get; set; }
		bool PandoraUsed { get; set; }
		int TotalMeterGained { get; set; }
		uint AlucardMapX { get; set; }
		uint AlucardMapY { get; set; }
		public bool IsInRoomList(List<MapLocation> rooms);
		public void GainKhaosMeter(short meter);
		void Bankrupt(string user = "Khaos");
		void BattleOrders(string user = "Khaos");
		void BloodMana(string user = "Khaos");
		void Endurance(string user = "Khaos");
		void FourBeasts(string user = "Khaos");
		void Gamble(string user = "Khaos");
		void Haste(string user = "Khaos");
		void HeavytHelp(string user = "Khaos");
		void HnK(string user = "Khaos");
		void Horde(string user = "Khaos");
		void KhaosEquipment(string user = "Khaos");
		void KhaosRelics(string user = "Khaos");
		void KhaosStats(string user = "Khaos");
		void KhaosStatus(string user = "Khaos");
		void KhaosTrack(string track, string user = "Khaos");
		void KhaoticBurst(string user = "Khaos");
		void LightHelp(string user = "Khaos");
		void Lord(string user = "Khaos");
		void Magician(string user = "Khaos");
		void MediumHelp(string user = "Khaos");
		void MeltyBlood(string user = "Khaos");
		void PandorasBox(string user = "Khaos");
		void RespawnBosses(string user = "Khaos");
		void Slow(string user = "Khaos");
		void StartKhaos();
		void StopKhaos();
		void SubweaponsOnly(string user = "Khaos");
		void Thirst(string user = "Khaos");
		void Update();
		void Vampire(string user = "Khaos");
		void Weaken(string user = "Khaos");
		void ZaWarudo(string user = "Khaos");
		void SetShaftHp();
		void SetGalamothtStats();
		void CheckMainMenu();
		void CheckCastleChanged();
	}
}

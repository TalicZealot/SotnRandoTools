using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IKhaosEventScheduler
	{
		BizhawkSafeTimer Action { get; set; }
		bool ActionTimer { set; }
		BizhawkSafeTimer AzureDragon { get; set; }
		bool AzureDragonTimer { set; }
		BizhawkSafeTimer BattleOrders { get; set; }
		bool BattleOrdersTimer { set; }
		BizhawkSafeTimer BloodMana { get; set; }
		BizhawkSafeTimer BloodManaDeath { get; set; }
		bool BloodManaDeathTimer { set; }
		bool BloodManaTimer { set; }
		bool DizzyTimer { set; }
		BizhawkSafeTimer Dizzy { get; set; }
		bool GuardianSpiritsTimer { set; }
		BizhawkSafeTimer GuardianSpirits { get; set; }
		BizhawkSafeTimer EnduranceSpawn { get; set; }
		bool EnduranceSpawnTimer { set; }
		BizhawkSafeTimer FastAction { get; set; }
		bool FastActionTimer { set; }
		BizhawkSafeTimer FourBeasts { get; set; }
		bool FourBeastsTimer { set; }
		BizhawkSafeTimer Haste { get; set; }
		BizhawkSafeTimer HasteOverdrive { get; set; }
		BizhawkSafeTimer HasteOverdriveOff { get; set; }
		bool HasteOverdriveOffTimer { set; }
		bool HasteOverdriveTimer { set; }
		bool HasteTimer { set; }
		BizhawkSafeTimer Hnk { get; set; }
		bool HnkTimer { set; }
		BizhawkSafeTimer Horde { get; set; }
		BizhawkSafeTimer HordeSpawn { get; set; }
		bool HordeSpawnTimer { set; }
		bool HordeTimer { set; }
		BizhawkSafeTimer KhaosTrack { get; set; }
		bool KhaosTrackTimer { set; }
		BizhawkSafeTimer Lord { get; set; }
		BizhawkSafeTimer LordSpawn { get; set; }
		bool LordSpawnTimer { set; }
		bool LordTimer { set; }
		BizhawkSafeTimer Magician { get; set; }
		bool MagicianTimer { set; }
		BizhawkSafeTimer Melty { get; set; }
		bool MeltyTimer { set; }
		BizhawkSafeTimer Slow { get; set; }
		bool SlowTimer { set; }
		BizhawkSafeTimer SubweaponsOnly { get; set; }
		bool SubweaponsOnlyTimer { set; }
		BizhawkSafeTimer Thirst { get; set; }
		BizhawkSafeTimer ThirstTick { get; set; }
		bool ThirstTickTimer { set; }
		bool ThirstTimer { set; }
		BizhawkSafeTimer Vampire { get; set; }
		bool VampireTimer { set; }
		BizhawkSafeTimer WhiteTigerBall { get; set; }
		bool WhiteTigerBallTimer { set; }
		BizhawkSafeTimer Zawarudo { get; set; }
		BizhawkSafeTimer ZawarudoCheck { get; set; }
		bool ZawarudoCheckTimer { set; }
		bool ZawarudoTimer { set; }

		void CheckSchedule();
		void StopTimers();
	}
}
using System;
using System.Collections.Generic;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos
{
	public class KhaosEventScheduler : IKhaosEventScheduler
	{
		private readonly IToolConfig toolConfig;
		private List<BizhawkSafeTimer> actions = new();

		public KhaosEventScheduler(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			InitializeTimers();
		}

		public bool FastActionTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					FastAction.Elapses = now.AddMilliseconds(FastAction.Interval);
					AddTimer(FastAction);
				}
				else
				{
					RemoveTimer(FastAction);
				}
			}
		}
		public bool ActionTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Action.Elapses = now.AddMilliseconds(Action.Interval);
					AddTimer(Action);
				}
				else
				{
					RemoveTimer(Action);
				}
			}
		}
		public bool BloodManaDeathTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					BloodManaDeath.Elapses = now.AddMilliseconds(BloodManaDeath.Interval);
					AddTimer(BloodManaDeath);
				}
				else
				{
					RemoveTimer(BloodManaDeath);
				}
			}
		}
		public bool KhaosTrackTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					KhaosTrack.Elapses = now.AddMilliseconds(KhaosTrack.Interval);
					AddTimer(KhaosTrack);
				}
				else
				{
					RemoveTimer(KhaosTrack);
				}
			}
		}
		public bool SubweaponsOnlyTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					SubweaponsOnly.Elapses = now.AddMilliseconds(SubweaponsOnly.Interval);
					AddTimer(SubweaponsOnly);
				}
				else
				{
					RemoveTimer(SubweaponsOnly);
				}
			}
		}
		public bool SlowTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Slow.Elapses = now.AddMilliseconds(Slow.Interval);
					AddTimer(Slow);
				}
				else
				{
					RemoveTimer(Slow);
				}
			}
		}
		public bool BloodManaTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					BloodMana.Elapses = now.AddMilliseconds(BloodMana.Interval);
					AddTimer(BloodMana);
				}
				else
				{
					RemoveTimer(BloodMana);
				}
			}
		}
		public bool ThirstTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Thirst.Elapses = now.AddMilliseconds(Thirst.Interval);
					AddTimer(Thirst);
				}
				else
				{
					RemoveTimer(Thirst);
				}
			}
		}
		public bool ThirstTickTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					ThirstTick.Elapses = now.AddMilliseconds(ThirstTick.Interval);
					AddTimer(ThirstTick);
				}
				else
				{
					RemoveTimer(ThirstTick);
				}
			}
		}
		public bool HordeTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Horde.Elapses = now.AddMilliseconds(Horde.Interval);
					AddTimer(Horde);
				}
				else
				{
					RemoveTimer(Horde);
				}
			}
		}
		public bool HordeSpawnTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					HordeSpawn.Elapses = now.AddMilliseconds(HordeSpawn.Interval);
					AddTimer(HordeSpawn);
				}
				else
				{
					RemoveTimer(HordeSpawn);
				}
			}
		}
		public bool EnduranceSpawnTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					EnduranceSpawn.Elapses = now.AddMilliseconds(EnduranceSpawn.Interval);
					AddTimer(EnduranceSpawn);
				}
				else
				{
					RemoveTimer(EnduranceSpawn);
				}
			}
		}
		public bool HnkTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Hnk.Elapses = now.AddMilliseconds(Hnk.Interval);
					AddTimer(Hnk);
				}
				else
				{
					RemoveTimer(Hnk);
				}
			}
		}
		public bool VampireTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Vampire.Elapses = now.AddMilliseconds(Vampire.Interval);
					AddTimer(Vampire);
				}
				else
				{
					RemoveTimer(Vampire);
				}
			}
		}
		public bool MagicianTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Magician.Elapses = now.AddMilliseconds(Magician.Interval);
					AddTimer(Magician);
				}
				else
				{
					RemoveTimer(Magician);
				}
			}
		}
		public bool BattleOrdersTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					BattleOrders.Elapses = now.AddMilliseconds(BattleOrders.Interval);
					AddTimer(BattleOrders);
				}
				else
				{
					RemoveTimer(BattleOrders);
				}
			}
		}
		public bool MeltyTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Melty.Elapses = now.AddMilliseconds(Melty.Interval);
					AddTimer(Melty);
				}
				else
				{
					RemoveTimer(Melty);
				}
			}
		}
		public bool FourBeastsTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					FourBeasts.Elapses = now.AddMilliseconds(FourBeasts.Interval);
					AddTimer(FourBeasts);
				}
				else
				{
					RemoveTimer(FourBeasts);
				}
			}
		}
		public bool AzureDragonTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					AzureDragon.Elapses = now.AddMilliseconds(AzureDragon.Interval);
					AddTimer(AzureDragon);
				}
				else
				{
					RemoveTimer(AzureDragon);
				}
			}
		}
		public bool WhiteTigerBallTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					WhiteTigerBall.Elapses = now.AddMilliseconds(WhiteTigerBall.Interval);
					AddTimer(WhiteTigerBall);
				}
				else
				{
					RemoveTimer(WhiteTigerBall);
				}
			}
		}
		public bool ZawarudoTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Zawarudo.Elapses = now.AddMilliseconds(Zawarudo.Interval);
					AddTimer(Zawarudo);
				}
				else
				{
					RemoveTimer(Zawarudo);
				}
			}
		}
		public bool ZawarudoCheckTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					ZawarudoCheck.Elapses = now.AddMilliseconds(ZawarudoCheck.Interval);
					AddTimer(ZawarudoCheck);
				}
				else
				{
					RemoveTimer(ZawarudoCheck);
				}
			}
		}
		public bool HasteTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Haste.Elapses = now.AddMilliseconds(Haste.Interval);
					AddTimer(Haste);
				}
				else
				{
					RemoveTimer(Haste);
				}
			}
		}
		public bool HasteOverdriveTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					HasteOverdrive.Elapses = now.AddMilliseconds(HasteOverdrive.Interval);
					AddTimer(HasteOverdrive);
				}
				else
				{
					RemoveTimer(HasteOverdrive);
				}
			}
		}
		public bool HasteOverdriveOffTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					HasteOverdriveOff.Elapses = now.AddMilliseconds(HasteOverdriveOff.Interval);
					AddTimer(HasteOverdriveOff);
				}
				else
				{
					RemoveTimer(HasteOverdriveOff);
				}
			}
		}
		public bool LordTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					Lord.Elapses = now.AddMilliseconds(Lord.Interval);
					AddTimer(Lord);
				}
				else
				{
					RemoveTimer(Lord);
				}
			}
		}
		public bool LordSpawnTimer
		{
			set
			{
				if (value)
				{
					DateTime now = DateTime.Now;

					LordSpawn.Elapses = now.AddMilliseconds(LordSpawn.Interval);
					AddTimer(LordSpawn);
				}
				else
				{
					RemoveTimer(LordSpawn);
				}
			}
		}

		public BizhawkSafeTimer FastAction { get; set; }
		public BizhawkSafeTimer Action { get; set; }
		public BizhawkSafeTimer BloodManaDeath { get; set; }
		public BizhawkSafeTimer KhaosTrack { get; set; }
		public BizhawkSafeTimer SubweaponsOnly { get; set; }
		public BizhawkSafeTimer Slow { get; set; }
		public BizhawkSafeTimer BloodMana { get; set; }
		public BizhawkSafeTimer Thirst { get; set; }
		public BizhawkSafeTimer ThirstTick { get; set; }
		public BizhawkSafeTimer Horde { get; set; }
		public BizhawkSafeTimer HordeSpawn { get; set; }
		public BizhawkSafeTimer EnduranceSpawn { get; set; }
		public BizhawkSafeTimer Hnk { get; set; }
		public BizhawkSafeTimer Vampire { get; set; }
		public BizhawkSafeTimer Magician { get; set; }
		public BizhawkSafeTimer BattleOrders { get; set; }
		public BizhawkSafeTimer Melty { get; set; }
		public BizhawkSafeTimer FourBeasts { get; set; }
		public BizhawkSafeTimer AzureDragon { get; set; }
		public BizhawkSafeTimer WhiteTigerBall { get; set; }
		public BizhawkSafeTimer Zawarudo { get; set; }
		public BizhawkSafeTimer ZawarudoCheck { get; set; }
		public BizhawkSafeTimer Haste { get; set; }
		public BizhawkSafeTimer HasteOverdrive { get; set; }
		public BizhawkSafeTimer HasteOverdriveOff { get; set; }
		public BizhawkSafeTimer Lord { get; set; }
		public BizhawkSafeTimer LordSpawn { get; set; }

		public void CheckSchedule()
		{
			if (actions.Count == 0)
			{
				return;
			}

			DateTime now = DateTime.Now;

			while (actions.Count > 0 && now >= actions[0].Elapses)
			{
				actions[0].Elapses = now.AddMilliseconds(actions[0].Interval);
				actions.Add(actions[0]);
				actions[0].Invoker.Invoke();
				actions.RemoveAt(0);
				if (actions.Count > 1)
				{
					actions.Sort((x, y) => x.Elapses.CompareTo(y.Elapses));
				}
			}

		}

		public void StopTimers()
		{
			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Invoker.Invoke();
			}
			actions.Clear();
		}

		private void InitializeTimers()
		{
			FastAction = new BizhawkSafeTimer { Interval = 2 * (1 * 1000) };
			Action = new BizhawkSafeTimer { Interval = 2 * (1 * 1000) };
			BloodManaDeath = new BizhawkSafeTimer { Interval = 1 * (1 * 1500) };
			KhaosTrack = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack].Duration.TotalMilliseconds };
			SubweaponsOnly = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly].Duration.TotalMilliseconds };
			Slow = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Slow].Duration.TotalMilliseconds };
			BloodMana = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana].Duration.TotalMilliseconds };
			Thirst = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Thirst].Duration.TotalMilliseconds };
			ThirstTick = new BizhawkSafeTimer { Interval = 1000 };
			Horde = new BizhawkSafeTimer { Interval = 5 * (60 * 1000) };
			HordeSpawn = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde].Interval.TotalMilliseconds };
			EnduranceSpawn = new BizhawkSafeTimer { Interval = 2 * (1000) };
			Hnk = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.HnK].Duration.TotalMilliseconds };
			Vampire = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Vampire].Duration.TotalMilliseconds };
			Magician = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Magician].Duration.TotalMilliseconds };
			BattleOrders = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders].Duration.TotalMilliseconds };
			Melty = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood].Duration.TotalMilliseconds };
			FourBeasts = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts].Duration.TotalMilliseconds };
			AzureDragon = new BizhawkSafeTimer { Interval = 10 * 1000 };
			WhiteTigerBall = new BizhawkSafeTimer { Interval = 2 * 1000 };
			Zawarudo = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO].Duration.TotalMilliseconds };
			ZawarudoCheck = new BizhawkSafeTimer { Interval = 2 * 1000 };
			Haste = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Haste].Duration.TotalMilliseconds };
			HasteOverdrive = new BizhawkSafeTimer { Interval = (1500) };
			HasteOverdriveOff = new BizhawkSafeTimer { Interval = (2 * 1000) };
			Lord = new BizhawkSafeTimer { Interval = 5 * (60 * 1000) };
			LordSpawn = new BizhawkSafeTimer { Interval = (int) toolConfig.Khaos.Actions[(int) Enums.Action.Lord].Interval.TotalMilliseconds };
		}

		private void AddTimer(BizhawkSafeTimer action)
		{
			if (actions.Contains(action))
			{
				return;
			}

			actions.Add(action);
			if (actions.Count > 1)
			{
				actions.Sort((x, y) => x.Elapses.CompareTo(y.Elapses));
			}
		}

		private void RemoveTimer(BizhawkSafeTimer action)
		{
			if (actions.Contains(action))
			{
				actions.Remove(action);
				if (actions.Count > 1)
				{
					actions.Sort((x, y) => x.Elapses.CompareTo(y.Elapses));
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos
{
	internal sealed class KhaosEventScheduler : IKhaosEventScheduler
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
				SetTimer(value, FastAction);
			}
		}
		public bool ActionTimer
		{
			set
			{
				SetTimer(value, Action);
			}
		}
		public bool DizzyTimer
		{
			set
			{
				SetTimer(value, Dizzy);
			}
		}
		public bool BloodManaDeathTimer
		{
			set
			{
				SetTimer(value, BloodManaDeath);
			}
		}
		public bool KhaosTrackTimer
		{
			set
			{
				SetTimer(value, KhaosTrack);
			}
		}
		public bool SubweaponsOnlyTimer
		{
			set
			{
				SetTimer(value, SubweaponsOnly);
			}
		}
		public bool SlowTimer
		{
			set
			{
				SetTimer(value, Slow);
			}
		}
		public bool BloodManaTimer
		{
			set
			{
				SetTimer(value, BloodMana);
			}
		}
		public bool ThirstTimer
		{
			set
			{
				SetTimer(value, Thirst);
			}
		}
		public bool ThirstTickTimer
		{
			set
			{
				SetTimer(value, ThirstTick);
			}
		}
		public bool HordeTimer
		{
			set
			{
				SetTimer(value, Horde);
			}
		}
		public bool HordeSpawnTimer
		{
			set
			{
				SetTimer(value, HordeSpawn);
			}
		}
		public bool EnduranceSpawnTimer
		{
			set
			{
				SetTimer(value, EnduranceSpawn);
			}
		}
		public bool HnkTimer
		{
			set
			{
				SetTimer(value, Hnk);
			}
		}
		public bool VampireTimer
		{
			set
			{
				SetTimer(value, Vampire);
			}
		}
		public bool MagicianTimer
		{
			set
			{
				SetTimer(value, Magician);
			}
		}
		public bool BattleOrdersTimer
		{
			set
			{
				SetTimer(value, BattleOrders);
			}
		}
		public bool MeltyTimer
		{
			set
			{
				SetTimer(value, Melty);
			}
		}
		public bool FourBeastsTimer
		{
			set
			{
				SetTimer(value, FourBeasts);
			}
		}
		public bool AzureDragonTimer
		{
			set
			{
				SetTimer(value, AzureDragon);
			}
		}
		public bool WhiteTigerBallTimer
		{
			set
			{
				SetTimer(value, WhiteTigerBall);
			}
		}
		public bool ZawarudoTimer
		{
			set
			{
				SetTimer(value, Zawarudo);
			}
		}
		public bool ZawarudoCheckTimer
		{
			set
			{
				SetTimer(value, ZawarudoCheck);
			}
		}
		public bool HasteTimer
		{
			set
			{
				SetTimer(value, Haste);
			}
		}
		public bool HasteOverdriveTimer
		{
			set
			{
				SetTimer(value, HasteOverdrive);
			}
		}
		public bool HasteOverdriveOffTimer
		{
			set
			{
				SetTimer(value, HasteOverdriveOff);
			}
		}
		public bool LordTimer
		{
			set
			{
				SetTimer(value, Lord);
			}
		}
		public bool LordSpawnTimer
		{
			set
			{
				SetTimer(value, LordSpawn);
			}
		}
		public bool GuardianSpiritsTimer
		{
			set
			{
				SetTimer(value, GuardianSpirits);
			}
		}

		public BizhawkSafeTimer FastAction { get; set; }
		public BizhawkSafeTimer Action { get; set; }
		public BizhawkSafeTimer Dizzy { get; set; }
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
		public BizhawkSafeTimer GuardianSpirits { get; set; }

		public void CheckSchedule()
		{
			if (actions.Count == 0)
			{
				return;
			}

			DateTime now = DateTime.Now;

			while (actions.Count > 0 && now >= actions[0].Elapses)
			{
				BizhawkSafeTimer refresh = new BizhawkSafeTimer { Invoker = actions[0].Invoker, Elapses = now.AddMilliseconds(actions[0].Interval), Interval = actions[0].Interval };
				actions.Add(refresh);
				actions.RemoveAt(0);
				refresh.Invoker.Invoke();
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
			Dizzy = new BizhawkSafeTimer { Interval = 20 * (1 * 1000) };
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
			GuardianSpirits = new BizhawkSafeTimer { Interval = 20 * (1 * 1000) };
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
			actions.RemoveAll(a => a.Invoker == action.Invoker);
			if (actions.Count > 1)
			{
				actions.Sort((x, y) => x.Elapses.CompareTo(y.Elapses));
			}
		}

		private void SetTimer(bool value, BizhawkSafeTimer timer)
		{
			if (value)
			{
				DateTime now = DateTime.Now;

				timer.Elapses = now.AddMilliseconds(timer.Interval);
				AddTimer(timer);
			}
			else
			{
				RemoveTimer(timer);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;

namespace SotnRandoTools.Khaos
{
	internal sealed class ActionDispatcher : IActionDispatcher
	{
		private readonly IToolConfig toolConfig;
		private readonly IKhaosController khaosController;
		private readonly INotificationService notificationService;
		private readonly ISotnApi sotnApi;
		private readonly Random rng = new();

		private System.Windows.Forms.Timer actionTimer = new();
		private System.Windows.Forms.Timer fastActionTimer = new();

		private int slowInterval;
		private int normalInterval;
		private int fastInterval;
		private int autoKhaosDifficulty;

		private List<QueuedAction> queuedActions = new();
		private Queue<MethodInvoker> queuedFastActions = new();

		public ActionDispatcher(IToolConfig toolConfig, IKhaosController khaosController, INotificationService notificationService, ISotnApi sotnApi, IKhaosActionsInfoDisplay statusInfoDisplay)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (khaosController is null) throw new ArgumentNullException(nameof(khaosController));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			this.toolConfig = toolConfig;
			this.khaosController = khaosController;
			this.notificationService = notificationService;
			this.sotnApi = sotnApi;

			switch (toolConfig.Khaos.AutoKhaosDifficulty)
			{
				case "Easy":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyEasy;
					break;
				case "Normal":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyNormal;
					break;
				case "Hard":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyHard;
					break;
				default:
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyNormal;
					break;
			}

			normalInterval = (int) toolConfig.Khaos.QueueInterval.TotalMilliseconds;
			slowInterval = (int) normalInterval * 2;
			fastInterval = (int) normalInterval / 2;

			statusInfoDisplay.ActionQueue = queuedActions;

			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 2 * (1 * 1000);

			switch (toolConfig.Khaos.AutoKhaosDifficulty)
			{
				case "Easy":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyEasy;
					break;
				case "Normal":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyNormal;
					break;
				case "Hard":
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyHard;
					break;
				default:
					autoKhaosDifficulty = Constants.Khaos.AutoKhaosDifficultyNormal;
					break;
			}
		}

		public bool AutoKhaosOn { get; set; }

		public void StartActions()
		{
			actionTimer.Start();
			fastActionTimer.Start();
		}

		public void StopActions()
		{
			actionTimer.Stop();
			fastActionTimer.Stop();
		}

		public void EnqueueAction(EventAddAction eventData)
		{
			if (eventData.ActionIndex < 0 || eventData.ActionIndex > toolConfig.Khaos.Actions.Count) throw new ArgumentOutOfRangeException(nameof(eventData.ActionIndex));
			if (eventData.UserName is null) throw new ArgumentNullException(nameof(eventData.UserName));
			if (eventData.UserName == "") throw new ArgumentException($"Parameter {nameof(eventData.UserName)} is empty!");
			string user = eventData.UserName;
			int action = eventData.ActionIndex;

			SotnRandoTools.Configuration.Models.Action? commandAction;
			switch (action)
			{
				#region Khaotic commands
				case (int) Enums.Action.KhaosStatus:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStatus];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.KhaosStatus(user)));
					}
					break;
				case (int) Enums.Action.KhaosEquipment:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosEquipment];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.KhaosEquipment(user)) });
					}
					break;
				case (int) Enums.Action.KhaosStats:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosStats];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.KhaosStats(user)) });
					}
					break;
				case (int) Enums.Action.KhaosRelics:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosRelics];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.KhaosRelics(user)) });
					}
					break;
				case (int) Enums.Action.PandorasBox:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.PandorasBox];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.PandorasBox(user)) });
					}
					break;
				case (int) Enums.Action.Gamble:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Gamble];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.Gamble(user)));
					}
					break;
				case (int) Enums.Action.Banish:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Banish];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.Banish(user)));
					}
					break;
				case (int) Enums.Action.KhaoticBurst:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaoticBurst];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.KhaoticBurst(user)));
					}
					break;
				case (int) Enums.Action.KhaosTrack:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosTrack];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.KhaosTrack(eventData.Data, user)));
					}
					break;
				#endregion
				#region Debuffs
				case (int) Enums.Action.Bankrupt:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Bankrupt];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.Bankrupt(user)) });
					}
					break;
				case (int) Enums.Action.Weaken:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Weaken];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.Weaken(user)) });
					}
					break;
				case (int) Enums.Action.RespawnBosses:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.RespawnBosses];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.RespawnBosses(user)));
					}
					break;
				case (int) Enums.Action.SubweaponsOnly:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.SubweaponsOnly];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => khaosController.SubweaponsOnly(user)) });
					}
					break;
				case (int) Enums.Action.Slow:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Slow];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpeed = true, Invoker = new MethodInvoker(() => khaosController.Slow(user)) });
					}
					break;
				case (int) Enums.Action.BloodMana:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.BloodMana];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => khaosController.BloodMana(user)) });
					}
					break;
				case (int) Enums.Action.Thirst:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Thirst];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.Thirst(user)) });
					}
					break;
				case (int) Enums.Action.KhaosHorde:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.KhaosHorde];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpawning = true, Invoker = new MethodInvoker(() => khaosController.Horde(user)) });
					}
					break;
				case (int) Enums.Action.Endurance:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Endurance];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.Endurance(user)));
					}
					break;
				case (int) Enums.Action.HnK:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.HnK];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.HnK(user)));
					}
					break;
				#endregion
				#region Buffs
				case (int) Enums.Action.Quad:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Quad];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.Quad(user)));
					}
					break;
				case (int) Enums.Action.LightHelp:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.LightHelp];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.LightHelp(user)));
					}
					break;
				case (int) Enums.Action.MediumHelp:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.MediumHelp];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.MediumHelp(user)));
					}
					break;
				case (int) Enums.Action.HeavyHelp:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.HeavyHelp];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.HeavytHelp(user)));
					}
					break;
				case (int) Enums.Action.BattleOrders:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.BattleOrders];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.BattleOrders(user)));
					}
					break;
				case (int) Enums.Action.Magician:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Magician];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksMana = true, Invoker = new MethodInvoker(() => khaosController.Magician(user)) });
					}
					break;
				case (int) Enums.Action.MeltyBlood:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.MeltyBlood];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, Invoker = new MethodInvoker(() => khaosController.MeltyBlood(user)) });
					}
					break;
				case (int) Enums.Action.FourBeasts:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.FourBeasts];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksInvincibility = true, Invoker = new MethodInvoker(() => khaosController.FourBeasts(user)) });
					}
					break;
				case (int) Enums.Action.ZAWARUDO:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.ZAWARUDO];
					if (commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => khaosController.ZaWarudo(user)));
					}
					break;
				case (int) Enums.Action.Haste:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Haste];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpeed = true, Invoker = new MethodInvoker(() => khaosController.Haste(user)) });
					}
					break;
				case (int) Enums.Action.Lord:
					commandAction = toolConfig.Khaos.Actions[(int) Enums.Action.Lord];
					if (commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = commandAction.Name, LocksSpawning = true, Invoker = new MethodInvoker(() => khaosController.Lord(user)) });
					}
					break;
				default:
					commandAction = null;
					break;
					#endregion
			}
			if (commandAction is not null)
			{
				khaosController.GainKhaosMeter(commandAction.Meter);
				commandAction.LastUsedAt = DateTime.Now;
			}

			notificationService.UpdateOverlayQueue(queuedActions);
		}

		private bool ActionViable()
		{
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() 
				&& sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& !khaosController.IsInRoomList(Constants.Khaos.RichterRooms)
				&& !khaosController.IsInRoomList(Constants.Khaos.LoadingRooms)
				&& !khaosController.IsInRoomList(Constants.Khaos.LibraryRoom))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool FastActionViable()
		{
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() 
				&& sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading
				&& sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox()
				&& !khaosController.IsInRoomList(Constants.Khaos.LibraryRoom)
				&& !sotnApi.AlucardApi.IsInvincible() && khaosController.AlucardMapX < 99)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (queuedActions.Count > 0)
			{
				khaosController.AlucardMapX = sotnApi.AlucardApi.MapX;
				khaosController.AlucardMapY = sotnApi.AlucardApi.MapY;

				if (ActionViable())
				{
					int index = 0;
					bool actionUnlocked = true;

					for (int i = 0; i < queuedActions.Count; i++)
					{
						index = i;
						actionUnlocked = true;
						if ((queuedActions[i].LocksSpeed && khaosController.SpeedLocked) ||
							(queuedActions[i].LocksMana && khaosController.ManaLocked) ||
							(queuedActions[i].LocksInvincibility && khaosController.InvincibilityLocked) ||
							(queuedActions[i].LocksSpawning && khaosController.SpawnActive))
						{
							actionUnlocked = false;
							continue;
						}
						break;
					}

					if (actionUnlocked)
					{
						queuedActions[index].Invoker();
						queuedActions.RemoveAt(index);
						notificationService.UpdateOverlayQueue(queuedActions);
						SetDynamicInterval();
					}
					else
					{
						Console.WriteLine($"All actions locked. speed: {khaosController.SpeedLocked}, invincibility: {khaosController.InvincibilityLocked}, mana: {khaosController.ManaLocked}");
					}
				}
			}
			else
			{
				actionTimer.Interval = 2000;
			}
		}

		private void ExecuteFastAction(Object sender, EventArgs e)
		{
			khaosController.AlucardMapX = sotnApi.AlucardApi.MapX;
			khaosController.AlucardMapY = sotnApi.AlucardApi.MapY;
			khaosController.CheckCastleChanged();
			khaosController.CheckMainMenu();

			bool keepRichterRoom = khaosController.IsInRoomList(Constants.Khaos.RichterRooms);
			bool galamothRoom = khaosController.IsInRoomList(Constants.Khaos.GalamothRooms);
			if (ActionViable() && sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading && !sotnApi.AlucardApi.IsInvincible() && khaosController.AlucardMapX < 99)
			{
				khaosController.ShaftHpSet = false;

				if (queuedFastActions.Count > 0)
				{
					queuedFastActions.Dequeue()();
				}
			}
			if (FastActionViable() && keepRichterRoom && !khaosController.ShaftHpSet)
			{
				khaosController.SetShaftHp();
			}
			if (FastActionViable() && galamothRoom && !khaosController.GalamothStatsSet)
			{
				khaosController.SetGalamothtStats();
			}
			if (!galamothRoom)
			{
				khaosController.GalamothStatsSet = false;
			}

			if (!khaosController.PandoraUsed && khaosController.TotalMeterGained >= toolConfig.Khaos.PandoraTrigger)
			{
				EnqueueAction(new EventAddAction { UserName = Constants.Khaos.KhaosName, ActionIndex = 4 });
				khaosController.PandoraUsed = true;
			}

			if (AutoKhaosOn)
			{
				AutoKhaosAction();
			}
		}

		private void SetDynamicInterval()
		{
			if (toolConfig.Khaos.DynamicInterval && queuedActions.Count < Constants.Khaos.SlowQueueIntervalEnd)
			{
				actionTimer.Interval = slowInterval;
			}
			else if (toolConfig.Khaos.DynamicInterval && queuedActions.Count >= Constants.Khaos.FastQueueIntervalStart)
			{
				actionTimer.Interval = fastInterval;
			}
			else
			{
				actionTimer.Interval = normalInterval;
			}
		}

		private void AutoKhaosAction()
		{
			int roll = rng.Next(0, 101);
			if (roll > autoKhaosDifficulty)
			{
				int index = rng.Next(0, toolConfig.Khaos.Actions.Count);
				EventAddAction? actionEvent = new() { UserName = "Auto Khaos", ActionIndex = index, Data = "random" };

				if (toolConfig.Khaos.Actions[index].Name != "Guilty Gear" && toolConfig.Khaos.Actions[index].Name != "Pandora's Box" && !toolConfig.Khaos.Actions[index].IsOnCooldown())
				{
					EnqueueAction(actionEvent);
				}
			}
		}
	}
}

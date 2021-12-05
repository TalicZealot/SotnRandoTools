using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools
{
	public partial class KhaosForm : Form, IKhaosActionsInfoDisplay, INotifyPropertyChanged, IVladRelicLocationDisplay
	{
		private readonly ICheatCollectionAdapter adaptedCheats;
		private KhaosController? khaosControler;
		private readonly IToolConfig toolConfig;
		private List<ActionTimer> actionTimers = new();
		private System.Timers.Timer countdownTimer;
		private string heartOfVladLocation;
		private string toothOfVladLocation;
		private string ribOfVladLocation;
		private string ringOfVladLocation;
		private string eyeOfVladLocation;
		private bool started = false;

		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, INotificationService notificationService, IInputService inputService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			this.toolConfig = toolConfig;

			adaptedCheats = new CheatCollectionAdapter(cheats);
			khaosControler = new KhaosController(toolConfig, gameApi, alucardApi, actorApi, adaptedCheats, notificationService, inputService, this);

			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			countdownTimer = new System.Timers.Timer();
			countdownTimer.Interval = 1000;
			countdownTimer.Elapsed += DecrementTimers;
			countdownTimer.Start();
			this.PropertyChanged += KhaosForm_PropertyChanged;

			heartLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "HeartOfVladLocation"));
			toothLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "ToothOfVladLocation"));
			ribLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "RibOfVladLocation"));
			ringLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "RingOfVladLocation"));
			eyeLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, "EyeOfVladLocation"));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public string HeartOfVladLocation
		{
			get
			{
				return heartOfVladLocation;
			}
			set
			{
				heartOfVladLocation = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("HeartOfVladLocation"));
				}
			}
		}
		public string ToothOfVladLocation
		{
			get
			{
				return toothOfVladLocation;
			}
			set
			{
				toothOfVladLocation = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("ToothOfVladLocation"));
				}
			}
		}
		public string RibOfVladLocation
		{
			get
			{
				return ribOfVladLocation;
			}
			set
			{
				ribOfVladLocation = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("RibOfVladLocation"));
				}
			}
		}
		public string RingOfVladLocation
		{
			get
			{
				return ringOfVladLocation;
			}
			set
			{
				ringOfVladLocation = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("RingOfVladLocation"));
				}
			}
		}
		public string EyeOfVladLocation
		{
			get
			{
				return eyeOfVladLocation;
			}
			set
			{
				eyeOfVladLocation = value;

				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("EyeOfVladLocation"));
				}
			}
		}
		public List<QueuedAction> ActionQueue { get; set; }

		public void AddTimer(ActionTimer timer)
		{
			actionTimers.Add(timer);
		}
		public void UpdateKhaosValues()
		{
			if (khaosControler is not null)
			{
				khaosControler.Update();
			}
		}

		private void KhaosForm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}
		private void DecrementTimers(object sender, ElapsedEventArgs e)
		{
			foreach (var timer in actionTimers)
			{
				if (timer.TotalDuration == 0)
				{
					timer.TotalDuration = (int) timer.Duration.TotalSeconds;
				}

				timer.Duration -= TimeSpan.FromSeconds(1);
				if (timer.Duration.TotalSeconds < 1)
				{
					actionTimers.Remove(timer);
				}
			}

			string timersLines = "";

			foreach (var timer in actionTimers)
			{
				timersLines += timer.Name.PadRight(16, ' ') + timer.Duration.Minutes + ":" + timer.Duration.Seconds + "\r\n";
			}

			timersTextBox.Text = timersLines;
			DrawQueue();

		}
		private void DrawQueue()
		{
			string queueLines = "";

			foreach (var action in ActionQueue)
			{
				queueLines += action.Name + "\r\n";
			}

			queueTextBox.Text = queueLines;
		}
		private void Khaos_Load(object sender, EventArgs e)
		{
			this.Location = toolConfig.Khaos.Location;
			queueRadio.Checked = toolConfig.Khaos.ControlPannelQueueActions;
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				queueRadio.Checked = true;
			}
			else
			{
				instantRadio.Checked = true;
			}
		}
		private void KhaosForm_Move(object sender, EventArgs e)
		{
			if (this.Location.X > 0)
			{
				toolConfig.Khaos.Location = this.Location;
			}
		}
		private void queueRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (queueRadio.Checked)
			{
				toolConfig.Khaos.ControlPannelQueueActions = true;
			}
			else
			{
				toolConfig.Khaos.ControlPannelQueueActions = false;
			}
		}
		private void startButton_Click(object sender, EventArgs e)
		{
			if (started)
			{
				started = false;
				khaosControler.StopKhaos();
				startButton.Text = "Start";
				startButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
			}
			else
			{
				started = true;
				khaosControler.StartKhaos();
				startButton.Text = "Stop";
				startButton.BackColor = System.Drawing.Color.FromArgb(169, 19, 7);
			}
		}
		private void autoKhaosButton_Click(object sender, EventArgs e)
		{
			if (khaosControler.AutoKhaosOn)
			{
				khaosControler.AutoKhaosOn = false;
				autoKhaosButton.Text = "Start Auto Khaos";
				autoKhaosButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
			}
			else
			{
				khaosControler.AutoKhaosOn = true;
				autoKhaosButton.Text = "Stop Auto Khaos";
				autoKhaosButton.BackColor = System.Drawing.Color.FromArgb(72, 81, 118);
			}
		}

		#region Khaotic effects
		private void randomStatusButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 0, UserName = "Khaos" });
			}
			else
			{
				khaosControler.KhaosStatus();
			}
		}
		private void randomEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 1, UserName = "Khaos" });
			}
			else
			{

				khaosControler.KhaosEquipment();
			}
		}
		private void randomizeStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 2, UserName = "Khaos" });
			}
			else
			{

				khaosControler.KhaosStats();
			}
		}
		private void randomizeRelicsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 3, UserName = "Khaos" });
			}
			else
			{

				khaosControler.KhaosRelics();
			}
		}
		private void pandorasBoxButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 4, UserName = "Khaos" });
			}
			else
			{

				khaosControler.PandorasBox();
			}
		}
		private void gambleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 5, UserName = "Khaos" });
			}
			else
			{

				khaosControler.Gamble();
			}
		}
		private void burstButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 6, UserName = "Khaos" });
			}
			else
			{

				khaosControler.KhaoticBurst();
			}
		}
		#endregion
		#region Debuffs
		private void bankruptButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 6, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Bankrupt();
			}
		}
		private void weakenButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 7, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Weaken();
			}
		}
		private void respawnBossesButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 8, UserName = "Khaos" });
			}
			else
			{
				khaosControler.RespawnBosses();
			}
		}
		private void subsonlyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 9, UserName = "Khaos" });
			}
			else
			{

				khaosControler.SubweaponsOnly();
			}
		}
		private void crippleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 10, UserName = "Khaos" });
			}
			else
			{

				khaosControler.Cripple();
			}
		}
		private void bloodManaButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 11, UserName = "Khaos" });
			}
			else
			{
				khaosControler.BloodMana();
			}
		}
		private void thurstButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 12, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Thirst();
			}
		}
		private void hordeButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 13, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Horde();
			}
		}
		private void enduranceButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 14, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Endurance();
			}
		}
		private void hnkButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 16, UserName = "Khaos" });
			}
			else
			{
				khaosControler.HnK();
			}
		}
		#endregion
		#region Buffs
		private void vampireButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 15, UserName = "Khaos" });
			}
			else
			{

				khaosControler.Vampire();
			}
		}
		private void lightHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 16, UserName = "Khaos" });
			}
			else
			{
				khaosControler.LightHelp();
			}
		}
		private void mediumHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 17, UserName = "Khaos" });
			}
			else
			{

				khaosControler.MediumHelp();
			}
		}
		private void heavyHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 18, UserName = "Khaos" });
			}
			else
			{

				khaosControler.HeavytHelp();
			}
		}
		private void battleOrdersButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 19, UserName = "Khaos" });
			}
			else
			{

				khaosControler.BattleOrders();
			}
		}
		private void magicianButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 20, UserName = "Khaos" });
			}
			else
			{

				khaosControler.Magician();
			}
		}
		private void meltyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 21, UserName = "Khaos" });
			}
			else
			{

				khaosControler.MeltyBlood();
			}
		}
		private void fourBeastsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 22, UserName = "Khaos" });
			}
			else
			{
				khaosControler.FourBeasts();
			}
		}
		private void zawarudoButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 23, UserName = "Khaos" });
			}
			else
			{
				khaosControler.ZaWarudo();
			}
		}
		private void hasteButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = 24, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Haste();
			}
		}
		#endregion

		private void KhaosForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (started)
			{
				started = false;
				khaosControler.StopKhaos();
				startButton.Text = "Start";
			}
			khaosControler = null;
		}
	}
}

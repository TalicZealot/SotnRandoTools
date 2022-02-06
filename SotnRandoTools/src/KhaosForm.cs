using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
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
		private ICheatCollectionAdapter adaptedCheats;
		private readonly IToolConfig toolConfig;
		private readonly KhaosController? khaosControler;
		private readonly TwitchListener? twitchListener;
		private readonly ChannelPointsController? channelPointsController;
		private List<ActionTimer> actionTimers = new();
		private System.Timers.Timer countdownTimer;

		private string heartOfVladLocation = String.Empty;
		private string toothOfVladLocation = String.Empty;
		private string ribOfVladLocation = String.Empty;
		private string ringOfVladLocation = String.Empty;
		private string eyeOfVladLocation = String.Empty;

		private string batLocation = String.Empty;
		private string mistLocation = String.Empty;
		private string jewelLocation = String.Empty;
		private string gravityBootsLocation = String.Empty;
		private string leapstoneLocation = String.Empty;
		private string mermanLocation = String.Empty;

		private bool started = false;
		private bool connected = false;

		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, ISotnApi sotnApi, INotificationService notificationService, IInputService inputService, IMemoryDomains memoryDomains)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			if (memoryDomains == null) throw new ArgumentNullException(nameof(memoryDomains));
			this.toolConfig = toolConfig;

			adaptedCheats = new CheatCollectionAdapter(cheats, memoryDomains);
			khaosControler = new KhaosController(toolConfig, sotnApi, adaptedCheats, notificationService, inputService, this);
			twitchListener = new TwitchListener(Paths.TwitchRedirectUri);
			channelPointsController = new ChannelPointsController(toolConfig, twitchListener, khaosControler, notificationService);

			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			countdownTimer = new System.Timers.Timer();
			countdownTimer.Interval = 1000;
			countdownTimer.Elapsed += DecrementTimers;
			countdownTimer.Start();
			this.PropertyChanged += KhaosForm_PropertyChanged;

			heartLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(HeartOfVladLocation)));
			toothLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(ToothOfVladLocation)));
			ribLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(RibOfVladLocation)));
			ringLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(RingOfVladLocation)));
			eyeLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(EyeOfVladLocation)));

			batLocationLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(BatLocation)));
			mistLocationLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(MistLocation)));
			jewelLocationLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(JewelOfOpenLocation)));
			gravLocationLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(GravityBootsLocation)));
			leapLocationLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(LepastoneLocation)));
			mermanLocationLabel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this, nameof(MermanLocation)));
		}

		public ICheatCollectionAdapter AdaptedCheats
		{
			get => adaptedCheats;

			set
			{
				adaptedCheats = value;
				khaosControler.GetCheats();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public string HeartOfVladLocation
		{
			get => heartOfVladLocation;
			set
			{
				heartOfVladLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeartOfVladLocation)));
			}
		}
		public string ToothOfVladLocation
		{
			get => toothOfVladLocation;
			set
			{
				toothOfVladLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToothOfVladLocation)));
			}
		}
		public string RibOfVladLocation
		{
			get => ribOfVladLocation;
			set
			{
				ribOfVladLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RibOfVladLocation)));
			}
		}
		public string RingOfVladLocation
		{
			get => ringOfVladLocation;
			set
			{
				ringOfVladLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RingOfVladLocation)));
			}
		}
		public string EyeOfVladLocation
		{
			get => eyeOfVladLocation;
			set
			{
				eyeOfVladLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EyeOfVladLocation)));
			}
		}
		public string BatLocation
		{
			get => batLocation;
			set
			{
				batLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatLocation)));
			}
		}
		public string MistLocation
		{
			get => mistLocation;
			set
			{
				mistLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MistLocation)));
			}
		}
		public string JewelOfOpenLocation
		{
			get => jewelLocation;
			set
			{
				jewelLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JewelOfOpenLocation)));
			}
		}
		public string GravityBootsLocation
		{
			get => gravityBootsLocation;
			set
			{
				gravityBootsLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GravityBootsLocation)));
			}
		}
		public string LepastoneLocation
		{
			get => leapstoneLocation;
			set
			{
				leapstoneLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LepastoneLocation)));
			}
		}
		public string MermanLocation
		{
			get => mermanLocation;
			set
			{
				mermanLocation = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MermanLocation)));
			}
		}
		public List<QueuedAction> ActionQueue { get; set; }

		public void AddTimer(ActionTimer timer)
		{
			actionTimers.Add(timer);
		}
		public bool ContainsTimer(string name)
		{
			return actionTimers.Where(timer => timer.Name == name).Any();
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
			foreach (ActionTimer? timer in actionTimers)
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

			foreach (ActionTimer? timer in actionTimers)
			{
				timersLines += timer.Name.PadRight(16, ' ') + timer.Duration.Minutes + ":" + timer.Duration.Seconds + "\r\n";
			}

			timersTextBox.Text = timersLines;
			DrawQueue();

		}
		private void DrawQueue()
		{
			string queueLines = "";

			foreach (QueuedAction? action in ActionQueue)
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
				startButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
				connectButton.Enabled = false;
				connectButton.Text = "Connect to Twitch";
				if (connected)
				{
					channelPointsController.Disconnect();
					connected = false;
				}
				autoKhaosButton.Enabled = false;
				khaosControler.AutoKhaosOn = false;
				autoKhaosButton.Text = "Start Auto Khaos";
				autoKhaosButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				autoKhaosButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
			}
			else
			{
				started = true;
				khaosControler.StartKhaos();
				startButton.Text = "Stop";
				startButton.BackColor = System.Drawing.Color.FromArgb(114, 32, 25);
				startButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(169, 19, 7);
				connectButton.Enabled = true;
				autoKhaosButton.Enabled = true;
			}
		}
		private void autoKhaosButton_Click(object sender, EventArgs e)
		{
			if (khaosControler.AutoKhaosOn)
			{
				khaosControler.AutoKhaosOn = false;
				autoKhaosButton.Text = "Start Auto Khaos";
				autoKhaosButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				autoKhaosButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
			}
			else
			{
				khaosControler.AutoKhaosOn = true;
				autoKhaosButton.Text = "Stop Auto Khaos";
				autoKhaosButton.BackColor = System.Drawing.Color.FromArgb(62, 68, 91);
				autoKhaosButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(72, 81, 118);
			}
		}
		private async void connectButton_Click(object sender, EventArgs e)
		{
			if (connected)
			{
				connectButton.Text = "Connect to Twitch";
				channelPointsController.Disconnect();
				connected = false;
				connectButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
			}
			else
			{
				bool result = await channelPointsController.Connect();
				if (result)
				{
					connectButton.Text = "Disonnect";
					connected = true;
					connectButton.BackColor = System.Drawing.Color.FromArgb(93, 56, 147);
					connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(145, 70, 255);
				}
			}
		}

		#region Khaotic effects
		private void randomStatusButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaosStatus, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaosEquipment, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaosStats, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaosRelics, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.PandorasBox, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Gamble, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaoticBurst, UserName = "Khaos" });
			}
			else
			{

				khaosControler.KhaoticBurst();
			}
		}
		private void khaosTrackButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaosTrack, UserName = "Khaos", Data = "random" });
			}
			else
			{

				khaosControler.KhaosTrack("random");
			}
		}
		#endregion
		#region Debuffs
		private void bankruptButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Bankrupt, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Weaken, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.RespawnBosses, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.SubweaponsOnly, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Slow, UserName = "Khaos" });
			}
			else
			{

				khaosControler.Slow();
			}
		}
		private void bloodManaButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.BloodMana, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Thirst, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.KhaosHorde, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Endurance, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.HnK, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Vampire, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.LightHelp, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.MediumHelp, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.HeavyHelp, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.BattleOrders, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Magician, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.MeltyBlood, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.FourBeasts, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.ZAWARUDO, UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Haste, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Haste();
			}
		}
		private void lordButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { ActionIndex = (int) Khaos.Enums.Action.Lord, UserName = "Khaos" });
			}
			else
			{
				khaosControler.Lord();
			}
		}
		#endregion

		private void KhaosForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (started)
			{
				started = false;
				khaosControler.StopKhaos();
			}
			startButton.Text = "Start";
			startButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
			connectButton.Enabled = false;
			connectButton.Text = "Connect to Twitch";
			if (connected)
			{
				channelPointsController.Disconnect();
				connected = false;
			};
			autoKhaosButton.Enabled = false;
			khaosControler.AutoKhaosOn = false;
			autoKhaosButton.Text = "Start Auto Khaos";
			autoKhaosButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
		}
	}
}

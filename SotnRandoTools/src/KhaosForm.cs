using System;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools
{
	public partial class KhaosForm : Form
	{
		private readonly ICheatCollectionAdapter adaptedCheats;
		private readonly KhaosController khaosControler;
		private readonly IToolConfig toolConfig;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IActorApi actorApi;
		private bool started = false;

		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (actorApi is null) throw new ArgumentNullException(nameof(actorApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			this.toolConfig = toolConfig;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.actorApi = actorApi;

			adaptedCheats = new CheatCollectionAdapter(cheats);
			khaosControler = new KhaosController(toolConfig, gameApi, alucardApi, actorApi, adaptedCheats, notificationService);

			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
		}

		public void UpdateKhaosValues()
		{
			khaosControler.Update();
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
			}
			else
			{
				started = true;
				khaosControler.StartKhaos();
				startButton.Text = "Stop";
			}
		}

		#region Khaotic effects
		private void randomStatusButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("kstatus Khaos");
			}
			else
			{
				khaosControler.InflictRandomStatus();
			}
		}
		private void randomEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("kequipment Khaos");
			}
			else
			{

				khaosControler.RandomizeEquipment();
			}
		}
		private void randomizeStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("kstats Khaos");
			}
			else
			{

				khaosControler.RandomizeStats();
			}
		}
		private void randomizeRelicsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("krelics Khaos");
			}
			else
			{

				khaosControler.RandomizeRelics();
			}
		}
		private void pandorasBoxButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("pandora Khaos");
			}
			else
			{

				khaosControler.PandorasBox();
			}
		}
		private void bankruptButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("bankrupt Khaos");
			}
			else
			{
				khaosControler.Bankrupt();
			}
		}
		private void gambleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("gamble Khaos");
			}
			else
			{

				khaosControler.Gamble();
			}
		}
		#endregion
		#region Debuffs
		private void weakenButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("weaken Khaos");
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
				khaosControler.EnqueueAction("respawnbosses Khaos");
			}
			else
			{
				khaosControler.RespawnBosses();
			}
		}
		private void honestButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("honest Khaos");
			}
			else
			{

				khaosControler.HonestGamer();
			}
		}
		private void subsonlyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("subsonly Khaos");
			}
			else
			{

				khaosControler.SubweaponsOnly();
			}
		}
		private void bloodManaButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("bloodmana Khaos");
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
				khaosControler.EnqueueAction("thirst Khaos");
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
				khaosControler.EnqueueAction("horde Khaos");
			}
			else
			{
				khaosControler.Horde();
			}
		}
		private void crippleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("cripple Khaos");
			}
			else
			{

				khaosControler.Cripple();
			}
		}
		#endregion
		#region Buffs
		private void lightHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("lighthelp Khaos");
			}
			else
			{
				khaosControler.RandomLightHelp();
			}
		}
		private void mediumHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("mediumhelp Khaos");
			}
			else
			{

				khaosControler.RandomMediumHelp();
			}
		}
		private void heavyHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("heavyhelp Khaos");
			}
			else
			{

				khaosControler.RandomHeavytHelp();
			}
		}
		private void battleOrdersButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("battleorders Khaos");
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
				khaosControler.EnqueueAction("magician Khaos");
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
				khaosControler.EnqueueAction("melty Khaos");
			}
			else
			{

				khaosControler.MeltyBlood();
			}
		}
		private void vampireButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("vampire Khaos");
			}
			else
			{

				khaosControler.Vampire();
			}
		}
		private void fourBeastsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("fourbeasts Khaos");
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
				khaosControler.EnqueueAction("zawarudo Khaos");
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
				khaosControler.EnqueueAction("haste Khaos");
			}
			else
			{
				khaosControler.Haste();
			}
		}
		#endregion
	}
}

using System;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools
{
	public partial class KhaosForm : Form
	{
		private readonly ICheatCollectionAdapter adaptedCheats;
		private KhaosController? khaosControler;
		private readonly IToolConfig toolConfig;
		private bool started = false;

		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, INotificationService notificationService, IInputService inputService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			this.toolConfig = toolConfig;

			adaptedCheats = new CheatCollectionAdapter(cheats);
			khaosControler = new KhaosController(toolConfig, gameApi, alucardApi, actorApi, adaptedCheats, notificationService, inputService);

			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
		}

		public void UpdateKhaosValues()
		{
			if (khaosControler is not null)
			{
				khaosControler.Update();
			}
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "kstatus", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "kequipment", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "kstats", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "krelics", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "pandora", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "bankrupt", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "gamble", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "weaken", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "respawnbosses", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "subsonly", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "bloodmana", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "thirst", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "horde", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "endurance", UserName = "Khaos" });
			}
			else
			{
				khaosControler.Endurance();
			}
		}
		private void crippleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "cripple", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "lighthelp", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "mediumhelp", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "heavyhelp", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "battleorders", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "magician", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "melty", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "vampire", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "fourbeasts", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "zawarudo", UserName = "Khaos" });
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
				khaosControler.EnqueueAction(new EventAddAction { Command = "haste", UserName = "Khaos" });
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

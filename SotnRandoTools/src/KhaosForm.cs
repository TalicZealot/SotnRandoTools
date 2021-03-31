using System;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos;
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

		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi)
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
			khaosControler = new KhaosController(toolConfig, gameApi, alucardApi, actorApi, adaptedCheats);

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

		private void randomStatusButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction("randomstatus Khaos");
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
				khaosControler.EnqueueAction("randomizeequipment Khaos");
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
				khaosControler.EnqueueAction("randomizestats Khaos");
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
				khaosControler.EnqueueAction("randomizerelics Khaos");
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
			khaosControler.StartKhaos();
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
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Emulation.Common;
using Newtonsoft.Json;
using SotnApi;
using SotnRandoTools.Configuration;
using SotnRandoTools.Constants;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	[ExternalTool("Symphony of the Night Randomizer Tools", Description = "A collection of tools to enhance the SotN randomizer experience.", LoadAssemblyFiles = new[] { "./SotnRandoTools/SotnApi.dll", "./SotnRandoTools/SimpleTCP.dll", "./SotnRandoTools/WatsonWebsocket.dll" })]
	[ExternalToolEmbeddedIcon("BizAlucard.ico")]
	//TODO: Revert when BIzhawk fixes Dump Status Report
	//[ExternalToolApplicability.SingleRom(CoreSystem.Playstation, "0DDCBC3D")]
	public partial class ToolMainForm : ToolFormBase, IExternalToolForm
	{
		[RequiredService]
		private IEmulator? _emu { get; set; }

		[RequiredService]
		private IMemoryDomains? _memoryDomains { get; set; }

		[RequiredApi]
		private ICommApi? _maybeCommAPI { get; set; }

		[RequiredApi]
		private IEmuClientApi? _maybeClientAPI { get; set; }

		[RequiredApi]
		private IJoypadApi? _maybeJoypadApi { get; set; }

		[RequiredApi]
		private IEmulationApi? _maybeEmuAPI { get; set; }

		[RequiredApi]
		private IGameInfoApi? _maybeGameInfoAPI { get; set; }

		[RequiredApi]
		private IGuiApi? _maybeGuiAPI { get; set; }

		[RequiredApi]
		private IMemoryApi? _maybeMemAPI { get; set; }

		private ApiContainer? _apis;

		private ApiContainer APIs => _apis ??= new ApiContainer(new Dictionary<Type, IExternalApi>
		{
			[typeof(ICommApi)] = _maybeCommAPI ?? throw new NullReferenceException(),
			[typeof(IEmuClientApi)] = _maybeClientAPI ?? throw new NullReferenceException(),
			[typeof(IJoypadApi)] = _maybeJoypadApi ?? throw new NullReferenceException(),
			[typeof(IEmulationApi)] = _maybeEmuAPI ?? throw new NullReferenceException(),
			[typeof(IGameInfoApi)] = _maybeGameInfoAPI ?? throw new NullReferenceException(),
			[typeof(IGuiApi)] = _maybeGuiAPI ?? throw new NullReferenceException(),
			[typeof(IMemoryApi)] = _maybeMemAPI ?? throw new NullReferenceException()
		});
		private Config GlobalConfig => (_maybeEmuAPI as EmulationApi ?? throw new Exception("required API wasn't fulfilled")).ForbiddenConfigReference;

		private ActorApi? actorApi;
		private AlucardApi? alucardApi;
		private GameApi? gameApi;
		private RenderingApi? renderingApi;
		private ToolConfig toolConfig;
		private WatchlistService? watchlistService;
		private NotificationService? notificationService;
		private InputService? inputService;
		private TrackerForm? trackerForm;
		private KhaosForm? khaosForm;
		private CoopForm? coopForm;
		private AutotrackerSettingsPanel? autotrackerSettingsPanel;
		private KhaosSettingsPanel? khaosSettingsPanel;
		private CoopSettingsPanel? coopSettingsPanel;
		private AboutPanel? aboutPanel;
		private string _windowTitle = "Symphony of the Night Randomizer Tools";
		private const int PanelOffset = 130;
		private int cooldown = 0;
		private StringWriter log = new();

		public ToolMainForm()
		{
			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			InitializeConfig();
		}

		private void InitializeConfig()
		{
			if (File.Exists(Paths.ConfigPath))
			{
				string configJson = File.ReadAllText(Paths.ConfigPath);
				toolConfig = JsonConvert.DeserializeObject<ToolConfig>(configJson,
					new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }) ?? new ToolConfig();
			}
			else
			{
				toolConfig = new ToolConfig();
			}
		}

		protected override string WindowTitle => _windowTitle;

		protected override string WindowTitleStatic => "Symphony of the Night Randomizer Tools";

		private void ToolMainForm_Load(object sender, EventArgs e)
		{
			this.Location = toolConfig.Location;
			notificationService = new NotificationService(toolConfig, _maybeGuiAPI, _maybeClientAPI);

			aboutPanel = new AboutPanel();
			aboutPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(aboutPanel);
			aboutPanel.UpdateButton_Click += AboutPanel_UpdateButton_Click;

			autotrackerSettingsPanel = new AutotrackerSettingsPanel(toolConfig);
			autotrackerSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(autotrackerSettingsPanel);

			khaosSettingsPanel = new KhaosSettingsPanel(toolConfig, notificationService);
			khaosSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(khaosSettingsPanel);

			coopSettingsPanel = new CoopSettingsPanel(toolConfig);
			coopSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(coopSettingsPanel);

			if (_memoryDomains is null)
			{
				string message = "Castlevania: Symphony of the Night must be open to use this tool";
				string caption = "Error Rom Not Loaded";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				DialogResult result;

				result = MessageBox.Show(message, caption, buttons);
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					this.Close();
					return;
				}
			}

			if (!File.Exists(Paths.CheatsPath))
			{
				File.Copy(Paths.CheatsPath + ".bkp", Paths.CheatsPath);
			}

			this.MainForm.CheatList.Load(_memoryDomains, Paths.CheatsPath, false);
			this.MainForm.CheatList.DisableAll();

			actorApi = new ActorApi(_maybeMemAPI);
			alucardApi = new AlucardApi(_maybeMemAPI);
			gameApi = new GameApi(_maybeMemAPI);
			renderingApi = new RenderingApi(_maybeMemAPI);
			watchlistService = new WatchlistService(_memoryDomains, _emu?.SystemId, GlobalConfig);
			inputService = new InputService(_maybeJoypadApi, alucardApi);

			Console.SetOut(log);
		}

		public override bool AskSaveChanges() => true;

		public override void Restart() { }

		public override void UpdateValues(ToolFormUpdateType type)
		{
			if (coopForm is not null || khaosForm is not null)
			{
				inputService.UpdateInputs();
			}
			cooldown++;
			if (cooldown == Globals.UpdateCooldownFrames)
			{
				cooldown = 0;
				if (trackerForm is not null)
				{
					trackerForm.UpdateTracker();
				}
				if (khaosForm is not null)
				{
					khaosForm.UpdateKhaosValues();
				}
				if (coopForm is not null)
				{
					coopForm.UpdateCoop();
				}
			}
		}

		private void ToolMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			toolConfig.SaveConfig();
			if (trackerForm != null)
			{
				trackerForm.Close();
				trackerForm.Dispose();
			}

			if (khaosForm != null)
			{
				khaosForm.Close();
				khaosForm.Dispose();
			}

			if (coopForm != null)
			{
				coopForm.Close();
				coopForm.Dispose();
			}

			string logPath = Paths.LogsPath + DateTime.Now.ToString("dd-MM-yy hh-mm-ss") + ".txt";
			if (!File.Exists(logPath))
			{
				using (StreamWriter w = File.AppendText(logPath))
				{
					w.Write(log.ToString());
				}
			}

			actorApi = null;
			alucardApi = null;
			gameApi = null;
			renderingApi = null;
			watchlistService = null;
			inputService = null;
		}

		private void autotrackerLaunch_Click(object sender, EventArgs e)
		{
			if (trackerForm is not null && renderingApi is not null && gameApi is not null && alucardApi is not null && watchlistService is not null)
			{
				trackerForm.Close();
				trackerForm = new TrackerForm(toolConfig, watchlistService, renderingApi, gameApi, alucardApi);
				trackerForm.Show();
			}
			else if (trackerForm is null && renderingApi is not null && gameApi is not null && alucardApi is not null && watchlistService is not null)
			{
				trackerForm = new TrackerForm(toolConfig, watchlistService, renderingApi, gameApi, alucardApi);
				trackerForm.Show();
			}
		}

		private void khaosChatLaunch_Click(object sender, EventArgs e)
		{
			if (khaosForm is not null && gameApi is not null && alucardApi is not null && actorApi is not null)
			{
				khaosForm.Close();
				khaosForm = new KhaosForm(toolConfig, this.MainForm.CheatList, gameApi, alucardApi, actorApi, notificationService, inputService);
				khaosForm.Show();
			}
			else if (khaosForm is null && gameApi is not null && alucardApi is not null && actorApi is not null)
			{
				khaosForm = new KhaosForm(toolConfig, this.MainForm.CheatList, gameApi, alucardApi, actorApi, notificationService, inputService);
				khaosForm.Show();
			}
		}

		private void multiplayerLaunch_Click(object sender, EventArgs e)
		{
			if (coopForm is not null && gameApi is not null && alucardApi is not null && watchlistService is not null && _maybeJoypadApi is not null)
			{
				coopForm.Close();
				coopForm = new CoopForm(toolConfig, watchlistService, inputService, gameApi, alucardApi, _maybeJoypadApi, notificationService);
				coopForm.Show();
			}
			else if (coopForm is null && gameApi is not null && alucardApi is not null && watchlistService is not null && _maybeJoypadApi is not null)
			{
				coopForm = new CoopForm(toolConfig, watchlistService, inputService, gameApi, alucardApi, _maybeJoypadApi, notificationService);
				coopForm.Show();
			}
		}

		private void autotrackerSelect_Click(object sender, EventArgs e)
		{
			autotrackerSettingsPanel.Visible = true;
			autotrackerSettingsPanel.Enabled = true;
			autotrackerLaunch.Visible = true;
			autotrackerLaunch.Enabled = true;

			khaosSettingsPanel.Visible = false;
			khaosSettingsPanel.Enabled = false;
			khaosChatLaunch.Visible = false;
			khaosChatLaunch.Enabled = false;

			coopSettingsPanel.Visible = false;
			coopSettingsPanel.Enabled = false;
			multiplayerLaunch.Visible = false;
			multiplayerLaunch.Enabled = false;

			aboutPanel.Visible = false;
			aboutPanel.Enabled = false;
		}

		private void khaosChatSelect_Click(object sender, EventArgs e)
		{
			khaosSettingsPanel.Visible = true;
			khaosSettingsPanel.Enabled = true;
			khaosChatLaunch.Visible = true;
			khaosChatLaunch.Enabled = true;

			autotrackerSettingsPanel.Visible = false;
			autotrackerSettingsPanel.Enabled = false;
			autotrackerLaunch.Visible = false;
			autotrackerLaunch.Enabled = false;

			coopSettingsPanel.Visible = false;
			coopSettingsPanel.Enabled = false;
			multiplayerLaunch.Visible = false;
			multiplayerLaunch.Enabled = false;

			aboutPanel.Visible = false;
			aboutPanel.Enabled = false;
		}

		private void multiplayerSelect_Click(object sender, EventArgs e)
		{
			coopSettingsPanel.Visible = true;
			coopSettingsPanel.Enabled = true;
			multiplayerLaunch.Visible = true;
			multiplayerLaunch.Enabled = true;

			autotrackerSettingsPanel.Visible = false;
			autotrackerSettingsPanel.Enabled = false;
			autotrackerLaunch.Visible = false;
			autotrackerLaunch.Enabled = false;

			khaosSettingsPanel.Visible = false;
			khaosSettingsPanel.Enabled = false;
			khaosChatLaunch.Visible = false;
			khaosChatLaunch.Enabled = false;

			aboutPanel.Visible = false;
			aboutPanel.Enabled = false;
		}

		private void aboutButton_Click(object sender, EventArgs e)
		{
			aboutPanel.Visible = true;
			aboutPanel.Enabled = true;

			autotrackerSettingsPanel.Visible = false;
			autotrackerSettingsPanel.Enabled = false;
			autotrackerLaunch.Visible = false;
			autotrackerLaunch.Enabled = false;

			khaosSettingsPanel.Visible = false;
			khaosSettingsPanel.Enabled = false;
			khaosChatLaunch.Visible = false;
			khaosChatLaunch.Enabled = false;

			coopSettingsPanel.Visible = false;
			coopSettingsPanel.Enabled = false;
			multiplayerLaunch.Visible = false;
			multiplayerLaunch.Enabled = false;
		}

		private void ToolMainForm_Move(object sender, EventArgs e)
		{
			if (this.Location.X > 0)
			{
				toolConfig.Location = this.Location;
			}
		}

		private void AboutPanel_UpdateButton_Click(object sender, EventArgs e)
		{
			string path = Directory.GetCurrentDirectory();
			var updater = new ProcessStartInfo() { FileName = path + Paths.UpdaterPath, UseShellExecute = false };
			updater.WorkingDirectory = (path + Paths.UpdaterFolderPath);
			Process.Start(updater);
			Application.Exit();
		}
	}
}

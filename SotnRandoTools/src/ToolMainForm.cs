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
using SotnRandoTools.Configuration;
using SotnRandoTools.Constants;
using SotnRandoTools.Properties;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	[ExternalTool("Symphony of the Night Randomizer Tools",
		Description = "A collection of tools to enhance the SotN randomizer experience.",
		LoadAssemblyFiles = new[]
		{
			"SotnRandoTools/dll/SotnApi.dll",
			"SotnRandoTools/dll/SimpleTCP.dll"
		})]
	[ExternalToolEmbeddedIcon("SotnRandoTools.Resources.BizAlucard.png")]
	[ExternalToolApplicability.SingleRom(CoreSystem.Playstation, "0DDCBC3D")]
	public partial class ToolMainForm : ToolFormBase, IExternalToolForm
	{
		[RequiredService]
		private IEmulator? _emu { get; set; }

		[RequiredService]
		private IMemoryDomains? _memoryDomains { get; set; }

		[RequiredApi]
		private IEmuClientApi? _maybeClientAPI { get; set; }

		[RequiredApi]
		private IJoypadApi? _maybeJoypadApi { get; set; }

		[RequiredApi]
		private IEmulationApi? _maybeEmuAPI { get; set; }

		[RequiredApi]
		private IGuiApi? _maybeGuiAPI { get; set; }

		[RequiredApi]
		private IMemoryApi? _maybeMemAPI { get; set; }

		private ApiContainer? _apis;

		private ApiContainer APIs => _apis ??= new ApiContainer(new Dictionary<Type, IExternalApi>
		{
			[typeof(IEmuClientApi)] = _maybeClientAPI ?? throw new NullReferenceException(),
			[typeof(IJoypadApi)] = _maybeJoypadApi ?? throw new NullReferenceException(),
			[typeof(IEmulationApi)] = _maybeEmuAPI ?? throw new NullReferenceException(),
			[typeof(IGuiApi)] = _maybeGuiAPI ?? throw new NullReferenceException(),
			[typeof(IMemoryApi)] = _maybeMemAPI ?? throw new NullReferenceException(),
		});
		private Config GlobalConfig => (_maybeEmuAPI as EmulationApi ?? throw new Exception("required API wasn't fulfilled")).ForbiddenConfigReference;

		private SotnApi.Main.SotnApi? sotnApi;
		private ToolConfig toolConfig;
		private WatchlistService? watchlistService;
		private NotificationService? notificationService;
		private InputService? inputService;
		private TrackerForm? trackerForm;
		private CoopForm? coopForm;
		private AutotrackerSettingsPanel? autotrackerSettingsPanel;
		private CoopSettingsPanel? coopSettingsPanel;
		private AboutPanel? aboutPanel;
		private string _windowTitle = "Symphony of the Night Randomizer Tools";
		private const int PanelOffset = 130;
		private int cooldown = 0;

		public ToolMainForm()
		{
			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			InitializeConfig();
		}

		private void InitializeConfig()
		{
			string currentVersion = typeof(AboutPanel).Assembly.GetName().Version.ToString().Substring(0, 5);
			if (File.Exists(Paths.ConfigPath))
			{
				string configJson = File.ReadAllText(Paths.ConfigPath);

				try
				{
					toolConfig = JsonConvert.DeserializeObject<ToolConfig>(configJson,
						new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, MissingMemberHandling = MissingMemberHandling.Ignore }) ?? new ToolConfig();
				}
				catch
				{
					toolConfig = new ToolConfig();
				}
			}
			else
			{
				toolConfig = new ToolConfig();
			}

			toolConfig.Version = currentVersion;
		}

		protected override string WindowTitle => _windowTitle;

		protected override string WindowTitleStatic => "Symphony of the Night Randomizer Tools";

		private void ToolMainForm_Load(object sender, EventArgs e)
		{
			if (SystemInformation.VirtualScreen.Width > toolConfig.Location.X && SystemInformation.VirtualScreen.Height > toolConfig.Location.Y)
			{
				this.Location = toolConfig.Location;
			}

			aboutPanel = new AboutPanel();
			aboutPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(aboutPanel);
			aboutPanel.UpdateButton_Click += AboutPanel_UpdateButton_Click;

			autotrackerSettingsPanel = new AutotrackerSettingsPanel(toolConfig);
			autotrackerSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(autotrackerSettingsPanel);

			coopSettingsPanel = new CoopSettingsPanel(toolConfig);
			coopSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(coopSettingsPanel);

			if (notificationService is not null)
			{
				coopSettingsPanel.NotificationService = notificationService;
			}
			autotrackerSelect.Image = Resources.Tracker;
			coopSelect.Image = Resources.coop;
			aboutButton.Image = Resources.VectorSimple;
#if WIN
			this.Icon = Resources.Icon;
#endif
		}

		public override bool AskSaveChanges() => true;

		public override void Restart()
		{
			if (trackerForm is not null && !trackerForm.IsDisposed)
			{
				trackerForm.Close();
				trackerForm.Dispose();
			}
			if (coopForm is not null && !coopForm.IsDisposed)
			{
				coopForm.Close();
				coopForm.Dispose();
			}

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

			_maybeMemAPI.UseMemoryDomain(_memoryDomains.MainMemory.Name);
			sotnApi = new SotnApi.Main.SotnApi(_maybeMemAPI);
			watchlistService = new WatchlistService(_memoryDomains);
			inputService = new InputService(_maybeJoypadApi, sotnApi);
			notificationService = new NotificationService(toolConfig, _maybeGuiAPI, _maybeClientAPI);
			if (coopSettingsPanel is not null)
			{
				coopSettingsPanel.NotificationService = notificationService;
			}
		}

		public override void UpdateValues(ToolFormUpdateType type)
		{
			if (type != ToolFormUpdateType.PostFrame)
			{
				return;
			}
			if (coopForm is not null)
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

			if (coopForm != null)
			{
				coopForm.Close();
				coopForm.Dispose();
			}

			if (notificationService != null)
			{
				notificationService.StopOverlayServer();
				notificationService = null;
			}

			sotnApi = null;
			watchlistService = null;
			inputService = null;
		}

		private void autotrackerLaunch_Click(object sender, EventArgs e)
		{
			if (sotnApi is not null && watchlistService is not null)
			{
				if (trackerForm is not null)
				{
					trackerForm.Close();
					trackerForm.Dispose();
				}
				trackerForm = new TrackerForm(toolConfig, watchlistService, sotnApi, notificationService);
				trackerForm.Show();
			}
		}

		private void coopLaunch_Click(object sender, EventArgs e)
		{
			if (sotnApi is not null && watchlistService is not null && APIs.Joypad is not null)
			{
				if (coopForm is not null)
				{
					coopForm.Close();
					coopForm.Dispose();
				}

				if (inputService.SupportsR3())
				{
					coopForm = new CoopForm(toolConfig, watchlistService, inputService, sotnApi, APIs.Joypad, notificationService);
					coopForm.Show();
				}
				else
				{
					MessageBox.Show("You can't use Co Op without setting the Player 1 controller to Dual Shock, in order to support the R3 button.", "Controller not supported",
								 MessageBoxButtons.OK,
								 MessageBoxIcon.Warning);
				}
			}
		}

		private void autotrackerSelect_Click(object sender, EventArgs e)
		{
			autotrackerSettingsPanel.Visible = true;
			autotrackerSettingsPanel.Enabled = true;
			autotrackerLaunch.Visible = true;
			autotrackerLaunch.Enabled = true;

			coopSettingsPanel.Visible = false;
			coopSettingsPanel.Enabled = false;
			coopLaunch.Visible = false;
			coopLaunch.Enabled = false;

			aboutPanel.Visible = false;
			aboutPanel.Enabled = false;
		}

		private void coopSelect_Click(object sender, EventArgs e)
		{
			coopSettingsPanel.Visible = true;
			coopSettingsPanel.Enabled = true;
			coopLaunch.Visible = true;
			coopLaunch.Enabled = true;

			autotrackerSettingsPanel.Visible = false;
			autotrackerSettingsPanel.Enabled = false;
			autotrackerLaunch.Visible = false;
			autotrackerLaunch.Enabled = false;

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

			coopSettingsPanel.Visible = false;
			coopSettingsPanel.Enabled = false;
			coopLaunch.Visible = false;
			coopLaunch.Enabled = false;
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

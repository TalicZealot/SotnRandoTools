﻿using System;
using System.Collections.Generic;
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
	[ExternalTool("Symphony of the Night Randomizer Tools", Description = "A collection of tools to enhance the SotN randomizer experience.", LoadAssemblyFiles = new[] { "./SotnRandoTools/SotnApi.dll", "./SotnRandoTools/SimpleTCP.dll" })]
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
		private TrackerForm? trackerForm;
		private KhaosForm? khaosForm;
		private CoopForm? coopForm;
		private AutotrackerSettingsPanel? autotrackerSettingsPanel;
		private KhaosSettingsPanel? khaosSettingsPanel;
		private CoopSettingsPanel? coopSettingsPanel;
		private string _windowTitle = "Symphony of the Night Randomizer Tools";
		private const int PanelOffset = 130;
		private const int UpdateCooldownFrames = 10;
		private int cooldown = 0;
		public ToolMainForm()
		{
			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
			if (File.Exists(Paths.ConfigPath))
			{
				string configJson = File.ReadAllText(Paths.ConfigPath);
				toolConfig = JsonConvert.DeserializeObject<ToolConfig>(configJson) ?? new ToolConfig();
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

			autotrackerSettingsPanel = new AutotrackerSettingsPanel(toolConfig);
			autotrackerSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(autotrackerSettingsPanel);

			khaosSettingsPanel = new KhaosSettingsPanel(toolConfig);
			khaosSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(khaosSettingsPanel);

			coopSettingsPanel = new CoopSettingsPanel(toolConfig);
			coopSettingsPanel.Location = new Point(0, PanelOffset);
			this.Controls.Add(coopSettingsPanel);

			this.MainForm.CheatList.Load(_memoryDomains, Paths.CheatsPath, false);
			this.MainForm.CheatList.DisableAll();

			actorApi = new ActorApi(_maybeMemAPI);
			alucardApi = new AlucardApi(_maybeMemAPI);
			gameApi = new GameApi(_maybeMemAPI);
			renderingApi = new RenderingApi(_maybeMemAPI);
			watchlistService = new WatchlistService(_memoryDomains, _emu?.SystemId, GlobalConfig);
		}
		public override bool AskSaveChanges() => true;

		public override void Restart() { }

		public override void UpdateValues(ToolFormUpdateType type)
		{
			cooldown++;
			if (cooldown == UpdateCooldownFrames)
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
				khaosForm = new KhaosForm(toolConfig, this.MainForm.CheatList, gameApi, alucardApi, actorApi);
				khaosForm.Show();
			}
			else if (khaosForm is null && gameApi is not null && alucardApi is not null && actorApi is not null)
			{
				khaosForm = new KhaosForm(toolConfig, this.MainForm.CheatList, gameApi, alucardApi, actorApi);
				khaosForm.Show();
			}
		}

		private void multiplayerLaunch_Click(object sender, EventArgs e)
		{
			if (coopForm is not null && gameApi is not null && alucardApi is not null && watchlistService is not null && _maybeJoypadApi is not null && _maybeGuiAPI is not null)
			{
				coopForm.Close();
				coopForm = new CoopForm(toolConfig, watchlistService, gameApi, alucardApi, _maybeJoypadApi, _maybeGuiAPI);
				coopForm.Show();
			}
			else if (coopForm is null && gameApi is not null && alucardApi is not null && watchlistService is not null && _maybeJoypadApi is not null && _maybeGuiAPI is not null)
			{
				coopForm = new CoopForm(toolConfig, watchlistService, gameApi, alucardApi, _maybeJoypadApi, _maybeGuiAPI);
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
		}

		private void aboutButton_Click(object sender, EventArgs e)
		{
			string targetURL = @"https://www.twitch.tv/taliczealot";
			System.Diagnostics.Process.Start(targetURL);
		}

		private void ToolMainForm_Move(object sender, EventArgs e)
		{
			if (this.Location.X > 0)
			{
				toolConfig.Location = this.Location;
			}
		}
	}
}

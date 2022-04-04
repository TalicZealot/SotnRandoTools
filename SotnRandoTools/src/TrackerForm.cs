using System;
using System.Drawing;
using System.Windows.Forms;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.RandoTracker;
using SotnRandoTools.RandoTracker.Adapters;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	public partial class TrackerForm : Form
	{
		private readonly IToolConfig toolConfig;
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

		private GraphicsAdapter? formGraphics;
		private Graphics? internalGraphics;
		private Tracker? tracker;

		public TrackerForm(IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.watchlistService = watchlistService;
			this.sotnApi = sotnApi;
			this.notificationService = notificationService;

			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
		}
		public void UpdateTracker()
		{
			if (tracker is not null)
			{
				this.tracker.Update();
			}
		}

		public void SetTrackerVladRelicLocationDisplay(IRelicLocationDisplay vladRelicLocationDisplay)
		{
			if (vladRelicLocationDisplay is null) throw new ArgumentNullException(nameof(vladRelicLocationDisplay));
			tracker.RelicLocationDisplay = vladRelicLocationDisplay;
		}

		private void TrackerForm_Load(object sender, EventArgs e)
		{
			this.TopMost = toolConfig.Tracker.AlwaysOnTop;
			this.Size = new Size(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			this.Location = toolConfig.Tracker.Location;
			this.internalGraphics = this.CreateGraphics();
			this.formGraphics = new GraphicsAdapter(internalGraphics);
			this.tracker = new Tracker(formGraphics, toolConfig, watchlistService, sotnApi, notificationService);
		}

		private void TrackerForm_Paint(object sender, PaintEventArgs e)
		{
			if (tracker != null)
			{
				tracker.DrawRelicsAndItems();
			}
		}

		private void TrackerForm_Resize(object sender, EventArgs e)
		{
			if (this.Location.X < 0)
			{
				return;
			}

			if (this.Width > toolConfig.Tracker.Width || this.Height > toolConfig.Tracker.Height)
			{
				internalGraphics = this.CreateGraphics();
				this.formGraphics = new GraphicsAdapter(internalGraphics);
			}

			toolConfig.Tracker.Width = this.Width;
			toolConfig.Tracker.Height = this.Height;

			if (tracker is not null && formGraphics is not null)
			{
				tracker.GraphicsEngine.ChangeGraphics(formGraphics);
				tracker.GraphicsEngine.CalculateGrid(this.Width, this.Height);
				tracker.DrawRelicsAndItems();
			}
		}

		private void TrackerForm_Move(object sender, EventArgs e)
		{
			if (this.Location.X > 0)
			{
				toolConfig.Tracker.Location = this.Location;
			}
		}

		private void TrackerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (toolConfig.Tracker.SaveReplays)
			{
				tracker.SaveReplay();
			}

			tracker.CloseAutosplitter();
			tracker = null;
		}
	}
}

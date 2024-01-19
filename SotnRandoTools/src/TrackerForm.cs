using System;
using System.Drawing;
using System.Windows.Forms;
using BizHawk.Bizware.Graphics;
using BizHawk.Bizware.Graphics.Controls;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	internal sealed partial class TrackerForm : Form
	{
		private readonly IToolConfig toolConfig;
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

		private IGL_OpenGL? gl;
		private GraphicsControl? graphicsControl;
		private Tracker? tracker;
		private TrackerRendererOpenGL? trackerRendererOpenGL;
		private Bitmap drawingSurface;
		private GuiRenderer guiRenderer;

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
				if (!this.trackerRendererOpenGL.Refreshed)
				{
					trackerRendererOpenGL.Render();
					this.trackerRendererOpenGL.Refreshed = true;

				}
			}
		}

		private void InitializeRenderer()
		{
			this.gl = new IGL_OpenGL();
			gl.ClearColor(Color.FromArgb(17, 00, 17));
			this.graphicsControl = GraphicsControlFactory.CreateGraphicsControl(gl);
			this.Controls.Add(graphicsControl);
			graphicsControl.Location = new Point(0, 0);
			graphicsControl.Width = this.Width;
			graphicsControl.Height = this.Height;
			graphicsControl.Visible = true;
			this.guiRenderer = new GuiRenderer(gl);
			guiRenderer.SetDefaultPipeline();
		}

		private void TrackerForm_Load(object sender, EventArgs e)
		{
			this.TopMost = toolConfig.Tracker.AlwaysOnTop;
			this.Size = new Size(toolConfig.Tracker.Width, toolConfig.Tracker.Height);

			if (SystemInformation.VirtualScreen.Width > toolConfig.Tracker.Location.X && SystemInformation.VirtualScreen.Height > toolConfig.Tracker.Location.Y)
			{
				this.Location = toolConfig.Tracker.Location;
			}

			InitializeRenderer();
			this.trackerRendererOpenGL = new TrackerRendererOpenGL(toolConfig, guiRenderer, graphicsControl);
			this.tracker = new Tracker(trackerRendererOpenGL, toolConfig, watchlistService, sotnApi, notificationService);
			trackerRendererOpenGL.CalculateGrid(this.Width, this.Height);

#if WIN
			this.Icon = SotnRandoTools.Properties.Resources.Icon;
#endif
		}

		private void TrackerForm_Resize(object sender, EventArgs e)
		{
			if (this.Location.X < 0)
			{
				return;
			}

			if (this.Width <= this.MaximumSize.Width && this.Height <= this.MaximumSize.Height)
			{
				toolConfig.Tracker.Width = this.Width;
				toolConfig.Tracker.Height = this.Height;
			}

			if (trackerRendererOpenGL is not null)
			{
				graphicsControl.Width = this.Width;
				graphicsControl.Height = this.Height;
				trackerRendererOpenGL.CalculateGrid(this.Width, this.Height);
				trackerRendererOpenGL.Render();
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

			notificationService.StopOverlayServer();

			tracker.CloseAutosplitter();
			tracker = null;
		}

	}
}

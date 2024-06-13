using System;
using System.Drawing;
using System.Windows.Forms;
//using BizHawk.Bizware.Graphics;
//using BizHawk.Bizware.Graphics.Controls;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.RandoTracker;
using SotnRandoTools.RandoTracker.Adapters;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	internal sealed partial class TrackerForm : Form
	{
		private readonly IToolConfig toolConfig;
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;

#if GL
		private IGL_OpenGL? gl;
		private GraphicsControl? graphicsControl;
		private TrackerRendererOpenGL? trackerRendererOpenGL;
		private GuiRenderer guiRenderer;
#endif
		private GraphicsAdapter? formGraphics;
		private TrackerRendererGDI? trackerRendererGDI;
		private Tracker? tracker;
		private Bitmap drawingSurface;

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
				tracker.Update();
#if GL
				if (!this.trackerRendererOpenGL.Refreshed)
				{
					trackerRendererOpenGL.Render();
					this.trackerRendererOpenGL.Refreshed = true;

				}
#endif
				if (trackerRendererGDI.Refreshed)
				{
					trackerRendererGDI.Refreshed = false;
					this.Invalidate();
				}
			}
		}

		private void InitializeRenderer()
		{
#if GL
			this.gl = new IGL_OpenGL();
			this.graphicsControl = GraphicsControlFactory.CreateGraphicsControl(gl);
			this.Controls.Add(graphicsControl);
			graphicsControl.Location = new Point(0, 0);
			graphicsControl.Width = this.Width;
			graphicsControl.Height = this.Height;
			graphicsControl.Visible = true;
			this.guiRenderer = new GuiRenderer(gl);
			guiRenderer.SetDefaultPipeline();
#endif
		}

		private void TrackerForm_Load(object sender, EventArgs e)
		{
			TopMost = toolConfig.Tracker.AlwaysOnTop;
			Size = new Size(toolConfig.Tracker.Width, toolConfig.Tracker.Height);

			if (SystemInformation.VirtualScreen.Width > toolConfig.Tracker.Location.X && SystemInformation.VirtualScreen.Height > toolConfig.Tracker.Location.Y)
			{
				Location = toolConfig.Tracker.Location;
			}

#if GL
			InitializeRenderer();
			trackerRendererOpenGL = new TrackerRendererOpenGL(toolConfig, guiRenderer, graphicsControl);
			trackerRendererOpenGL.CalculateGrid(this.Width, this.Height);
#endif
			drawingSurface = new Bitmap(this.Width, this.Height);
			Graphics internalGraphics = Graphics.FromImage(drawingSurface);
			this.formGraphics = new GraphicsAdapter(internalGraphics);
			trackerRendererGDI = new TrackerRendererGDI(formGraphics, toolConfig);
			tracker = new Tracker(trackerRendererGDI, toolConfig, watchlistService, sotnApi, notificationService);

#if WIN
			this.Icon = SotnRandoTools.Properties.Resources.Icon;
#endif
		}

		private void TrackerForm_Paint(object sender, PaintEventArgs e)
		{
			if (tracker != null)
			{
				e.Graphics.DrawImage(drawingSurface, 0, 0);
			}
		}

		private void TrackerForm_Resize(object sender, EventArgs e)
		{
			if (this.Location.X < 0)
			{
				return;
			}
#if GL
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
#endif
			if (this.Width > toolConfig.Tracker.Width || this.Height > toolConfig.Tracker.Height)
			{
				drawingSurface = new Bitmap(this.Width, this.Height);
				Graphics internalGraphics = Graphics.FromImage(drawingSurface);
				this.formGraphics = new GraphicsAdapter(internalGraphics);
			}

			if (this.Width <= this.MaximumSize.Width && this.Height <= this.MaximumSize.Height)
			{
				toolConfig.Tracker.Width = this.Width;
				toolConfig.Tracker.Height = this.Height;
			}

			if (tracker is not null && formGraphics is not null)
			{
				trackerRendererGDI.ChangeGraphics(formGraphics);
				trackerRendererGDI.CalculateGrid(this.Width, this.Height);
				this.Invalidate();
				trackerRendererGDI.Render();
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
			//Dispose of tracker properly.
			tracker = null;
		}
	}
}

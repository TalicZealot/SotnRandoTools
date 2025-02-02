using System;
using System.Threading;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools.RandoTracker
{
	internal class TrackerWIndow : IDisposable
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;
		private TrackerRendererOpenGL renderer;
		private Thread renderThread;
		private Tracker? tracker;
		private bool disposed = false;

		public TrackerWIndow(IToolConfig toolConfig, ISotnApi sotnApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.sotnApi = sotnApi;
			this.notificationService = notificationService;

			tracker = new Tracker(toolConfig, sotnApi, notificationService);
			renderThread = new Thread(Run);
			renderThread.Name = "Render Thread";
			renderThread.Start();
		}

		public Tracker Tracker
		{
			get
			{
				return tracker;
			}
		}

		public void Run()
		{
			renderer = new TrackerRendererOpenGL(toolConfig, tracker);
			renderer.Run();
		}

		public void Dispose()
		{
			if (disposed) return;
			if (renderer != null)
			{
				renderer.Dispose();
			}
			renderThread.Abort();

			if (toolConfig.Tracker.SaveReplays)
			{
				tracker.SaveReplay();
			}
			notificationService.StopOverlayServer();
			tracker.CloseAutosplitter();
			tracker = null;
			disposed = true;
		}

		public void UpdateTracker()
		{
			tracker.Update();
		}
	}
}

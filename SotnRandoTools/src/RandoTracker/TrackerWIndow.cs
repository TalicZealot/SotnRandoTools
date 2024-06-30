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
		private readonly IWatchlistService watchlistService;
		private readonly ISotnApi sotnApi;
		private readonly INotificationService notificationService;
		private TrackerRendererOpenGL renderer;
		private Thread renderThread;
		private readonly Tracker tracker;
		private bool started = false;

		public TrackerWIndow(IToolConfig toolConfig, IWatchlistService watchlistService, ISotnApi sotnApi, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (watchlistService is null) throw new ArgumentNullException(nameof(watchlistService));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.watchlistService = watchlistService;
			this.sotnApi = sotnApi;
			this.notificationService = notificationService;

			tracker = new Tracker(toolConfig, watchlistService, sotnApi, notificationService);
			renderThread = new Thread(Run);
			renderThread.Start();
		}

		public void Run()
		{
			renderer = new TrackerRendererOpenGL(toolConfig, tracker);
			renderer.Run();
		}

		public void Dispose()
		{
			if (renderer != null)
			{
				renderer.Dispose();
			}
			renderThread.Abort();
		}

		public void UpdateTracker()
		{
			tracker.Update();
		}
	}
}

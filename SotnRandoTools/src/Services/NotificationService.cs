using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Services
{
	internal sealed class NotificationService : INotificationService
	{
		private OverlaySocketServer overlaySocketServer;
		private const int NotificationTime = 5 * 1000;
		private const int NotificationTimeFast = 3 * 1000;
		private const int MapOffsetX = 16;
		private const int MapOffsetY = 20;
		private Color WallColor = Color.FromArgb(192, 192, 192);

		private readonly IGuiApi guiApi;
		private readonly IToolConfig toolConfig;
		private readonly IEmuClientApi clientAPI;

		private System.Timers.Timer messageTimer;
		private System.Timers.Timer countdownTimer;
		private int scale;
		private Image textbox;
		private Dictionary<string, Image> relicImages = new();
		private Dictionary<string, MapCoordinates> relicCoordinates = new();
		private Dictionary<string, MapCoordinates> invertedRelicCoordinates = new();
		private System.Windows.Media.MediaPlayer audioPlayer = new();
		private List<string> messageQueue = new();

		public NotificationService(IToolConfig toolConfig, IGuiApi guiApi, IEmuClientApi clientAPI)
		{
			if (guiApi is null) throw new ArgumentNullException(nameof(guiApi));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (clientAPI is null) throw new ArgumentNullException(nameof(clientAPI));
			this.guiApi = guiApi;
			this.toolConfig = toolConfig;
			this.clientAPI = clientAPI;

			overlaySocketServer = new OverlaySocketServer(toolConfig);
			messageTimer = new();
			messageTimer.Interval = NotificationTime;
			messageTimer.Elapsed += DequeueMessage;
			messageTimer.Start();
			countdownTimer = new();
			countdownTimer.Interval = 1000;
			countdownTimer.Elapsed += RefreshUI;
			scale = GetScale();
			ResizeImages();
			audioPlayer.Volume = (double) toolConfig.Coop.Volume / 10F;
		}

		public double Volume
		{
			set
			{
				audioPlayer.Volume = value;
			}
		}

		public void PlayAlert(string url)
		{
			if (String.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));

			if (url == String.Empty)
			{
				return;
			}
			try
			{
				audioPlayer.Dispatcher.Invoke(() =>
				{
					audioPlayer.Open(new Uri(url, UriKind.Relative));
				});
				audioPlayer.Dispatcher.Invoke(() =>
				{
					audioPlayer.Play();
				});
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void AddMessage(string message)
		{
			messageQueue.Add(message);
			if (messageQueue.Count == 1)
			{
				messageTimer.Stop();
				messageTimer.Start();
			}
			if (messageQueue.Count > 1)
			{
				messageTimer.Interval = NotificationTimeFast;
			}
			else
			{
				messageTimer.Interval = NotificationTime;
			}
			if (!countdownTimer.Enabled)
			{
				countdownTimer.Start();
			}
		}

		public void StartOverlayServer()
		{
			overlaySocketServer.StartServer();
		}

		public void StopOverlayServer()
		{
			overlaySocketServer.StopServer();
		}

		public void UpdateTrackerOverlay(int relics, int items)
		{
			overlaySocketServer.UpdateTracker(relics, items);
		}

		public void SetRelicCoordinates(string relic, int mapCol, int mapRow)
		{
			if (relicCoordinates.ContainsKey(relic))
			{
				return;
			}
			relicCoordinates.Add(relic, new MapCoordinates { Xpos = (mapCol * 2) + MapOffsetX, Ypos = mapRow + MapOffsetY });
		}

		public void SetInvertedRelicCoordinates(string relic, int mapCol, int mapRow)
		{
			if (invertedRelicCoordinates.ContainsKey(relic))
			{
				return;
			}
			invertedRelicCoordinates.Add(relic, new MapCoordinates { Xpos = (mapCol * 2) + MapOffsetX, Ypos = mapRow + MapOffsetY });
		}

		private void DrawUI()
		{
			int newScale = GetScale();
			if (scale != newScale)
			{
				scale = newScale;
				ResizeImages();
				Console.WriteLine($"Changed scale to {scale}");
			}

			int fontSize = 11 * scale;

			int screenWidth = clientAPI.ScreenWidth();
			int screenHeight = clientAPI.ScreenHeight();
			int scaledBufferWidth = (clientAPI.BufferWidth() * scale);
			int scaledBufferHeight = (clientAPI.BufferHeight() * scale);
			float pixelScaleX = (float) screenWidth / (float) scaledBufferWidth;
			float pixelScaleY = (float) screenHeight / (float) scaledBufferHeight;

			guiApi.WithSurface(DisplaySurfaceID.Client, () =>
			{
				guiApi.ClearGraphics();
				if (messageQueue.Count > 0)
				{
					DrawMessage(messageQueue[0], scale, (int) (screenWidth * 0.45), (int) (screenHeight * 0.1), fontSize);
				}
			});
		}

		private int GetScale()
		{
			int scale = clientAPI.GetWindowSize();
			if (IsPixelPro())
			{
				scale *= 2;
			}
			return scale;
		}

		private bool IsPixelPro()
		{
			int bufferWidth = clientAPI.BufferWidth();
			return bufferWidth == 800;
		}

		private void DrawMessage(string message, int scale, int xpos, int ypos, int fontSize)
		{
			int messageFontSize = fontSize;
			while (TextRenderer.MeasureText(messageQueue[0], new Font("Arial", messageFontSize)).Width > (textbox.Width - (20 * scale)))
			{
				messageFontSize -= 2;
			}
			guiApi.DrawImage(textbox, xpos, ypos, textbox.Width, textbox.Height, true);
			guiApi.DrawString(xpos + (int) (textbox.Width / 2), ypos + (11 * scale), message, Color.White, null, messageFontSize, "Arial", "bold", "center", "center");
		}

		private void DequeueMessage(Object sender, EventArgs e)
		{
			if (messageQueue.Count > 0)
			{
				messageQueue.RemoveAt(0);
			}
		}

		private void RefreshUI(Object sender, EventArgs e)
		{
			DrawUI();
			if (messageQueue.Count == 0)
			{
				countdownTimer.Stop();
			}
		}

		private Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.PixelOffsetMode = PixelOffsetMode.Half;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		private void ResizeImages()
		{
			Image unscaledTextbox = Image.FromFile(Paths.TextboxImage);

			textbox = ResizeImage(unscaledTextbox, unscaledTextbox.Width * scale, unscaledTextbox.Height * scale);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Models;
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
		private Image vermillionBirdIcon;
		private Image azureDragonIcon;
		private Image whiteTigerIcon;
		private Image blackTortoiseIcon;
		private Dictionary<string, Image> relicImages = new();
		private Dictionary<string, MapCoordinates> relicCoordinates = new();
		private Dictionary<string, MapCoordinates> invertedRelicCoordinates = new();
		private System.Windows.Media.MediaPlayer audioPlayer = new();
		private List<string> messageQueue = new();
		private bool fourBeastsIconsInitialized = false;
		private bool relicImagesInitialized = false;
		private bool mapOpen = false;
		private bool invertedMapOpen = false;

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
			audioPlayer.Volume = (double) toolConfig.Khaos.Volume / 10F;
			VermillionBirds = 0;
			AzureDragons = 0;
			BlackTortoises = 0;
			WhiteTigers = 0;
		}

		public double Volume
		{
			set
			{
				audioPlayer.Volume = value;
			}
		}

		public bool MapOpen
		{
			get
			{
				return mapOpen;
			}
			set
			{
				mapOpen = value;
				if (!countdownTimer.Enabled)
				{
					countdownTimer.Start();
				}
			}
		}

		public bool InvertedMapOpen
		{
			get
			{
				return invertedMapOpen;
			}
			set
			{
				invertedMapOpen = value;
				if (!countdownTimer.Enabled)
				{
					countdownTimer.Start();
				}
			}
		}

		public int VermillionBirds { get; set; }
		public int AzureDragons { get; set; }
		public int BlackTortoises { get; set; }
		public int WhiteTigers { get; set; }

		public void PlayAlert(string url)
		{
			if (String.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));

			if (url == String.Empty || !toolConfig.Khaos.Alerts)
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

		public void AddOverlayTimer(string name, int duration)
		{
			overlaySocketServer.AddTimer(name, duration);
		}

		public void UpdateOverlayQueue(List<QueuedAction> actionQueue)
		{
			overlaySocketServer.UpdateQueue(actionQueue);
		}

		public void UpdateOverlayMeter(int meter)
		{
			overlaySocketServer.UpdateMeter(meter);
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
				if (MapOpen)
				{
					DrawRelics(pixelScaleX, pixelScaleY);
					DrawFourBeastsUI((int) (screenWidth * 0.05), (int) (screenHeight * 0.86));
				}
				else if (InvertedMapOpen)
				{
					DrawInvertedRelics(pixelScaleX, pixelScaleY);
					DrawFourBeastsUI((int) (screenWidth * 0.05), (int) (screenHeight * 0.86));
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

		private void DrawRelics(float pixelScaleX, float pixelScaleY)
		{
			if (relicImagesInitialized == false)
			{
				InitializeRelicImageas();
			}

			foreach (var relic in relicCoordinates)
			{
				DrawRelic(relicImages[relic.Key], relic.Value.Xpos, relic.Value.Ypos, pixelScaleX, pixelScaleY);
			}
		}

		private void DrawFourBeastsUI(int xpos, int ypos)
		{
			if (fourBeastsIconsInitialized == false)
			{
				InitializeFourBeastsIcons();
			}

			guiApi.DrawImage(textbox, xpos, ypos, textbox.Width, textbox.Height, true);

			int iconWidth = vermillionBirdIcon.Width;

			xpos = xpos + (3 * scale);
			guiApi.DrawImage(vermillionBirdIcon, xpos, ypos + (1 * scale), vermillionBirdIcon.Width, vermillionBirdIcon.Height, true);
			guiApi.DrawString(iconWidth + xpos + (2 * scale), ypos + (11 * scale), VermillionBirds.ToString(), Color.White, null, 34, "Arial", "bold", "center", "center");

			xpos = xpos + (11 * scale) + iconWidth;
			guiApi.DrawImage(azureDragonIcon, xpos, ypos + (1 * scale), azureDragonIcon.Width, azureDragonIcon.Height, true);
			guiApi.DrawString(iconWidth + xpos + (2 * scale), ypos + (11 * scale), AzureDragons.ToString(), Color.White, null, 34, "Arial", "bold", "center", "center");

			xpos = xpos + (11 * scale) + iconWidth;
			guiApi.DrawImage(blackTortoiseIcon, xpos, ypos + (1 * scale), blackTortoiseIcon.Width, blackTortoiseIcon.Height, true);
			guiApi.DrawString(iconWidth + xpos + (2 * scale), ypos + (11 * scale), BlackTortoises.ToString(), Color.White, null, 34, "Arial", "bold", "center", "center");

			xpos = xpos + (11 * scale) + iconWidth;
			guiApi.DrawImage(whiteTigerIcon, xpos, ypos + (1 * scale), whiteTigerIcon.Width, whiteTigerIcon.Height, true);
			guiApi.DrawString(iconWidth + xpos + (2 * scale), ypos + (11 * scale), WhiteTigers.ToString(), Color.White, null, 34, "Arial", "bold", "center", "center");
		}

		private void DrawInvertedRelics(float pixelScaleX, float pixelScaleY)
		{
			if (relicImagesInitialized == false)
			{
				InitializeRelicImageas();
			}

			foreach (var relic in invertedRelicCoordinates)
			{
				DrawRelic(relicImages[relic.Key], relic.Value.Xpos, relic.Value.Ypos, pixelScaleX, pixelScaleY);
			}
		}

		private void DrawRelic(Image relic, int xpos, int ypos, float pixelScaleX, float pixelScaleY)
		{
			int finalXpos = (int) Math.Round(xpos * scale * pixelScaleX);
			int finalYpos = (int) Math.Round(ypos * scale * pixelScaleY);
			int scaledPixelX = (int) Math.Round(1 * scale * pixelScaleX);
			int scaledPixelY = (int) Math.Round(1 * scale * pixelScaleY);

			guiApi.DrawBox(finalXpos - scaledPixelX, finalYpos - scaledPixelY, finalXpos + (4 * scaledPixelX), finalYpos + (4 * scaledPixelY), null, WallColor);
			guiApi.DrawImage(relic, finalXpos, finalYpos, relic.Width, relic.Height, true);
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
			if (messageQueue.Count == 0 && !MapOpen && !InvertedMapOpen)
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

			if (fourBeastsIconsInitialized)
			{
				Image unscaledVermillionBirdIcon = Image.FromFile(Paths.IconVermillionBird);
				Image unscaledWhiteTigerIcon = Image.FromFile(Paths.IconWhiteTiger);
				Image unscaledAzureDragonIcon = Image.FromFile(Paths.IconAzureDragon);
				Image unscaledBlackTortoiseIcon = Image.FromFile(Paths.IconBlackTortoise);

				vermillionBirdIcon = ResizeImage(unscaledVermillionBirdIcon, unscaledVermillionBirdIcon.Width * scale, unscaledVermillionBirdIcon.Height * scale);
				whiteTigerIcon = ResizeImage(unscaledWhiteTigerIcon, unscaledWhiteTigerIcon.Width * scale, unscaledWhiteTigerIcon.Height * scale);
				azureDragonIcon = ResizeImage(unscaledAzureDragonIcon, unscaledAzureDragonIcon.Width * scale, unscaledAzureDragonIcon.Height * scale);
				blackTortoiseIcon = ResizeImage(unscaledBlackTortoiseIcon, unscaledBlackTortoiseIcon.Width * scale, unscaledBlackTortoiseIcon.Height * scale);
			}
		}

		private void InitializeRelicImageas()
		{
			foreach (var relic in Constants.Paths.RelicImages)
			{
				relicImages.Add(relic.Key, Image.FromFile(relic.Value));
			}
			relicImagesInitialized = true;
		}

		private void InitializeFourBeastsIcons()
		{
			Image unscaledVermillionBirdIcon = Image.FromFile(Paths.IconVermillionBird);
			Image unscaledWhiteTigerIcon = Image.FromFile(Paths.IconWhiteTiger);
			Image unscaledAzureDragonIcon = Image.FromFile(Paths.IconAzureDragon);
			Image unscaledBlackTortoiseIcon = Image.FromFile(Paths.IconBlackTortoise);

			vermillionBirdIcon = ResizeImage(unscaledVermillionBirdIcon, unscaledVermillionBirdIcon.Width * scale, unscaledVermillionBirdIcon.Height * scale);
			whiteTigerIcon = ResizeImage(unscaledWhiteTigerIcon, unscaledWhiteTigerIcon.Width * scale, unscaledWhiteTigerIcon.Height * scale);
			azureDragonIcon = ResizeImage(unscaledAzureDragonIcon, unscaledAzureDragonIcon.Width * scale, unscaledAzureDragonIcon.Height * scale);
			blackTortoiseIcon = ResizeImage(unscaledBlackTortoiseIcon, unscaledBlackTortoiseIcon.Width * scale, unscaledBlackTortoiseIcon.Height * scale);

			fourBeastsIconsInitialized = true;
		}
	}
}

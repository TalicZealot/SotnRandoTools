using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	internal sealed class NotificationService : INotificationService
	{
		private OverlaySocketServer overlaySocketServer;
		private const int MessageDuration = 120;
		private const int MessageDurationFast = 60;
		private const int MapOffsetX = 16;
		private const int MapOffsetY = 20;
		private Color WallColor = Color.FromArgb(192, 192, 192);

		private readonly IGuiApi guiApi;
		private readonly IToolConfig toolConfig;
		private readonly IEmuClientApi clientAPI;

		private int scale;
		private Image textbox;
#if WIN
		private System.Windows.Media.MediaPlayer audioPlayer = new();
#endif
		private Queue<string> messageQueue = new();
		private int messageFrames = 0;
		private bool updated = false;

		public NotificationService(IToolConfig toolConfig, IGuiApi guiApi, IEmuClientApi clientAPI)
		{
			this.guiApi = guiApi ?? throw new ArgumentNullException(nameof(guiApi));
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.clientAPI = clientAPI ?? throw new ArgumentNullException(nameof(clientAPI));

			overlaySocketServer = new OverlaySocketServer(toolConfig);
			scale = clientAPI.GetWindowSize();
			ResizeImages();
#if WIN
			audioPlayer.Volume = (double) toolConfig.Coop.Volume / 10F;
#endif
		}

		public double Volume
		{
			set
			{
#if WIN
				audioPlayer.Volume = value;
#endif
			}
		}

		public void PlayAlert(string url)
		{
			if (String.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
#if WIN
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
#endif
		}

		public void AddMessage(string message)
		{
			if (messageQueue.Count > 0 && messageQueue.Peek() == message)
			{
				return;
			}
			updated = true;
			messageQueue.Enqueue(message);
			if (messageQueue.Count == 1)
			{
				messageFrames = MessageDuration;
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

		public void Refresh()
		{
			if (messageQueue.Count > 0)
			{
				if (messageFrames == 0)
				{
					messageQueue.Dequeue();
					updated = true;
					if (messageQueue.Count > 0)
					{
						messageFrames = MessageDuration;
					}
					if (messageQueue.Count > 2)
					{
						messageFrames = MessageDurationFast;
					}
				}
				messageFrames--;
			}
			if (updated)
			{
				DrawUI();
				updated = false;
			}
		}

		private void DrawUI()
		{
			int newScale = clientAPI.GetWindowSize();
			if (scale != newScale)
			{
				scale = newScale;
				ResizeImages();
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
					DrawMessage(messageQueue.Peek(), scale, (int) (screenWidth * 0.45), (int) (screenHeight * 0.1), fontSize);
				}
			});
		}

		private void DrawMessage(string message, int scale, int xpos, int ypos, int fontSize)
		{
			int messageFontSize = fontSize;
			while (TextRenderer.MeasureText(message, new Font("Arial", messageFontSize)).Width > (textbox.Width - (20 * scale)))
			{
				messageFontSize -= 2;
			}
			guiApi.DrawImage(textbox, xpos, ypos, textbox.Width, textbox.Height, true);
			guiApi.DrawString(xpos + (int) (textbox.Width / 2), ypos + (11 * scale), message, Color.White, null, messageFontSize, "Arial", "bold", "center", "center");
		}

		private static Bitmap ResizeImage(Image image, int width, int height)
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
			if (!File.Exists(Paths.TextboxImage)) return;

			Image unscaledTextbox = Image.FromFile(Paths.TextboxImage);

			textbox = ResizeImage(unscaledTextbox, unscaledTextbox.Width * scale, unscaledTextbox.Height * scale);
		}
	}
}

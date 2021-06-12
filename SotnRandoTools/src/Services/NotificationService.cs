using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Timers;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Enums;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Services
{
	public class NotificationService : INotificationService
	{
		const int NotificationTime = 4 * 1000;
		const int NotificationTimeFast = 1 * 1000;

		private readonly IGuiApi guiApi;
		private readonly IToolConfig toolConfig;
		private readonly IEmuClientApi clientAPI;

		private System.Timers.Timer messageTimer;
		private System.Timers.Timer countdownTimer;
		private Image textbox;
		private Image iconSkull;
		private Image iconFairy;
		private Image iconEye;
		private System.Windows.Media.MediaPlayer audioPlayer = new();
		private List<string> messageQueue = new();
		private List<ActionType> actionQueue = new();
		private List<ActionTimer> actionTimers = new();
		private bool cleared = false;

		public NotificationService(IToolConfig toolConfig, IGuiApi guiApi, IEmuClientApi clientAPI)
		{
			if (guiApi is null) throw new ArgumentNullException(nameof(guiApi));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (clientAPI is null) throw new ArgumentNullException(nameof(clientAPI));
			this.guiApi = guiApi;
			this.toolConfig = toolConfig;
			this.clientAPI = clientAPI;

			messageTimer = new System.Timers.Timer();
			messageTimer.Interval = NotificationTime;
			messageTimer.Elapsed += DequeueMessage;
			messageTimer.Start();
			countdownTimer = new System.Timers.Timer();
			countdownTimer.Interval = 1000;
			countdownTimer.Elapsed += DecrementTimers;
			countdownTimer.Start();
			textbox = Image.FromFile(Paths.TextboxImage);
			iconSkull = Image.FromFile(Paths.IconSkull);
			iconFairy = Image.FromFile(Paths.IconFairy);
			iconEye = Image.FromFile(Paths.IconEye);
			audioPlayer.Volume = (double) toolConfig.Khaos.Volume / 10F;
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

		public void AddAction(ActionType action)
		{
			actionQueue.Add(action);
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
		}

		public void DequeueAction()
		{
			if (actionQueue.Count > 0)
			{
				actionQueue.RemoveAt(0);
			}
		}

		public void AddTimer(ActionTimer timer)
		{
			actionTimers.Add(timer);
		}

		private void DrawUI()
		{
			if (actionQueue.Count == 0 && actionTimers.Count == 0 && messageQueue.Count == 0)
			{
				if (!cleared)
				{
					guiApi.WithSurface(DisplaySurfaceID.Client, () =>
					{
						guiApi.ClearGraphics();
					});
				}
				cleared = true;
				return;
			}
			cleared = false;
			int bufferWidth = clientAPI.BufferWidth();
			int scale = clientAPI.GetWindowSize();
			bool pixelPro = bufferWidth == 800;
			if (pixelPro)
			{
				scale *= 2;
			}
			int fontSize = 11 * scale;

			Image scaledTextbox = ResizeImage(textbox, textbox.Width * scale, textbox.Height * scale);
			Image scaledIconSkull = ResizeImage(iconSkull, iconSkull.Width * scale, iconSkull.Height * scale);
			Image scaledIconFairy = ResizeImage(iconFairy, iconFairy.Width * scale, iconFairy.Height * scale);
			Image scaledIconEye = ResizeImage(iconEye, iconEye.Width * scale, iconEye.Height * scale);

			int screenWidth = clientAPI.ScreenWidth();
			int screenHeight = clientAPI.ScreenHeight();
			int xpos = (int) (screenWidth * 0.45);
			int ypos = (int) (screenHeight * 0.1);
			int col = 0;
			int row = 0;

			guiApi.WithSurface(DisplaySurfaceID.Client, () =>
			{
				guiApi.ClearGraphics();
				if (messageQueue.Count > 0)
				{
					while (TextRenderer.MeasureText(messageQueue[0], new Font("Arial", fontSize)).Width > (scaledTextbox.Width - (20 * scale)))
					{
						fontSize--;
					}
					DrawMessage(messageQueue[0], scale, scaledTextbox, xpos, ypos, fontSize);
				}
				else
				{
					DrawQueue(scale, scaledIconSkull, scaledIconFairy, scaledIconEye, xpos, ypos, ref col, ref row);
				}

				xpos = pixelPro ? (int) (screenWidth * 0.17) : (int) (screenWidth * 0.05);
				ypos = (int) (screenHeight * 0.15);
				row = 0;
				fontSize = 8 * scale;
				DrawTimers(scale, fontSize, scaledIconSkull, scaledIconFairy, scaledIconEye, xpos, ypos, ref row);
			});
		}

		private void DrawMessage(string message, int scale, Image scaledTextbox, int xpos, int ypos, int fontSize)
		{
			guiApi.DrawImage(scaledTextbox, xpos, ypos, scaledTextbox.Width, scaledTextbox.Height, true);
			guiApi.DrawString(xpos + (int) (scaledTextbox.Width / 2), ypos + (11 * scale), message, Color.White, null, fontSize, "Arial", "bold", "center", "center");
		}

		private void DrawQueue(int scale, Image scaledIconSkull, Image scaledIconFairy, Image scaledIconEye, int xpos, int ypos, ref int col, ref int row)
		{
			foreach (var action in actionQueue)
			{
				if (col > 8)
				{
					col = 0;
					row++;
				}
				switch (action)
				{
					case ActionType.Khaotic:
						guiApi.DrawImage(scaledIconEye, xpos + (col * scaledIconEye.Width) + (1 * scale), ypos + (row * scaledIconEye.Height) + (1 * scale), scaledIconEye.Width, scaledIconEye.Height, true);
						break;
					case ActionType.Debuff:
						guiApi.DrawImage(scaledIconSkull, xpos + (col * scaledIconSkull.Width) + (1 * scale), ypos + (row * scaledIconSkull.Height) + (1 * scale), scaledIconSkull.Width, scaledIconSkull.Height, true);
						break;
					case ActionType.Buff:
						guiApi.DrawImage(scaledIconFairy, xpos + (col * scaledIconFairy.Width) + (1 * scale), ypos + (row * scaledIconFairy.Height) + (1 * scale), scaledIconFairy.Width, scaledIconFairy.Height, true);
						break;
					default:
						break;
				}
				col++;
			}
		}

		private void DrawTimers(int scale, int fontSize, Image scaledIconSkull, Image scaledIconFairy, Image scaledIconEye, int xpos, int ypos, ref int row)
		{
			foreach (var timer in actionTimers)
			{
				row++;
				switch (timer.Type)
				{
					case ActionType.Khaotic:
						guiApi.DrawImage(scaledIconEye, xpos + (1 * scale), ypos + (row * scaledIconEye.Height) + (1 * scale), scaledIconEye.Width, scaledIconEye.Height, true);
						break;
					case ActionType.Debuff:
						guiApi.DrawImage(scaledIconSkull, xpos + (1 * scale), ypos + (row * scaledIconSkull.Height) + (1 * scale), scaledIconSkull.Width, scaledIconSkull.Height, true);
						break;
					case ActionType.Buff:
						guiApi.DrawImage(scaledIconFairy, xpos + (1 * scale), ypos + (row * scaledIconFairy.Height) + (1 * scale), scaledIconFairy.Width, scaledIconFairy.Height, true);
						break;
					default:
						break;
				}
				guiApi.DrawString(xpos + (scaledIconEye.Width) + (1 * scale), ypos + (row * scaledIconEye.Height) + (4 * scale), timer.Name + " " + timer.Duration.ToString(@"mm\:ss"), Color.White, null, fontSize, "Arial", "bold");
			}
		}

		private void DequeueMessage(object sender, ElapsedEventArgs e)
		{
			if (messageQueue.Count > 0)
			{
				messageQueue.RemoveAt(0);
			}
		}

		private void DecrementTimers(object sender, ElapsedEventArgs e)
		{
			foreach (var timer in actionTimers)
			{
				timer.Duration -= TimeSpan.FromSeconds(1);
				if (timer.Duration.TotalSeconds < 1)
				{
					actionTimers.Remove(timer);
				}
			}
			DrawUI();
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
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using BizHawk.Client.Common;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	public class NotificationService : INotificationService
	{
		const int NotificationTime = 5 * 1000;

		private readonly IGuiApi guiApi;
		private readonly IToolConfig toolConfig;

		private System.Timers.Timer messageTimer;
		private int screenWidth = 0;
		private int screenHeight = 0;
		private Image textbox;
		private WMPLib.WindowsMediaPlayer audioPlayer = new WMPLib.WindowsMediaPlayer();

		public NotificationService(IToolConfig toolConfig, IGuiApi guiApi, int screenWidth, int screenHeight)
		{
			if (guiApi is null) throw new ArgumentNullException(nameof(guiApi));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.guiApi = guiApi;
			this.toolConfig = toolConfig;
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;

			messageTimer = new System.Timers.Timer();
			messageTimer.Interval = NotificationTime;
			messageTimer.Elapsed += ClearMessages;
			textbox = Image.FromFile(Paths.TextboxImage);
			audioPlayer.settings.volume = toolConfig.Khaos.Volume * 10;
		}

		private void ClearMessages(object sender, ElapsedEventArgs e)
		{
			guiApi.WithSurface(DisplaySurfaceID.Client, () =>
			{
				guiApi.ClearGraphics();
			});
			messageTimer.Stop();
		}

		public void DisplayMessage(string message)
		{
			int fontSize = 22;
			while (TextRenderer.MeasureText(message, new Font("Arial", fontSize)).Width > 428)
			{
				fontSize--;
			}

			guiApi.WithSurface(DisplaySurfaceID.Client, () =>
			{
				guiApi.ClearGraphics();
				guiApi.DrawRectangle((int) (screenWidth * 0.6) - 215, (int) (screenHeight * 0.1) + 6, 430, 30, Color.FromArgb(100, Color.Blue), Color.FromArgb(100, Color.Blue));
				guiApi.DrawString((int) (screenWidth * 0.6), (int) (screenHeight * 0.1) + 22, message, Color.White, null, fontSize, "Arial", "bold", "center", "center");
				guiApi.DrawImage(textbox, (int) (screenWidth * 0.6) - 220, (int) (screenHeight * 0.1));
			});

			messageTimer.Start();
		}

		public void PlayAlert(string url)
		{
			if (url == String.Empty || !toolConfig.Khaos.Alerts)
			{
				return;
			}
			audioPlayer.URL = url;
			audioPlayer.controls.play();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using BizHawk.Client.Common;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	public class NotificationService : INotificationService
	{
		private Timer messageTimer;
		private readonly IGuiApi guiApi;
		private int screenWidth = 0;
		private int screenHeight = 0;
		private Image textbox;

		public NotificationService(IGuiApi guiApi, int screenWidth, int screenHeight)
		{
			if (guiApi is null) throw new ArgumentNullException(nameof(guiApi));
			this.guiApi = guiApi;
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;

			messageTimer = new Timer();
			messageTimer.Interval = 4 * 1000;
			messageTimer.Elapsed += ClearMessages;
			textbox = Image.FromFile(Paths.TextboxImage);
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
			Console.WriteLine(screenWidth);
			guiApi.WithSurface(DisplaySurfaceID.Client, () =>
			{
				guiApi.ClearGraphics();
				guiApi.DrawRectangle((int) (screenWidth * 0.6) - 215, (int) (screenHeight * 0.1) + 6, 430, 30, Color.FromArgb(100, Color.Blue), Color.FromArgb(100, Color.Blue));
				guiApi.DrawString((int) (screenWidth * 0.6), (int) (screenHeight * 0.1) + 22, message, Color.White, null, 22, "Arial", "bold", "center", "center");
				guiApi.DrawImage(textbox, (int) (screenWidth * 0.6) - 220, (int) (screenHeight * 0.1));
			});

			messageTimer.Start();
		}
	}
}

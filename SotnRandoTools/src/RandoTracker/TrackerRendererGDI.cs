using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.RandoTracker
{
	internal sealed class TrackerRendererGDI : ITrackerRenderer
	{
		private const int TextPadding = 5;
		private const int LabelOffset = 50;
		private const int ImageSize = 14;
		private const int CellPadding = 2;
		private const int Columns = 8;
		private const int SeedInfoFontSize = 19;
		private const int CellSize = ImageSize + CellPadding;
		private const double PixelPerfectSnapMargin = 0.18;
		private readonly Color DefaultBackground = Color.FromArgb(17, 00, 17);
		private readonly SolidBrush textBrush = new SolidBrush(Color.White);
		private Font infoFont = new Font("Tahoma", SeedInfoFontSize);

		private IGraphics formGraphics;
		private readonly IToolConfig toolConfig;

		private TrackerRelic[]? relics;
		private Item[]? progressionItems;
		private Item[]? thrustSwords;

		private Bitmap texture;
		private Bitmap greyscaleTexture;

		private List<Rectangle> relicSlots = new List<Rectangle>();
		private List<Rectangle> vladRelicSlots = new List<Rectangle>();
		private List<Rectangle> progressionItemSlots = new List<Rectangle>();

		private bool initialized = false;
		private float scale = 1;
		private int progressionRelics = 0;
		private bool vladProgression = true;

		public TrackerRendererGDI(IGraphics formGraphics, IToolConfig toolConfig)
		{
			this.formGraphics = formGraphics ?? throw new ArgumentNullException(nameof(formGraphics));
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
		}

		public bool Refreshed { get; set; }
		public string SeedInfo { get; set; }

		public void InitializeItems(TrackerRelic[] relics, Item[] progressionItems, Item[] thrustSwords)
		{
			if (relics is null) throw new ArgumentNullException(nameof(relics));
			if (progressionItems is null) throw new ArgumentNullException(nameof(progressionItems));
			if (thrustSwords is null) throw new ArgumentNullException(nameof(thrustSwords));
			this.relics = relics;
			this.progressionItems = progressionItems;
			this.thrustSwords = thrustSwords;

			foreach (var relic in relics)
			{
				if (relic.Progression)
				{
					progressionRelics++;
				}
			}

			vladProgression = relics[25].Progression;

			LoadImages();

			initialized = true;
		}

		public void SetProgression()
		{
			progressionRelics = 0;

			foreach (var relic in relics)
			{
				if (relic.Progression)
				{
					progressionRelics++;
				}
			}
			vladProgression = relics[25].Progression;
		}

		public void ChangeGraphics(IGraphics formGraphics)
		{
			if (formGraphics is null) throw new ArgumentNullException(nameof(formGraphics));
			this.formGraphics = formGraphics;
		}

		public void CalculateGrid(int width, int height)
		{
			int adjustedColumns = (int) (Columns * (((float) width / (float) height)));
			if (adjustedColumns < 5)
			{
				adjustedColumns = 5;
			}

			int relicCount = 25;
			if (toolConfig.Tracker.ProgressionRelicsOnly)
			{
				relicCount = progressionRelics - 5;
			}

			int normalRelicRows = (int) Math.Ceiling((float) (relicCount + 1) / (float) adjustedColumns);

			float cellsPerColumn = (float) (height - (LabelOffset * 2)) / ((CellSize * (2 + normalRelicRows)));
			float cellsPerRow = (float) (width - (CellPadding * 5)) / ((CellSize * adjustedColumns));
			scale = cellsPerColumn <= cellsPerRow ? cellsPerColumn : cellsPerRow;

			double roundedScale = Math.Floor(scale);

			if (scale - roundedScale < PixelPerfectSnapMargin)
			{
				scale = (float) roundedScale;
			}

			relicSlots = new List<Rectangle>();
			vladRelicSlots = new List<Rectangle>();
			progressionItemSlots = new List<Rectangle>();


			int row = 0;
			int col = 0;

			for (int i = 0; i < relicCount + 1; i++)
			{
				if (col == adjustedColumns)
				{
					row++;
					col = 0;
				}
				relicSlots.Add(new Rectangle((int) (CellPadding + (col * (ImageSize + CellPadding) * scale)), LabelOffset + (int) (row * (ImageSize + CellPadding) * scale), (int) (ImageSize * scale), (int) (ImageSize * scale)));
				col++;
			}

			if (vladProgression)
			{
				row++;
				col = 0;
				for (int i = 0; i < 6; i++)
				{
					vladRelicSlots.Add(new Rectangle((int) (CellPadding + (col * (ImageSize + CellPadding) * scale)), LabelOffset + (int) (row * (ImageSize + CellPadding) * scale), (int) (ImageSize * scale), (int) (ImageSize * scale)));
					col++;
				}
			}

			row++;
			col = 0;
			for (int i = 0; i < 5; i++)
			{
				progressionItemSlots.Add(new Rectangle((int) (CellPadding + (col * (ImageSize + CellPadding) * scale)), LabelOffset + (int) (row * (ImageSize + CellPadding) * scale), (int) (ImageSize * scale), (int) (ImageSize * scale)));
				col++;
			}
			ResizeInfo();
		}

		public void Render()
		{
			if (!initialized)
			{
				return;
			}

			if (toolConfig.Tracker.GridLayout)
			{
				GridRender();
			}
			else
			{
				CollectedRender();
			}
			formGraphics.DrawString(SeedInfo, infoFont, textBrush, TextPadding, TextPadding);

			Refreshed = true;
		}

		private void ResizeInfo()
		{
			int fontSize = SeedInfoFontSize;
			infoFont = new Font("Tahoma", fontSize);
			while (formGraphics.MeasureString(SeedInfo, infoFont).Width > (toolConfig.Tracker.Width - (TextPadding * 3)))
			{
				fontSize--;
				infoFont = new Font("Tahoma", fontSize);
			}
		}

		private void GridRender()
		{
			formGraphics.Clear(DefaultBackground);
			int normalRelicCount = 0;
			for (int i = 0; i < 25; i++)
			{
				if (toolConfig.Tracker.ProgressionRelicsOnly && !relics[i].Progression)
				{
					continue;
				}
				formGraphics.DrawImage(relics[i].Collected ? texture : greyscaleTexture, relicSlots[normalRelicCount], 20 * i, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
				normalRelicCount++;
			}
			Item? thrustSword = null;
			for (int i = 0; i < thrustSwords.Length; i++)
			{
				if (thrustSwords[i].Status)
				{
					thrustSword = thrustSwords[i];
					break;
				}
			}
			formGraphics.DrawImage(thrustSword is not null ? texture : greyscaleTexture, relicSlots[normalRelicCount], 680, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);

			if (vladProgression)
			{
				for (int i = 25; i < relics.Length; i++)
				{
					formGraphics.DrawImage(relics[i].Collected ? texture : greyscaleTexture, vladRelicSlots[i - 25], 20 * i, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
				}
			}

			for (int i = 0; i < progressionItems.Length; i++)
			{
				formGraphics.DrawImage(progressionItems[i].Status ? texture : greyscaleTexture, progressionItemSlots[i], 600 + 20 * i, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
			}
		}

		private void CollectedRender()
		{
			formGraphics.Clear(DefaultBackground);
			int normalRelicCount = 0;
			for (int i = 0; i < 25; i++)
			{
				if (relics[i].Collected && ((toolConfig.Tracker.ProgressionRelicsOnly && relics[i].Progression) || !toolConfig.Tracker.ProgressionRelicsOnly))
				{
					formGraphics.DrawImage(texture, relicSlots[normalRelicCount], 20 * i, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
					normalRelicCount++;
				}
			}
			var thrustSword = thrustSwords.Where(x => x.Status).FirstOrDefault();
			if (thrustSword != null)
			{
				formGraphics.DrawImage(texture, relicSlots[normalRelicCount], 680, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
			}

			if (vladProgression)
			{
				int vladRelicCount = 0;
				for (int i = 25; i < relics.Length; i++)
				{
					if (relics[i].Collected)
					{
						formGraphics.DrawImage(texture, vladRelicSlots[vladRelicCount], 20 * i, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
						vladRelicCount++;
					}
				}
			}

			int progressionItemCount = 0;
			for (int i = 0; i < progressionItems.Length; i++)
			{
				if (progressionItems[i].Status)
				{
					formGraphics.DrawImage(texture, progressionItemSlots[progressionItemCount], 600 + 20 * i, 0, ImageSize, ImageSize, GraphicsUnit.Pixel);
					progressionItemCount++;
				}
			}
		}

		private void LoadImages()
		{
			texture = new Bitmap(Paths.CombinedTexture);
			greyscaleTexture = new Bitmap(Paths.CombinedTexture);
			for (int i = 0; i < greyscaleTexture.Width; i++)
			{
				for (int j = 0; j < greyscaleTexture.Height; j++)
				{
					Color pixelColor = greyscaleTexture.GetPixel(i, j);
					int grayScale = (int) ((pixelColor.R * 0.1) + (pixelColor.G * 0.3) + (pixelColor.B * 0.1));
					Color adjusted = Color.FromArgb(pixelColor.A, grayScale, grayScale, grayScale);
					greyscaleTexture.SetPixel(i, j, adjusted);
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BizHawk.Bizware.BizwareGL;
using BizHawk.Bizware.Graphics.Controls;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.RandoTracker
{
	internal sealed class TrackerRendererOpenGL : ITrackerRenderer
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
		private int Width = 100;
		private int Height = 100;

		private readonly IToolConfig toolConfig;
		private readonly IGuiRenderer guiRenderer;
		private readonly GraphicsControl graphicsControl;

		private List<TrackerRelic>? relics;
		private List<Item>? progressionItems;
		private List<Item>? thrustSwords;

		private Texture2d texture;
		private Texture2d greyscaleTexture;
		private Texture2d infoTexture;
		private Bitmap seedInfoBitmap;
		private readonly SolidBrush textBrush = new SolidBrush(Color.White);

		private List<Rectangle> relicSlots = new List<Rectangle>();
		private List<Rectangle> vladRelicSlots = new List<Rectangle>();
		private List<Rectangle> progressionItemSlots = new List<Rectangle>();

		private bool initialized = false;
		private float scale = 1;
		private int progressionRelics = 0;
		private bool vladProgression = true;
		private string seedInfo = "seed(preset)";

		public TrackerRendererOpenGL(IToolConfig toolConfig, IGuiRenderer guiRenderer, GraphicsControl graphicsControl)
		{
			this.toolConfig = toolConfig ?? throw new ArgumentNullException(nameof(toolConfig));
			this.guiRenderer = guiRenderer ?? throw new ArgumentNullException(nameof(guiRenderer));
			this.graphicsControl = graphicsControl ?? throw new ArgumentNullException(nameof(graphicsControl));
			LoadImages();
		}

		public bool Refreshed { get; set; }
		public string SeedInfo
		{
			get
			{
				return seedInfo;
			}
			set
			{
				seedInfo = value;
				GetSeedInfo();
			}
		}

		public void InitializeItems(List<TrackerRelic> relics, List<Item> progressionItems, List<Item> thrustSwords)
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

		public void CalculateGrid(int width, int height)
		{
			this.Width = width;
			this.Height = height;

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
			GetSeedInfo();
		}

		public void Render()
		{
			if (!initialized)
			{
				return;
			}

			graphicsControl.Begin();
			guiRenderer.Begin(Width, Height);
			guiRenderer.EnableBlending();
			guiRenderer.Owner.ClearColor(DefaultBackground);
			if (toolConfig.Tracker.GridLayout)
			{
				GridRender();
			}
			else
			{
				CollectedRender();
			}
			GetSeedInfo();
			guiRenderer.Draw(infoTexture, 0, 0);
			guiRenderer.End();
			graphicsControl.SwapBuffers();
		}

		private void GetSeedInfo()
		{
			seedInfoBitmap = new Bitmap(toolConfig.Tracker.Width - (TextPadding * 3), LabelOffset);
			Graphics graphics = Graphics.FromImage(seedInfoBitmap);
			int fontSize = SeedInfoFontSize;
			Font infoFont = new Font("Tahoma", fontSize);
			while (graphics.MeasureString(SeedInfo, infoFont).Width > (toolConfig.Tracker.Width - (TextPadding * 3)))
			{
				fontSize--;
				infoFont = new Font("Tahoma", fontSize);
			}
			graphics.DrawString(SeedInfo, infoFont, textBrush, TextPadding, TextPadding);
			infoTexture = guiRenderer.Owner.LoadTexture(seedInfoBitmap);
		}

		private void GridRender()
		{
			int normalRelicCount = 0;
			int slotWidth = relicSlots[0].Width;
			int slotHeight = relicSlots[0].Height;
			float offset = 14 / texture.Width;
			float itemsOffset = 600 / texture.Width;
			for (int i = 0; i < 25; i++)
			{
				if (toolConfig.Tracker.ProgressionRelicsOnly && !relics[i].Progression)
				{
					continue;
				}
				guiRenderer.DrawSubrect(relics[i].Collected ? texture : greyscaleTexture, relicSlots[normalRelicCount].Location.X, relicSlots[normalRelicCount].Location.Y, slotWidth, slotHeight, 20 * i / texture.Width, 0, (20 * i / texture.Width) + offset, 1);
				normalRelicCount++;
			}
			var thrustSword = thrustSwords.Where(x => x.Status).FirstOrDefault();
			guiRenderer.DrawSubrect(thrustSword is not null ? texture : greyscaleTexture, relicSlots[normalRelicCount].Location.X, relicSlots[normalRelicCount].Location.Y, slotWidth, slotHeight, 680 / texture.Width, 0, 694 / texture.Width, 1);


			if (vladProgression)
			{
				for (int i = 25; i < relics.Count; i++)
				{
					guiRenderer.DrawSubrect(relics[i].Collected ? texture : greyscaleTexture, vladRelicSlots[i - 25].Location.X, vladRelicSlots[i - 25].Location.Y, slotWidth, slotHeight, 20 * i / texture.Width, 0, (20 * i / texture.Width) + offset, 1);
				}
			}

			for (int i = 0; i < progressionItems.Count; i++)
			{
				guiRenderer.DrawSubrect(progressionItems[i].Status ? texture : greyscaleTexture, progressionItemSlots[i].Location.X, progressionItemSlots[i].Location.Y, slotWidth, slotHeight, (20 * i / texture.Width) + itemsOffset, 0, (20 * i / texture.Width) + itemsOffset + offset, 1);
			}
		}

		private void CollectedRender()
		{
			int normalRelicCount = 0;
			int slotWidth = relicSlots[0].Width;
			int slotHeight = relicSlots[0].Height;
			float offset = 14 / texture.Width;
			float itemsOffset = 600 / texture.Width;
			for (int i = 0; i < 25; i++)
			{
				if (relics[i].Collected && ((toolConfig.Tracker.ProgressionRelicsOnly && relics[i].Progression) || !toolConfig.Tracker.ProgressionRelicsOnly))
				{
					guiRenderer.DrawSubrect(texture, relicSlots[normalRelicCount].Location.X, relicSlots[normalRelicCount].Location.Y, slotWidth, slotHeight, 20 * i / texture.Width, 0, (20 * i / texture.Width) + offset, 1);
					normalRelicCount++;
				}
			}
			var thrustSword = thrustSwords.Where(x => x.Status).FirstOrDefault();
			if (thrustSword != null)
			{
				guiRenderer.DrawSubrect(texture, relicSlots[normalRelicCount].Location.X, relicSlots[normalRelicCount].Location.Y, slotWidth, slotHeight, 680 / texture.Width, 0, 694 / texture.Width, 1);
			}

			if (vladProgression)
			{
				int vladRelicCount = 0;
				for (int i = 25; i < relics.Count; i++)
				{
					if (relics[i].Collected)
					{
						guiRenderer.DrawSubrect(texture, vladRelicSlots[vladRelicCount].Location.X, vladRelicSlots[vladRelicCount].Location.Y, slotWidth, slotHeight, 20 * i / texture.Width, 0, (20 * i / texture.Width) + offset, 1);
						vladRelicCount++;
					}
				}
			}

			int progressionItemCount = 0;
			for (int i = 0; i < progressionItems.Count; i++)
			{
				if (progressionItems[i].Status)
				{
					guiRenderer.DrawSubrect(texture, progressionItemSlots[progressionItemCount].Location.X, progressionItemSlots[progressionItemCount].Location.Y, slotWidth, slotHeight, (20 * i / texture.Width) + itemsOffset, 0, (20 * i / texture.Width) + itemsOffset + offset, 1);
					progressionItemCount++;
				}
			}
		}

		private void LoadImages()
		{
			Bitmap textureBitmap = new Bitmap(Paths.CombinedTexture);
			Bitmap greyscaleTextureBitmap = new Bitmap(Paths.CombinedTexture);
			for (int i = 0; i < greyscaleTextureBitmap.Width; i++)
			{
				for (int j = 0; j < greyscaleTextureBitmap.Height; j++)
				{
					Color pixelColor = greyscaleTextureBitmap.GetPixel(i, j);
					int grayScale = (int) ((pixelColor.R * 0.1) + (pixelColor.G * 0.3) + (pixelColor.B * 0.1));
					Color adjusted = Color.FromArgb(pixelColor.A, grayScale, grayScale, grayScale);
					greyscaleTextureBitmap.SetPixel(i, j, adjusted);
				}
			}

			texture = guiRenderer.Owner.LoadTexture(textureBitmap);
			greyscaleTexture = guiRenderer.Owner.LoadTexture(greyscaleTextureBitmap);
		}

		public void ChangeGraphics(IGraphics formGraphics)
		{
			return;
		}
	}
}

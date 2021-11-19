using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.RandoTracker.Interfaces;
using SotnRandoTools.RandoTracker.Models;

namespace SotnRandoTools.RandoTracker
{
	public class TrackerGraphicsEngine : ITrackerGraphicsEngine
	{
		private const int TextPadding = 5;
		private const int LabelOffset = 40;
		private const int ImageSize = 14;
		private const int CellPadding = 1;
		private const int Columns = 8;
		private const int CellSize = ImageSize + CellPadding;
		private Color DefaultBackground = Color.FromArgb(17, 00, 17);

		private IGraphics formGraphics;
		private readonly IToolConfig toolConfig;

		private readonly List<Relic> relics;
		private List<Item> progressionItems;
		private List<Item> thrustSwords;

		private List<Bitmap> relicImages = new List<Bitmap>();
		private List<Bitmap> progressionItemImages = new List<Bitmap>();
		private Bitmap? thrustSwordImage;

		private List<Rectangle> relicSlots = new List<Rectangle>();
		private List<Rectangle> vladRelicSlots = new List<Rectangle>();
		private List<Rectangle> progressionItemSlots = new List<Rectangle>();

		private int scale = 1;
		private int progressionRelics = 0;
		private bool vladProgression = true;
		private ColorMatrix greyscaleColorMatrix = new ColorMatrix(
			new float[][]
			{
				new float[] {.1f, .1f, .1f, 0, 0},
				new float[] {.3f, .3f, .3f, 0, 0},
				new float[] {.1f, .1f, .1f, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {0, 0, 0, 0, 1}
			});

		public TrackerGraphicsEngine(IGraphics formGraphics, List<Relic> relics, List<Item> progressionItems, List<Item> thrustSwords, IToolConfig toolConfig)
		{
			if (formGraphics is null) throw new ArgumentNullException(nameof(formGraphics));
			if (relics is null) throw new ArgumentNullException(nameof(relics));
			if (progressionItems is null) throw new ArgumentNullException(nameof(progressionItems));
			if (thrustSwords is null) throw new ArgumentNullException(nameof(thrustSwords));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.formGraphics = formGraphics;
			this.relics = relics;
			this.progressionItems = progressionItems;
			this.thrustSwords = thrustSwords;
			this.toolConfig = toolConfig;

			foreach (var relic in relics)
			{
				if (relic.Progression)
				{
					progressionRelics++;
				}
			}

			vladProgression = relics[25].Progression;

			LoadImages();
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
			int cellsPerColumn = height / ((CellSize * (2 + normalRelicRows)) + LabelOffset);
			int cellsPerRow = width / ((CellSize * adjustedColumns) + (CellPadding * scale));
			scale = cellsPerColumn <= cellsPerRow ? cellsPerColumn : cellsPerRow;
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
				relicSlots.Add(new Rectangle((CellPadding * scale) + (col * (ImageSize + CellPadding) * scale), LabelOffset + (row * (ImageSize + CellPadding) * scale), ImageSize * scale, ImageSize * scale));
				col++;
			}

			if (vladProgression)
			{
				row++;
				col = 0;
				for (int i = 0; i < 6; i++)
				{
					vladRelicSlots.Add(new Rectangle((CellPadding * scale) + (col * (ImageSize + CellPadding) * scale), LabelOffset + (row * (ImageSize + CellPadding) * scale), ImageSize * scale, ImageSize * scale));
					col++;
				}
			}

			row++;
			col = 0;
			for (int i = 0; i < 5; i++)
			{
				progressionItemSlots.Add(new Rectangle((CellPadding * scale) + (col * ((ImageSize + CellPadding) * scale)), LabelOffset + (row * ((ImageSize + CellPadding) * scale)), ImageSize * scale, ImageSize * scale));
				col++;
			}
		}

		public void DrawSeedInfo(string seedInfo)
		{
			if (seedInfo is null) throw new ArgumentNullException(nameof(seedInfo));
			if (seedInfo == String.Empty) throw new ArgumentException("Parameter seedInfo is empty!");

			int fontSize = 16;
			while (formGraphics.MeasureString(seedInfo, new Font("Tahoma", fontSize)).Width > (toolConfig.Tracker.Width - (TextPadding * 3)))
			{
				fontSize--;
			}

			formGraphics.DrawString(seedInfo, new Font("Tahoma", fontSize), new SolidBrush(Color.White), TextPadding, TextPadding);
		}

		public void Render()
		{
			if (toolConfig.Tracker.GridLayout)
			{
				GridRender();
			}
			else
			{
				CollectedRender();
			}
		}

		private void GridRender()
		{
			formGraphics.Clear(DefaultBackground);
			ImageAttributes greyscaleAttributes = new ImageAttributes();
			greyscaleAttributes.SetColorMatrix(greyscaleColorMatrix);

			int normalRelicCount = 0;
			for (int i = 0; i < 25; i++)
			{
				if (relics[i].Status && ((toolConfig.Tracker.ProgressionRelicsOnly && relics[i].Progression) || !toolConfig.Tracker.ProgressionRelicsOnly))
				{
					formGraphics.DrawImage(relicImages[i], relicSlots[normalRelicCount], 0, 0, relicImages[i].Width, relicImages[i].Height, GraphicsUnit.Pixel);
					normalRelicCount++;
				}
				else if ((toolConfig.Tracker.ProgressionRelicsOnly && relics[i].Progression) || !toolConfig.Tracker.ProgressionRelicsOnly)
				{
					formGraphics.DrawImage(relicImages[i], relicSlots[normalRelicCount], 0, 0, relicImages[i].Width, relicImages[i].Height, GraphicsUnit.Pixel, greyscaleAttributes);
					normalRelicCount++;
				}
			}
			var thrustSword = thrustSwords.Where(x => x.Status).FirstOrDefault();
			if (thrustSword != null && thrustSwordImage != null)
			{
				formGraphics.DrawImage(thrustSwordImage, relicSlots[normalRelicCount], 0, 0, thrustSwordImage.Width, thrustSwordImage.Height, GraphicsUnit.Pixel);
			}
			else if (thrustSwordImage != null)
			{
				formGraphics.DrawImage(thrustSwordImage, relicSlots[normalRelicCount], 0, 0, thrustSwordImage.Width, thrustSwordImage.Height, GraphicsUnit.Pixel, greyscaleAttributes);
			}

			if (vladProgression)
			{
				for (int i = 25; i < relics.Count; i++)
				{
					if (relics[i].Status)
					{
						formGraphics.DrawImage(relicImages[i], vladRelicSlots[i - 25], 0, 0, relicImages[i].Width, relicImages[i].Height, GraphicsUnit.Pixel);
					}
					else
					{
						formGraphics.DrawImage(relicImages[i], vladRelicSlots[i - 25], 0, 0, relicImages[i].Width, relicImages[i].Height, GraphicsUnit.Pixel, greyscaleAttributes);
					}
				}
			}

			for (int i = 0; i < progressionItems.Count; i++)
			{
				if (progressionItems[i].Status)
				{
					formGraphics.DrawImage(progressionItemImages[i], progressionItemSlots[i], 0, 0, progressionItemImages[i].Width, progressionItemImages[i].Height, GraphicsUnit.Pixel);
				}
				else
				{
					formGraphics.DrawImage(progressionItemImages[i], progressionItemSlots[i], 0, 0, progressionItemImages[i].Width, progressionItemImages[i].Height, GraphicsUnit.Pixel, greyscaleAttributes);
				}
			}
		}

		private void CollectedRender()
		{
			formGraphics.Clear(DefaultBackground);

			int normalRelicCount = 0;
			for (int i = 0; i < 25; i++)
			{
				if (relics[i].Status && ((toolConfig.Tracker.ProgressionRelicsOnly && relics[i].Progression) || !toolConfig.Tracker.ProgressionRelicsOnly))
				{
					formGraphics.DrawImage(relicImages[i], relicSlots[normalRelicCount], 0, 0, relicImages[i].Width, relicImages[i].Height, GraphicsUnit.Pixel);
					normalRelicCount++;
				}
			}
			var thrustSword = thrustSwords.Where(x => x.Status).FirstOrDefault();
			if (thrustSword != null && thrustSwordImage != null)
			{
				formGraphics.DrawImage(thrustSwordImage, relicSlots[normalRelicCount], 0, 0, thrustSwordImage.Width, thrustSwordImage.Height, GraphicsUnit.Pixel);
			}

			if (vladProgression)
			{
				int vladRelicCount = 0;
				for (int i = 25; i < relics.Count; i++)
				{
					if (relics[i].Status)
					{
						formGraphics.DrawImage(relicImages[i], vladRelicSlots[vladRelicCount], 0, 0, relicImages[i].Width, relicImages[i].Height, GraphicsUnit.Pixel);
						vladRelicCount++;
					}
				}
			}

			int progressionItemCount = 0;
			for (int i = 0; i < progressionItems.Count; i++)
			{
				if (progressionItems[i].Status)
				{
					formGraphics.DrawImage(progressionItemImages[i], progressionItemSlots[progressionItemCount], 0, 0, progressionItemImages[i].Width, progressionItemImages[i].Height, GraphicsUnit.Pixel);
					progressionItemCount++;
				}
			}
		}

		private void LoadImages()
		{
			foreach (var relic in relics)
			{
				relicImages.Add(new Bitmap(Image.FromFile(Paths.ImagesPath + relic.Name + ".png")));
			}

			foreach (var item in progressionItems)
			{
				progressionItemImages.Add(new Bitmap(Image.FromFile(Paths.ImagesPath + item.Name + ".png")));
			}

			thrustSwordImage = new Bitmap(Image.FromFile(Paths.ImagesPath + "Claymore.png"));
		}

	}
}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SotnRandoTools.RandoTracker.Interfaces;

namespace SotnRandoTools.RandoTracker.Adapters
{
	public class GraphicsAdapter : IGraphics
	{
		private readonly Graphics g;
		public GraphicsAdapter(Graphics g)
		{
			if (g is null) throw new ArgumentNullException(nameof(g));
			this.g = g;
			g.PixelOffsetMode = PixelOffsetMode.Half;
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
		}

		public void Clear(Color color)
		{
			g.Clear(color);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes attributes)
		{
			g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, attributes);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
		{
			g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
		}

		public void DrawString(string s, Font font, Brush brush, float x, float y)
		{
			g.DrawString(s, font, brush, x, y);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return g.MeasureString(text, font);
		}
	}
}

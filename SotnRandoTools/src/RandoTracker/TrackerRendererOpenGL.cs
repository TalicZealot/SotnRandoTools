using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;

namespace SotnRandoTools.RandoTracker
{
	internal sealed class Text : IDisposable
	{
		private const int TextPadding = 5;
		private int vertexBuffer;
		private int vertexArrayObject;
		private int textIndexBuffer;
		private int windowWidth = 0;
		private int windowHeight = 0;
		private float scale = 0;
		private int collectedUniform;
		private const int glyphWidth = 6;
		private const int glyphHeight = 8;
		private const float textureItemWidth = 6f / 540f;
		private List<float> vertices = new();
		private uint[] indices;
		public string text;
		public Text(string text, int windowWidth, int windowHeight, int collectedUniform)
		{
			this.collectedUniform = collectedUniform;
			this.text = text;
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
			float rawScale = (float) (windowWidth - (TextPadding * 2)) / (float) ((text.Length) * (glyphWidth + 1));
			scale = (float) Math.Floor((double) rawScale);

			if (scale < 2)
			{
				scale = rawScale;
			}

			if (scale > 4)
			{
				scale = 4;
			}

			indices = new uint[text.Length * 6];

			int ind = 0;
			for (int i = 0; i < text.Length; i++)
			{
				indices[ind++] = (uint) (0 + (i * 4));
				indices[ind++] = (uint) (1 + (i * 4));
				indices[ind++] = (uint) (2 + (i * 4));
				indices[ind++] = (uint) (2 + (i * 4));
				indices[ind++] = (uint) (3 + (i * 4));
				indices[ind++] = (uint) (0 + (i * 4));
			}

			float xpos = TextPadding;
			float ypos = windowHeight - (TextPadding + (glyphHeight * scale));
			for (int i = 0; i < text.Length; i++)
			{
				float textureCoordsX = ((byte) text[i] - 33) * textureItemWidth;
				if (text[i] < '!' || text[i] > 'z')
				{
					textureCoordsX = ('?' - 33) * textureItemWidth;
				}
				//Vert1
				vertices.Add(xpos);
				vertices.Add(ypos + (glyphHeight * scale));

				vertices.Add(textureCoordsX);
				vertices.Add(1.0f);

				vertices.Add(35.0f);
				//Vert2
				vertices.Add(xpos + (glyphWidth * scale));
				vertices.Add(ypos + (glyphHeight * scale));

				vertices.Add(textureCoordsX + textureItemWidth);
				vertices.Add(1.0f);

				vertices.Add(35.0f);
				//Vert3
				vertices.Add(xpos + (glyphWidth * scale));
				vertices.Add(ypos);

				vertices.Add(textureCoordsX + textureItemWidth);
				vertices.Add(0.0f);

				vertices.Add(35.0f);
				//Vert4
				vertices.Add(xpos);
				vertices.Add(ypos);

				vertices.Add(textureCoordsX);
				vertices.Add(0.0f);

				vertices.Add(35.0f);

				xpos += scale * (glyphWidth + 1);
			}

			vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(vertexArrayObject);

			vertexBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
			GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);

			textIndexBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, textIndexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
		}

		public void Draw()
		{
			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, textIndexBuffer);
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
		}
		public void Dispose()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DeleteBuffer(vertexBuffer);
			GL.BindVertexArray(0);
			GL.DeleteVertexArray(vertexArrayObject);
		}
	}
	internal sealed class Sprites : IDisposable
	{
		private int vertexBuffer;
		private int vertexArrayObject;
		private int indexBuffer;
		private int collectedUniform;
		private const int itemSize = 14;
		private const float textureItemWidth = 1f / 7f;
		private const float textureItemHeight = 0.2f;
		private List<float> vertices = new();
		private uint[] indices;
		private float scale;
		private Vector2[] relicSlots;
		private int columns;

		public Sprites(float scale, Vector2[] relicSlots, Tracker tracker, int columns, bool grid)
		{
			this.scale = scale;
			this.relicSlots = relicSlots;
			this.columns = columns;

			indices = new uint[35 * 6];

			int ind = 0;
			for (int i = 0; i < 35; i++)
			{
				indices[ind++] = (uint) (0 + (i * 4));
				indices[ind++] = (uint) (1 + (i * 4));
				indices[ind++] = (uint) (2 + (i * 4));
				indices[ind++] = (uint) (2 + (i * 4));
				indices[ind++] = (uint) (3 + (i * 4));
				indices[ind++] = (uint) (0 + (i * 4));
			}

			int itemCount = 0;
			for (int i = 0; i < 25; i++)
			{
				if (!grid && !tracker.relics[i].Collected)
				{
					continue;
				}
				AddQuad(itemCount, i);
				itemCount++;
			}
			int remainder = itemCount % columns;
			if (remainder != 0)
			{
				itemCount += columns - remainder;
			}
			for (int i = 0; i < 5; i++)
			{
				if (!grid && !tracker.relics[25 + i].Collected)
				{
					continue;
				}
				AddQuad(itemCount, 25 + i);
				itemCount++;
			}
			remainder = itemCount % columns;
			if (remainder != 0)
			{
				itemCount += columns - remainder;
			}
			for (int i = 0; i < 4; i++)
			{
				if (!grid && !tracker.progressionItems[i].Collected)
				{
					continue;
				}
				AddQuad(itemCount, 30 + i);
				itemCount++;
			}
			bool swordCollected = false;
			for (int i = 0; i < tracker.thrustSwords.Length; i++)
			{
				if (tracker.thrustSwords[i].Collected)
				{
					swordCollected = true;
					break;
				}
			}
			if (grid || swordCollected)
			{
				AddQuad(itemCount, 34);
			}

			if (vertices.Count == 0)
			{
				return;
			}

			vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(vertexArrayObject);

			vertexBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
			GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);

			indexBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
		}
		public void Draw()
		{
			if (vertices.Count == 0)
			{
				return;
			}
			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
		}
		public void Dispose()
		{
			if (vertices.Count == 0)
			{
				return;
			}
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DeleteBuffer(vertexBuffer);
			GL.BindVertexArray(0);
			GL.DeleteVertexArray(vertexArrayObject);
		}
		private void AddQuad(int itemCount, int i)
		{
			int row = i / 7;
			int col = i % 7;
			//Vert1
			vertices.Add(relicSlots[itemCount].X);
			vertices.Add(relicSlots[itemCount].Y + (itemSize * scale));

			vertices.Add(textureItemWidth * col);
			vertices.Add(textureItemHeight * (row + 1));

			vertices.Add(i);
			//Vert2
			vertices.Add(relicSlots[itemCount].X + (itemSize * scale));
			vertices.Add(relicSlots[itemCount].Y + (itemSize * scale));

			vertices.Add(textureItemWidth * (col + 1));
			vertices.Add(textureItemHeight * (row + 1));

			vertices.Add(i);
			//Vert3
			vertices.Add(relicSlots[itemCount].X + (itemSize * scale));
			vertices.Add(relicSlots[itemCount].Y);

			vertices.Add(textureItemWidth * (col + 1));
			vertices.Add(textureItemHeight * row);

			vertices.Add(i);
			//Vert4
			vertices.Add(relicSlots[itemCount].X);
			vertices.Add(relicSlots[itemCount].Y);

			vertices.Add(textureItemWidth * col);
			vertices.Add(textureItemHeight * row);

			vertices.Add(i);
		}
	}
	internal sealed class TrackerRendererOpenGL : GameWindow
	{
		private const int LabelOffset = 37;
		private const int ItemSize = 14;
		private const int CellPadding = 2;
		private const double PixelPerfectSnapMargin = 0.22;

		private readonly Tracker tracker;
		private readonly IToolConfig toolConfig;
		private int shaderProgram;
		private int texture;
		private int font;
		private Color clear = Color.FromArgb(17, 0, 17);
		private int collectedUniform;
		private int gridUniform;
		private int uTextureUniform;
		private Sprites sprites;
		private Text seedInfo;
		private int columns = 5;
		private Vector2[] relicSlots = new Vector2[120];
		private List<Vector2> vladRelicSlots;
		private List<Vector2> progressionItemSlots;
		private float[] collected = new float[36];

		public TrackerRendererOpenGL(IToolConfig toolConfig, Tracker tracker)
			: base(toolConfig.Tracker.Width, toolConfig.Tracker.Height, GraphicsMode.Default, "Autotracker")
		{
			this.toolConfig = toolConfig;
			this.X = toolConfig.Tracker.Location.X;
			this.Y = toolConfig.Tracker.Location.Y;
			CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);
			//this.Icon = new Icon(Paths.BizAlucardIcon);
			this.WindowBorder = OpenTK.WindowBorder.Resizable;
			this.TargetRenderFrequency = 60d;
			this.TargetUpdateFrequency = 60d;
			this.tracker = tracker;

			for (int i = 0; i < collected.Length; i++)
			{
				collected[i] = 0.0f;
			}
			collected[35] = 2.0f;
		}

		public float Scale { get; set; }

		private int LoadTexture(string path)
		{
			int textureId = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureId);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

			using (var image = new Bitmap(path))
			{
				image.RotateFlip(RotateFlipType.Rotate180FlipX);
				var data = image.LockBits(
					new Rectangle(0, 0, image.Width, image.Height),
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
					OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

				image.UnlockBits(data);
			}

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			return textureId;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			GL.ClearColor(clear);

			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, File.ReadAllText(Paths.VertexShader));
			GL.CompileShader(vertexShader);

			unsafe
			{
				int status;
				GL.GetShader(vertexShader, ShaderParameter.CompileStatus, &status);
				Console.WriteLine(status);
			}
			string infoLog = GL.GetShaderInfoLog(vertexShader);
			Console.WriteLine(infoLog);

			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, File.ReadAllText(Paths.FragmentShader));
			GL.CompileShader(fragmentShader);

			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, vertexShader);
			GL.AttachShader(shaderProgram, fragmentShader);
			GL.LinkProgram(shaderProgram);

			unsafe
			{
				int status;
				GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, &status);
				Console.WriteLine(status);
			}
			infoLog = GL.GetProgramInfoLog(shaderProgram);
			Console.WriteLine(infoLog);

			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);

			int[] viewportData = new int[4];
			GL.GetInteger(GetPName.Viewport, viewportData);
			GL.UseProgram(shaderProgram);
			GL.Uniform2(GL.GetUniformLocation(shaderProgram, "viewportSize"), (float) viewportData[2], (float) viewportData[3]);
			collectedUniform = GL.GetUniformLocation(shaderProgram, "collected");
			gridUniform = GL.GetUniformLocation(shaderProgram, "grid");
			uTextureUniform = GL.GetUniformLocation(shaderProgram, "uTexture");
			GL.Uniform1(gridUniform, 1);

			GL.UseProgram(shaderProgram);
			texture = LoadTexture(Paths.CombinedTexture);
			font = LoadTexture(Paths.FontAtlas);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, font);
			int[] textures = new int[2];
			textures[0] = 0;
			textures[1] = 1;
			GL.Uniform1(uTextureUniform, 2, textures);

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.UseProgram(shaderProgram);

			CheckForErrors();
		}

		private void CheckForErrors()
		{
			ErrorCode error = GL.GetError();
			while (error != ErrorCode.NoError)
			{
				Console.WriteLine(error.ToString());
				error = GL.GetError();
			}
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			bool changes = false;
			for (int i = 0; i < tracker.relics.Length; i++)
			{
				if (collected[i] == 0.0f && tracker.relics[i].Collected)
				{
					changes = true;
					collected[i] = 0.1f;
				}
				if (collected[i] != 0.0f && !tracker.relics[i].Collected)
				{
					changes = true;
					collected[i] = 0.0f;
				}
			}
			for (int i = 0; i < tracker.progressionItems.Length; i++)
			{
				if (collected[30 + i] == 0.0f && tracker.progressionItems[i].Collected)
				{
					changes = true;
					collected[30 + i] = 0.1f;
				}
				if (collected[30 + i] != 0.0f && !tracker.progressionItems[i].Collected)
				{
					changes = true;
					collected[30 + i] = 0.0f;
				}
			}
			bool swordCollected = false;
			for (int i = 0; i < tracker.thrustSwords.Length; i++)
			{
				if (tracker.thrustSwords[i].Collected)
				{
					changes = true;
					swordCollected = true;
					break;
				}
			}
			if (collected[34] == 0.0f && swordCollected)
			{
				collected[34] = 0.1f;
			}
			if (collected[34] != 0.0f && !swordCollected)
			{
				collected[34] = 0.0f;
			}

			for (int i = 0; i < collected.Length; i++)
			{
				if (collected[i] > 0.0f && collected[i] < 1.73322f)
				{
					collected[i] += 0.01014f;
				}
			}

			if (seedInfo.text != tracker.SeedInfo)
			{
				seedInfo.Dispose();
				seedInfo = new Text(tracker.SeedInfo, Width, Height, collectedUniform);
			}

			if (changes || !toolConfig.Tracker.GridLayout)
			{
				sprites = new Sprites(Scale, relicSlots, tracker, columns, toolConfig.Tracker.GridLayout);
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Uniform1(collectedUniform, collected.Length, collected);
			sprites.Draw();
			seedInfo.Draw();
			SwapBuffers();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (Width > 1500 || Height > 1500 || Width < 200 || Height < 200)
			{
				Width = toolConfig.Tracker.Width;
				Height = toolConfig.Tracker.Height;
			}

			GL.Viewport(0, 0, Width, Height);

			int[] viewportData = new int[4];
			GL.GetInteger(GetPName.Viewport, viewportData);
			GL.UseProgram(shaderProgram);
			int viewportSizeUniform = GL.GetUniformLocation(shaderProgram, "viewportSize");
			GL.Uniform2(viewportSizeUniform, (float) viewportData[2], (float) viewportData[3]);

			CalculateGrid(Width, Height);
			ClearSprites();
			seedInfo = new Text(tracker.SeedInfo, this.Width, this.Height, collectedUniform);
			sprites = new Sprites(Scale, relicSlots, tracker, columns, toolConfig.Tracker.GridLayout);
		}

		private void ClearSprites()
		{
			if (sprites != null)
			{
				sprites.Dispose();
			}
			if (seedInfo != null)
			{
				seedInfo.Dispose();
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			ClearSprites();

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			GL.UseProgram(0);
			GL.DeleteProgram(shaderProgram);

			GL.DeleteTexture(texture);
			GL.DeleteTexture(font);

			base.OnUnload(e);
		}

		public void CalculateGrid(int width, int height)
		{
			columns = (int) (8 * (((float) width / (float) height)));
			if (columns < 5)
			{
				columns = 5;
			}

			int relicCount = 25;

			int normalRelicRows = (int) Math.Ceiling((float) (relicCount) / (float) columns);

			int cellSize = ItemSize + CellPadding;
			float cellsPerColumn = (float) (height - (LabelOffset + CellPadding)) / ((cellSize * (2 + normalRelicRows)));
			float cellsPerRow = (float) (width - (CellPadding * 5)) / ((cellSize * columns));
			Scale = (float) Math.Round((cellsPerColumn <= cellsPerRow ? cellsPerColumn : cellsPerRow), 2);

			double roundedScale = Math.Floor(Scale);

			if (Scale - roundedScale < PixelPerfectSnapMargin)
			{
				Scale = (float) roundedScale;
			}

			vladRelicSlots = new List<Vector2>();
			progressionItemSlots = new List<Vector2>();


			int row = 0;
			int col = 0;
			int totalCells = columns * (normalRelicRows + 2);

			for (int i = 0; i < totalCells; i++)
			{
				if (col == columns)
				{
					row++;
					col = 0;
				}
				relicSlots[i] = new Vector2(CellPadding + (col * (ItemSize + CellPadding) * Scale), this.Height - (LabelOffset + (ItemSize + CellPadding) * Scale) - (row * (ItemSize + CellPadding) * Scale));
				col++;
			}
		}
	}
}

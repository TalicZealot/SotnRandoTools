using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using BizHawk.Common;
using SDL2;
using Silk.NET.OpenGL;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using static SDL2.SDL;

namespace SotnRandoTools.RandoTracker
{
	internal sealed class Text : IDisposable
	{
		private const int TextPadding = 5;
		private uint vertexBuffer;
		private uint vertexArrayObject;
		private uint textIndexBuffer;
		private int windowWidth = 0;
		private int windowHeight = 0;
		private float scale = 0;
		private int collectedUniform;
		private const int glyphWidth = 6;
		private const int glyphHeight = 8;
		private const float textureItemWidth = 6f / 540f;
		private List<float> vertices = new();
		private float[] verticeArray;
		private uint[] indices;
		public string text;
		private GL Gl;

		public unsafe Text(string text, int windowWidth, int windowHeight, int collectedUniform, GL gl)
		{
			Gl = gl;
			this.collectedUniform = collectedUniform;
			this.text = text;
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
			float rawScale = (float) (windowWidth - (TextPadding * 2)) / (float) ((text.Length) * (glyphWidth + 1));
			scale = (float) Math.Floor((double) rawScale);

			if (rawScale < 1)
			{
				scale = 1;
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

			vertexArrayObject = Gl.GenVertexArray();
			Gl.BindVertexArray(vertexArrayObject);

			vertexBuffer = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vertexBuffer);
			verticeArray = vertices.ToArray();
			fixed (void* buf = verticeArray)
			{
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint) (vertices.Count * sizeof(float)), buf, BufferUsageARB.StaticDraw);
			}

			Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
			Gl.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
			Gl.EnableVertexAttribArray(0);
			Gl.EnableVertexAttribArray(1);
			Gl.EnableVertexAttribArray(2);

			textIndexBuffer = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, textIndexBuffer);
			fixed (void* buf = indices)
			{
				Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint) (indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
			}
		}

		public unsafe void Draw()
		{
			Gl.BindVertexArray(vertexArrayObject);
			Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, textIndexBuffer);
			Gl.DrawElements(PrimitiveType.Triangles, (uint) indices.Length, DrawElementsType.UnsignedInt, (void*) 0);
		}
		public void Dispose()
		{
			Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
			Gl.DeleteBuffer(vertexBuffer);
			Gl.BindVertexArray(0);
			Gl.DeleteVertexArray(vertexArrayObject);
		}
	}
	internal sealed class Sprites : IDisposable
	{
		private uint vertexBuffer;
		private uint vertexArrayObject;
		private uint indexBuffer;
		private int collectedUniform;
		private const int itemSize = 14;
		private const float textureItemWidth = 1f / 7f;
		private const float textureItemHeight = 0.2f;
		private List<float> vertices = new();
		private float[] verticeArray;
		private uint[] indices;
		private float scale;
		private Vector2[] relicSlots;
		private int columns;
		private GL Gl;

		public unsafe Sprites(float scale, Vector2[] relicSlots, Tracker tracker, int columns, bool grid, bool progression, GL gl)
		{
			Gl = gl;
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
				if ((!grid && !tracker.relics[i].Collected) || (progression && !tracker.relics[i].Progression))
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

			vertexArrayObject = Gl.GenVertexArray();
			Gl.BindVertexArray(vertexArrayObject);

			vertexBuffer = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vertexBuffer);
			verticeArray = vertices.ToArray();
			fixed (void* buf = verticeArray)
			{
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint) (vertices.Count * sizeof(float)), buf, BufferUsageARB.StaticDraw);
			}

			Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
			Gl.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
			Gl.EnableVertexAttribArray(0);
			Gl.EnableVertexAttribArray(1);
			Gl.EnableVertexAttribArray(2);

			indexBuffer = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indexBuffer);
			fixed (void* buf = indices)
			{
				Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint) (indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
			}
		}
		public unsafe void Draw()
		{
			if (vertices.Count == 0)
			{
				return;
			}
			Gl.BindVertexArray(vertexArrayObject);
			Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indexBuffer);
			Gl.DrawElements(PrimitiveType.Triangles, (uint) indices.Length, DrawElementsType.UnsignedInt, (void*) 0);
		}
		public void Dispose()
		{
			if (vertices.Count == 0)
			{
				return;
			}
			Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
			Gl.DeleteBuffer(vertexBuffer);
			Gl.BindVertexArray(0);
			Gl.DeleteVertexArray(vertexArrayObject);
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
	internal sealed class TrackerRendererOpenGL : IDisposable
	{
		private const int LabelOffset = 37;
		private const int ItemSize = 14;
		private const int CellPadding = 2;
		private const double PixelPerfectSnapMargin = 0.22;
		private Color clear = Color.FromArgb(17, 0, 17);

		private readonly Tracker tracker;
		private readonly IToolConfig toolConfig;
		private uint shaderProgram;
		private uint texture;
		private uint font;
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
		private GL Gl;
		private IntPtr Glc;
		private IntPtr window;
		private bool closing = false;
		private bool alwaysOnTop = false;

		public unsafe TrackerRendererOpenGL(IToolConfig toolConfig, Tracker tracker)
		{
			this.toolConfig = toolConfig;
			this.tracker = tracker;
			this.X = toolConfig.Tracker.Location.X;
			this.Y = toolConfig.Tracker.Location.Y;
			this.Width = toolConfig.Tracker.Width;
			this.Height = toolConfig.Tracker.Height;

			if (SDL_Init(SDL_INIT_VIDEO) != 0)
			{
				throw new($"SDL failed to init, SDL error: {SDL_GetError()}");
			}

			SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_SHOWN;

			if (toolConfig.Tracker.AlwaysOnTop)
			{
				alwaysOnTop = true;
				flags |= SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP;
			}

			window = SDL_CreateWindow("Autotracker", X, Y, Width, Height, flags);
			if (window == IntPtr.Zero)
			{
				throw new Exception($"Failed to create SDL window: {SDL.SDL_GetError()}");
			}
			SDL_SetWindowMinimumSize(window, 200, 220);
			SDL_SetWindowMaximumSize(window, 1600, 1000);
			Glc = SDL_GL_CreateContext(window);
			if (Glc == IntPtr.Zero)
			{
				throw new($"Could not create GL Context! SDL Error: {SDL_GetError()}");
			}
			if (SDL_GL_MakeCurrent(window, Glc) != 0)
			{
				throw new($"Failed to set context to current! SDL error: {SDL_GetError()}");
			}
			Gl = GL.GetApi(SDL_GL_GetProcAddress);

			SDL_SetEventFilter((IntPtr userdata, IntPtr e) =>
			{
				if (((SDL_Event*) e)->type is SDL_EventType.SDL_WINDOWEVENT or SDL_EventType.SDL_JOYDEVICEADDED or SDL_EventType.SDL_JOYDEVICEREMOVED)
				{
					return 1;
				}
				return 0;
			}, IntPtr.Zero);

			for (int i = 0; i < collected.Length; i++)
			{
				collected[i] = 0.0f;
			}
			collected[35] = 2.0f;
		}

		public float Scale { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public void Run()
		{
			OnLoad();
			OnResize();
			while (!closing)
			{
				ProcessEvents();
				Update();
				Render();
#if DEBUG
				CheckForErrors();
#endif
				SDL_Delay(15);
			}
			OnUnload();
		}

		private unsafe uint LoadTexture(string path)
		{
			uint textureId = Gl.GenTexture();
			Gl.BindTexture(TextureTarget.Texture2D, textureId);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

			using (var image = new Bitmap(path))
			{
				image.RotateFlip(RotateFlipType.Rotate180FlipX);
				var data = image.LockBits(
					new Rectangle(0, 0, image.Width, image.Height),
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint) image.Width, (uint) image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0.ToPointer());

				image.UnlockBits(data);
			}

			Gl.GenerateMipmap(TextureTarget.Texture2D);
			return textureId;
		}

		private void OnLoad()
		{
			CalculateGrid(toolConfig.Tracker.Width, toolConfig.Tracker.Height);

			Gl.ClearColor(clear);

			uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
			Gl.ShaderSource(vertexShader, File.ReadAllText(Paths.VertexShader));
			Gl.CompileShader(vertexShader);

			Gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
			if (vStatus != (int) GLEnum.True)
				throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vertexShader));

			string infoLog = Gl.GetShaderInfoLog(vertexShader);
			Console.WriteLine(infoLog);

			uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
			Gl.ShaderSource(fragmentShader, File.ReadAllText(Paths.FragmentShader));
			Gl.CompileShader(fragmentShader);
			Gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
			if (fStatus != (int) GLEnum.True)
				throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fragmentShader));

			shaderProgram = Gl.CreateProgram();
			Gl.AttachShader(shaderProgram, vertexShader);
			Gl.AttachShader(shaderProgram, fragmentShader);
			Gl.LinkProgram(shaderProgram);

			Gl.GetProgram(shaderProgram, ProgramPropertyARB.LinkStatus, out int lStatus);
			if (lStatus != (int) GLEnum.True)
				throw new Exception("Program failed to link: " + Gl.GetProgramInfoLog(shaderProgram));

			infoLog = Gl.GetProgramInfoLog(shaderProgram);
			Console.WriteLine(infoLog);

			Gl.DeleteShader(vertexShader);
			Gl.DeleteShader(fragmentShader);

			int[] viewportData = new int[4];
			Gl.GetInteger(GetPName.Viewport, viewportData);
			Gl.UseProgram(shaderProgram);
			Gl.Uniform2(Gl.GetUniformLocation(shaderProgram, "viewportSize"), (float) viewportData[2], (float) viewportData[3]);
			collectedUniform = Gl.GetUniformLocation(shaderProgram, "collected");
			gridUniform = Gl.GetUniformLocation(shaderProgram, "grid");
			uTextureUniform = Gl.GetUniformLocation(shaderProgram, "uTexture");
			Gl.Uniform1(gridUniform, 1);

			Gl.UseProgram(shaderProgram);
			texture = LoadTexture(Paths.CombinedTexture);
			font = LoadTexture(Paths.FontAtlas);
			CheckForErrors();
			Gl.ActiveTexture(TextureUnit.Texture0);
			Gl.BindTexture(GLEnum.Texture2D, texture);
			Gl.ActiveTexture(TextureUnit.Texture1);
			Gl.BindTexture(TextureTarget.Texture2D, font);
			int[] textures = new int[2];
			textures[0] = 0;
			textures[1] = 1;
			Gl.Uniform1(uTextureUniform, 2, textures);

			Gl.Enable(EnableCap.Blend);
			Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			Gl.UseProgram(shaderProgram);

			CheckForErrors();
		}

		private void ProcessEvents()
		{
			var e = new SDL_Event[1];
			while (true)
			{
				if (!OSTailoredCode.IsUnixHost)
				{
					while (WmImports.PeekMessageW(out var msg, IntPtr.Zero, 0, 0, WmImports.PM_REMOVE))
					{
						WmImports.TranslateMessage(ref msg);
						WmImports.DispatchMessageW(ref msg);
					}
				}
				if (SDL_PeepEvents(e, 1, SDL_eventaction.SDL_GETEVENT, SDL_EventType.SDL_WINDOWEVENT, SDL_EventType.SDL_WINDOWEVENT) != 1)
				{
					break;
				}

				switch (e[0].window.windowEvent)
				{
					case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
						X = e[0].window.data1;
						Y = e[0].window.data2;
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
						Width = e[0].window.data1;
						Height = e[0].window.data2;
						OnResize();
						break;
					case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
						closing = true;
						break;
				}
			}
		}

		private void CheckForErrors()
		{
			GLEnum error = Gl.GetError();
			while (error != GLEnum.NoError)
			{
				Console.WriteLine(error.ToString());
				error = Gl.GetError();
			}
			string sdlError = SDL_GetError();
			while (sdlError != "")
			{
				Console.WriteLine(sdlError);
				sdlError = SDL_GetError();
			}
		}

		private void Update()
		{
			if (alwaysOnTop != toolConfig.Tracker.AlwaysOnTop)
			{
				alwaysOnTop = toolConfig.Tracker.AlwaysOnTop;
				SDL_SetWindowAlwaysOnTop(window, alwaysOnTop ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
			}

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
				if (collected[30 + i] == 0.0f && (tracker.progressionItems[i].Collected || tracker.progressionItems[i].Equipped))
				{
					changes = true;
					collected[30 + i] = 0.1f;
				}
				if (collected[30 + i] != 0.0f && !tracker.progressionItems[i].Collected && !tracker.progressionItems[i].Equipped)
				{
					changes = true;
					collected[30 + i] = 0.0f;
				}
			}
			bool swordCollected = false;
			for (int i = 0; i < tracker.thrustSwords.Length; i++)
			{
				if (tracker.thrustSwords[i].Collected || tracker.thrustSwords[i].Equipped)
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
				seedInfo = new Text(tracker.SeedInfo, Width, Height, collectedUniform, Gl);
				sprites.Dispose();
				sprites = new Sprites(Scale, relicSlots, tracker, columns, toolConfig.Tracker.GridLayout, toolConfig.Tracker.ProgressionRelicsOnly, Gl);
			}

			if (changes || !toolConfig.Tracker.GridLayout)
			{
				sprites.Dispose();
				sprites = new Sprites(Scale, relicSlots, tracker, columns, toolConfig.Tracker.GridLayout, toolConfig.Tracker.ProgressionRelicsOnly, Gl);
			}
		}

		private void Render()
		{
			Gl.Clear(ClearBufferMask.ColorBufferBit);
			Gl.Uniform1(collectedUniform, collected);
			sprites.Draw();
			seedInfo.Draw();
			SDL_GL_SwapWindow(window);
		}

		private void OnResize()
		{
			Size size = new Size(Width, Height);
			Gl.Viewport(size);

			int[] viewportData = new int[4];
			Gl.GetInteger(GetPName.Viewport, viewportData);
			Gl.UseProgram(shaderProgram);
			int viewportSizeUniform = Gl.GetUniformLocation(shaderProgram, "viewportSize");
			Gl.Uniform2(viewportSizeUniform, (float) viewportData[2], (float) viewportData[3]);

			CalculateGrid(Width, Height);
			ClearSprites();

			if (sprites != null)
			{
				sprites.Dispose();
			}
			if (seedInfo != null)
			{
				seedInfo.Dispose();
			}
			seedInfo = new Text(tracker.SeedInfo, Width, Height, collectedUniform, Gl);
			sprites = new Sprites(Scale, relicSlots, tracker, columns, toolConfig.Tracker.GridLayout, toolConfig.Tracker.ProgressionRelicsOnly, Gl);
			CheckForErrors();
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

		private void OnUnload()
		{
			toolConfig.Tracker.Width = Width;
			toolConfig.Tracker.Height = Height;
			Point location = new Point();
			location.X = X;
			location.Y = Y;
			toolConfig.Tracker.Location = location;

			ClearSprites();

			Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

			Gl.UseProgram(0);
			Gl.DeleteProgram(shaderProgram);

			Gl.DeleteTexture(texture);
			Gl.DeleteTexture(font);
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
				relicSlots[i] = new Vector2(CellPadding + (col * (ItemSize + CellPadding) * Scale), Height - (LabelOffset + (ItemSize + CellPadding) * Scale) - (row * (ItemSize + CellPadding) * Scale));
				col++;
			}
		}

		public void Dispose()
		{
			closing = true;
		}
	}
}
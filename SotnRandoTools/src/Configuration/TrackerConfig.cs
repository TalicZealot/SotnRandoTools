using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace SotnRandoTools.Configuration
{
	public class ByteArrayConverter : JsonConverter
	{
		public override void WriteJson(
			JsonWriter writer,
			object value,
			JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			byte[] data = (byte[]) value;

			// Compose an array.
			writer.WriteStartArray();

			for (var i = 0; i < data.Length; i++)
			{
				writer.WriteValue(data[i]);
			}

			writer.WriteEndArray();
		}

		public override object ReadJson(
			JsonReader reader,
			Type objectType,
			object existingValue,
			JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartArray)
			{
				var byteList = new List<byte>();

				while (reader.Read())
				{
					switch (reader.TokenType)
					{
						case JsonToken.Integer:
							byteList.Add(Convert.ToByte(reader.Value));
							break;
						case JsonToken.EndArray:
							return byteList.ToArray();
						case JsonToken.Comment:
							// skip
							break;
						default:
							throw new Exception(
							string.Format(
								"Unexpected token when reading bytes: {0}",
								reader.TokenType));
					}
				}

				throw new Exception("Unexpected end when reading bytes.");
			}
			else
			{
				return TrackerConfig.GetDefaultOverlay();
			}
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(byte[]);
		}
	}
	public class TrackerConfig
	{
		public TrackerConfig()
		{
			Default();
		}
		public bool ProgressionRelicsOnly { get; set; }
		public bool GridLayout { get; set; }
		public bool AlwaysOnTop { get; set; }
		public bool Locations { get; set; }
		public bool SaveReplays { get; set; }
		public bool EnableAutosplitter { get; set; }
		public bool UseOverlay { get; set; }
		public bool MuteMusic { get; set; }
		public bool Stereo { get; set; }
		public string Username { get; set; }
		[JsonConverter(typeof(ByteArrayConverter))]
		public byte[] OverlaySlots { get; set; }
		public string CustomExtension { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public Point Location { get; set; }

		public void Default()
		{
			ProgressionRelicsOnly = false;
			GridLayout = true;
			AlwaysOnTop = false;
			Locations = true;
			SaveReplays = true;
			EnableAutosplitter = true;
			UseOverlay = true;
			MuteMusic = false;
			Stereo = true;
			CustomExtension = "";
			Width = 260;
			Height = 440;
			Username = "";
			Location = new Point(20,50);
			//First byte is reserved
			OverlaySlots = new byte[]
			{
				1  , 
				0  , 1  , 2  , 3  , 4  ,
				15 , 16 , 17 , 18 , 19 ,
				30 , 31 , 32 , 33 , 34 ,
				45 , 46 , 47 , 48 , 49 ,
				60 , 61 , 62 , 63 , 64 ,
				75 , 76 , 77 , 78 , 79 ,
				90 , 91 , 92 , 93 , 94 ,
				6  , 7  , 8  , 9  , 10 ,
				21 , 22 , 23 , 24 , 25 ,
				36 , 37 , 38 , 39 , 40 ,
				51 , 52 , 53 , 54 , 55 ,
				66
			};
		}

		public static byte[] GetDefaultOverlay()
		{
			return new byte[]
			{
				1  ,
				0  , 1  , 2  , 3  , 4  ,
				15 , 16 , 17 , 18 , 19 ,
				30 , 31 , 32 , 33 , 34 ,
				45 , 46 , 47 , 48 , 49 ,
				60 , 61 , 62 , 63 , 64 ,
				75 , 76 , 77 , 78 , 79 ,
				90 , 91 , 92 , 93 , 94 ,
				6  , 7  , 8  , 9  , 10 ,
				21 , 22 , 23 , 24 , 25 ,
				36 , 37 , 38 , 39 , 40 ,
				51 , 52 , 53 , 54 , 55 ,
				66
			};
		}

		public void SaveOverlayLayout(string path)
		{
			JsonConverter[] converters = new JsonConverter[] { new ByteArrayConverter() };
			string output = JsonConvert.SerializeObject(OverlaySlots, Formatting.Indented, converters);
			if (File.Exists(path))
			{
				File.WriteAllText(path, output);
			}
			else
			{
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.Write(output);
				}
			}
		}

		public void LoadOverlayLayout(string path)
		{
			if (File.Exists(path))
			{
				string layoutJson = File.ReadAllText(path);
				byte[] layout = JsonConvert.DeserializeObject<byte[]>(layoutJson,
					new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, MissingMemberHandling = MissingMemberHandling.Ignore });

				if (layout.Length > 0)
				{
					OverlaySlots = layout;
				}
			}
		}
	}
}

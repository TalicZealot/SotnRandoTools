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
							return TrackerConfig.GetDefaultOverlay();
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
		public bool CustomLocationsGuarded { get; set; }
		public bool CustomLocationsEquipment { get; set; }
		public bool CustomLocationsClassic { get; set; }
		public bool CustomLocationsSpread { get; set; }
		public bool CustomLocationsCustom { get; set; }
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
			UseOverlay = false;
			MuteMusic = false;
			Stereo = true;
			CustomLocationsGuarded = true;
			CustomLocationsEquipment = false;
			CustomLocationsClassic = false;
			CustomLocationsSpread = false;
			CustomLocationsCustom = false;
			CustomExtension = "";
			Width = 260;
			Height = 440;
			Username = "";
			OverlaySlots = new byte[]
			{
				1, 1, 6, 11, 16, 21, 35, 26, 31, 0, 0,
				2, 7, 12, 17, 22, 0, 27, 32, 0, 0,
				3, 8, 13, 18, 23, 0, 28, 33, 0, 0,
				4, 9, 14, 19, 24, 0, 29, 34, 0, 0,
				5, 10, 15, 20, 25, 0, 30, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
		}

		public static byte[] GetDefaultOverlay()
		{
			return new byte[]
			{
				1, 1, 6, 11, 16, 21, 35, 26, 31, 0, 0,
				2, 7, 12, 17, 22, 0, 27, 32, 0, 0,
				3, 8, 13, 18, 23, 0, 28, 33, 0, 0,
				4, 9, 14, 19, 24, 0, 29, 34, 0, 0,
				5, 10, 15, 20, 25, 0, 30, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
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

using System;
using System.IO;
using System.Threading.Tasks;
using BizHawk.Client.Common;
using Silk.NET.OpenAL;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	internal unsafe sealed class WavPlayer : IDisposable
	{
		private AL Al;
		private ALContext Alc;
		private uint buffer;
		private uint source;
		private Device* device;
		private Context* context;

		public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			using (BinaryReader reader = new BinaryReader(stream))
			{
				// RIFF header
				string signature = new string(reader.ReadChars(4));
				if (signature != "RIFF")
					throw new NotSupportedException("Specified stream is not a wave file.");

				int riff_chunck_size = reader.ReadInt32();

				string format = new string(reader.ReadChars(4));
				if (format != "WAVE")
					throw new NotSupportedException("Specified stream is not a wave file.");

				// WAVE header
				string format_signature = new string(reader.ReadChars(4));
				if (format_signature != "fmt ")
					throw new NotSupportedException("Specified wave file is not supported.");

				int format_chunk_size = reader.ReadInt32();
				int audio_format = reader.ReadInt16();
				int num_channels = reader.ReadInt16();
				int sample_rate = reader.ReadInt32();
				int byte_rate = reader.ReadInt32();
				int block_align = reader.ReadInt16();
				int bits_per_sample = reader.ReadInt16();

				string data_signature = new string(reader.ReadChars(4));
				if (data_signature != "data")
					throw new NotSupportedException("Specified wave file is not supported.");

				int data_chunk_size = reader.ReadInt32();

				channels = num_channels;
				bits = bits_per_sample;
				rate = sample_rate;

				return reader.ReadBytes((int) reader.BaseStream.Length);
			}
		}

		public unsafe WavPlayer(float volume)
		{
			Alc = ALContext.GetApi();
			Al = AL.GetApi();
			device = Alc.OpenDevice("");
			context = Alc.CreateContext(device, null);
			Alc.MakeContextCurrent(context);

			buffer = Al.GenBuffer();
			source = Al.GenSource();

			int channels, bits_per_sample, sample_rate;
			byte[] sound_data = LoadWave(File.Open(Paths.ItemPickupSound, FileMode.Open), out channels, out bits_per_sample, out sample_rate);

			fixed (void* d = sound_data)
			{
				Al.BufferData(buffer, BufferFormat.Stereo16, d, sound_data.Length, sample_rate);
			}

			Al.SetSourceProperty(source, SourceBoolean.Looping, false);
			Al.SetSourceProperty(source, SourceFloat.Gain, volume);
			Al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
		}

		public async Task Play()
		{
			Al.SourcePlay(source);

			int state;
			do
			{
				System.Threading.Thread.Sleep(250);
				Al.GetSourceProperty(source, GetSourceInteger.SourceState, out state);
			} while ((SourceState) state == SourceState.Playing);
		}

		public void SetVolume(float volume)
		{
			Al.SetSourceProperty(source, SourceFloat.Gain, volume);
		}

		public void Dispose()
		{
			Al.SourceStop(source);
			Al.DeleteSource(source);
			Al.DeleteBuffer(buffer);
			Alc.DestroyContext(context);
			Alc.CloseDevice(device);
			Al.Dispose();
			Alc.Dispose();
		}
	}
}

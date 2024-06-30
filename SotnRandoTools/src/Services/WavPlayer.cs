﻿using System;
using System.IO;
using System.Threading.Tasks;
using BizHawk.Client.Common;
using OpenTK;
using OpenTK.Audio.OpenAL;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	internal sealed class WavPlayer : IDisposable
	{
		int buffer;
		int source;
		IntPtr device;
		ContextHandle context;

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

		public static ALFormat GetSoundFormat(int channels, int bits)
		{
			switch (channels)
			{
				case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
				case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
				default: throw new NotSupportedException("The specified sound format is not supported.");
			}
		}

		public WavPlayer(float volume, Config globalConfig)
		{
			if (globalConfig.SoundOutputMethod != ESoundOutputMethod.OpenAL)
			{
				device = Alc.OpenDevice(null);
				context = Alc.CreateContext(device, (int[]) null);
				Alc.MakeContextCurrent(context);
			}

			buffer = AL.GenBuffer();
			source = AL.GenSource();

			int channels, bits_per_sample, sample_rate;
			byte[] sound_data = LoadWave(File.Open(Paths.ItemPickupSound, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
			AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);

			AL.Source(source, ALSourcei.Buffer, buffer);
			AL.Source(source, ALSourceb.Looping, false);
			AL.Source(source, ALSourcef.Gain, volume);
		}

		public async Task Play()
		{
			AL.SourcePlay(source);

			int state;
			do
			{
				System.Threading.Thread.Sleep(250);
				AL.GetSource(source, ALGetSourcei.SourceState, out state);
			} while ((ALSourceState) state == ALSourceState.Playing);
		}

		public void SetVolume(float volume)
		{
			AL.Source(source, ALSourcef.Gain, volume);
		}

		public void Dispose()
		{
			AL.SourceStop(source);
			AL.DeleteSource(source);
			AL.DeleteBuffer(buffer);
		}
	}
}

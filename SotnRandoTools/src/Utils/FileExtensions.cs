using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SotnRandoTools.Utils
{
	public static class FileExtensions
	{
		private const uint retries = 10;

		private static bool WaitForFile(string fullPath, uint retries)
		{
			if (String.IsNullOrEmpty(fullPath)) throw new ArgumentNullException(nameof(fullPath));
			if (retries == 0 && retries < 100) throw new ArgumentOutOfRangeException(nameof(fullPath));

			Thread.Sleep(50);
			for (int numTries = 0; numTries < retries; numTries++)
			{
				try
				{
					IEquatable<string> fs = File.ReadLines(fullPath).FirstOrDefault();
					return true;
				}
				catch (IOException e)
				{
					Console.WriteLine(e.Message + "with code: " + e.HResult);
					Thread.Sleep(50);
				}
			}
			return false;
		}

		public static string GetLastLine(string fullPath)
		{
			if (String.IsNullOrEmpty(fullPath)) throw new ArgumentNullException(nameof(fullPath));

			string lastLine = "";
			if (!File.Exists(fullPath))
			{
				return "";
			}
			if (WaitForFile(fullPath, retries))
			{
				lastLine = File.ReadLines(fullPath).LastOrDefault();
			}
			return lastLine;
		}

		public static string GetText(string fullPath)
		{
			if (String.IsNullOrEmpty(fullPath)) throw new ArgumentNullException(nameof(fullPath));

			string text = "";
			if (!File.Exists(fullPath))
			{
				return "";
			}
			if (WaitForFile(fullPath, retries))
			{
				text = File.ReadAllText(fullPath);
			}
			return text;
		}

		public static string[]? GetLines(string fullPath)
		{
			if (String.IsNullOrEmpty(fullPath)) throw new ArgumentNullException(nameof(fullPath));

			if (!File.Exists(fullPath))
			{
				return null;
			}
			if (WaitForFile(fullPath, retries))
			{
				return File.ReadLines(fullPath).ToArray();
			}
			return null;
		}
	}
}

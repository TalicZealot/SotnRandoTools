using System;
using System.IO;
using System.IO.Pipes;

namespace SotnRandoTools.Services
{
	internal sealed class Autosplitter
	{
		NamedPipeClientStream pipeClient;
		StreamWriter pipeWriter;

		public Autosplitter()
		{
			pipeClient = new NamedPipeClientStream("LiveSplit");
		}

		public bool AtemptConnect()
		{
			if (!pipeClient.IsConnected)
			{
				try
				{
					pipeClient.Connect(5);
				}
				catch (Exception e)
				{
					Console.WriteLine("Could not connect. " + e.Message);
				}
				return false;
			}
			else
			{
				pipeWriter = new StreamWriter(pipeClient);
				return true;
			}
		}

		public bool IsConnected()
		{
			try
			{
				pipeWriter.WriteLine("check");
				pipeWriter.Flush();
			}
			catch (Exception e)
			{
				Console.WriteLine("Pipe Error:" + e.Message);
			}
			return pipeClient.IsConnected;
		}

		public void StartTImer()
		{
			SendString("starttimer");
		}

		public void Restart()
		{
			SendString("reset");
		}

		public void Split()
		{
			SendString("split");
		}

		public void Disconnect()
		{
			if (pipeClient is not null && pipeClient.IsConnected)
			{
				try
				{
					pipeClient.Dispose();
				}
				catch
				{
					Console.WriteLine("Pipe closed");
				}
			}
			if (pipeWriter is not null)
			{
				try
				{
					pipeWriter.Dispose();
				}
				catch
				{
					Console.WriteLine("Pipe closed");
				}
			};
		}

		private void SendString(string data)
		{
			if (!pipeClient.IsConnected)
			{
				Console.WriteLine("LiveSplit pipe is not connected!");
				return;
			}

			try
			{
				pipeWriter.WriteLine(data);
				pipeWriter.Flush();
			}
			catch (Exception e)
			{
				Console.WriteLine("Pipe Error:" + e.Message);
			}
		}
	}
}

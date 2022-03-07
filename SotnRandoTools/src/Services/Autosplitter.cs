using System;
using System.IO;
using System.IO.Pipes;

namespace SotnRandoTools.Services
{
	public class Autosplitter
	{
		NamedPipeClientStream pipeClient;
		StreamWriter pipeWriter;

		public Autosplitter()
		{
			pipeClient = new NamedPipeClientStream("LiveSplit");
		}

		public bool Started { get; set; }

		public bool AtemptConnect()
		{
			if (!pipeClient.IsConnected)
			{
				try
				{
					pipeClient.Connect(10);
				}
				catch
				{
				}
				return false;
			}
			else
			{
				pipeWriter = new StreamWriter(pipeClient);
				return true;
			}
		}


		public void StartTImer()
		{
			SendString("starttimer");
			Started = true;
		}

		public void Restart()
		{
			SendString("reset");
			Started = false;
		}

		public void Split()
		{
			SendString("split");
			Started = false;
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
				Console.WriteLine(e.Message);
			}
		}
	}
}

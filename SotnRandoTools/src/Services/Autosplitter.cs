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
			if (!pipeClient.IsConnected)
			{
				Console.WriteLine("LiveSplit pipe is not connected!");
				return;
			}

			pipeWriter.WriteLine("starttimer");
			pipeWriter.Flush();
			Started = true;
		}

		public void Restart()
		{
			if (!pipeClient.IsConnected)
			{
				Console.WriteLine("LiveSplit pipe is not connected!");
				return;
			}

			pipeWriter.WriteLine("reset");
			pipeWriter.Flush();
			Started = false;
		}

		public void Split()
		{
			if (!pipeClient.IsConnected)
			{
				Console.WriteLine("LiveSplit pipe is not connected!");
				return;
			}

			pipeWriter.WriteLine("split");
			pipeWriter.Flush();
			Started = false;
		}

		public void Disconnect()
		{
			if (pipeClient is not null)
			{
				pipeClient.Dispose();
			}
			if (pipeWriter is not null)
			{
				pipeWriter.Dispose();
			};

		}
	}
}

using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SotnRandoTools.Services.Interfaces;

namespace SotnRandoTools.Services
{
	//Try IDisposable to wrap in a using block
	public class TwitchListener : ITwitchListener
	{
		private HttpListener listener;

		public TwitchListener(string uri)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(uri);
		}

		public async Task<Models.Authorization> Listen()
		{
			listener.Start();
			return await onRequest();
		}

		private async Task<Models.Authorization> onRequest()
		{
			while (listener.IsListening)
			{
				var ctx = await listener.GetContextAsync();
				var req = ctx.Request;
				var resp = ctx.Response;

				using (var writer = new StreamWriter(resp.OutputStream))
				{
					if (req.QueryString.AllKeys.Any("code".Contains))
					{
						writer.WriteLine("Authorization started! Check your application!");
						writer.Flush();
						return new Models.Authorization(req.QueryString["code"]);
					}
					else
					{
						writer.WriteLine("No code found in query string!");
						writer.Flush();
					}
				}
			}
			return new Models.Authorization(string.Empty);
		}

		public void Stop()
		{
			if (listener.IsListening)
			{
				listener.Close();
			}
		}
	}
}

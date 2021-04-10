using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Utils
{
	public static class WebRequests
	{
		internal class Release
		{
			public string tag_name { get; set; }
		}
		public static string getExternalIP()
		{
			using (WebClient client = new WebClient())
			{
				try
				{
					return client.DownloadString("http://ipv4.icanhazip.com/");
				}
				catch (WebException e)
				{
					Console.WriteLine(e.Message);
				}

				try
				{
					return client.DownloadString("https://api.ipify.org/");
				}
				catch (WebException e)
				{
					Console.WriteLine(e.Message);
				}

				return "Could not retrieve IP";
			}
		}

		public static async Task<bool> NewReleaseAvaiable(string currentVersion)
		{
			using (var handler = new HttpClientHandler())
			{
				handler.UseDefaultCredentials = true;

				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
					client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

					HttpResponseMessage response = await client.GetAsync(Paths.LatestReleaseApi);
					if (response.IsSuccessStatusCode)
					{
						string data = await response.Content.ReadAsStringAsync();
						string latest = JsonConvert.DeserializeObject<IEnumerable<Release>>(data).FirstOrDefault().tag_name;
						if (String.CompareOrdinal(latest, 0, currentVersion, 0, 5) > 0)
						{
							Console.WriteLine($"Release {latest} is newer than current version {currentVersion}");
							return true;
						}
						Console.WriteLine($"Release {latest} is not newer than current version {currentVersion}");
						return false;
					}

					Console.WriteLine($"Bad response: {response.StatusCode}");
					return false;
				}
			}
		}
	}
}

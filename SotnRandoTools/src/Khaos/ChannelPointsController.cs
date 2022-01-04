using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Interfaces;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomReward;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace SotnRandoTools.Khaos
{
	public class ChannelPointsController
	{
		private const int RetryCount = 4;
		private readonly IToolConfig toolConfig;
		private readonly ITwitchListener twitchListener;
		private readonly IKhaosController khaosController;
		private List<string> scopes = new List<string> { "channel:read:subscriptions", "channel:read:redemptions", "channel:manage:redemptions" };
		private TwitchAPI api = new TwitchAPI();
		private TwitchPubSub client = new TwitchPubSub();
		private string broadcasterId;
		private string refreshToken;
		private List<string> customRewardIds = new();

		public ChannelPointsController(IToolConfig toolConfig, ITwitchListener twitchListener, IKhaosController khaosController)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (twitchListener is null) throw new ArgumentNullException(nameof(twitchListener));
			if (khaosController is null) throw new ArgumentNullException(nameof(khaosController));
			this.toolConfig = toolConfig;
			this.twitchListener = twitchListener;
			this.khaosController = khaosController;
		}

		public async void Connect()
		{
			Console.WriteLine($"Authenticating...");
			validateCreds();
			api.Settings.ClientId = TwitchConfiguration.TwitchClientId;
			Process.Start(getAuthorizationCodeUrl(api.Settings.ClientId, Paths.TwitchRedirectUri, scopes));
			var auth = await twitchListener.Listen();
			var resp = await api.Auth.GetAccessTokenFromCodeAsync(auth.Code, TwitchConfiguration.TwitchClientSecret, Paths.TwitchRedirectUri);
			api.Settings.AccessToken = resp.AccessToken;
			refreshToken = resp.RefreshToken;

			var user = (await api.Helix.Users.GetUsersAsync()).Users[0];
			broadcasterId = user.Id;
			Console.WriteLine($"Authorization success!\r\nUser: {user.DisplayName}\r\nId: {user.Id}");

			await GetSubscribers();
			await CreateRewards();

			client.OnPubSubServiceConnected += onPubSubServiceConnected;
			client.OnListenResponse += onListenResponse;
			client.OnChannelPointsRewardRedeemed += Client_OnChannelPointsRewardRedeemed;
			client.OnPubSubServiceClosed += Client_OnPubSubServiceClosed;
			client.ListenToChannelPoints(user.Id);
			client.Connect();
		}

		public async Task<bool> Disconnect()
		{
			await DeleteRewards();
			twitchListener.Stop();
			client.Disconnect();
			return true;
		}

		private async Task<bool> GetSubscribers()
		{
			Console.WriteLine($"Fetching subscribers...");

			try
			{
				var subs = await api.Helix.Subscriptions.GetBroadcasterSubscriptions(
				broadcasterId,
				null,
				100,
				api.Settings.AccessToken
				);
				khaosController.OverwriteBossNames(subs.Data.Select(u => u.UserName).ToArray());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return true;
		}
		//Can alternatively create rewards once, store ids and onyl thrn them on and adjust prices or off.

		private async Task<bool> CreateRewards()
		{
			Console.WriteLine($"Creating rewards...");
			foreach (var action in toolConfig.Khaos.Actions)
			{
				if (action.IsUsable && action.ChannelPoints > 0)
				{

					CreateCustomRewardsRequest request = new CreateCustomRewardsRequest
					{
						Title = action.Name,
						Prompt = action.Description,
						Cost = (int) action.ChannelPoints,
						IsEnabled = true,
						ShouldRedemptionsSkipRequestQueue = true
					};


					if (action.Cooldown.TotalSeconds > 0)
					{
						request.IsGlobalCooldownEnabled = true;
						request.GlobalCooldownSeconds = (int) action.Cooldown.TotalSeconds;
					}

					Console.WriteLine($"Request parameters: Title: {request.Title} Cost: {request.Cost} Cooldown: {request.GlobalCooldownSeconds}");


					for (int i = 0; i <= RetryCount; i++)
					{
						try
						{
							CreateCustomRewardsResponse response = await api.Helix.ChannelPoints.CreateCustomRewards(
							broadcasterId,
							request,
							api.Settings.AccessToken
							);
							customRewardIds.Add(response.Data[0].Id);
							break;
						}
						catch (Exception ex)
						{
							if (i == RetryCount)
							{
								throw new Exception("Server error while creating Rewards. Please click Continue, then Disconnect and Reconnect.", ex);
							}
							else
							{
								Console.WriteLine(ex.Message);
								Thread.Sleep((int)Math.Pow(500, i));
							}
						}
					}

				}
			}
			return true;
		}

		private async Task<bool> DeleteRewards()
		{
			Console.WriteLine($"Deleting rewards...");
			for (int i = 0; i < customRewardIds.Count; i++)
			{
				Console.WriteLine($"Deleting reward with id: {customRewardIds[i]}");

				for (int j = 0; j <= RetryCount; j++)
				{
					try
					{
						await api.Helix.ChannelPoints.DeleteCustomReward(
						broadcasterId,
						customRewardIds[i],
						api.Settings.AccessToken
					);
					}
					catch (Exception ex)
					{
						if (j == RetryCount)
						{
							throw new Exception("Server error while deleting Rewards. Please delete the remainint rewards from the  Dashboard.", ex);
						}
						else
						{
							Console.WriteLine(ex.Message);
							Thread.Sleep((int) Math.Pow(500, j));
						}
					}
				}
			}

			return true;
		}

		//No need to refresh for now, as a normal Khaos session wouldn't last 4 hours.
		private async void RefreshToken()
		{
			try
			{
				var refresh = await api.Auth.RefreshAuthTokenAsync(refreshToken, TwitchConfiguration.TwitchClientSecret, api.Settings.ClientId);
				api.Settings.AccessToken = refresh.AccessToken;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private async void Client_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
		{
			Console.WriteLine($"Channel point reward redeemed: {e.RewardRedeemed.Redemption.Reward.Title}");

			var action = toolConfig.Khaos.Actions.Where(a => a.Name == e.RewardRedeemed.Redemption.Reward.Title).FirstOrDefault();

			if (action is null)
			{
				return;
			}

			//No need as of now, since rewards auto-redeem.
			//In the future I might leave them unredeemed until the end and include a tab with a list of redemptions, so that the user can refund channel points.
			//Complete the redemption
			/*await api.Helix.ChannelPoints.UpdateCustomRewardRedemptionStatus(
				broadcasterId,
				e.RewardRedeemed.Redemption.Reward.Id,
				new List<string> { e.RewardRedeemed.Redemption.Id },
				new UpdateCustomRewardRedemptionStatusRequest { Status = TwitchLib.Api.Core.Enums.CustomRewardRedemptionStatus.FULFILLED },
				api.Settings.AccessToken
				);*/

			//Scale cost
			int NewCost = (int) Math.Round(e.RewardRedeemed.Redemption.Reward.Cost * action.Scaling);
			if (action.MaximumChannelPoints != 0 && NewCost > action.MaximumChannelPoints)
			{
				NewCost = (int)action.MaximumChannelPoints;
			}
			if (action.MaximumChannelPoints != 0 && toolConfig.Khaos.CostDecay && e.RewardRedeemed.Redemption.Reward.Cost == action.MaximumChannelPoints)
			{
				NewCost = (int) action.MaximumChannelPoints / 2;
			}

			await api.Helix.ChannelPoints.UpdateCustomReward(
				broadcasterId,
				e.RewardRedeemed.Redemption.Reward.Id,
				new UpdateCustomRewardRequest { Cost = NewCost, },
				api.Settings.AccessToken);

			var actionEvent = new EventAddAction { UserName = e.RewardRedeemed.Redemption.User.DisplayName, ActionIndex = toolConfig.Khaos.Actions.IndexOf(action) };

			khaosController.EnqueueAction(actionEvent);
		}

		private void onListenResponse(object sender, OnListenResponseArgs e)
		{
			if (!e.Successful)
				throw new Exception($"Failed to listen! Response: {e.Response}");
			Console.WriteLine(e.ChannelId + " " + e.Topic);
		}

		private void onPubSubServiceConnected(object sender, EventArgs e)
		{
			client.SendTopics(api.Settings.AccessToken);
		}

		private void Client_OnPubSubServiceClosed(object sender, EventArgs e)
		{
			for (int i = 0; i <= RetryCount; i++)
			{
				try
				{
					client.Connect();
				}
				catch (Exception ex)
				{
					if (i == RetryCount)
					{
						throw new Exception("Failed to reconnect! Click continue and disconnect to delete rewards.", ex);
					}
					else
					{
						Console.WriteLine(ex.Message);
						Thread.Sleep((int) Math.Pow(500, i));
					}
					throw;
				}
			}
		}

		private string getAuthorizationCodeUrl(string clientId, string redirectUri, List<string> scopes)
		{
			var scopesStr = String.Join("+", scopes);

			return "https://id.twitch.tv/oauth2/authorize?" +
				   $"client_id={clientId}&" +
				   $"redirect_uri={HttpUtility.UrlEncode(redirectUri)}&" +
				   "response_type=code&" +
				   $"scope={scopesStr}";
		}

		private void validateCreds()
		{
			if (String.IsNullOrEmpty(TwitchConfiguration.TwitchClientId))
				throw new Exception("client id cannot be null or empty");
			if (String.IsNullOrEmpty(TwitchConfiguration.TwitchClientSecret))
				throw new Exception("client secret cannot be null or empty");
			if (String.IsNullOrEmpty(Paths.TwitchRedirectUri))
				throw new Exception("redirect uri cannot be null or empty");
		}

	}
}

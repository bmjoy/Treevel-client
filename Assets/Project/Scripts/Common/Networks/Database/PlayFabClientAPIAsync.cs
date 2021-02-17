
/*
MIT License
Copyright (c) 2019 Yoshitaka Kimisaki
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#if !DISABLE_PLAYFABCLIENT_API
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

namespace Treevel.Common.Networks.PlayFab
{
	public static class PlayFabClientAPIAsync
	{
		public static UniTask<AcceptTradeResponse> AcceptTradeAsync(AcceptTradeRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AcceptTradeRequest, AcceptTradeResponse>(PlayFabClientAPI.AcceptTrade, request, customData, extraHeaders);
		}
		public static UniTask<AddFriendResult> AddFriendAsync(AddFriendRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AddFriendRequest, AddFriendResult>(PlayFabClientAPI.AddFriend, request, customData, extraHeaders);
		}
		public static UniTask<AddGenericIDResult> AddGenericIDAsync(AddGenericIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AddGenericIDRequest, AddGenericIDResult>(PlayFabClientAPI.AddGenericID, request, customData, extraHeaders);
		}
		public static UniTask<AddOrUpdateContactEmailResult> AddOrUpdateContactEmailAsync(AddOrUpdateContactEmailRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AddOrUpdateContactEmailRequest, AddOrUpdateContactEmailResult>(PlayFabClientAPI.AddOrUpdateContactEmail, request, customData, extraHeaders);
		}
		public static UniTask<AddSharedGroupMembersResult> AddSharedGroupMembersAsync(AddSharedGroupMembersRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AddSharedGroupMembersRequest, AddSharedGroupMembersResult>(PlayFabClientAPI.AddSharedGroupMembers, request, customData, extraHeaders);
		}
		public static UniTask<AddUsernamePasswordResult> AddUsernamePasswordAsync(AddUsernamePasswordRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AddUsernamePasswordRequest, AddUsernamePasswordResult>(PlayFabClientAPI.AddUsernamePassword, request, customData, extraHeaders);
		}
		public static UniTask<ModifyUserVirtualCurrencyResult> AddUserVirtualCurrencyAsync(AddUserVirtualCurrencyRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AddUserVirtualCurrencyRequest, ModifyUserVirtualCurrencyResult>(PlayFabClientAPI.AddUserVirtualCurrency, request, customData, extraHeaders);
		}
		public static UniTask<AndroidDevicePushNotificationRegistrationResult> AndroidDevicePushNotificationRegistrationAsync(AndroidDevicePushNotificationRegistrationRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AndroidDevicePushNotificationRegistrationRequest, AndroidDevicePushNotificationRegistrationResult>(PlayFabClientAPI.AndroidDevicePushNotificationRegistration, request, customData, extraHeaders);
		}
		public static UniTask<AttributeInstallResult> AttributeInstallAsync(AttributeInstallRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<AttributeInstallRequest, AttributeInstallResult>(PlayFabClientAPI.AttributeInstall, request, customData, extraHeaders);
		}
		public static UniTask<CancelTradeResponse> CancelTradeAsync(CancelTradeRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<CancelTradeRequest, CancelTradeResponse>(PlayFabClientAPI.CancelTrade, request, customData, extraHeaders);
		}
		public static UniTask<ConfirmPurchaseResult> ConfirmPurchaseAsync(ConfirmPurchaseRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ConfirmPurchaseRequest, ConfirmPurchaseResult>(PlayFabClientAPI.ConfirmPurchase, request, customData, extraHeaders);
		}
		public static UniTask<ConsumeItemResult> ConsumeItemAsync(ConsumeItemRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ConsumeItemRequest, ConsumeItemResult>(PlayFabClientAPI.ConsumeItem, request, customData, extraHeaders);
		}
		public static UniTask<ConsumePSNEntitlementsResult> ConsumePSNEntitlementsAsync(ConsumePSNEntitlementsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ConsumePSNEntitlementsRequest, ConsumePSNEntitlementsResult>(PlayFabClientAPI.ConsumePSNEntitlements, request, customData, extraHeaders);
		}
		public static UniTask<ConsumeXboxEntitlementsResult> ConsumeXboxEntitlementsAsync(ConsumeXboxEntitlementsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ConsumeXboxEntitlementsRequest, ConsumeXboxEntitlementsResult>(PlayFabClientAPI.ConsumeXboxEntitlements, request, customData, extraHeaders);
		}
		public static UniTask<CreateSharedGroupResult> CreateSharedGroupAsync(CreateSharedGroupRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<CreateSharedGroupRequest, CreateSharedGroupResult>(PlayFabClientAPI.CreateSharedGroup, request, customData, extraHeaders);
		}
		public static UniTask<ExecuteCloudScriptResult> ExecuteCloudScriptAsync(ExecuteCloudScriptRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>(PlayFabClientAPI.ExecuteCloudScript, request, customData, extraHeaders);
		}
		public static UniTask<ExecuteCloudScriptResult> ExecuteCloudScriptAsync<TOut>(ExecuteCloudScriptRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>(PlayFabClientAPI.ExecuteCloudScript<TOut>, request, customData, extraHeaders);
		}
		public static UniTask<GetAccountInfoResult> GetAccountInfoAsync(GetAccountInfoRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetAccountInfoRequest, GetAccountInfoResult>(PlayFabClientAPI.GetAccountInfo, request, customData, extraHeaders);
		}
		public static UniTask<ListUsersCharactersResult> GetAllUsersCharactersAsync(ListUsersCharactersRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ListUsersCharactersRequest, ListUsersCharactersResult>(PlayFabClientAPI.GetAllUsersCharacters, request, customData, extraHeaders);
		}
		public static UniTask<GetCatalogItemsResult> GetCatalogItemsAsync(GetCatalogItemsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetCatalogItemsRequest, GetCatalogItemsResult>(PlayFabClientAPI.GetCatalogItems, request, customData, extraHeaders);
		}
		public static UniTask<GetCharacterDataResult> GetCharacterDataAsync(GetCharacterDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetCharacterDataRequest, GetCharacterDataResult>(PlayFabClientAPI.GetCharacterData, request, customData, extraHeaders);
		}
		public static UniTask<GetCharacterInventoryResult> GetCharacterInventoryAsync(GetCharacterInventoryRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetCharacterInventoryRequest, GetCharacterInventoryResult>(PlayFabClientAPI.GetCharacterInventory, request, customData, extraHeaders);
		}
		public static UniTask<GetCharacterLeaderboardResult> GetCharacterLeaderboardAsync(GetCharacterLeaderboardRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetCharacterLeaderboardRequest, GetCharacterLeaderboardResult>(PlayFabClientAPI.GetCharacterLeaderboard, request, customData, extraHeaders);
		}
		public static UniTask<GetCharacterDataResult> GetCharacterReadOnlyDataAsync(GetCharacterDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetCharacterDataRequest, GetCharacterDataResult>(PlayFabClientAPI.GetCharacterReadOnlyData, request, customData, extraHeaders);
		}
		public static UniTask<GetCharacterStatisticsResult> GetCharacterStatisticsAsync(GetCharacterStatisticsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetCharacterStatisticsRequest, GetCharacterStatisticsResult>(PlayFabClientAPI.GetCharacterStatistics, request, customData, extraHeaders);
		}
		public static UniTask<GetContentDownloadUrlResult> GetContentDownloadUrlAsync(GetContentDownloadUrlRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetContentDownloadUrlRequest, GetContentDownloadUrlResult>(PlayFabClientAPI.GetContentDownloadUrl, request, customData, extraHeaders);
		}
		public static UniTask<CurrentGamesResult> GetCurrentGamesAsync(CurrentGamesRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<CurrentGamesRequest, CurrentGamesResult>(PlayFabClientAPI.GetCurrentGames, request, customData, extraHeaders);
		}
		public static UniTask<GetLeaderboardResult> GetFriendLeaderboardAsync(GetFriendLeaderboardRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetFriendLeaderboardRequest, GetLeaderboardResult>(PlayFabClientAPI.GetFriendLeaderboard, request, customData, extraHeaders);
		}
		public static UniTask<GetFriendLeaderboardAroundPlayerResult> GetFriendLeaderboardAroundPlayerAsync(GetFriendLeaderboardAroundPlayerRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetFriendLeaderboardAroundPlayerRequest, GetFriendLeaderboardAroundPlayerResult>(PlayFabClientAPI.GetFriendLeaderboardAroundPlayer, request, customData, extraHeaders);
		}
		public static UniTask<GetFriendsListResult> GetFriendsListAsync(GetFriendsListRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetFriendsListRequest, GetFriendsListResult>(PlayFabClientAPI.GetFriendsList, request, customData, extraHeaders);
		}
		public static UniTask<GameServerRegionsResult> GetGameServerRegionsAsync(GameServerRegionsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GameServerRegionsRequest, GameServerRegionsResult>(PlayFabClientAPI.GetGameServerRegions, request, customData, extraHeaders);
		}
		public static UniTask<GetLeaderboardResult> GetLeaderboardAsync(GetLeaderboardRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetLeaderboardRequest, GetLeaderboardResult>(PlayFabClientAPI.GetLeaderboard, request, customData, extraHeaders);
		}
		public static UniTask<GetLeaderboardAroundCharacterResult> GetLeaderboardAroundCharacterAsync(GetLeaderboardAroundCharacterRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetLeaderboardAroundCharacterRequest, GetLeaderboardAroundCharacterResult>(PlayFabClientAPI.GetLeaderboardAroundCharacter, request, customData, extraHeaders);
		}
		public static UniTask<GetLeaderboardAroundPlayerResult> GetLeaderboardAroundPlayerAsync(GetLeaderboardAroundPlayerRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(PlayFabClientAPI.GetLeaderboardAroundPlayer, request, customData, extraHeaders);
		}
		public static UniTask<GetLeaderboardForUsersCharactersResult> GetLeaderboardForUserCharactersAsync(GetLeaderboardForUsersCharactersRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetLeaderboardForUsersCharactersRequest, GetLeaderboardForUsersCharactersResult>(PlayFabClientAPI.GetLeaderboardForUserCharacters, request, customData, extraHeaders);
		}
		public static UniTask<GetPaymentTokenResult> GetPaymentTokenAsync(GetPaymentTokenRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPaymentTokenRequest, GetPaymentTokenResult>(PlayFabClientAPI.GetPaymentToken, request, customData, extraHeaders);
		}
		public static UniTask<GetPhotonAuthenticationTokenResult> GetPhotonAuthenticationTokenAsync(GetPhotonAuthenticationTokenRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPhotonAuthenticationTokenRequest, GetPhotonAuthenticationTokenResult>(PlayFabClientAPI.GetPhotonAuthenticationToken, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerCombinedInfoResult> GetPlayerCombinedInfoAsync(GetPlayerCombinedInfoRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerCombinedInfoRequest, GetPlayerCombinedInfoResult>(PlayFabClientAPI.GetPlayerCombinedInfo, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerProfileResult> GetPlayerProfileAsync(GetPlayerProfileRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerProfileRequest, GetPlayerProfileResult>(PlayFabClientAPI.GetPlayerProfile, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerSegmentsResult> GetPlayerSegmentsAsync(GetPlayerSegmentsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerSegmentsRequest, GetPlayerSegmentsResult>(PlayFabClientAPI.GetPlayerSegments, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerStatisticsResult> GetPlayerStatisticsAsync(GetPlayerStatisticsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerStatisticsRequest, GetPlayerStatisticsResult>(PlayFabClientAPI.GetPlayerStatistics, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerStatisticVersionsResult> GetPlayerStatisticVersionsAsync(GetPlayerStatisticVersionsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerStatisticVersionsRequest, GetPlayerStatisticVersionsResult>(PlayFabClientAPI.GetPlayerStatisticVersions, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerTagsResult> GetPlayerTagsAsync(GetPlayerTagsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerTagsRequest, GetPlayerTagsResult>(PlayFabClientAPI.GetPlayerTags, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayerTradesResponse> GetPlayerTradesAsync(GetPlayerTradesRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayerTradesRequest, GetPlayerTradesResponse>(PlayFabClientAPI.GetPlayerTrades, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromFacebookIDsResult> GetPlayFabIDsFromFacebookIDsAsync(GetPlayFabIDsFromFacebookIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromFacebookIDsRequest, GetPlayFabIDsFromFacebookIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromFacebookIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromFacebookInstantGamesIdsResult> GetPlayFabIDsFromFacebookInstantGamesIdsAsync(GetPlayFabIDsFromFacebookInstantGamesIdsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromFacebookInstantGamesIdsRequest, GetPlayFabIDsFromFacebookInstantGamesIdsResult>(PlayFabClientAPI.GetPlayFabIDsFromFacebookInstantGamesIds, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromGameCenterIDsResult> GetPlayFabIDsFromGameCenterIDsAsync(GetPlayFabIDsFromGameCenterIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromGameCenterIDsRequest, GetPlayFabIDsFromGameCenterIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromGameCenterIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromGenericIDsResult> GetPlayFabIDsFromGenericIDsAsync(GetPlayFabIDsFromGenericIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromGenericIDsRequest, GetPlayFabIDsFromGenericIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromGenericIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromGoogleIDsResult> GetPlayFabIDsFromGoogleIDsAsync(GetPlayFabIDsFromGoogleIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromGoogleIDsRequest, GetPlayFabIDsFromGoogleIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromGoogleIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromKongregateIDsResult> GetPlayFabIDsFromKongregateIDsAsync(GetPlayFabIDsFromKongregateIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromKongregateIDsRequest, GetPlayFabIDsFromKongregateIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromKongregateIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult> GetPlayFabIDsFromNintendoSwitchDeviceIdsAsync(GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest, GetPlayFabIDsFromNintendoSwitchDeviceIdsResult>(PlayFabClientAPI.GetPlayFabIDsFromNintendoSwitchDeviceIds, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromPSNAccountIDsResult> GetPlayFabIDsFromPSNAccountIDsAsync(GetPlayFabIDsFromPSNAccountIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromPSNAccountIDsRequest, GetPlayFabIDsFromPSNAccountIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromPSNAccountIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromSteamIDsResult> GetPlayFabIDsFromSteamIDsAsync(GetPlayFabIDsFromSteamIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromSteamIDsRequest, GetPlayFabIDsFromSteamIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromSteamIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromTwitchIDsResult> GetPlayFabIDsFromTwitchIDsAsync(GetPlayFabIDsFromTwitchIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromTwitchIDsRequest, GetPlayFabIDsFromTwitchIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromTwitchIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPlayFabIDsFromXboxLiveIDsResult> GetPlayFabIDsFromXboxLiveIDsAsync(GetPlayFabIDsFromXboxLiveIDsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPlayFabIDsFromXboxLiveIDsRequest, GetPlayFabIDsFromXboxLiveIDsResult>(PlayFabClientAPI.GetPlayFabIDsFromXboxLiveIDs, request, customData, extraHeaders);
		}
		public static UniTask<GetPublisherDataResult> GetPublisherDataAsync(GetPublisherDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPublisherDataRequest, GetPublisherDataResult>(PlayFabClientAPI.GetPublisherData, request, customData, extraHeaders);
		}
		public static UniTask<GetPurchaseResult> GetPurchaseAsync(GetPurchaseRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetPurchaseRequest, GetPurchaseResult>(PlayFabClientAPI.GetPurchase, request, customData, extraHeaders);
		}
		public static UniTask<GetSharedGroupDataResult> GetSharedGroupDataAsync(GetSharedGroupDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetSharedGroupDataRequest, GetSharedGroupDataResult>(PlayFabClientAPI.GetSharedGroupData, request, customData, extraHeaders);
		}
		public static UniTask<GetStoreItemsResult> GetStoreItemsAsync(GetStoreItemsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetStoreItemsRequest, GetStoreItemsResult>(PlayFabClientAPI.GetStoreItems, request, customData, extraHeaders);
		}
		public static UniTask<GetTimeResult> GetTimeAsync(GetTimeRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetTimeRequest, GetTimeResult>(PlayFabClientAPI.GetTime, request, customData, extraHeaders);
		}
		public static UniTask<GetTitleDataResult> GetTitleDataAsync(GetTitleDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetTitleDataRequest, GetTitleDataResult>(PlayFabClientAPI.GetTitleData, request, customData, extraHeaders);
		}
		public static UniTask<GetTitleNewsResult> GetTitleNewsAsync(GetTitleNewsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetTitleNewsRequest, GetTitleNewsResult>(PlayFabClientAPI.GetTitleNews, request, customData, extraHeaders);
		}
		public static UniTask<GetTitlePublicKeyResult> GetTitlePublicKeyAsync(GetTitlePublicKeyRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetTitlePublicKeyRequest, GetTitlePublicKeyResult>(PlayFabClientAPI.GetTitlePublicKey, request, customData, extraHeaders);
		}
		public static UniTask<GetTradeStatusResponse> GetTradeStatusAsync(GetTradeStatusRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetTradeStatusRequest, GetTradeStatusResponse>(PlayFabClientAPI.GetTradeStatus, request, customData, extraHeaders);
		}
		public static UniTask<GetUserDataResult> GetUserDataAsync(GetUserDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetUserDataRequest, GetUserDataResult>(PlayFabClientAPI.GetUserData, request, customData, extraHeaders);
		}
		public static UniTask<GetUserInventoryResult> GetUserInventoryAsync(GetUserInventoryRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetUserInventoryRequest, GetUserInventoryResult>(PlayFabClientAPI.GetUserInventory, request, customData, extraHeaders);
		}
		public static UniTask<GetUserDataResult> GetUserPublisherDataAsync(GetUserDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetUserDataRequest, GetUserDataResult>(PlayFabClientAPI.GetUserPublisherData, request, customData, extraHeaders);
		}
		public static UniTask<GetUserDataResult> GetUserPublisherReadOnlyDataAsync(GetUserDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetUserDataRequest, GetUserDataResult>(PlayFabClientAPI.GetUserPublisherReadOnlyData, request, customData, extraHeaders);
		}
		public static UniTask<GetUserDataResult> GetUserReadOnlyDataAsync(GetUserDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetUserDataRequest, GetUserDataResult>(PlayFabClientAPI.GetUserReadOnlyData, request, customData, extraHeaders);
		}
		public static UniTask<GetWindowsHelloChallengeResponse> GetWindowsHelloChallengeAsync(GetWindowsHelloChallengeRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GetWindowsHelloChallengeRequest, GetWindowsHelloChallengeResponse>(PlayFabClientAPI.GetWindowsHelloChallenge, request, customData, extraHeaders);
		}
		public static UniTask<GrantCharacterToUserResult> GrantCharacterToUserAsync(GrantCharacterToUserRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<GrantCharacterToUserRequest, GrantCharacterToUserResult>(PlayFabClientAPI.GrantCharacterToUser, request, customData, extraHeaders);
		}
		public static UniTask<LinkAndroidDeviceIDResult> LinkAndroidDeviceIDAsync(LinkAndroidDeviceIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkAndroidDeviceIDRequest, LinkAndroidDeviceIDResult>(PlayFabClientAPI.LinkAndroidDeviceID, request, customData, extraHeaders);
		}
		public static UniTask<LinkCustomIDResult> LinkCustomIDAsync(LinkCustomIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkCustomIDRequest, LinkCustomIDResult>(PlayFabClientAPI.LinkCustomID, request, customData, extraHeaders);
		}
		public static UniTask<LinkFacebookAccountResult> LinkFacebookAccountAsync(LinkFacebookAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkFacebookAccountRequest, LinkFacebookAccountResult>(PlayFabClientAPI.LinkFacebookAccount, request, customData, extraHeaders);
		}
		public static UniTask<LinkFacebookInstantGamesIdResult> LinkFacebookInstantGamesIdAsync(LinkFacebookInstantGamesIdRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkFacebookInstantGamesIdRequest, LinkFacebookInstantGamesIdResult>(PlayFabClientAPI.LinkFacebookInstantGamesId, request, customData, extraHeaders);
		}
		public static UniTask<LinkGameCenterAccountResult> LinkGameCenterAccountAsync(LinkGameCenterAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkGameCenterAccountRequest, LinkGameCenterAccountResult>(PlayFabClientAPI.LinkGameCenterAccount, request, customData, extraHeaders);
		}
		public static UniTask<LinkGoogleAccountResult> LinkGoogleAccountAsync(LinkGoogleAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkGoogleAccountRequest, LinkGoogleAccountResult>(PlayFabClientAPI.LinkGoogleAccount, request, customData, extraHeaders);
		}
		public static UniTask<LinkIOSDeviceIDResult> LinkIOSDeviceIDAsync(LinkIOSDeviceIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkIOSDeviceIDRequest, LinkIOSDeviceIDResult>(PlayFabClientAPI.LinkIOSDeviceID, request, customData, extraHeaders);
		}
		public static UniTask<LinkKongregateAccountResult> LinkKongregateAsync(LinkKongregateAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkKongregateAccountRequest, LinkKongregateAccountResult>(PlayFabClientAPI.LinkKongregate, request, customData, extraHeaders);
		}
		public static UniTask<LinkNintendoSwitchDeviceIdResult> LinkNintendoSwitchDeviceIdAsync(LinkNintendoSwitchDeviceIdRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkNintendoSwitchDeviceIdRequest, LinkNintendoSwitchDeviceIdResult>(PlayFabClientAPI.LinkNintendoSwitchDeviceId, request, customData, extraHeaders);
		}
		public static UniTask<EmptyResult> LinkOpenIdConnectAsync(LinkOpenIdConnectRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkOpenIdConnectRequest, EmptyResult>(PlayFabClientAPI.LinkOpenIdConnect, request, customData, extraHeaders);
		}
		public static UniTask<LinkPSNAccountResult> LinkPSNAccountAsync(LinkPSNAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkPSNAccountRequest, LinkPSNAccountResult>(PlayFabClientAPI.LinkPSNAccount, request, customData, extraHeaders);
		}
		public static UniTask<LinkSteamAccountResult> LinkSteamAccountAsync(LinkSteamAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkSteamAccountRequest, LinkSteamAccountResult>(PlayFabClientAPI.LinkSteamAccount, request, customData, extraHeaders);
		}
		public static UniTask<LinkTwitchAccountResult> LinkTwitchAsync(LinkTwitchAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkTwitchAccountRequest, LinkTwitchAccountResult>(PlayFabClientAPI.LinkTwitch, request, customData, extraHeaders);
		}
		public static UniTask<LinkWindowsHelloAccountResponse> LinkWindowsHelloAsync(LinkWindowsHelloAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkWindowsHelloAccountRequest, LinkWindowsHelloAccountResponse>(PlayFabClientAPI.LinkWindowsHello, request, customData, extraHeaders);
		}
		public static UniTask<LinkXboxAccountResult> LinkXboxAccountAsync(LinkXboxAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LinkXboxAccountRequest, LinkXboxAccountResult>(PlayFabClientAPI.LinkXboxAccount, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithAndroidDeviceIDAsync(LoginWithAndroidDeviceIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithAndroidDeviceIDRequest, LoginResult>(PlayFabClientAPI.LoginWithAndroidDeviceID, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithCustomIDAsync(LoginWithCustomIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithCustomIDRequest, LoginResult>(PlayFabClientAPI.LoginWithCustomID, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithEmailAddressAsync(LoginWithEmailAddressRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithEmailAddressRequest, LoginResult>(PlayFabClientAPI.LoginWithEmailAddress, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithFacebookAsync(LoginWithFacebookRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithFacebookRequest, LoginResult>(PlayFabClientAPI.LoginWithFacebook, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithFacebookInstantGamesIdAsync(LoginWithFacebookInstantGamesIdRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithFacebookInstantGamesIdRequest, LoginResult>(PlayFabClientAPI.LoginWithFacebookInstantGamesId, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithGameCenterAsync(LoginWithGameCenterRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithGameCenterRequest, LoginResult>(PlayFabClientAPI.LoginWithGameCenter, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithGoogleAccountAsync(LoginWithGoogleAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithGoogleAccountRequest, LoginResult>(PlayFabClientAPI.LoginWithGoogleAccount, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithIOSDeviceIDAsync(LoginWithIOSDeviceIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithIOSDeviceIDRequest, LoginResult>(PlayFabClientAPI.LoginWithIOSDeviceID, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithKongregateAsync(LoginWithKongregateRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithKongregateRequest, LoginResult>(PlayFabClientAPI.LoginWithKongregate, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithNintendoSwitchDeviceIdAsync(LoginWithNintendoSwitchDeviceIdRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithNintendoSwitchDeviceIdRequest, LoginResult>(PlayFabClientAPI.LoginWithNintendoSwitchDeviceId, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithOpenIdConnectAsync(LoginWithOpenIdConnectRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithOpenIdConnectRequest, LoginResult>(PlayFabClientAPI.LoginWithOpenIdConnect, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithPlayFabAsync(LoginWithPlayFabRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithPlayFabRequest, LoginResult>(PlayFabClientAPI.LoginWithPlayFab, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithPSNAsync(LoginWithPSNRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithPSNRequest, LoginResult>(PlayFabClientAPI.LoginWithPSN, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithSteamAsync(LoginWithSteamRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithSteamRequest, LoginResult>(PlayFabClientAPI.LoginWithSteam, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithTwitchAsync(LoginWithTwitchRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithTwitchRequest, LoginResult>(PlayFabClientAPI.LoginWithTwitch, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithWindowsHelloAsync(LoginWithWindowsHelloRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithWindowsHelloRequest, LoginResult>(PlayFabClientAPI.LoginWithWindowsHello, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> LoginWithXboxAsync(LoginWithXboxRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<LoginWithXboxRequest, LoginResult>(PlayFabClientAPI.LoginWithXbox, request, customData, extraHeaders);
		}
		public static UniTask<MatchmakeResult> MatchmakeAsync(MatchmakeRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<MatchmakeRequest, MatchmakeResult>(PlayFabClientAPI.Matchmake, request, customData, extraHeaders);
		}
		public static UniTask<OpenTradeResponse> OpenTradeAsync(OpenTradeRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<OpenTradeRequest, OpenTradeResponse>(PlayFabClientAPI.OpenTrade, request, customData, extraHeaders);
		}
		public static UniTask<PayForPurchaseResult> PayForPurchaseAsync(PayForPurchaseRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<PayForPurchaseRequest, PayForPurchaseResult>(PlayFabClientAPI.PayForPurchase, request, customData, extraHeaders);
		}
		public static UniTask<PurchaseItemResult> PurchaseItemAsync(PurchaseItemRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<PurchaseItemRequest, PurchaseItemResult>(PlayFabClientAPI.PurchaseItem, request, customData, extraHeaders);
		}
		public static UniTask<RedeemCouponResult> RedeemCouponAsync(RedeemCouponRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RedeemCouponRequest, RedeemCouponResult>(PlayFabClientAPI.RedeemCoupon, request, customData, extraHeaders);
		}
		public static UniTask<EmptyResponse> RefreshPSNAuthTokenAsync(RefreshPSNAuthTokenRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RefreshPSNAuthTokenRequest, EmptyResponse>(PlayFabClientAPI.RefreshPSNAuthToken, request, customData, extraHeaders);
		}
		public static UniTask<RegisterForIOSPushNotificationResult> RegisterForIOSPushNotificationAsync(RegisterForIOSPushNotificationRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RegisterForIOSPushNotificationRequest, RegisterForIOSPushNotificationResult>(PlayFabClientAPI.RegisterForIOSPushNotification, request, customData, extraHeaders);
		}
		public static UniTask<RegisterPlayFabUserResult> RegisterPlayFabUserAsync(RegisterPlayFabUserRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RegisterPlayFabUserRequest, RegisterPlayFabUserResult>(PlayFabClientAPI.RegisterPlayFabUser, request, customData, extraHeaders);
		}
		public static UniTask<LoginResult> RegisterWithWindowsHelloAsync(RegisterWithWindowsHelloRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RegisterWithWindowsHelloRequest, LoginResult>(PlayFabClientAPI.RegisterWithWindowsHello, request, customData, extraHeaders);
		}
		public static UniTask<RemoveContactEmailResult> RemoveContactEmailAsync(RemoveContactEmailRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RemoveContactEmailRequest, RemoveContactEmailResult>(PlayFabClientAPI.RemoveContactEmail, request, customData, extraHeaders);
		}
		public static UniTask<RemoveFriendResult> RemoveFriendAsync(RemoveFriendRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RemoveFriendRequest, RemoveFriendResult>(PlayFabClientAPI.RemoveFriend, request, customData, extraHeaders);
		}
		public static UniTask<RemoveGenericIDResult> RemoveGenericIDAsync(RemoveGenericIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RemoveGenericIDRequest, RemoveGenericIDResult>(PlayFabClientAPI.RemoveGenericID, request, customData, extraHeaders);
		}
		public static UniTask<RemoveSharedGroupMembersResult> RemoveSharedGroupMembersAsync(RemoveSharedGroupMembersRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RemoveSharedGroupMembersRequest, RemoveSharedGroupMembersResult>(PlayFabClientAPI.RemoveSharedGroupMembers, request, customData, extraHeaders);
		}
		public static UniTask<EmptyResponse> ReportDeviceInfoAsync(DeviceInfoRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<DeviceInfoRequest, EmptyResponse>(PlayFabClientAPI.ReportDeviceInfo, request, customData, extraHeaders);
		}
		public static UniTask<ReportPlayerClientResult> ReportPlayerAsync(ReportPlayerClientRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ReportPlayerClientRequest, ReportPlayerClientResult>(PlayFabClientAPI.ReportPlayer, request, customData, extraHeaders);
		}
		public static UniTask<RestoreIOSPurchasesResult> RestoreIOSPurchasesAsync(RestoreIOSPurchasesRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<RestoreIOSPurchasesRequest, RestoreIOSPurchasesResult>(PlayFabClientAPI.RestoreIOSPurchases, request, customData, extraHeaders);
		}
		public static UniTask<SendAccountRecoveryEmailResult> SendAccountRecoveryEmailAsync(SendAccountRecoveryEmailRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<SendAccountRecoveryEmailRequest, SendAccountRecoveryEmailResult>(PlayFabClientAPI.SendAccountRecoveryEmail, request, customData, extraHeaders);
		}
		public static UniTask<SetFriendTagsResult> SetFriendTagsAsync(SetFriendTagsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<SetFriendTagsRequest, SetFriendTagsResult>(PlayFabClientAPI.SetFriendTags, request, customData, extraHeaders);
		}
		public static UniTask<SetPlayerSecretResult> SetPlayerSecretAsync(SetPlayerSecretRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<SetPlayerSecretRequest, SetPlayerSecretResult>(PlayFabClientAPI.SetPlayerSecret, request, customData, extraHeaders);
		}
		public static UniTask<StartGameResult> StartGameAsync(StartGameRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<StartGameRequest, StartGameResult>(PlayFabClientAPI.StartGame, request, customData, extraHeaders);
		}
		public static UniTask<StartPurchaseResult> StartPurchaseAsync(StartPurchaseRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<StartPurchaseRequest, StartPurchaseResult>(PlayFabClientAPI.StartPurchase, request, customData, extraHeaders);
		}
		public static UniTask<ModifyUserVirtualCurrencyResult> SubtractUserVirtualCurrencyAsync(SubtractUserVirtualCurrencyRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<SubtractUserVirtualCurrencyRequest, ModifyUserVirtualCurrencyResult>(PlayFabClientAPI.SubtractUserVirtualCurrency, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkAndroidDeviceIDResult> UnlinkAndroidDeviceIDAsync(UnlinkAndroidDeviceIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkAndroidDeviceIDRequest, UnlinkAndroidDeviceIDResult>(PlayFabClientAPI.UnlinkAndroidDeviceID, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkCustomIDResult> UnlinkCustomIDAsync(UnlinkCustomIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkCustomIDRequest, UnlinkCustomIDResult>(PlayFabClientAPI.UnlinkCustomID, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkFacebookAccountResult> UnlinkFacebookAccountAsync(UnlinkFacebookAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkFacebookAccountRequest, UnlinkFacebookAccountResult>(PlayFabClientAPI.UnlinkFacebookAccount, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkFacebookInstantGamesIdResult> UnlinkFacebookInstantGamesIdAsync(UnlinkFacebookInstantGamesIdRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkFacebookInstantGamesIdRequest, UnlinkFacebookInstantGamesIdResult>(PlayFabClientAPI.UnlinkFacebookInstantGamesId, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkGameCenterAccountResult> UnlinkGameCenterAccountAsync(UnlinkGameCenterAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkGameCenterAccountRequest, UnlinkGameCenterAccountResult>(PlayFabClientAPI.UnlinkGameCenterAccount, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkGoogleAccountResult> UnlinkGoogleAccountAsync(UnlinkGoogleAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkGoogleAccountRequest, UnlinkGoogleAccountResult>(PlayFabClientAPI.UnlinkGoogleAccount, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkIOSDeviceIDResult> UnlinkIOSDeviceIDAsync(UnlinkIOSDeviceIDRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkIOSDeviceIDRequest, UnlinkIOSDeviceIDResult>(PlayFabClientAPI.UnlinkIOSDeviceID, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkKongregateAccountResult> UnlinkKongregateAsync(UnlinkKongregateAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkKongregateAccountRequest, UnlinkKongregateAccountResult>(PlayFabClientAPI.UnlinkKongregate, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkNintendoSwitchDeviceIdResult> UnlinkNintendoSwitchDeviceIdAsync(UnlinkNintendoSwitchDeviceIdRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkNintendoSwitchDeviceIdRequest, UnlinkNintendoSwitchDeviceIdResult>(PlayFabClientAPI.UnlinkNintendoSwitchDeviceId, request, customData, extraHeaders);
		}
		// public static UniTask<EmptyResponse> UnlinkOpenIdConnectAsync(UninkOpenIdConnectRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		// {
		// 	return ToUniTask<UninkOpenIdConnectRequest, EmptyResponse>(PlayFabClientAPI.UnlinkOpenIdConnect, request, customData, extraHeaders);
		// }
		public static UniTask<UnlinkPSNAccountResult> UnlinkPSNAccountAsync(UnlinkPSNAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkPSNAccountRequest, UnlinkPSNAccountResult>(PlayFabClientAPI.UnlinkPSNAccount, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkSteamAccountResult> UnlinkSteamAccountAsync(UnlinkSteamAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkSteamAccountRequest, UnlinkSteamAccountResult>(PlayFabClientAPI.UnlinkSteamAccount, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkTwitchAccountResult> UnlinkTwitchAsync(UnlinkTwitchAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkTwitchAccountRequest, UnlinkTwitchAccountResult>(PlayFabClientAPI.UnlinkTwitch, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkWindowsHelloAccountResponse> UnlinkWindowsHelloAsync(UnlinkWindowsHelloAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkWindowsHelloAccountRequest, UnlinkWindowsHelloAccountResponse>(PlayFabClientAPI.UnlinkWindowsHello, request, customData, extraHeaders);
		}
		public static UniTask<UnlinkXboxAccountResult> UnlinkXboxAccountAsync(UnlinkXboxAccountRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlinkXboxAccountRequest, UnlinkXboxAccountResult>(PlayFabClientAPI.UnlinkXboxAccount, request, customData, extraHeaders);
		}
		public static UniTask<UnlockContainerItemResult> UnlockContainerInstanceAsync(UnlockContainerInstanceRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlockContainerInstanceRequest, UnlockContainerItemResult>(PlayFabClientAPI.UnlockContainerInstance, request, customData, extraHeaders);
		}
		public static UniTask<UnlockContainerItemResult> UnlockContainerItemAsync(UnlockContainerItemRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UnlockContainerItemRequest, UnlockContainerItemResult>(PlayFabClientAPI.UnlockContainerItem, request, customData, extraHeaders);
		}
		public static UniTask<EmptyResponse> UpdateAvatarUrlAsync(UpdateAvatarUrlRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateAvatarUrlRequest, EmptyResponse>(PlayFabClientAPI.UpdateAvatarUrl, request, customData, extraHeaders);
		}
		public static UniTask<UpdateCharacterDataResult> UpdateCharacterDataAsync(UpdateCharacterDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateCharacterDataRequest, UpdateCharacterDataResult>(PlayFabClientAPI.UpdateCharacterData, request, customData, extraHeaders);
		}
		public static UniTask<UpdateCharacterStatisticsResult> UpdateCharacterStatisticsAsync(UpdateCharacterStatisticsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateCharacterStatisticsRequest, UpdateCharacterStatisticsResult>(PlayFabClientAPI.UpdateCharacterStatistics, request, customData, extraHeaders);
		}
		public static UniTask<UpdatePlayerStatisticsResult> UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdatePlayerStatisticsRequest, UpdatePlayerStatisticsResult>(PlayFabClientAPI.UpdatePlayerStatistics, request, customData, extraHeaders);
		}
		public static UniTask<UpdateSharedGroupDataResult> UpdateSharedGroupDataAsync(UpdateSharedGroupDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateSharedGroupDataRequest, UpdateSharedGroupDataResult>(PlayFabClientAPI.UpdateSharedGroupData, request, customData, extraHeaders);
		}
		public static UniTask<UpdateUserDataResult> UpdateUserDataAsync(UpdateUserDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateUserDataRequest, UpdateUserDataResult>(PlayFabClientAPI.UpdateUserData, request, customData, extraHeaders);
		}
		public static UniTask<UpdateUserDataResult> UpdateUserPublisherDataAsync(UpdateUserDataRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateUserDataRequest, UpdateUserDataResult>(PlayFabClientAPI.UpdateUserPublisherData, request, customData, extraHeaders);
		}
		public static UniTask<UpdateUserTitleDisplayNameResult> UpdateUserTitleDisplayNameAsync(UpdateUserTitleDisplayNameRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(PlayFabClientAPI.UpdateUserTitleDisplayName, request, customData, extraHeaders);
		}
		public static UniTask<ValidateAmazonReceiptResult> ValidateAmazonIAPReceiptAsync(ValidateAmazonReceiptRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ValidateAmazonReceiptRequest, ValidateAmazonReceiptResult>(PlayFabClientAPI.ValidateAmazonIAPReceipt, request, customData, extraHeaders);
		}
		public static UniTask<ValidateGooglePlayPurchaseResult> ValidateGooglePlayPurchaseAsync(ValidateGooglePlayPurchaseRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ValidateGooglePlayPurchaseRequest, ValidateGooglePlayPurchaseResult>(PlayFabClientAPI.ValidateGooglePlayPurchase, request, customData, extraHeaders);
		}
		public static UniTask<ValidateIOSReceiptResult> ValidateIOSReceiptAsync(ValidateIOSReceiptRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ValidateIOSReceiptRequest, ValidateIOSReceiptResult>(PlayFabClientAPI.ValidateIOSReceipt, request, customData, extraHeaders);
		}
		public static UniTask<ValidateWindowsReceiptResult> ValidateWindowsStoreReceiptAsync(ValidateWindowsReceiptRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<ValidateWindowsReceiptRequest, ValidateWindowsReceiptResult>(PlayFabClientAPI.ValidateWindowsStoreReceipt, request, customData, extraHeaders);
		}
		public static UniTask<WriteEventResponse> WriteCharacterEventAsync(WriteClientCharacterEventRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<WriteClientCharacterEventRequest, WriteEventResponse>(PlayFabClientAPI.WriteCharacterEvent, request, customData, extraHeaders);
		}
		public static UniTask<WriteEventResponse> WritePlayerEventAsync(WriteClientPlayerEventRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<WriteClientPlayerEventRequest, WriteEventResponse>(PlayFabClientAPI.WritePlayerEvent, request, customData, extraHeaders);
		}
		public static UniTask<WriteEventResponse> WriteTitleEventAsync(WriteTitleEventRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return ToUniTask<WriteTitleEventRequest, WriteEventResponse>(PlayFabClientAPI.WriteTitleEvent, request, customData, extraHeaders);
		}
		private static UniTask<TResult> ToUniTask<TRequest, TResult>(Action<TRequest, Action<TResult>, Action<PlayFabError>, object, Dictionary<string, string>> api, TRequest request, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			var source = new UniTaskCompletionSource<TResult>();
			api(request , x => source.TrySetResult(x), x => source.TrySetException(new Exception(x.GenerateErrorReport())), customData, extraHeaders);
			return source.Task;
		}
	}
}
#endif

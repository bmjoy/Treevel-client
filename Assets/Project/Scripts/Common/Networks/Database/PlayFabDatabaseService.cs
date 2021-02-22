using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Treevel.Common.Networks.PlayFab;
using UnityEngine;

namespace Treevel.Common.Networks.Database
{
    /// <summary>
    /// PlayFabは仕様上、API呼び出す時に渡すエラーコールバック以外、エラーの検知はできない（リザルト構造体には成功状態が含まれていない）。
    /// そのため、taskの状態でAPI呼び出しの成功判定を行います。
    /// </summary>
    public class PlayFabDatabaseService : DatabaseService
    {
        public override async UniTask<T> GetData<T>(string key)
        {
            var request = new GetUserDataRequest
            {
                Keys = new List<string>{ key },
            };

            try {
                var task = PlayFabClientAPIAsync.GetUserDataAsync(request);
                var result = await task;

                if (task.Status != UniTaskStatus.Succeeded) {
                    throw new NetworkErrorException();
                }

                return JsonUtility.FromJson<T>(result.Data[key].Value);
            } catch (Exception e) {
                // ローカルに切り替えるため呼び出し先に投げる
                Debug.LogError(e.Message + e.StackTrace);
                throw;
            }
        }

        public override async UniTask<bool> UpdateData<T>(string key, T data)
        {
            Debug.Assert(typeof(T).IsSerializable, $"type {typeof(T)} should be serializable");

            var value = JsonUtility.ToJson(data);

            var request = new UpdateUserDataRequest {
                Data = new Dictionary<string, string> { { key, value } },
                Permission = UserDataPermission.Private,
            };

            try {
                var task = PlayFabClientAPIAsync.UpdateUserDataAsync(request);
                // UpdateUserDataResultは現状使いところがないが、念の為変数で保存しておく
                var result = await task;

                // 成功状態を返す
                return task.Status == UniTaskStatus.Succeeded;
            } catch(Exception e) {
                // ローカルに切り替えるため呼び出し先に投げる
                Debug.LogError(e.Message + e.StackTrace);
                throw;
            }
        }

        public override async UniTask<bool> Login()
        {
            // 色々オプション付けれるやつ
            var infoRequestParams = new GetPlayerCombinedInfoRequestParams();

            var request = new LoginWithCustomIDRequest {
                TitleId = PlayFabSettings.TitleId,
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = infoRequestParams,
            };
            var task = PlayFabClientAPIAsync.LoginWithCustomIDAsync(request);
            // LoginResultは現状使いところがないが、念の為変数で保存しておく
            var result = await task;

            return task.Status == UniTaskStatus.Succeeded;
        }
    }
}

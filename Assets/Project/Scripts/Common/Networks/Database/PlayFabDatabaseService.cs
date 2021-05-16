using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Treevel.Common.Networks.PlayFab;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Networks.Database
{
    /// <summary>
    /// PlayFabは仕様上、API呼び出す時に渡すエラーコールバック以外、エラーの検知はできない（リザルト構造体には成功状態が含まれていない）。
    /// そのため、taskの状態でAPI呼び出しの成功判定を行います。
    /// </summary>
    public class PlayFabDatabaseService : IDatabaseService
    {
        public async UniTask<T> GetDataAsync<T>(string key)
        {
            var request = new GetUserDataRequest
            {
                Keys = new List<string>{ key },
            };

            var task = PlayFabClientAPIAsync.GetUserDataAsync(request);
            var result = await task;

            if (task.Status != UniTaskStatus.Succeeded) {
                throw new NetworkErrorException();
            }

            if (!result.Data.ContainsKey(key)) {
                // PlayFab にデータがない場合、DataException とする
                throw new DataException();
            }

            return JsonUtility.FromJson<T>(result.Data[key].Value);
        }

        public async UniTask<IEnumerable<T>> GetListDataAsync<T>(IEnumerable<string> keys)
        {
            var request = new GetUserDataRequest {
                Keys = keys.ToList(),
            };

            try {
                var task = PlayFabClientAPIAsync.GetUserDataAsync(request);
                var result = await task;

                if (task.Status != UniTaskStatus.Succeeded) {
                    throw new NetworkErrorException();
                }

                return result.Data.Values
                    .Select(record => JsonUtility.FromJson<T>(record.Value));
            } catch (Exception e) {
                Debug.LogError(e.Message + e.StackTrace);
                throw;
            }
        }

        public async UniTask<bool> UpdateDataAsync<T>(string key, T data)
        {
            if (key == string.Empty) {
                Debug.LogWarning("Key cannot be empty");
                return true;
            }

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
                if (task.Status == UniTaskStatus.Succeeded) {
                    Debug.Log($"key: {key} で PlayFab に保存しました");
                    return true;
                }

                return false;
            } catch(Exception e) {
                // ローカルに切り替えるため呼び出し先に投げる
                Debug.LogError(e.Message + e.StackTrace);
                throw;
            }
        }

        public async UniTask<bool> LoginAsync()
        {
            // TODO 引き継ぎ行った場合の処理

            // 引き継ぎ行っていない場合：発行したGUID or GUIDを新規発行 でログイン
            var createAccount = false;
            string customId;
            if (PlayerPrefs.HasKey(Constants.PlayerPrefsKeys.DATABASE_LOGIN_ID)) {
                customId = PlayerPrefs.GetString(Constants.PlayerPrefsKeys.DATABASE_LOGIN_ID);
            } else {
                createAccount = true;
                customId = Guid.NewGuid().ToString();
            }

            // ログインと同時に取得したいデータを指定できます
            var infoRequestParams = new GetPlayerCombinedInfoRequestParams();

            #if UNITY_IOS
            var request = new LoginWithIOSDeviceIDRequest() {
                TitleId = PlayFabSettings.TitleId,
                DeviceId = customId,
                DeviceModel = SystemInfo.deviceModel,
                OS = SystemInfo.operatingSystem,
                CreateAccount = createAccount,
            };
            var task = PlayFabClientAPIAsync.LoginWithIOSDeviceIDAsync(request);
            #elif UNITY_ANDROID
            // Get the device id from native android
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            var secure = new AndroidJavaClass("android.provider.Settings$Secure");
            var deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");

            // Login with the android device ID
            var request = new LoginWithAndroidDeviceIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                AndroidDevice = SystemInfo.deviceModel,
                OS = SystemInfo.operatingSystem,
                AndroidDeviceId = customId,
                CreateAccount = createAccount,
                InfoRequestParameters = infoRequestParams
            };
            var task = PlayFabClientAPIAsync.LoginWithAndroidDeviceIDAsync(request);
            #else
            var request = new LoginWithCustomIDRequest {
                TitleId = PlayFabSettings.TitleId,
                CustomId = customId,
                CreateAccount = createAccount,
                InfoRequestParameters = infoRequestParams,
            };
            var task = PlayFabClientAPIAsync.LoginWithCustomIDAsync(request);
            #endif

            // LoginResultは現状使いところがないが、念の為変数で保存しておく
            var result = await task;

            var isSuccess = task.Status == UniTaskStatus.Succeeded;

            // 新規作成の場合PlayerPrefsに保存
            if (isSuccess && createAccount) {
                PlayerPrefs.SetString(Constants.PlayerPrefsKeys.DATABASE_LOGIN_ID, customId);
            }

            return isSuccess;
        }

        public async UniTask<bool> DeleteDataAsync(IEnumerable<string> keys)
        {
            var request = new UpdateUserDataRequest {
                KeysToRemove = keys.ToList(),
            };

            try {
                var task = PlayFabClientAPIAsync.UpdateUserDataAsync(request);
                // UpdateUserDataResultは現状使いところがないが、念の為変数で保存しておく
                var result = await task;

                // 成功状態を返す
                if (task.Status == UniTaskStatus.Succeeded) {
                    return true;
                }

                return false;
            } catch(Exception e) {
                // ローカルに切り替えるため呼び出し先に投げる
                Debug.LogError(e.Message + e.StackTrace);
                throw;
            }
        }
    }
}

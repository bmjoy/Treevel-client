using System.Collections.Generic;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Project.Scripts.MenuSelectScene;

namespace Project.Scripts.Utils
{
    public static class AddressableAssetManager
    {
        /// <summary>
        /// 初期化フラグ
        /// </summary>
        static private bool _initialized = false;

        /// <summary>
        /// アンロードのためにハンドルを一時保存
        /// </summary>
        /// <typeparam name="string">アッセとのアドレス（キー）</typeparam>
        /// <typeparam name="AsyncOperationHandle">ロードに用いたハンドル</typeparam>
        /// <returns></returns>
        static private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new Dictionary<string, AsyncOperationHandle>();

        /// <summary>
        /// AASを初期化
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        static public void Initalize()
        {
            Addressables.InitializeAsync().Completed += (obj) => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    _initialized = true;
                } else {
                    throw new System.Exception("Fail to initialize Addressable Asset System.");
                }
            };
        }

        /// <summary>
        /// シーンをロードする
        /// </summary>
        /// <param name="sceneName">ロードするシーンのaddress</param>
        /// <param name="loadSceneMode">ロードモード（Single/Additive)を指定</param>
        /// <returns>呼び出し先もイベントを登録できるよう、ハンドルを返す</returns>
        static public AsyncOperationHandle<SceneInstance> LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            // 辞書にシーンのインスタンスが入ってる場合
            if (_loadedAssets.ContainsKey(sceneName)) {
                var scene = ((SceneInstance)_loadedAssets[sceneName].Result).Scene;

                if (!scene.isLoaded) {
                    // 自動でアンロードされたら削除（他のシーンがSingleでロードした時）
                    _loadedAssets.Remove(sceneName);
                } else {
                    // シーンがすでにロードしている
                    return default;
                }
            }

            var ret = Addressables.LoadSceneAsync(sceneName, loadSceneMode);

            // プログレスバーを表示
            UIManager.Instance.ShowProgress(ret);

            ret.Completed += (obj) => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    // ロード完了したら、ハンドルを保存する（今後アンロードするために）
                    _loadedAssets.Add(sceneName, ret);
                } else {
                    // TODO popup error message box, return to top
                    Debug.LogError($"Load Scene {sceneName} Failed.");
                }
            };
            return ret;
        }

        /// <summary>
        /// シーンをアンロードする
        /// </summary>
        /// <param name="sceneName">アンロードするシーンのaddress</param>
        /// <returns>呼び出し先もイベントを登録できるよう、ハンドルを返す</returns>
        static public AsyncOperationHandle<SceneInstance> UnloadScene(string sceneName)
        {
            // シーンがロードしていなければ終了
            if (!_loadedAssets.ContainsKey(sceneName))
                return default;

            var handle = _loadedAssets[sceneName];
            var ret = Addressables.UnloadSceneAsync(handle);
            ret.Completed += (obj) => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    // アンロード終了後、辞書から削除
                    _loadedAssets.Remove(sceneName);
                } else {
                    // TODO popup error message box, return to top
                    Debug.LogError($"Unload Scene {sceneName} Failed.");
                }
            };

            return ret;
        }
    }
}

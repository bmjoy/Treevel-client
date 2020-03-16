using System.Collections.Generic;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Project.Scripts.Utils
{
    public class AddressableAssetManager : Singleton<AddressableAssetManager>
    {
        /// <summary>
        /// あとでアンロードするために保持するロードした各シーンのハンドル
        /// </summary>
        private readonly Dictionary<string, SceneInstance> _loadedScenes;

        public AddressableAssetManager()
        {
            _loadedScenes = new Dictionary<string, SceneInstance>();
        }

        /// <summary>
        /// シーンをロードする
        /// </summary>
        /// <param name="sceneName">ロードするシーンのaddress</param>
        /// <param name="loadSceneMode">ロードモード（Single/Additive)を指定</param>
        /// <returns>呼び出し先もイベントを登録できるよう、ハンドルを返す</returns>
        public AsyncOperationHandle<SceneInstance> LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            // 辞書にシーンのインスタンスが入ってる場合
            if (_loadedScenes.ContainsKey(sceneName)) {
                var scene = _loadedScenes[sceneName].Scene;

                if (!scene.isLoaded) {
                    // 自動でアンロードされたら削除（他のシーンがSingleでロードした時）
                    _loadedScenes.Remove(sceneName);
                } else {
                    // シーンがすでにロードしている
                    return default;
                }
            }

            var ret = Addressables.LoadSceneAsync(sceneName, loadSceneMode);

            // TODO show progress bar

            ret.Completed += (obj) => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    // ロード完了したら、ハンドルを保存する（今後アンロードするために）
                    _loadedScenes.Add(sceneName, ret.Result);
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
        public AsyncOperationHandle<SceneInstance> UnloadScene(string sceneName)
        {
            // シーンがロードしていなければ終了
            if (!_loadedScenes.ContainsKey(sceneName))
                return default;

            var handle = _loadedScenes[sceneName];
            var ret = Addressables.UnloadSceneAsync(handle);
            ret.Completed += (obj) => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    // アンロード終了後、辞書から削除
                    _loadedScenes.Remove(sceneName);
                } else {
                    // TODO popup error message box, return to top
                    Debug.LogError($"Unload Scene {sceneName} Failed.");
                }
            };

            return ret;
        }
    }
}

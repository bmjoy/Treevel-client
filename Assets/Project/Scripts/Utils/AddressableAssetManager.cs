using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.GameDatas;
using System.Threading.Tasks;

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
        static private readonly Dictionary<object, AsyncOperationHandle> _loadedAssets = new Dictionary<object, AsyncOperationHandle>();

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
        /// 
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        static public AsyncOperationHandle<TObject> LoadAsset<TObject> (object key)
        {
            if (_loadedAssets.ContainsKey(key)) {
                return _loadedAssets[key].Convert<TObject>();
            }

            var op = Addressables.LoadAssetAsync<TObject>(key);

            _loadedAssets.Add(key, op);

            UIManager.Instance.ProgressBar.Load(op);
            
            return op;
        }

        /// <summary>
        /// ロードしたアセットを取得する
        /// （ロードしていなければロードするまで待つ）
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        static public async Task<TObject> GetAsset<TObject> (object key)
        {
            if (_loadedAssets.ContainsKey(key)) {
                return _loadedAssets[key].Convert<TObject>().Result;
            }

            await LoadAsset<TObject>(key).Task;

            return _loadedAssets[key].Convert<TObject>().Result;
        }


        /// <summary>
        /// シーンをロードする
        /// </summary>
        /// <param name="sceneName">ロードするシーンのaddress</param>
        /// <param name="loadSceneMode">ロードモード（Single/Additive)を指定</param>
        /// <returns>呼び出し先もイベントを登録できるよう、ハンドルを返す</returns>
        static public AsyncOperationHandle<SceneInstance> LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            //// 辞書にシーンのインスタンスが入ってる場合
            //if (loadSceneMode != LoadSceneMode.Single && _loadedAssets.ContainsKey(sceneName)) {
            //    var scene = ((SceneInstance)_loadedAssets[sceneName].Result).Scene;

            //    if (!scene.isLoaded) {
            //        // 自動でアンロードされたら削除（他のシーンがSingleでロードした時）
            //        _loadedAssets.Remove(sceneName);
            //    } else {
            //        // シーンがすでにロードしている
            //        return default;
            //    }
            //}

            var ret = Addressables.LoadSceneAsync(sceneName, loadSceneMode);

            // 辞書に追加
            //_loadedAssets.Add(sceneName, ret);

            // プログレスバーを表示
            UIManager.Instance.ProgressBar.Load(ret);

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <param name="instantiateInWorldSpace"></param>
        /// <returns></returns>
        public static AsyncOperationHandle<GameObject> Instantiate(object key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);

            return op;
        }


        /// <summary>
        /// ステージに必要なアセットをロード
        /// </summary>
        /// <param name="stageId"></param>
        internal static void LoadStageDependecies(int stageId)
        {
            StageData stage = GameDataBase.Instance.GetStage(stageId);

            stage.PanelDatas.ForEach((panelData) => {
                switch (panelData.type) {
                    case Definitions.EPanelType.Dynamic:
                        LoadAsset<GameObject>("dynamicDummyPanelPrefab");
                        LoadAsset<Sprite>("dynamicDummyPanel");
                        break;
                    case Definitions.EPanelType.Static:
                        LoadAsset<GameObject>("staticDummyPanelPrefab");
                        LoadAsset<Sprite>("staticDummyPanel");
                        break;
                    case Definitions.EPanelType.Number:
                        LoadAsset<GameObject>("numberPanelPrefab");
                        LoadAsset<Sprite>($"numberPanel{panelData.number}");
                        break;
                    case Definitions.EPanelType.LifeNumber:
                        LoadAsset<GameObject>("lifeNumberPanelPrefab");
                        LoadAsset<Sprite>($"lifeNumberPanel{panelData.number}");
                        break;
                }
            });
        }
    }
}

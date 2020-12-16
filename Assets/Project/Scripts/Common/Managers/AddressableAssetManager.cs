using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Treevel.Common.Managers
{
    public static class AddressableAssetManager
    {
        /// <summary>
        /// 初期化フラグ
        /// </summary>
        private static bool _initialized = false;

        public static readonly Subject<AsyncOperationHandle> OnAssetStartLoad = new Subject<AsyncOperationHandle>();

        /// <summary>
        /// アンロードのためにハンドルを一時保存
        /// </summary>
        /// <typeparam name="string">アッセとのアドレス（キー）</typeparam>
        /// <typeparam name="AsyncOperationHandle">ロードに用いたハンドル</typeparam>
        /// <returns></returns>
        private static readonly Dictionary<object, AsyncOperationHandle> _loadedAssets = new Dictionary<object, AsyncOperationHandle>();

        /// <summary>
        /// AASを初期化
        /// </summary>
        public static AsyncOperationHandle Initialize()
        {
            var handle =  Addressables.InitializeAsync();
            handle.Completed += (obj) => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    _initialized = true;
                } else {
                    throw new System.Exception("Fail to initialize Addressable Asset System.");
                }
            };
            return handle;
        }

        /// <summary>
        /// アセットをロードする
        /// </summary>
        /// <typeparam name="TObject">ロードするアセットの型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static AsyncOperationHandle<TObject> LoadAsset<TObject> (object key)
        {
            if (_loadedAssets.ContainsKey(key)) {
                return _loadedAssets[key].Convert<TObject>();
            }

            var op = Addressables.LoadAssetAsync<TObject>(key);

            _loadedAssets.Add(key, op);

            OnAssetStartLoad.OnNext(op);

            return op;
        }

        /// <summary>
        /// ロードしたアセットを取得する
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key">アドレス</param>
        /// <returns></returns>
        public static TObject GetAsset<TObject> (object key)
        {
            if (_loadedAssets.ContainsKey(key)) {
                return _loadedAssets[key].Convert<TObject>().Result;
            } else {
                Debug.LogWarning($"Asset with key:{key} is not loaded.");
                return default;
            }
        }


        /// <summary>
        /// シーンをロードする
        /// </summary>
        /// <param name="sceneName">ロードするシーンのaddress</param>
        /// <param name="loadSceneMode">ロードモード（Single/Additive)を指定</param>
        /// <returns>呼び出し先もイベントを登録できるよう、ハンドルを返す</returns>
        public static AsyncOperationHandle<SceneInstance> LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
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
            OnAssetStartLoad.OnNext(ret);

            return ret;
        }

        /// <summary>
        /// シーンをアンロードする
        /// </summary>
        /// <param name="sceneName">アンロードするシーンのaddress</param>
        /// <returns>呼び出し先もイベントを登録できるよう、ハンドルを返す</returns>
        public static AsyncOperationHandle<SceneInstance> UnloadScene(string sceneName)
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
                    UIManager.Instance.ShowErrorMessage(EErrorCode.LoadDataError);
                }
            };

            return ret;
        }

        /// <summary>
        /// プレハブを実体化させる
        /// </summary>
        /// <param name="key">キー（アドレス）</param>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="instantiateInWorldSpace">Option to retain world space when instantiated with a parent.</param>
        /// <returns></returns>
        public static AsyncOperationHandle<GameObject> Instantiate(object key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);

            return op;
        }


        /// <summary>
        /// ステージに必要なアセットをロード
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        internal static async UniTask LoadStageDependencies(ETreeId treeId, int stageNumber)
        {
            StageData stage = GameDataManager.GetStage(treeId, stageNumber);

            var tasks = new List<UniTask>();
            stage.BottleDatas.ForEach((bottleData) => {
                switch (bottleData.type) {
                    case EBottleType.Dynamic:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.DYNAMIC_DUMMY_BOTTLE_PREFAB).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.DYNAMIC_DUMMY_BOTTLE_SPRITE).ToUniTask());
                        break;
                    case EBottleType.Static:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.STATIC_DUMMY_BOTTLE_PREFAB).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.STATIC_DUMMY_BOTTLE_SPRITE).ToUniTask());
                        break;
                    case EBottleType.Normal:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.NORMAL_BOTTLE_PREFAB).ToUniTask());
                        break;
                    case EBottleType.AttackableDummy:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB).ToUniTask());
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
                if (bottleData.bottleSprite.RuntimeKeyIsValid()) tasks.Add(LoadAsset<Sprite>(bottleData.bottleSprite).ToUniTask());
                // 対応するTileのSpriteを先に読み込む
                if (bottleData.targetTileSprite.RuntimeKeyIsValid()) tasks.Add(LoadAsset<Sprite>(bottleData.targetTileSprite).ToUniTask());
            });

            stage.TileDatas.ForEach(tileData => {
                switch (tileData.type) {
                    case ETileType.Normal:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.NORMAL_TILE_PREFAB).ToUniTask());
                        break;
                    case ETileType.Warp:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.WARP_TILE_PREFAB).ToUniTask());
                        break;
                    case ETileType.Holy:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.HOLY_TILE_PREFAB).ToUniTask());
                        break;
                    case ETileType.Spiderweb:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.SPIDERWEB_TILE_PREFAB).ToUniTask());
                        break;
                    case ETileType.Ice:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.ICE_TILE_PREFAB).ToUniTask());
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            });

            stage.GimmickDatas.ForEach(gimmick => {
                switch (gimmick.type) {
                    case EGimmickType.Tornado:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.TORNADO_PREFAB).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.TORNADO_WARNING_SPRITE).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.TURN_WARNING_LEFT_SPRITE).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.TURN_WARNING_RIGHT_SPRITE).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.TURN_WARNING_UP_SPRITE).ToUniTask());
                        tasks.Add(LoadAsset<Sprite>(Constants.Address.TURN_WARNING_BOTTOM_SPRITE).ToUniTask());
                        break;
                    case EGimmickType.Meteorite:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.METEORITE_PREFAB).ToUniTask());
                        break;
                    case EGimmickType.AimingMeteorite:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.AIMING_METEORITE_PREFAB).ToUniTask());
                        break;
                    case EGimmickType.Thunder:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.THUNDER_PREFAB).ToUniTask());
                        break;
                    case EGimmickType.SolarBeam:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.SOLAR_BEAM_PREFAB).ToUniTask());
                        break;
                    case EGimmickType.GustWind:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.GUST_WIND_PREFAB).ToUniTask());
                        break;
                    case EGimmickType.Fog:
                        tasks.Add(LoadAsset<GameObject>(Constants.Address.FOG_PREFAB).ToUniTask());
                        break;
                    case EGimmickType.Powder:
                        switch (treeId.GetSeasonId()) {
                            case ESeasonId.Spring:
                            case ESeasonId.Summer:
                            case ESeasonId.Autumn:
                            case ESeasonId.Winter:
                                // TODO: 季節ごとにアセットを変更する
                                tasks.Add(LoadAsset<Sprite>(Constants.Address.SAND_POWDER_BACKGROUND_SPRITE).ToUniTask());
                                tasks.Add(LoadAsset<Material>(Constants.Address.SAND_POWDER_PARTICLE_MATERIAL).ToUniTask());
                                tasks.Add(LoadAsset<GameObject>(Constants.Address.POWDER_PREFAB).ToUniTask());
                                tasks.Add(LoadAsset<GameObject>(Constants.Address.SAND_PILED_UP_POWDER_PREFAB).ToUniTask());
                                break;
                            default:
                                throw new System.ArgumentOutOfRangeException();
                        }
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            });

            await UniTask.WhenAll(tasks);
        }
    }
}

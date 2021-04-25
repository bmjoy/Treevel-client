using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
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

        private static readonly Subject<AsyncOperationHandle> _onAssetStartLoadSubject = new Subject<AsyncOperationHandle>();
        public static readonly IObservable<AsyncOperationHandle> OnAssetStartLoad = _onAssetStartLoadSubject.AsObservable();

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
            var handle = Addressables.InitializeAsync();
            handle.Completed += obj => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    _initialized = true;
                } else {
                    throw new Exception("Fail to initialize Addressable Asset System.");
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
        private static UniTask<TObject> LoadAssetAsync<TObject>(object key)
        {
            if (_loadedAssets.ContainsKey(key)) {
                return _loadedAssets[key].Convert<TObject>().ToUniTask();
            }

            var op = Addressables.LoadAssetAsync<TObject>(key);

            _loadedAssets.Add(key, op);

            _onAssetStartLoadSubject.OnNext(op);

            return op.ToUniTask();
        }

        /// <summary>
        /// ロードしたアセットを取得する
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key">アドレス</param>
        /// <returns></returns>
        public static TObject GetAsset<TObject>(object key)
        {
            if (_loadedAssets.ContainsKey(key)) {
                return _loadedAssets[key].Convert<TObject>().Result;
            }

            Debug.LogWarning($"Asset with key:{key} is not loaded.");
            return default;
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
            _onAssetStartLoadSubject.OnNext(ret);

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
            if (!_loadedAssets.ContainsKey(sceneName)) return default;

            var handle = _loadedAssets[sceneName];
            var ret = Addressables.UnloadSceneAsync(handle);
            ret.Completed += obj => {
                if (obj.Status == AsyncOperationStatus.Succeeded) {
                    // アンロード終了後、辞書から削除
                    _loadedAssets.Remove(sceneName);
                } else {
                    UIManager.Instance.ShowErrorMessageAsync(EErrorCode.LoadDataError).Forget();
                }
            };

            return ret;
        }

        /// <summary>
        /// プレハブを実体化させる
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
        internal static async UniTask LoadStageDependenciesAsync(ETreeId treeId, int stageNumber)
        {
            var stage = GameDataManager.GetStage(treeId, stageNumber);

            var tasks = new List<UniTask>();
            stage.BottleDatas.ForEach(bottleData => {
                switch (bottleData.type) {
                    case EBottleType.Dynamic:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.DYNAMIC_DUMMY_BOTTLE_PREFAB));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.DYNAMIC_DUMMY_BOTTLE_SPRITE));
                        break;
                    case EBottleType.Static:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.STATIC_DUMMY_BOTTLE_PREFAB));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.STATIC_DUMMY_BOTTLE_SPRITE));
                        break;
                    case EBottleType.Normal:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.GOAL_BOTTLE_PREFAB));
                        for (var life = 1; life <= LifeAttributeController.MAX_LIFE; life++) {
                            // 残りライフの数字画像
                            tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.LIFE_VALUE_SPRITE_PREFIX + life));
                        }

                        break;
                    case EBottleType.AttackableDummy:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                // ボトルの色に対応したボトル画像、タイル画像を読み込む
                if (bottleData.goalColor != EGoalColor.None) {
                    tasks.Add(LoadAssetAsync<Sprite>(bottleData.goalColor.GetBottleAddress()));
                    tasks.Add(LoadAssetAsync<Sprite>(bottleData.goalColor.GetBottleAddress() + Constants.Address.LIFE_CRACK_SPRITE_INFIX + "1"));
                    tasks.Add(LoadAssetAsync<Sprite>(bottleData.goalColor.GetBottleAddress() + Constants.Address.LIFE_CRACK_SPRITE_INFIX + "2"));
                    tasks.Add(LoadAssetAsync<Sprite>(bottleData.goalColor.GetTileAddress()));
                }
            });

            // NormalTileのSpriteを読み込む
            // シーンに配置したノーマルタイルを初期化
            for (var tileNum = 1; tileNum <= Constants.StageSize.ROW * Constants.StageSize.COLUMN; ++tileNum) {
                tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.NORMAL_TILE_SPRITE_PREFIX + tileNum));
            }

            stage.TileDatas.ForEach(tileData => {
                switch (tileData.type) {
                    case ETileType.Normal:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.NORMAL_TILE_PREFAB));
                        break;
                    case ETileType.Goal:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.GOAL_TILE_PREFAB));
                        break;
                    case ETileType.Warp:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.WARP_TILE_PREFAB));
                        break;
                    case ETileType.Holy:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.HOLY_TILE_PREFAB));
                        break;
                    case ETileType.Spiderweb:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.SPIDERWEB_TILE_PREFAB));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.SPIDERWEB_TILE_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.SPIDERWEB_TILE_BOTTOM_LEFT_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.SPIDERWEB_TILE_BOTTOM_RIGHT_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.SPIDERWEB_TILE_TOP_LEFT_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.SPIDERWEB_TILE_TOP_RIGHT_SPRITE));
                        break;
                    case ETileType.Ice:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.ICE_TILE_PREFAB));
                        tasks.Add(LoadAssetAsync<Material>(Constants.Address.ICE_LAYER_MATERIAL));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });

            stage.GimmickDatas.ForEach(gimmick => {
                switch (gimmick.type) {
                    case EGimmickType.Tornado:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.TORNADO_PREFAB));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.TORNADO_WARNING_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.TURN_WARNING_LEFT_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.TURN_WARNING_RIGHT_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.TURN_WARNING_UP_SPRITE));
                        tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.TURN_WARNING_BOTTOM_SPRITE));
                        break;
                    case EGimmickType.Meteorite:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.METEORITE_PREFAB));
                        break;
                    case EGimmickType.AimingMeteorite:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.AIMING_METEORITE_PREFAB));
                        break;
                    case EGimmickType.Thunder:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.THUNDER_PREFAB));
                        break;
                    case EGimmickType.SolarBeam:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.SOLAR_BEAM_PREFAB));
                        break;
                    case EGimmickType.GustWind:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.GUST_WIND_PREFAB));
                        break;
                    case EGimmickType.Fog:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.FOG_PREFAB));
                        break;
                    case EGimmickType.Powder:
                        switch (treeId.GetSeasonId()) {
                            case ESeasonId.Spring:
                            case ESeasonId.Summer:
                            case ESeasonId.Autumn:
                            case ESeasonId.Winter:
                                // TODO: 季節ごとにアセットを変更する
                                tasks.Add(LoadAssetAsync<Sprite>(Constants.Address.SAND_POWDER_BACKGROUND_SPRITE));
                                tasks.Add(LoadAssetAsync<Material>(Constants.Address.SAND_POWDER_PARTICLE_MATERIAL));
                                tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.POWDER_PREFAB));
                                tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.SAND_PILED_UP_POWDER_PREFAB));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case EGimmickType.Erasable:
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.ERASABLE_PREFAB));
                        tasks.Add(LoadAssetAsync<GameObject>(Constants.Address.ERASABLE_BOTTLE_PREFAB));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            await UniTask.WhenAll(tasks);
        }
    }
}

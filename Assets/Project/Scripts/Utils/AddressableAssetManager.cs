﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.Utils
{
    public static class AddressableAssetManager
    {
        /// <summary>
        /// 初期化フラグ
        /// </summary>
        private static bool _initialized = false;

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

            UIManager.Instance.ProgressBar.Load(op);

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
            UIManager.Instance.ProgressBar.Load(ret);

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
                    UIManager.Instance.ShowErrorMessage(EErrorCode.LoadSceneError);
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
        internal static void LoadStageDependencies(ETreeId treeId, int stageNumber)
        {
            StageData stage = GameDataBase.GetStage(treeId, stageNumber);

            stage.BottleDatas.ForEach((bottleData) => {
                switch (bottleData.type) {
                    case EBottleType.Dynamic:
                        LoadAsset<GameObject>(Address.DYNAMIC_DUMMY_BOTTLE_PREFAB);
                        LoadAsset<Sprite>(Address.DYNAMIC_DUMMY_BOTTLE_SPRITE);
                        break;
                    case EBottleType.Static:
                        LoadAsset<GameObject>(Address.STATIC_DUMMY_BOTTLE_PREFAB);
                        LoadAsset<Sprite>(Address.STATIC_DUMMY_BOTTLE_SPRITE);
                        break;
                    case Definitions.EBottleType.Normal:
                        LoadAsset<GameObject>(Address.NORMAL_BOTTLE_PREFAB);
                        break;
                    case Definitions.EBottleType.Life:
                        LoadAsset<GameObject>(Address.LIFE_BOTTLE_PREFAB);
                        break;
                    case EBottleType.AttackableDummy:
                        LoadAsset<GameObject>(Address.ATTACKABLE_DUMMY_BOTTLE_PREFAB);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
                if (bottleData.bottleSprite.RuntimeKeyIsValid()) LoadAsset<Sprite>(bottleData.bottleSprite);
                // 対応するTileのSpriteを先に読み込む
                if (bottleData.targetTileSprite.RuntimeKeyIsValid()) LoadAsset<Sprite>(bottleData.targetTileSprite);
            });

            stage.TileDatas.ForEach(tileData => {
                switch (tileData.type) {
                    case ETileType.Normal:
                        LoadAsset<GameObject>(Address.NORMAL_TILE_PREFAB);
                        break;
                    case ETileType.Warp:
                        LoadAsset<GameObject>(Address.WARP_TILE_PREFAB);
                        break;
                    case ETileType.Holy:
                        LoadAsset<GameObject>(Address.HOLY_TILE_PREFAB);
                        break;
                    case ETileType.Spiderweb:
                        LoadAsset<GameObject>(Address.SPIDERWEB_TILE_PREFAB);
                        break;
                    case ETileType.Ice:
                        LoadAsset<GameObject>(Address.ICE_TILE_PREFAB);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            });

            stage.GimmickDatas.ForEach(gimmick => {
                switch (gimmick.type) {
                    case EGimmickType.Tornado:
                        LoadAsset<GameObject>(Address.TORNADO_PREFAB);
                        LoadAsset<Sprite>(Address.TORNADO_WARNING_SPRITE);
                        LoadAsset<Sprite>(Address.TURN_WARNING_LEFT_SPRITE);
                        LoadAsset<Sprite>(Address.TURN_WARNING_RIGHT_SPRITE);
                        LoadAsset<Sprite>(Address.TURN_WARNING_UP_SPRITE);
                        LoadAsset<Sprite>(Address.TURN_WARNING_BOTTOM_SPRITE);
                        break;
                    case EGimmickType.Meteorite:
                        LoadAsset<GameObject>(Address.METEORITE_PREFAB);
                        break;
                    case EGimmickType.AimingMeteorite:
                        LoadAsset<GameObject>(Address.AIMING_METEORITE_PREFAB);
                        break;
                    case EGimmickType.Thunder:
                        LoadAsset<GameObject>(Address.THUNDER_PREFAB);
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            });
        }
    }
}

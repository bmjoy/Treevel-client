using System.Threading;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    public class RoadController : LineControllerBase
    {
        private LevelTreeController _endObjectController;

        /// <summary>
        /// 道の解放アニメーションのフレーム数
        /// </summary>
        private const int _CLEAR_ANIMATION_FRAMES = 200;

        /// <summary>
        /// 解放時の道の色
        /// </summary>
        private static readonly Color _ROAD_RELEASED_COLOR = Color.white;

        /// <summary>
        /// 非解放時の道の色
        /// </summary>
        /// <returns></returns>
        private static readonly Color _ROAD_UNRELEASED_COLOR = new Color(0.2f, 0.2f, 0.7f);

        protected override void Awake()
        {
            base.Awake();
            _endObjectController = endObject.GetComponent<LevelTreeController>();
        }

        protected override void Start()
        {
            base.Start();
            lineRenderer.material.SetTextureScale("_MainTex", new Vector2(8 / lineLength, 1f));
        }

        protected override void SetSaveKey()
        {
            saveKey =
                $"{startObject.GetComponent<LevelTreeController>().treeId}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponent<LevelTreeController>().treeId}";
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(saveKey);
        }

        /// <summary>
        /// 道の状態の更新
        /// </summary>
        public override async UniTask UpdateStateAsync()
        {
            var endTreeData = GameDataManager.GetTreeData(_endObjectController.treeId);

            if (endTreeData.constraintTrees.Count == 0) {
                // 初期解放
                lineRenderer.startColor = lineRenderer.endColor = _ROAD_RELEASED_COLOR;
                return;
            }

            if (_endObjectController.state == ETreeState.Unreleased) {
                // 未解放
                lineRenderer.startColor = lineRenderer.endColor = _ROAD_UNRELEASED_COLOR;
            } else if (LevelSelectDirector.Instance.releaseAnimationPlayedRoads.Contains(saveKey)) {
                // 演出を再生したことがあればそのまま解放
                lineRenderer.startColor = lineRenderer.endColor = _ROAD_RELEASED_COLOR;
            } else {
                // 演出開始
                // 画面切り替える際に強制的に終わらせる
                var cancelTokenSource = new CancellationTokenSource();
                this.OnDisableAsObservable()
                    .Subscribe(_ => {
                        cancelTokenSource.Cancel();
                    })
                    .AddTo(cancelTokenSource.Token);

                // 道が非解放状態から解放状態に変わった時
                await ReleaseEndObjectAsync(cancelTokenSource.Token);

                // 再生状態を保存
                LevelSelectDirector.Instance.releaseAnimationPlayedRoads.Add(saveKey);
            }
        }

        /// <summary>
        /// 道が非解放状態から解放状態に変わった時のアニメーション(100フレームで色を変化させる)
        /// </summary>
        /// <returns></returns>
        private async UniTask ReleaseEndObjectAsync(CancellationToken cancelToken)
        {
            // 終点の木の状態の更新
            _endObjectController.state = ETreeState.Released;

            // 道の更新アニメーション
            for (var i = 0; i < _CLEAR_ANIMATION_FRAMES; i++) {
                if (cancelToken.IsCancellationRequested) break;

                // 非解放状態から解放状態まで線形補間
                lineRenderer.startColor = lineRenderer.endColor =
                    Color.Lerp(_ROAD_UNRELEASED_COLOR, _ROAD_RELEASED_COLOR, (float)i / _CLEAR_ANIMATION_FRAMES);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }

            // 終点の木の状態の更新アニメーション
            _endObjectController.ReflectTreeState();
            // 木の解放演出見たことを記録する
            LevelSelectDirector.Instance.releaseAnimationPlayedTrees.Add(_endObjectController.treeId);
        }
    }
}

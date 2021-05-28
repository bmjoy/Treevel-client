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
        /// 解放時のテクスチャの占領比率(1.0だと全解放)
        /// </summary>
        private static readonly int _SHADER_PARAM_FILL_AMOUNT = Shader.PropertyToID("_fillAmount");

        /// <summary>
        /// LineRendererのマテリアル
        /// </summary>
        private Material _material;

        protected override void Awake()
        {
            base.Awake();
            _endObjectController = endObject.GetComponentInParent<LevelTreeController>();
            _material = lineRenderer.material;
        }

        protected override void SetSaveKey()
        {
            saveKey =
                $"{startObject.GetComponentInParent<LevelTreeController>().treeId}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponentInParent<LevelTreeController>().treeId}";
        }

        protected override (Vector3, Vector3) GetEdgePointPosition()
        {
            // 木の下の領域の中心座標は親オブジェクトの相対位置から計算する
            var startPointPosition = startObject.transform.parent.localPosition + startObject.transform.localPosition;
            var endPointPosition = endObject.transform.parent.localPosition + endObject.transform.localPosition;
            return (startPointPosition, endPointPosition);
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
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, 1.0f);
                return;
            }

            if (_endObjectController.state == ETreeState.Unreleased) {
                // 未解放
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, 0.0f);
            } else if (LevelSelectDirector.Instance.releaseAnimationPlayedRoads.Contains(saveKey)) {
                // 演出を再生したことがあればそのまま解放
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, 1.0f);
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

                // 先端から末端まで明るくなる
                _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, Mathf.Lerp(0, 1, (float)i / _CLEAR_ANIMATION_FRAMES));
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }

            // 終点の木の状態の更新アニメーション
            _endObjectController.ReflectTreeState();
            // 木の解放演出見たことを記録する
            LevelSelectDirector.Instance.releaseAnimationPlayedTrees.Add(_endObjectController.treeId);
        }
    }
}

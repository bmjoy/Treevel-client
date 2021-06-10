using UniRx;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(LineRenderer))]
    public abstract class LineControllerBase : MonoBehaviour
    {
        /// <summary>
        /// 開始地点のGameObject
        /// </summary>
        [SerializeField] protected GameObject startObject;

        /// <summary>
        /// 終了地点のGameObject
        /// </summary>
        [SerializeField] protected GameObject endObject;

        /// <summary>
        /// 終点を解放するためにクリアする必要のあるオブジェクト
        /// </summary>
        [SerializeField] protected GameObject[] constraintObjects;

        /// <summary>
        /// 1つ目の制御点の位置
        /// </summary>
        [SerializeField] protected Vector2 firstControlPoint;

        /// <summary>
        /// 2つ目の制御点の位置
        /// </summary>
        [SerializeField] protected Vector2 secondControlPoint;

        /// <summary>
        /// 中間地点の個数
        /// </summary>
        [SerializeField] [Range(0, 100)] private int _middlePointNum;

        /// <summary>
        /// 道の幅
        /// </summary>
        [SerializeField] [Range(0, 0.2f)] protected float width;

        /// <summary>
        /// 拡大率
        /// </summary>
        public ReactiveProperty<float> Scale = new ReactiveProperty<float>(1f);

        [SerializeField] protected LineRenderer lineRenderer;

        /// <summary>
        /// データを保存するときのキー
        /// </summary>
        public string saveKey { get; protected set; }

        /// <summary>
        /// 先端のポジション
        /// </summary>
        private Vector3 _startPointPosition;

        /// <summary>
        /// 末端のポジション
        /// </summary>
        private Vector3 _endPointPosition;

        protected virtual void Awake()
        {
            SetSaveKey();
            Scale.Subscribe(scale => {
                lineRenderer.startWidth = lineRenderer.endWidth = Screen.width * width * scale;
            }).AddTo(this);
        }

        protected virtual void Start()
        {
            firstControlPoint *= SavableScrollRect.CONTENT_SCALE;
            firstControlPoint += SavableScrollRect.CONTENT_MARGIN;
            secondControlPoint *= SavableScrollRect.CONTENT_SCALE;
            secondControlPoint += SavableScrollRect.CONTENT_MARGIN;
            (_startPointPosition, _endPointPosition) = GetEdgePointPosition();
            SetPointPosition();
        }

        protected abstract void SetSaveKey();

        /// <summary>
        /// 端点の位置を求める
        /// </summary>
        /// <returns> (始点の位置, 終点の位置) </returns>
        protected abstract (Vector3, Vector3) GetEdgePointPosition();

        public abstract void UpdateState();

        /// <summary>
        /// 曲線の通過点の位置を求める
        /// </summary>
        public void SetPointPosition()
        {
            if (lineRenderer == null || startObject == null || endObject == null) return;
            lineRenderer.positionCount = _middlePointNum + 2;
            lineRenderer.startWidth = lineRenderer.endWidth = Screen.width * width * Scale.Value;

            // 点の位置と線の長さを求める
            for (var i = 0; i <= _middlePointNum + 1; i++) {
                var ratio = (float)i / (_middlePointNum + 1);
                var targetPosition = CalcCubicBezierPointPosition(_startPointPosition, firstControlPoint,
                                                                  secondControlPoint, _endPointPosition, ratio);
                lineRenderer.SetPosition(i, targetPosition);
            }
        }

        public Vector3 GetPositionAtRatio(float ratio)
        {
            ratio = Mathf.Clamp(ratio, 0, 1);
            return CalcCubicBezierPointPosition(_startPointPosition, firstControlPoint, secondControlPoint, _endPointPosition, ratio);
        }

        /// <summary>
        /// 3次ベジェ曲線上のある点の位置を求める
        /// </summary>
        /// <param name="p0"> 1つ目の制御点 </param>
        /// <param name="p1"> 2つ目の制御点 </param>
        /// <param name="p2"> 3つ目の制御点 </param>
        /// <param name="p3"> 4つ目の制御点 </param>
        /// <param name="ratio"> 内分比 </param>
        /// <returns> 点の位置 </returns>
        private Vector2 CalcCubicBezierPointPosition(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float ratio)
        {
            var oneMinusRatio = 1f - ratio;
            return oneMinusRatio * oneMinusRatio * oneMinusRatio * p0
                   + 3f * oneMinusRatio * oneMinusRatio * ratio * p1
                   + 3f * oneMinusRatio * ratio * ratio * p2
                   + ratio * ratio * ratio * p3;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(LineRenderer))]
    public abstract class LineController : MonoBehaviour
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
        [SerializeField] [Range(0, 100)] protected int _middlePointNum;

        /// <summary>
        /// 道の幅
        /// </summary>
        [SerializeField] [Range(0, 0.2f)] protected float _width;

        [SerializeField] protected LineRenderer _render;

        /// <summary>
        /// 解放状態
        /// </summary>
        protected bool released = false;

        protected Button button;

        /// <summary>
        /// データを保存するときのキー
        /// </summary>
        protected string saveKey;

        protected virtual void Awake()
        {
            button = endObject.GetComponent<Button>();
            SetSaveKey();
        }

        private void Start()
        {
            firstControlPoint *= SavableScrollRect.CONTENT_SCALE;
            firstControlPoint += SavableScrollRect.CONTENT_MARGIN;
            secondControlPoint *= SavableScrollRect.CONTENT_SCALE;
            secondControlPoint += SavableScrollRect.CONTENT_MARGIN;
            SetPointPosition();
        }

        protected abstract void SetSaveKey();

        public abstract void Reset();

        public abstract void UpdateReleased();

        public abstract void SaveReleased();

        /// <summary>
        /// 曲線の通過点の位置を求める
        /// </summary>
        public void SetPointPosition()
        {
            _render.positionCount = _middlePointNum + 2;
            _render.startWidth = _render.endWidth = (float)Screen.width * _width;

            var startPointLocalPosition = startObject.transform.localPosition;
            var endPointLocalPosition = endObject.transform.localPosition;

            // 点の位置を求める
            for (int i = 0; i <= _middlePointNum + 1; i++) {
                var ratio = (float)i / (_middlePointNum + 1);
                _render.SetPosition(i, CalcCubicBezierPointPosition(startPointLocalPosition, firstControlPoint, secondControlPoint, endPointLocalPosition, ratio));
            }
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

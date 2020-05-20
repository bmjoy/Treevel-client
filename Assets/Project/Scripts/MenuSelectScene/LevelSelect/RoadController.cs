using UnityEngine;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.MenuSelectScene;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(LineRenderer))]
    public class RoadController : MonoBehaviour
    {
        /// <summary>
        /// 開始地点の木の位置
        /// </summary>
        [SerializeField] private Transform _startPoint;

        /// <summary>
        /// 終了地点の木の位置
        /// </summary>
        [SerializeField] private Transform _endPoint;

        /// <summary>
        /// 1つ目の制御点の位置
        /// </summary>
        [SerializeField] private Vector2 _firstControlPoint;

        /// <summary>
        /// 2つ目の制御点の位置
        /// </summary>
        [SerializeField] private Vector2 _secondControlPoint;

        /// <summary>
        /// 中間地点の個数
        /// </summary>
        [SerializeField] private int _middlePointNum;

        /// <summary>
        /// 道の幅
        /// </summary>
        [SerializeField] private float _width;

        [SerializeField] private LineRenderer render;

        // Use this for initialization
        private void Start()
        {
            // SafeAreaを考慮したContentの拡大縮小差分を制御点にも適用
            _firstControlPoint *= ScaleContent.CONTENT_SCALE;
            _firstControlPoint += ScaleContent.CONTENT_MARGIN;
            _secondControlPoint *= ScaleContent.CONTENT_SCALE;
            _secondControlPoint += ScaleContent.CONTENT_MARGIN;
            SetPointPosition();
        }

        /// <summary>
        /// 曲線の通過点の位置を求める
        /// </summary>
        public void SetPointPosition()
        {
            render.positionCount = _middlePointNum + 2;
            render.startWidth = render.endWidth = _width;

            var startPointLocalPosition = _startPoint.localPosition;
            var endPointLocalPosition = _endPoint.localPosition;

            // 開始地点
            render.SetPosition(0, startPointLocalPosition);
            
            // 中間地点
            for (int i = 1; i <= _middlePointNum; i++)
            {
                var ratio = (float)i / (_middlePointNum + 1);
                render.SetPosition(i, CalcMiddlePointPosition(startPointLocalPosition, _firstControlPoint, _secondControlPoint, endPointLocalPosition, ratio));
            }

            // 終了地点
            render.SetPosition(_middlePointNum + 1, endPointLocalPosition);
        }

        /// <summary>
        /// 3次ベジェ曲線のある点の位置を求める
        /// </summary>
        /// <param name="p0"> 1つ目の制御点 </param>
        /// <param name="p1"> 2つ目の制御点 </param>
        /// <param name="p2"> 3つ目の制御点 </param>
        /// <param name="p3"> 4つ目の制御点 </param>
        /// <param name="ratio"> 内分比 </param>
        /// <returns> 点の位置 </returns>
        private Vector2 CalcMiddlePointPosition(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float ratio)
        {
            var oneMinusRatio = 1f - ratio;
            return oneMinusRatio * oneMinusRatio * oneMinusRatio * p0
                 + 3f * oneMinusRatio * oneMinusRatio * ratio * p1
                 + 3f * oneMinusRatio * ratio * ratio * p2
                 + ratio * ratio * ratio * p3;
        }
    }
}

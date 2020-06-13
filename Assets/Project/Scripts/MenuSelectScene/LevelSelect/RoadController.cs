using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(LineRenderer))]
    public class RoadController : MonoBehaviour
    {
        /// <summary>
        /// 開始地点の木
        /// </summary>
        [SerializeField] private GameObject _startTree;

        /// <summary>
        /// 終了地点の木
        /// </summary>
        [SerializeField] private GameObject _endTree;

        [SerializeField] private GameObject[] _constraintTrees;

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
        [SerializeField] [Range(0, 100)] private int _middlePointNum;

        /// <summary>
        /// 道の幅
        /// </summary>
        [SerializeField] [Range(0, 0.2f)] private float _width;

        [SerializeField] private LineRenderer _render;

        private bool _released = false;

        [SerializeField] private ERoadId _roadId;

        private void Start()
        {
            // SafeAreaを考慮したContentの拡大縮小差分を制御点にも適用
            _firstControlPoint *= SavableScrollRect.CONTENT_SCALE;
            _firstControlPoint += SavableScrollRect.CONTENT_MARGIN;
            _secondControlPoint *= SavableScrollRect.CONTENT_SCALE;
            _secondControlPoint += SavableScrollRect.CONTENT_MARGIN;
            SetPointPosition();
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.ROAD + _roadId.ToString());
        }

        public void UpdateReleased()
        {
            _released = PlayerPrefs.GetInt(PlayerPrefsKeys.ROAD + _roadId.ToString(), Default.ROAD_RELEASED) == 1;

            if (!_released) {
                if (_constraintTrees.Length == 0) {
                    // 初期状態で解放されている道
                    _released = true;
                } else {
                    var constraintTreeClear = true;
                    foreach (var constraintTree in _constraintTrees) {
                        Debug.Log(constraintTree.GetComponent<TreeController>().cleared);
                        constraintTreeClear = constraintTreeClear && constraintTree.GetComponent<TreeController>().cleared;
                    }
                    _released = constraintTreeClear;
                }
            }

            _endTree.GetComponent<TreeController>().released = _released;
            _endTree.GetComponent<Button>().enabled = _released;

            if (!_released) {
                // 非解放時
                _render.startColor = new Color(0.2f, 0.2f, 0.7f);
                _render.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        public void SaveReleased()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.ROAD + _roadId.ToString(), Convert.ToInt32(_released));
        }

        /// <summary>
        /// 曲線の通過点の位置を求める
        /// </summary>
        public void SetPointPosition()
        {
            _render.positionCount = _middlePointNum + 2;
            _render.startWidth = _render.endWidth = (float)Screen.width * _width;

            var startPointLocalPosition = _startTree.transform.localPosition;
            var endPointLocalPosition = _endTree.transform.localPosition;

            // 点の位置を求める
            for (int i = 0; i <= _middlePointNum + 1; i++) {
                var ratio = (float)i / (_middlePointNum + 1);
                _render.SetPosition(i, CalcCubicBezierPointPosition(startPointLocalPosition, _firstControlPoint, _secondControlPoint, endPointLocalPosition, ratio));
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

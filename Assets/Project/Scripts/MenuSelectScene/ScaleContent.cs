using System;
using System.Linq;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.MenuSelectScene
{
    public class ScaleContent : MonoBehaviour
    {
        private ScrollRect _scrollRect;
        private TransformGesture _transformGesture;
        [SerializeField] private GameObject _content;
        private RectTransform _contentRect;

        /// <summary>
        /// タッチした2点
        /// </summary>
        private Vector2 _prePoint1;
        private Vector2 _prePoint2;

        /// <summary>
        /// タッチした2点間の距離
        /// </summary>
        private float _preScreenDist = 0.0f;

        /// <summary>
        /// タッチした2点の中点
        /// </summary>
        private Vector3 _preMeanPoint;

        /// <summary>
        /// 拡大率
        /// </summary>
        private float _preScale;
        /// <summary>
        /// 拡大率の上限
        /// </summary>
        private const float _SCALE_MAX = 1.50f;
        /// <summary>
        /// 拡大率の下限
        /// </summary>
        private const float _SCALE_MIN = 0.5f;

        /// <summary>
        /// 補正後のキャンバスの大きさ
        /// </summary>
        private Vector2 _scaledCanvas;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _transformGesture = GetComponent<TransformGesture>();
            _scaledCanvas = ScaledCanvasSize.SIZE_DELTA;
            _contentRect = _scrollRect.content;
        }

        private void OnEnable()
        {
            _preScale = UserSettings.LevelSelectCanvasScale;
            _contentRect.localScale = new Vector2(_preScale, _preScale);
            // 2点のタッチ開始時
            _transformGesture.TransformStarted += OnTransformStarted;
            // 2点のタッチ中
            _transformGesture.Transformed += OnTransformed;
        }

        private void OnDisable()
        {
            _transformGesture.TransformStarted -= OnTransformStarted;
            _transformGesture.Transformed -= OnTransformed;

            UserSettings.LevelSelectCanvasScale = _preScale;
        }

        private void OnTransformStarted(object sender, EventArgs e)
        {
            // タッチ位置を取得
            _prePoint1 = _transformGesture.ActivePointers[0].Position;
            _prePoint2 = _transformGesture.ActivePointers[1].Position;
            // スクリーン上の距離を計算
            _preScreenDist = Vector2.Distance(_prePoint1, _prePoint2);
            // スクリーン上の中点座標を計算
            _preMeanPoint = (_prePoint1 + _prePoint2) / 2;
        }

        private void OnTransformed(object sender, EventArgs e)
        {
            // タッチ位置等の計算
            var _newPoint1 = _transformGesture.ActivePointers[0].Position;
            var _newPoint2 = _transformGesture.ActivePointers[1].Position;
            var _newScreenDist = Vector2.Distance(_newPoint1, _newPoint2);
            var _newMeanPoint = (Vector3)((_newPoint1 + _newPoint2) / 2);

            // 2点のタッチ開始時の距離と現在の距離の比から拡大率を求める
            var _newScale = _preScale * _newScreenDist / _preScreenDist;
            // 拡大率を閾値内に抑える
            _newScale = Mathf.Clamp(_newScale, _SCALE_MIN, _SCALE_MAX);
            // Contentの拡大縮小
            _contentRect.localScale = new Vector2(_newScale, _newScale);

            // 拡大縮小前の中点を拡大縮小後の中点に合わせるようにContentの平行移動量を求める
            var _preContentPoint = ConvertFromScreenToContent(_preMeanPoint, _preScale);
            var _newContentPoint = ConvertFromScreenToContent(_newMeanPoint, _preScale);
            // Content空間での2点の差分
            var moveAmount = _newContentPoint - _preContentPoint * _newScale / _preScale;
            moveAmount = KeepInContent(moveAmount, _newScale);
            // Contentの平行移動
            _contentRect.transform.localPosition += new Vector3(moveAmount.x, moveAmount.y, 0);

            // 値の更新
            _prePoint1 = _newPoint1;
            _prePoint2 = _newPoint2;
            _preScreenDist = _newScreenDist;
            _preMeanPoint = _newMeanPoint;
            _preScale = _newScale;
        }

        /// <summary>
        /// スクリーン空間の座標をContent空間の座標に変換する
        /// </summary>
        /// <param name="screenPoint"> スクリーン上の座標 </param>
        /// <param name="scale"> Contentの拡大率 </param>
        /// <returns> Content空間の座標 </returns>
        private Vector3 ConvertFromScreenToContent(Vector3 screenCoordinate, float scale)
        {
            return ((-1) * _contentRect.transform.localPosition + (screenCoordinate - new Vector3(_scaledCanvas.x / 2, _scaledCanvas.y / 2))) / scale;
        }

        /// <summary>
        /// 平行移動後にContentの外を表示しないようにする
        /// </summary>
        /// <param name="moveAmount"> 平行移動量 </param>
        /// <param name="scale"> Contentの拡大率 </param>
        /// <returns> 平行移動量 </returns>
        private Vector2 KeepInContent(Vector2 moveAmount, float scale)
        {
            // 平行移動前のスクリーン中心のContent空間での座標
            var _preLocalPosition = _contentRect.transform.localPosition;

            // Contentの左端のチェック
            var leftLimit = (SaveScrollRect._LEFT_OFFSET * scale - 0.5f) * _scaledCanvas.x;
            if (_preLocalPosition.x + moveAmount.x >= leftLimit) {
                moveAmount.x = leftLimit - _preLocalPosition.x;
            }
            // Contentの右端のチェック
            var rightLimit = (-1) * ((SaveScrollRect._RIGHT_OFFSET * scale - 0.5f) * _scaledCanvas.x);
            if (_preLocalPosition.x + moveAmount.x <= rightLimit) {
                moveAmount.x = rightLimit - _preLocalPosition.x;
            }
            // Contentの上端のチェック
            var topLimit = (-1) * ((SaveScrollRect._TOP_OFFSET * scale - 0.5f) * _scaledCanvas.y);
            if (_preLocalPosition.y + moveAmount.y <= topLimit) {
                moveAmount.y = topLimit - _preLocalPosition.y;
            }
            // Contentの下端のチェック
            var bottomLimit = (SaveScrollRect._BOTTOM_OFFSET * scale - 0.5f) * _scaledCanvas.y;
            if (_preLocalPosition.y + moveAmount.y >= bottomLimit) {
                moveAmount.y = bottomLimit - _preLocalPosition.y;
            }

            return moveAmount;
        }
    }
}

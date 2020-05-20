using System;
using System.Linq;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.UIComponents;

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
        /// 拡大率 (初期値 : 1.0f)
        /// </summary>
        private float _preScale = 1.0f;
        /// <summary>
        /// 拡大率の上限
        /// </summary>
        private const float _SCALE_MAX = 1.50f;
        /// <summary>
        /// 拡大率の下限
        /// </summary>
        private const float _SCALE_MIN = 0.5f;

        /// <summary>
        /// Contentの余白(Screen何個分の余白があるか)
        /// </summary>
        private static float _LEFT_OFFSET;
        private static float _RIGHT_OFFSET;
        private static float _TOP_OFFSET;
        private static float _BOTTOM_OFFSET;

        /// <summary>
        /// 補正後のキャンバスの大きさ
        /// </summary>
        private Vector2 _scaledCanvas;

        public static Vector2 CONTENT_SCALE = Vector2.one;

        public static Vector2 CONTENT_MARGIN = Vector2.zero;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _transformGesture = GetComponent<TransformGesture>();
            _scaledCanvas = GameObject.Find("LevelSelect/Canvas").GetComponent<RectTransform>().sizeDelta;
            _contentRect = _content.GetComponent<RectTransform>();

            ExpandContent();
            // Contentの余白を取得
            _LEFT_OFFSET = Mathf.Abs(_contentRect.anchorMin.x - _contentRect.pivot.x);
            _RIGHT_OFFSET = _contentRect.anchorMax.x - _contentRect.pivot.x;
            _TOP_OFFSET = _contentRect.anchorMax.y - _contentRect.pivot.y;
            _BOTTOM_OFFSET = Mathf.Abs(_contentRect.anchorMin.y - _contentRect.pivot.y);
            // 初期位置の調整
            _contentRect.transform.localPosition += new Vector3(0, _scaledCanvas.y / 2, 0);
        }

        private void OnEnable()
        {
            // 2点のタッチ開始時
            _transformGesture.TransformStarted += OnTransformStarted;
            // 2点のタッチ中
            _transformGesture.Transformed += OnTransformed;
            // 2点のタッチ終了時
            _transformGesture.TransformCompleted += OnTransformCompleted;
        }

        private void OnDisable()
        {
            _transformGesture.TransformStarted -= OnTransformStarted;
            _transformGesture.Transformed -= OnTransformed;
            _transformGesture.TransformCompleted -= OnTransformCompleted;
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
            // 2点タッチしている時はスクロールしない
            _scrollRect.enabled = false;
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
            var leftLimit = (_LEFT_OFFSET * scale - 0.5f) * _scaledCanvas.x;
            if (_preLocalPosition.x + moveAmount.x >= leftLimit) {
                moveAmount.x = leftLimit - _preLocalPosition.x;
            }
            // Contentの右端のチェック
            var rightLimit = (-1) * ((_RIGHT_OFFSET * scale - 0.5f) * _scaledCanvas.x);
            if (_preLocalPosition.x + moveAmount.x <= rightLimit) {
                moveAmount.x = rightLimit - _preLocalPosition.x;
            }
            // Contentの上端のチェック
            var topLimit = (-1) * ((_TOP_OFFSET * scale - 0.5f) * _scaledCanvas.y);
            if (_preLocalPosition.y + moveAmount.y <= topLimit) {
                moveAmount.y = topLimit - _preLocalPosition.y;
            }
            // Contentの下端のチェック
            var bottomLimit = (_BOTTOM_OFFSET * scale - 0.5f) * _scaledCanvas.y;
            if (_preLocalPosition.y + moveAmount.y >= bottomLimit) {
                moveAmount.y = bottomLimit - _preLocalPosition.y;
            }

            return moveAmount;
        }

        private void OnTransformCompleted(object sender, EventArgs e)
        {
            // スクロール制限を解除する
            _scrollRect.enabled = true;
        }

        /// <summary>
        /// ContentのサイズをSafeAreaの分だけ拡大する
        /// </summary>
        private void ExpandContent()
        {
            // ContentのサイズをSafeAreaの分だけ拡大する
            var beforeAnchorMin = _contentRect.anchorMin;
            var beforeAnchorMax = _contentRect.anchorMax;
            var(anchorMin, anchorMax) = SafeAreaPanel.GetSafeAreaAnchor();
            _contentRect.anchorMin -= anchorMin;
            _contentRect.anchorMax += (Vector2.one - anchorMax);
            // Contentの拡大率
            CONTENT_SCALE = (beforeAnchorMax - beforeAnchorMin) / (_contentRect.anchorMax - _contentRect.anchorMin);

            if (CONTENT_SCALE == Vector2.one) return;
            // Content内の全オブジェクトのanchor位置の調整
            CONTENT_MARGIN = anchorMin / (_contentRect.anchorMax - _contentRect.anchorMin);
            foreach (var tree in _content.GetComponentsInChildren<Transform>().Where(t => t != _content.transform).Select(t => t.gameObject)) {
                var treeRect = tree.GetComponent<RectTransform>();
                treeRect.anchorMin *= CONTENT_SCALE;
                treeRect.anchorMin += CONTENT_MARGIN;
                treeRect.anchorMax *= CONTENT_SCALE;
                treeRect.anchorMax += CONTENT_MARGIN;
            }
        }
    }
}

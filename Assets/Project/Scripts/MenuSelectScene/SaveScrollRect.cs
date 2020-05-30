using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.MenuSelectScene.Settings;

namespace Project.Scripts.MenuSelectScene
{
    public class SaveScrollRect : ScrollRect
    {
        private TransformGesture _transformGesture;

        /// <summary>
        /// Contentの余白(Screen何個分の余白があるか)
        /// </summary>
        public static float _LEFT_OFFSET;
        public static float _RIGHT_OFFSET;
        public static float _TOP_OFFSET;
        public static float _BOTTOM_OFFSET;

        public static Vector2 CONTENT_SCALE = Vector2.one;
        public static Vector2 CONTENT_MARGIN = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
            _transformGesture = GetComponent<TransformGesture>();
            ExpandContent();
            // Contentの余白を取得
            _LEFT_OFFSET = Mathf.Abs(content.anchorMin.x - content.pivot.x);
            _RIGHT_OFFSET = content.anchorMax.x - content.pivot.x;
            _TOP_OFFSET = content.anchorMax.y - content.pivot.y;
            _BOTTOM_OFFSET = Mathf.Abs(content.anchorMin.y - content.pivot.y);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _transformGesture.TransformStarted += OnTransformStarted;
            _transformGesture.TransformCompleted += OnTransformCompleted;
            // 初期位置の調整
            content.transform.localPosition = UserSettings.LevelSelectScrollPosition;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UserSettings.LevelSelectScrollPosition = content.transform.localPosition;
        private void OnTransformStarted(object sender, EventArgs e)
        {
            // 2点タッチしている時はスクロールしない
            horizontal = false;
            vertical = false;
        }

        private void OnTransformCompleted(object sender, EventArgs e)
        {
            // スクロール制限を解除する
            horizontal = true;
            vertical = true;
        }

        /// <summary>
        /// ContentのサイズをSafeAreaの分だけ拡大する
        /// </summary>
        private void ExpandContent()
        {
            // ContentのサイズをSafeAreaの分だけ拡大する
            var beforeAnchorMin = content.anchorMin;
            var beforeAnchorMax = content.anchorMax;
            var(anchorMin, anchorMax) = SafeAreaPanel.GetSafeAreaAnchor();
            content.anchorMin -= anchorMin;
            content.anchorMax += (Vector2.one - anchorMax);
            // Contentの拡大率
            CONTENT_SCALE = (beforeAnchorMax - beforeAnchorMin) / (content.anchorMax - content.anchorMin);

            if (CONTENT_SCALE == Vector2.one) return;
            // Content内の全オブジェクトのanchor位置の調整
            CONTENT_MARGIN = anchorMin / (content.anchorMax - content.anchorMin);
            foreach (var tree in content.GetComponentsInChildren<Transform>().Where(t => t != content.transform).Select(t => t.gameObject)) {
                var treeRect = tree.GetComponent<RectTransform>();
                treeRect.anchorMin *= CONTENT_SCALE;
                treeRect.anchorMin += CONTENT_MARGIN;
                treeRect.anchorMax *= CONTENT_SCALE;
                treeRect.anchorMax += CONTENT_MARGIN;
            }
        }
    }
}

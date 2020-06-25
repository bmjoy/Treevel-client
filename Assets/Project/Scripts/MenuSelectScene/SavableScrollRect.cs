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
    public class SavableScrollRect : ScrollRect
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
        }

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
    }
}

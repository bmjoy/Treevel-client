using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Utils
{
    public class CustomTimer : MonoBehaviour
    {
        /// <summary>
        /// タイマーの結果を書き込むテキストオブジェクト
        /// </summary>
        private Text _timerText;

        /// <summary>
        /// タイマーの秒数 (小数第一位まで保持)
        /// </summary>
        private double _second;

        /// <summary>
        /// 初期化 (表示 UI あり)
        /// </summary>
        public void Initialize(Text timerText)
        {
            _timerText = timerText;
        }

        /// <summary>
        /// 初期化 (表示 UI なし)
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// タイマーを動作
        /// </summary>
        public void StartTimer()
        {
            if (_timerText != null) _timerText.text = "0";
            _second = 0;
            enabled = true;
        }

        /// <summary>
        /// タイマーを停止
        /// </summary>
        public void StopTimer()
        {
            enabled = false;
        }

        private void Update()
        {
            // 秒数を更新
            _second += Time.deltaTime;

            if (_timerText == null) return;

            if (_second - int.Parse(_timerText.text) < 1) return;

            // 更新した秒数 (整数) をテキストオブジェクトに表示
            var timeStr = Math.Floor(_second).ToString();
            _timerText.text = timeStr;
        }
    }
}

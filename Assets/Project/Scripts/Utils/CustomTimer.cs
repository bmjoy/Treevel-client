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
        /// タイマーが動き始めた時間
        /// </summary>
        private double _startTime;

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
            _startTime = Time.time;
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
            _second = Math.Round(Time.time - _startTime, 1, MidpointRounding.AwayFromZero);

            if (_timerText == null) return;

            // 更新した秒数 (整数) をテキストオブジェクトに表示
            var timeStr = Math.Floor(_second).ToString();
            _timerText.text = timeStr;
        }
    }
}

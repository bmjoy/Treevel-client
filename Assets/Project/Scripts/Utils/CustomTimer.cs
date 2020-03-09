using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Utils
{
    public class CustomTimer : MonoBehaviour
    {
        /// <summary>
        /// タイマーが動作中か
        /// </summary>
        private bool _active = false;

        /// <summary>
        /// タイマーの結果を書き込むテキストオブジェクト
        /// </summary>
        private Text _timerText;

        /// <summary>
        /// タイマーの秒数
        /// </summary>
        private int _second;

        /// <summary>
        /// タイマーが動き始めた時間
        /// </summary>
        private float _startTime;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Text timerText)
        {
            _timerText = timerText;
        }

        /// <summary>
        /// タイマーを動作
        /// </summary>
        public void StartTimer()
        {
            _startTime = Time.time;
            _active = true;
        }

        /// <summary>
        /// タイマーを停止
        /// </summary>
        public void StopTimer()
        {
            _active = false;
        }

        private void Update()
        {
            // タイマーが動作中でなければ無視
            if (!_active) return;

            // 秒数を更新
            _second = (int)(Time.time - _startTime);

            if (_timerText == null) return;

            // 更新した秒数をテキストオブジェクトに表示
            var timeStr = _second.ToString();
            _timerText.text = timeStr;
        }
    }
}

using System;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    public class RecordData : SingletonObject<RecordData>
    {
        /// <summary>
        /// 各失敗原因に対する失敗回数
        /// </summary>
        private Dictionary<EFailureReasonType, int> _failureReasonCount;

        public Dictionary<EFailureReasonType, int> FailureReasonCount
        {
            get => _failureReasonCount;
            set
            {
                _failureReasonCount = value;
                MyPlayerPrefs.SetDictionary(PlayerPrefsKeys.FAILURE_REASONS_COUNT, _failureReasonCount);
            }
        }

        /// <summary>
        /// 起動日数
        /// </summary>
        private int _startupDays;

        public int StartupDays
        {
            get => _startupDays;
            private set
            {
                _startupDays = value;
                PlayerPrefs.SetInt(PlayerPrefsKeys.STARTUP_DAYS, _startupDays);
            }
        }

        /// <summary>
        /// 最終起動日に応じて，起動日数を更新する
        /// </summary>
        public void UpdateStartupDays()
        {
            var lastStartupDate = LastStartupDate;

            if (lastStartupDate is DateTime date) {
                if (date < DateTime.Today) {
                    // 起動日数を加算する
                    var startupDays = StartupDays + 1;
                    // 起動日数を保存する
                    StartupDays = startupDays;
                }
            }

            ScheduleManager.AddEvent(this, "UpdateStartupDays", DateTime.Today.AddDays(1));
        }

        /// <summary>
        /// 最終起動日
        /// </summary>
        private DateTime? _lastStartupDate;

        public DateTime? LastStartupDate
        {
            get => _lastStartupDate;
            set
            {
                _lastStartupDate = value;

                if (_lastStartupDate is DateTime date) {
                    MyPlayerPrefs.SetDateTime(PlayerPrefsKeys.LAST_STARTUP_DATE, date);
                }
            }
        }

        private void Awake()
        {
            _failureReasonCount = MyPlayerPrefs.GetDictionary<EFailureReasonType, int>(PlayerPrefsKeys.FAILURE_REASONS_COUNT);
            _startupDays = PlayerPrefs.GetInt(PlayerPrefsKeys.STARTUP_DAYS, 1);
            _lastStartupDate = MyPlayerPrefs.GetDateTime(PlayerPrefsKeys.LAST_STARTUP_DATE);
        }

        /// <summary>
        /// 記録情報のリセット
        /// </summary>
        public void Reset()
        {
            // 最終起動日だけはリセットしない
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.FAILURE_REASONS_COUNT);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STARTUP_DAYS);
        }
    }
}

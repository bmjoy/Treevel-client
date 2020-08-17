using System;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    public static class RecordData
    {
        /// <summary>
        /// 各失敗原因に対する失敗回数
        /// </summary>
        private static Dictionary<EFailureReasonType, int> _failureReasonCount = MyPlayerPrefs.GetDictionary<EFailureReasonType, int>(PlayerPrefsKeys.FAILURE_REASONS_COUNT);

        public static Dictionary<EFailureReasonType, int> FailureReasonCount
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
        private static int _startupDays = PlayerPrefs.GetInt(PlayerPrefsKeys.STARTUP_DAYS, 1);

        public static int StartupDays
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
        public static void UpdateStartupDays()
        {
            var lastStartupDate = LastStartupDate;

            if (lastStartupDate is DateTime date) {
                if (date < DateTime.Today) {
                    // 起動日数を加算する
                    var startupDays = StartupDays;
                    // 起動日数を保存する
                    StartupDays = startupDays;
                }
            }
        }

        /// <summary>
        /// 最終起動日
        /// </summary>
        private static DateTime? _lastStartupDate = MyPlayerPrefs.GetDateTime(PlayerPrefsKeys.LAST_STARTUP_DATE);

        public static DateTime? LastStartupDate
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

        /// <summary>
        /// 記録情報のリセット
        /// </summary>
        public static void Reset()
        {
            // 最終起動日だけはリセットしない
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.FAILURE_REASONS_COUNT);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STARTUP_DAYS);
        }
    }
}

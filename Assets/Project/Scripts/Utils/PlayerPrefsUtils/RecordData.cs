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
        private static int _startupDays = PlayerPrefs.GetInt(PlayerPrefsKeys.STARTUP_DAYS, 0);

        public static int StartupDays
        {
            get => _startupDays;
            set
            {
                _startupDays = value;
                PlayerPrefs.SetInt(PlayerPrefsKeys.STARTUP_DAYS, _startupDays);
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
    }
}

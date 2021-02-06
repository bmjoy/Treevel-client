using System;
using System.Collections.Generic;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.Settings;
using UniRx;
using UnityEngine;

namespace Treevel.Common.Entities
{
    public class RecordData : SingletonObject<RecordData>
    {
        /// <summary>
        /// 各失敗原因に対する失敗回数
        /// </summary>
        private Dictionary<EFailureReasonType, int> _failureReasonCount;

        public Dictionary<EFailureReasonType, int> FailureReasonCount {
            get => _failureReasonCount;
            set {
                _failureReasonCount = value;
                PlayerPrefsUtility.SetDictionary(Constants.PlayerPrefsKeys.FAILURE_REASONS_COUNT, _failureReasonCount);
            }
        }

        /// <summary>
        /// 起動日数
        /// </summary>
        private int _startupDays;

        public int StartupDays {
            get => _startupDays;
            private set {
                _startupDays = value;
                PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.STARTUP_DAYS, _startupDays);
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

        public DateTime? LastStartupDate {
            get => _lastStartupDate;
            set {
                _lastStartupDate = value;

                if (_lastStartupDate is DateTime date) {
                    PlayerPrefsUtility.SetDateTime(Constants.PlayerPrefsKeys.LAST_STARTUP_DATE, date);
                }
            }
        }

        private void Awake()
        {
            Initialize();

            ResetController.DataReset.Subscribe(_ => {

                // 最終起動日だけはリセットしない
                PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.FAILURE_REASONS_COUNT);
                PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.STARTUP_DAYS);

                Initialize();
            }).AddTo(this);
        }

        private void Initialize()
        {
            _failureReasonCount = PlayerPrefsUtility.GetDictionary(Constants.PlayerPrefsKeys.FAILURE_REASONS_COUNT,
                                                                   new Dictionary<EFailureReasonType, int> {
                                                                       { EFailureReasonType.Others, 0 },
                                                                       { EFailureReasonType.Tornado, 0 },
                                                                       { EFailureReasonType.Meteorite, 0 },
                                                                       { EFailureReasonType.AimingMeteorite, 0 },
                                                                       { EFailureReasonType.Thunder, 0 },
                                                                       { EFailureReasonType.SolarBeam, 0 },
                                                                   });
            _startupDays = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.STARTUP_DAYS, 1);
            _lastStartupDate = PlayerPrefsUtility.GetDateTime(Constants.PlayerPrefsKeys.LAST_STARTUP_DATE);
        }
    }
}

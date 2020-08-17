using System;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;

namespace Project.Scripts.Utils
{
    /// <summary>
    /// アプリの起動，終了を管理
    /// </summary>
    public class AppManager : SingletonObject<AppManager>
    {
        /// <summary>
        /// アプリ起動時の処理
        /// </summary>
        public void OnApplicationStart()
        {
            UpdateStartupDays();
            RecordData.LastStartupDate = DateTime.Today;

            // FIXME: マージ前に消す
            Debug.Log($"起動日数：{RecordData.StartupDays}");
            Debug.Log($"最終起動日：{RecordData.LastStartupDate}");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) {
                // アプリがバックグラウンドになった時の処理
            }
            else {
                // アプリがバックグラウンドから復帰した時の処理
                UpdateStartupDays();
                RecordData.LastStartupDate = DateTime.Today;

                // FIXME: マージ前に消す
                Debug.Log($"起動日数：{RecordData.StartupDays}");
                Debug.Log($"最終起動日：{RecordData.LastStartupDate}");
            }
        }

        /// <summary>
        /// 起動日数を更新する
        /// </summary>
        private static void UpdateStartupDays()
        {
            var lastStartupDate = RecordData.LastStartupDate;

            if (lastStartupDate is DateTime date) {
                if (date < DateTime.Today) {
                    // 起動日数を加算する
                    var startupDays = RecordData.StartupDays;
                    // 起動日数を保存する
                    RecordData.StartupDays = startupDays;
                }
            }
            else {
                // 初起動日は，起動日数を 1 とする
                RecordData.StartupDays = 1;
            }
        }
    }
}

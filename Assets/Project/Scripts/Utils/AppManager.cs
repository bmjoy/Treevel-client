using System;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;

namespace Project.Scripts.Utils
{
    /// <summary>
    /// アプリの起動，終了を管理
    /// </summary>
    public static class AppManager
    {
        /// <summary>
        /// アプリ起動時の処理
        /// </summary>
        public static void OnApplicationStart()
        {
            RecordData.Instance.UpdateStartupDays();
            RecordData.Instance.LastStartupDate = DateTime.Today;

            // FIXME: マージ前に消す
            Debug.Log($"起動日数：{RecordData.Instance.StartupDays}");
            Debug.Log($"最終起動日：{RecordData.Instance.LastStartupDate}");
        }
    }
}

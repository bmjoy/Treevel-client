using System;
using Treevel.Common.Entities;

namespace Treevel.Common.Managers
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
            UserRecord.Instance.UpdateStartupDays();
            UserRecord.Instance.LastStartupDate = DateTime.Today;
        }
    }
}

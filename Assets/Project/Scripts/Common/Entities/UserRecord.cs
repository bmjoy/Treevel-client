using System;
using UnityEngine;

namespace Treevel.Common.Entities
{
    [Serializable]
    public struct UserRecord
    {
        /// <summary>
        /// 起動日数
        /// </summary>
        public int startupDays;

        /// <summary>
        /// 最終起動日（String）
        /// </summary>
        [SerializeField]
        private string stringLastStartupDate;

        /// <summary>
        /// 最終起動日（DateTime）
        /// </summary>
        public DateTime LastStartupDate {
            get => DateTime.ParseExact(stringLastStartupDate, "yyyyMMddHHmmss", null);
            set => stringLastStartupDate = $"{value:yyyyMMddHHmmss}";
        }

        public UserRecord(int startupDays, DateTime lastStartupDate)
        {
            this.startupDays = startupDays;
            stringLastStartupDate = $"{lastStartupDate:yyyyMMddHHmmss}";
        }
    }
}

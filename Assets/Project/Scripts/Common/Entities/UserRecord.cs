using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Treevel.Common.Entities
{
    [Serializable]
    public class UserRecord
    {
        /// <summary>
        /// 起動日数
        /// </summary>
        public ReactiveProperty<int> startupDays;

        /// <summary>
        /// 最終起動日（String）
        /// </summary>
        [FormerlySerializedAs("lastStartupDate")]
        [SerializeField]
        private string stringLastStartupDate;

        /// <summary>
        /// 最終起動日（DateTime）
        /// </summary>
        public DateTime LastStartupDate {
            get => DateTime.ParseExact(stringLastStartupDate, "yyyyMMddHHmmss", null);
            set => stringLastStartupDate = $"{value:yyyyMMddHHmmss}";
        }

        public UserRecord()
        {
            startupDays = new ReactiveProperty<int>(1);
            LastStartupDate = DateTime.Today;
        }
    }
}

using System;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
using Treevel.Common.Patterns.Singleton;
using UnityEngine;

namespace Treevel.Common.Utils
{
    public sealed class UserRecordService : SingletonObjectBase<UserRecordService>
    {
        /// <summary>
        /// オンメモリに UserRecord を保持する
        /// </summary>
        private UserRecord _cachedUserRecord;

        /// <summary>
        /// UserRecord を読み込んでおく
        /// </summary>
        public async UniTask PreloadUserRecordAsync()
        {
            try {
                _cachedUserRecord = await NetworkService.Execute(new GetUserRecordRequest());
            } catch {
                _cachedUserRecord = PlayerPrefsUtility.GetObjectOrDefault(Constants.PlayerPrefsKeys.USER_RECORD,
                                                                          new UserRecord(1, DateTime.Today));
                // リモートにまだデータがない場合には、保存する
                await SaveAsync(_cachedUserRecord);
            }
        }

        /// <summary>
        /// UserRecord を取得する
        /// </summary>
        /// <returns> UserRecord </returns>
        public UserRecord Get()
        {
            return _cachedUserRecord;
        }

        /// <summary>
        /// UserRecord を保存する
        /// </summary>
        /// <param name="userRecord"> UserRecord </param>
        private async UniTask SaveAsync(UserRecord userRecord)
        {
            await NetworkService.Execute(new UpdateUserRecordRequest(userRecord))
                .ContinueWith(isSuccess => {
                    if (isSuccess) {
                        // データの保存に成功したら、PlayerPrefs とオンメモリにも保存する
                        PlayerPrefsUtility.SetObject(Constants.PlayerPrefsKeys.USER_RECORD, userRecord);
                        _cachedUserRecord = userRecord;
                    }
                });
        }

        /// <summary>
        /// 最終起動日に応じて，起動日数を更新する
        /// </summary>
        public async UniTask UpdateStartupDaysAsync()
        {
            var lastStartupDate = _cachedUserRecord.LastStartupDate;

            if (lastStartupDate < DateTime.Today) {
                var newUserRecord = _cachedUserRecord;
                newUserRecord.startupDays++;
                newUserRecord.LastStartupDate = DateTime.Today;

                await SaveAsync(newUserRecord);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) {
                UpdateStartupDaysAsync().Forget();
            }
        }
    }
}

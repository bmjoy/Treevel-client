using System;
using Cysharp.Threading.Tasks;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Networks.Database
{
    public class PlayerPrefsDatabaseService : IDatabaseService
    {
        public UniTask<T> GetDataAsync<T>(string key) where T : new()
        {
            return new UniTask<T>(PlayerPrefsUtility.GetObject<T>(key));
        }

        public UniTask<bool> UpdateDataAsync<T>(string key, T data)
        {
            PlayerPrefsUtility.SetObject(key, data);
            Debug.Log($"key: {key} で PlayerPrefs に保存しました");

            // 現時点では、必ず成功することにする
            return new UniTask<bool>(true);
        }

        public UniTask<bool> LoginAsync()
        {
            throw new NotImplementedException();
        }
    }
}

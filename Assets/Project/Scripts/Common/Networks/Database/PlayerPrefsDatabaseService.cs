using System;
using Cysharp.Threading.Tasks;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Networks.Database
{
    public class PlayerPrefsDatabaseService : DatabaseService
    {
        public override UniTask<T> GetData<T>(string key)
        {
            return new UniTask<T>(PlayerPrefsUtility.GetObject<T>(key));
        }

        public override UniTask<bool> UpdateData<T>(string key, T data)
        {
            PlayerPrefsUtility.SetObject(key, data);
            Debug.Log($"key: {key} で PlayerPrefs に保存しました");

            // 現時点では、必ず成功することにする
            return new UniTask<bool>(true);
        }

        public override UniTask<bool> Login()
        {
            throw new NotImplementedException();
        }
    }
}

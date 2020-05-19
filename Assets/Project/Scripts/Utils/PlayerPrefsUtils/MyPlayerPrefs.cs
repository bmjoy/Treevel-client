using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    public class MyPlayerPrefs
    {
        /// <summary>
        /// 指定されたオブジェクト情報を保存
        /// </summary>
        /// <param name="key"> キー </param>
        /// <param name="obj"> 保存したいオブジェクト </param>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        public static void SetObject<T>(string key, T obj)
        {
            var json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 指定されたオブジェクト情報を読み込む
        /// </summary>
        /// <param name="key"> キー </param>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        /// <returns> 読み込みたいオブジェクト </returns>
        public static T GetObject<T>(string key) where T : new ()
        {
            var json = PlayerPrefs.GetString(key);
            if (json == "") return new T();
            var obj = JsonUtility.FromJson<T>(json);
            return obj;
        }
    }
}

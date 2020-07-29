using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
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
        /// 指定された辞書型オブジェクト情報を保存
        /// </summary>
        /// <param name="key"> キー </param>
        /// <param name="dictionary"> 保存したい辞書型オブジェクト </param>
        /// <typeparam name="Key"> 辞書型オブジェクトのキーの型 </typeparam>
        /// <typeparam name="Value"> 辞書型オブジェクトの値の型 </typeparam>
        /// <returns></returns>
        public  static void SetDictionary<Key, Value>(string key, Dictionary<Key, Value> dictionary)
        {
            string serizlizedDict = Serialize<Dictionary<Key, Value>> (dictionary);
            PlayerPrefs.SetString(key, serizlizedDict);
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

        /// <summary>
        /// 指定された辞書型オブジェクト情報を読み込む
        /// </summary>
        /// <param name="key"> キー </param>
        /// <typeparam name="Key"> 辞書型オブジェクトのキーの型</typeparam>
        /// <typeparam name="Value"> 辞書型オブジェクトの値の型 </typeparam>
        /// <returns></returns>
        public static Dictionary<Key, Value> GetDictionary<Key, Value> (string key)
        {
            if (PlayerPrefs.HasKey(key)) {
                string serizlizedDictionary = PlayerPrefs.GetString(key);
                return Deserialize<Dictionary<Key, Value>> (serizlizedDictionary);
            }

            return new Dictionary<Key, Value> ();
        }

        /// <summary>
        /// シリアライズ
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string Serialize<T> (T obj)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, obj);
            return Convert.ToBase64String(memoryStream.GetBuffer());
        }

        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="str"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T Deserialize<T> (string str)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(Convert.FromBase64String(str));
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}

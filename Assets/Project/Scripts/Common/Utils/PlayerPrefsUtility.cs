using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Treevel.Common.Utils
{
    public class PlayerPrefsUtility
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
        public static void SetDictionary<Key, Value>(string key, Dictionary<Key, Value> dictionary)
        {
            var serizlizedDict = Serialize(dictionary);
            PlayerPrefs.SetString(key, serizlizedDict);
        }

        /// <summary>
        /// 指定されたリスト型オブジェクト情報を保存
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="list">保存するリストオブジェクト</param>
        /// <typeparam name="T">リストの要素の型</typeparam>
        public static void SetList<T>(string key, List<T> list)
        {
            var serializedList = Serialize(list);
            PlayerPrefs.SetString(key, serializedList);
        }

        /// <summary>
        /// DateTime 型を PlayerPrefs に保存する
        /// </summary>
        /// <param name="key"> PlayerPrefs のキー </param>
        /// <param name="dateTime"> 保存したい DateTime </param>
        public static void SetDateTime(string key, DateTime dateTime)
        {
            // DateTime -> Binary -> String
            var dateTimeStr = dateTime.ToBinary().ToString();
            // String として保存
            PlayerPrefs.SetString(key, dateTimeStr);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 指定されたオブジェクト情報を読み込む
        /// </summary>
        /// <param name="key"> キー </param>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        /// <returns> 読み込みたいオブジェクト </returns>
        public static T GetObject<T>(string key) where T : new()
        {
            var json = PlayerPrefs.GetString(key);
            if (json == "") return new T();
            var obj = JsonUtility.FromJson<T>(json);
            return obj;
        }

        /// <summary>
        /// 指定されたオブジェクト情報を読み込む
        /// </summary>
        /// <param name="key"> キー </param>
        /// <param name="defaultValue"> デフォルト値 </param>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        /// <returns> 読み込みたいオブジェクト </returns>
        public static T GetObjectOrDefault<T>(string key, T defaultValue)
        {
            var json = PlayerPrefs.GetString(key);
            if (json == "") return defaultValue;
            var obj = JsonUtility.FromJson<T>(json);
            return obj;
        }

        /// <summary>
        /// 指定された辞書型オブジェクト情報を読み込む
        /// </summary>
        /// <param name="key"> キー </param>
        /// <param name="defaultDic"> デフォルト値 </param>
        /// <typeparam name="TKey"> 辞書型オブジェクトのキーの型</typeparam>
        /// <typeparam name="TValue"> 辞書型オブジェクトの値の型 </typeparam>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(
            string key, Dictionary<TKey, TValue> defaultDic = null)
        {
            if (!PlayerPrefs.HasKey(key)) {
                return defaultDic ?? new Dictionary<TKey, TValue>();
            }

            var serizlizedDictionary = PlayerPrefs.GetString(key);
            return Deserialize<Dictionary<TKey, TValue>>(serizlizedDictionary);
        }

        /// <summary>
        /// 指定されたリスト型オブジェクト情報を読み込む
        /// </summary>
        /// <param name="key">キー</param>
        /// <typeparam name="T">リストの要素の型</typeparam>
        /// <returns></returns>
        public static List<T> GetList<T>(string key)
        {
            if (!PlayerPrefs.HasKey(key)) {
                return new List<T>();
            }

            var serializedList = PlayerPrefs.GetString(key);
            return Deserialize<List<T>>(serializedList);
        }

        /// <summary>
        /// DateTime 型を PlayerPrefs から取得する
        /// </summary>
        /// <param name="key"> PlayerPrefs のキー </param>
        public static DateTime? GetDateTime(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return null;

            // String として取得
            var dateTimeStr = PlayerPrefs.GetString(key);

            // String -> Binary
            var dateTimeBinary = Convert.ToInt64(dateTimeStr);
            // Binary -> DateTime
            var dateTime = DateTime.FromBinary(dateTimeBinary);

            return dateTime;
        }

        /// <summary>
        /// シリアライズ
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string Serialize<T>(T obj)
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
        private static T Deserialize<T>(string str)
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(Convert.FromBase64String(str));
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}

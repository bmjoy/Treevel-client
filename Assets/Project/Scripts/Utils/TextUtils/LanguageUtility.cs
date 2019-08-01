using System;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.TextUtils
{
    /// <summary>
    /// 使える言語のリスト
    /// 列挙の値は<see cref="SystemLanguage"/>で定義する列挙値に合わせること
    /// 又、順序はResources/GameDatas/translation.csvに合わせること
    /// </summary>
    public enum ELanguage
    {
        Japanese, // 日本語
        English, // 英語
    }
    public class LanguageUtility
    {
        private static ELanguage _currentLanguage;
        private static Dictionary<KeyValuePair<ELanguage, ETextIndex>, string> _stringTable;
        private static string DATA_PATH = "GameDatas/translation";

        public delegate void LanguageChangeEvent();
        public static event LanguageChangeEvent OnLanguageChange;


        /// <summary>
        /// アプリ起動するとき自動でデバイスの言語を取得し設定する。
        /// </summary>

        #if UNITY_EDITOR
            [UnityEditor.Callbacks.DidReloadScripts]
        #endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            string systemLanguage = Application.systemLanguage.ToString();
            if (!Enum.TryParse(systemLanguage, out _currentLanguage)) {
                _currentLanguage = ELanguage.Japanese;
            }
            Load();
        }

        /// <summary>
        /// csvファイルからテキストの対応表を読み込む
        /// </summary>
        #if UNITY_EDITOR
            [UnityEditor.Callbacks.DidReloadScripts]
            [UnityEditor.MenuItem("Tools/Load Text")]
        #endif
        public static void Load()
        {
            _stringTable = new Dictionary<KeyValuePair<ELanguage, ETextIndex>, string>();
            List<string[]> datas = CSVReader.LoadCSV(DATA_PATH);

            // skip headers
            for (int i = 1; i < datas.Count; i++)
            {
                string[] line = datas[i];

                ETextIndex index;
                if (ETextIndex.TryParse(line[0], out index)) {
                    foreach (ELanguage language in Enum.GetValues(typeof(ELanguage))) {
                        KeyValuePair<ELanguage, ETextIndex> key = new KeyValuePair<ELanguage, ETextIndex>(language, index);
                        string newText = line[(int)language + 1];
                        _stringTable.Add(key, newText);
                    }
                }
                else {
                    Debug.LogWarning(message: $"{line[0]} is not a valid enum value");
                }
            }
        }
        public static string GetText(ETextIndex index)
        {
            KeyValuePair<ELanguage, ETextIndex> key = new KeyValuePair<ELanguage, ETextIndex>(_currentLanguage, index);
            if (_stringTable.ContainsKey(key)){
                return _stringTable[key];
            }
            else {
                Debug.LogWarning($"{CurrentLanguage.ToString()}の{index.ToString()}が存在していない");
                return "";
            }
        }

        public static ELanguage CurrentLanguage {
            get {
                return _currentLanguage;
            }
            set {
                _currentLanguage = value;
                OnLanguageChange.Invoke();
            }
        }
    }
}

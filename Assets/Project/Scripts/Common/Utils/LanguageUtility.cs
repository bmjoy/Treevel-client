using System;
using System.Collections.Generic;
using Treevel.Common.Entities;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Treevel.Common.Utils
{
    /// <summary>
    /// 使える言語のリスト
    /// 列挙の値は<see cref="SystemLanguage"/>で定義する列挙値に合わせること
    /// 又、順序はResources/GameDatas/translation.csvに合わせること
    /// </summary>
    public enum ELanguage
    {
        Japanese, // 日本語
        English,  // 英語
    }

    public static class LanguageUtility
    {
        private static Dictionary<KeyValuePair<ELanguage, ETextIndex>, string> _stringTable;
        private const string _DATA_PATH = "GameDatas/translation";

        static LanguageUtility()
        {
            LoadTranslationData();
        }

        /// <summary>
        /// csvファイルからテキストの対応表を読み込む
        /// </summary>
        #if UNITY_EDITOR
        [DidReloadScripts, MenuItem("Tools/Load Text")]
        #endif
        public static void LoadTranslationData()
        {
            _stringTable = new Dictionary<KeyValuePair<ELanguage, ETextIndex>, string>();
            var datas = CSVReader.LoadCSV(_DATA_PATH);

            // skip headers
            for (var i = 1; i < datas.Count; i++) {
                var line = datas[i];

                ETextIndex index;
                if (Enum.TryParse(line[0], out index)) {
                    foreach (ELanguage language in Enum.GetValues(typeof(ELanguage))) {
                        var key = new KeyValuePair<ELanguage, ETextIndex>(language, index);
                        var newText = line[(int)language + 1];
                        _stringTable.Add(key, newText);
                    }
                } else {
                    Debug.LogWarning($"{line[0]} is not a valid enum value");
                }
            }
        }

        public static string GetText(ETextIndex index)
        {
            var key = new KeyValuePair<ELanguage, ETextIndex>(UserSettings.CurrentLanguage.Value, index);
            if (_stringTable.ContainsKey(key)) {
                return _stringTable[key];
            }

            Debug.LogWarning($"{UserSettings.CurrentLanguage}の{index.ToString()}が存在していない");
            return "";
        }
    }
}

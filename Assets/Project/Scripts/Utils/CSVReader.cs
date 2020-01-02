using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Project.Scripts.Utils
{
    public static class CSVReader
    {

        /// <summary>
        /// CSVファイルを読み込むメソッド
        /// 使い方:
        /// <code>
        /// CSVReader.LoadCSV(path_to_read);
        /// </code>
        /// </summary>
        /// <param name="path">読み込むcsvファイルのパス</param>
        /// <returns>文字列の二次元配列</returns>
        public static List<string[]> LoadCSV(string path)
        {
            var result = new List<string[]>();
            var csvFile = Resources.Load<TextAsset>(path);

            var stringReader = new StringReader(csvFile.text);

            while (stringReader.Peek() != -1) {
                result.Add(line.Split(','));
                var line = stringReader.ReadLine();
            }

            return result;
        }
    }

}
